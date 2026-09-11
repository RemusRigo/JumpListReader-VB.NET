'--------------------------------------------------------------------------------------------------
' libRights - User rights functions
'    © 2026 Remus Rigo
'       v1.1 20260828
'--------------------------------------------------------------------------------------------------

Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Security.AccessControl
Imports System.Security.Principal

Public Module libRights

   '-----------------------------------------------------------------------------------------------
   ' Check if app is running with administrative privileges
   Public Function IsAppElevated() As Boolean
      Dim identity As WindowsIdentity = WindowsIdentity.GetCurrent()
      Dim principal As New WindowsPrincipal(identity)

      Return principal.IsInRole(WindowsBuiltInRole.Administrator)
   End Function

   '-----------------------------------------------------------------------------------------------
   Public Sub EnablePrivilege(privName As String)
      Dim hToken As IntPtr = IntPtr.Zero

      If Not OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_ADJUST_PRIVILEGES Or TOKEN_QUERY, hToken) Then
         Throw New System.ComponentModel.Win32Exception()
      End If

      Dim luid As New LUID()
      If Not LookupPrivilegeValue(Nothing, privName, luid) Then
         Throw New System.ComponentModel.Win32Exception()
      End If

      Dim tp As New TOKEN_PRIVILEGES()
      tp.PrivilegeCount = 1
      tp.Privileges = New LUID_AND_ATTRIBUTES With {
          .Luid = luid,
          .Attributes = SE_PRIVILEGE_ENABLED
      }

      Dim retLen As Integer = 0
      Dim prev As New TOKEN_PRIVILEGES()

      If Not AdjustTokenPrivileges(hToken, False, tp, Marshal.SizeOf(tp), prev, retLen) Then
         Throw New System.ComponentModel.Win32Exception()
      End If
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' Ownership
   Public Sub TakeOwnership(path As String)
      If String.IsNullOrWhiteSpace(path) Then
         Throw New ArgumentNullException(NameOf(path))
      End If

      If Not Directory.Exists(path) AndAlso Not File.Exists(path) Then
         Throw New IO.FileNotFoundException($"Path not found: {path}")
      End If

      ' Enable required privileges
      EnablePrivilege("SeTakeOwnershipPrivilege")
      EnablePrivilege("SeRestorePrivilege")

      Dim sid As SecurityIdentifier = WindowsIdentity.GetCurrent().User
      Dim sidObj As SecurityIdentifier = DirectCast(sid.Translate(GetType(SecurityIdentifier)), SecurityIdentifier)
      Dim sidPtr As IntPtr = sidObj.BinaryFormPtr()

      ' Set owner
      Dim result = SetNamedSecurityInfo(path, SE_FILE_OBJECT, OWNER_SECURITY_INFORMATION, sidPtr, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero)

      If result <> 0UI Then
         Throw New Win32Exception(CInt(result), $"SetNamedSecurityInfo failed for '{path}'") 'Throw New Exception("SetNamedSecurityInfo failed with error " & result)
      End If
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' ACL
   Public Sub GrantFullControl(path As String)
      Dim user As String = WindowsIdentity.GetCurrent().Name

      Dim di As New DirectoryInfo(path)
      Dim ds = di.GetAccessControl()

      ds.AddAccessRule(New FileSystemAccessRule(
        user,
        FileSystemRights.FullControl,
        InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit,
        PropagationFlags.None,
        AccessControlType.Allow))

      di.SetAccessControl(ds)
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' Convert SecurityIdentifier to IntPtr
   <System.Runtime.CompilerServices.Extension>
   Public Function BinaryFormPtr(sid As SecurityIdentifier) As IntPtr
      Dim bytes(sid.BinaryLength - 1) As Byte
      sid.GetBinaryForm(bytes, 0)
      Dim ptr = Marshal.AllocHGlobal(bytes.Length)
      Marshal.Copy(bytes, 0, ptr, bytes.Length)
      Return ptr
   End Function

End Module

