Imports System
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text

<Flags>
Public Enum FileAttributeFlags As UInt32
   FILE_ATTRIBUTE_READONLY = &H1
   FILE_ATTRIBUTE_HIDDEN = &H2
   FILE_ATTRIBUTE_SYSTEM = &H4
   FILE_ATTRIBUTE_DIRECTORY = &H10
   FILE_ATTRIBUTE_ARCHIVE = &H20
   FILE_ATTRIBUTE_DEVICE = &H40
   FILE_ATTRIBUTE_NORMAL = &H80
   FILE_ATTRIBUTE_TEMPORARY = &H100
   FILE_ATTRIBUTE_SPARSE_FILE = &H200
   FILE_ATTRIBUTE_REPARSE_POINT = &H400
   FILE_ATTRIBUTE_COMPRESSED = &H800
   FILE_ATTRIBUTE_OFFLINE = &H1000
   FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = &H2000
   FILE_ATTRIBUTE_ENCRYPTED = &H4000
End Enum
<Flags>
Public Enum LinkFlags As UInt32
   HasLinkTargetIDList = &H1
   HasLinkInfo = &H2
   HasName = &H4
   HasRelativePath = &H8
   HasWorkingDir = &H10
   HasArguments = &H20
   HasIconLocation = &H40
   IsUnicode = &H80
   ForceNoLinkInfo = &H100
   HasExpString = &H200
   RunInSeparateProcess = &H400
   Unused1 = &H800
   HasDarwinID = &H1000
   RunAsUser = &H2000
   HasExpIcon = &H4000
   NoPidlAlias = &H8000
   Unused2 = &H10000
   RunWithShimLayer = &H20000
   ForceNoLinkTrack = &H40000
   EnableTargetMetadata = &H80000
   DisableLinkPathTracking = &H100000
   DisableKnownFolderTracking = &H200000
   DisableKnownFolderAlias = &H400000
   AllowLinkToLink = &H800000
   UnaliasOnSave = &H1000000
   PreferEnvironmentPath = &H2000000
   KeepLocalIDListForUNCTarget = &H4000000
End Enum


Public Class parseShellLink

   ' GUID for Shell Link header
   Private Shared ReadOnly CLSID_ShellLink As Guid =
          New Guid("00021401-0000-0000-C000-000000000046")

   <StructLayout(LayoutKind.Sequential, Pack:=1)>
   Private Structure SHELL_LINK_HEADER
      Public HeaderSize As UInt32
      Public LinkCLSID As Guid
      Public LinkFlags As UInt32
      Public FileAttributes As UInt32
      Public CreationTime As Long
      Public AccessTime As Long
      Public WriteTime As Long
      Public FileSize As UInt32
      Public IconIndex As Int32
      Public ShowCommand As UInt32
      Public HotKey As UInt16
      Public Reserved1 As UInt16
      Public Reserved2 As UInt32
      Public Reserved3 As UInt32
   End Structure


   Public Class ShellLinkInfo
      Public Property TargetPath As String
      Public Property Arguments As String
      Public Property WorkingDirectory As String
      Public Property Description As String
      Public Property IconLocation As String
      Public Property FileSize As UInt32
      Public Property CreationTime As DateTime?
      Public Property AccessTime As DateTime?
      Public Property WriteTime As DateTime?
      Public Property Attributes As FileAttributeFlags
      Public Property Flags As LinkFlags
   End Class

   Public Shared Function ParseLnkFile(path As String) As ShellLinkInfo
      Dim bytes = File.ReadAllBytes(path)
      Return ParseLnkBytes(bytes)
   End Function

   Public Shared Function ParseLnkBytes(data As Byte()) As ShellLinkInfo
      Dim info As New ShellLinkInfo()

      Dim offset As Integer = 0

      ' --- Header ---
      Dim header As SHELL_LINK_HEADER = ReadHeader(data, offset)

      If header.HeaderSize <> 76UI OrElse header.LinkCLSID <> CLSID_ShellLink Then
         Throw New InvalidDataException("Not a valid Shell Link header.")
      End If

      info.Flags = CType(header.LinkFlags, LinkFlags)
      info.Attributes = CType(header.FileAttributes, FileAttributeFlags)
      info.FileSize = header.FileSize
      info.CreationTime = FileTimeToDate(header.CreationTime)
      info.AccessTime = FileTimeToDate(header.AccessTime)
      info.WriteTime = FileTimeToDate(header.WriteTime)

      offset += CInt(header.HeaderSize)

      ' --- LinkTargetIDList (optional) ---
      If info.Flags.HasFlag(LinkFlags.HasLinkTargetIDList) Then
         Dim idListSize As UInt16 = BitConverter.ToUInt16(data, offset)
         offset += 2

         ' IDList is a sequence of ITEMID structures ending with 0x0000
         ' We don't fully parse the PIDL here; we just skip it.
         offset += idListSize
      End If

      ' --- LinkInfo (optional) ---
      If info.Flags.HasFlag(LinkFlags.HasLinkInfo) Then
         Dim linkInfoSize As UInt32 = BitConverter.ToUInt32(data, offset)
         Dim linkInfoOffset As Integer = offset
         offset += CInt(linkInfoSize)

         ' Basic LinkInfo parsing to get local path if present
         Dim localPath = ParseLinkInfo_LocalPath(data, linkInfoOffset)
         If Not String.IsNullOrEmpty(localPath) Then
            info.TargetPath = localPath
         End If
      End If

      ' --- StringData (optional) ---
      Dim isUnicode As Boolean = info.Flags.HasFlag(LinkFlags.IsUnicode)

      If info.Flags.HasFlag(LinkFlags.HasName) Then
         info.Description = ReadStringData(data, offset, isUnicode)
      End If

      If info.Flags.HasFlag(LinkFlags.HasRelativePath) Then
         Dim relPath = ReadStringData(data, offset, isUnicode)
         If String.IsNullOrEmpty(info.TargetPath) Then
            info.TargetPath = relPath
         End If
      End If

      If info.Flags.HasFlag(LinkFlags.HasWorkingDir) Then
         info.WorkingDirectory = ReadStringData(data, offset, isUnicode)
      End If

      If info.Flags.HasFlag(LinkFlags.HasArguments) Then
         info.Arguments = ReadStringData(data, offset, isUnicode)
      End If

      If info.Flags.HasFlag(LinkFlags.HasIconLocation) Then
         info.IconLocation = ReadStringData(data, offset, isUnicode)
      End If

      Return info
   End Function

   Private Shared Function ReadHeader(data As Byte(), offset As Integer) As SHELL_LINK_HEADER
      Dim size = Marshal.SizeOf(GetType(SHELL_LINK_HEADER))
      Dim ptr = Marshal.AllocHGlobal(size)
      Try
         Marshal.Copy(data, offset, ptr, size)
         Return CType(Marshal.PtrToStructure(ptr, GetType(SHELL_LINK_HEADER)), SHELL_LINK_HEADER)
      Finally
         Marshal.FreeHGlobal(ptr)
      End Try
   End Function

   Private Shared Function FileTimeToDate(fileTime As Long) As DateTime?
      If fileTime = 0 Then Return Nothing
      Return DateTime.FromFileTimeUtc(fileTime).ToLocalTime()
   End Function

   Private Shared Function ReadStringData(data As Byte(), ByRef offset As Integer, isUnicode As Boolean) As String
      Dim charCount As UInt16 = BitConverter.ToUInt16(data, offset)
      offset += 2

      If charCount = 0 Then
         Return String.Empty
      End If

      Dim byteCount As Integer
      Dim s As String

      If isUnicode Then
         byteCount = charCount * 2
         s = Encoding.Unicode.GetString(data, offset, byteCount)
      Else
         byteCount = charCount
         s = Encoding.Default.GetString(data, offset, byteCount)
      End If

      offset += byteCount
      Return s.TrimEnd(ChrW(0))
   End Function

   Private Shared Function ParseLinkInfo_LocalPath(data As Byte(), offset As Integer) As String
      Try
         Dim linkInfoSize As UInt32 = BitConverter.ToUInt32(data, offset)
         Dim linkInfoHeaderSize As UInt32 = BitConverter.ToUInt32(data, offset + 4)
         Dim linkInfoFlags As UInt32 = BitConverter.ToUInt32(data, offset + 8)

         Dim volumeIDOffset As UInt32 = BitConverter.ToUInt32(data, offset + 12)
         Dim localBasePathOffset As UInt32 = BitConverter.ToUInt32(data, offset + 16)
         Dim commonNetworkRelativeLinkOffset As UInt32 = BitConverter.ToUInt32(data, offset + 20)
         Dim commonPathSuffixOffset As UInt32 = BitConverter.ToUInt32(data, offset + 24)

         Dim localBasePath As String = Nothing

         If localBasePathOffset <> 0UI Then
            Dim lpOffset = offset + CInt(localBasePathOffset)
            localBasePath = ReadNullTerminatedAnsi(data, lpOffset)
         End If

         Dim commonSuffix As String = Nothing
         If commonPathSuffixOffset <> 0UI Then
            Dim csOffset = offset + CInt(commonPathSuffixOffset)
            commonSuffix = ReadNullTerminatedAnsi(data, csOffset)
         End If

         If Not String.IsNullOrEmpty(localBasePath) Then
            If Not String.IsNullOrEmpty(commonSuffix) Then
               Return Path.Combine(localBasePath, commonSuffix)
            Else
               Return localBasePath
            End If
         End If

         Return Nothing
      Catch
         Return Nothing
      End Try
   End Function

   Private Shared Function ReadNullTerminatedAnsi(data As Byte(), offset As Integer) As String
      Dim start = offset
      While offset < data.Length AndAlso data(offset) <> 0
         offset += 1
      End While
      Dim len = offset - start
      If len <= 0 Then Return String.Empty
      Return Encoding.Default.GetString(data, start, len)
   End Function

End Class
