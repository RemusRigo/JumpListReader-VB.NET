Imports System
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Collections.Generic

Public Class parseJumpList

   <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
   Public Structure STATSTG
      <MarshalAs(UnmanagedType.LPWStr)>
      Public pwcsName As String
      Public type As Integer
      Public cbSize As Long
      Public mtime As Long
      Public ctime As Long
      Public atime As Long
      Public grfMode As Integer
      Public grfLocksSupported As Integer
      Public clsid As Guid
      Public grfStateBits As Integer
      Public reserved As Integer
   End Structure

   <ComImport(), Guid("0000000D-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
   Public Interface IEnumSTATSTG
      Function [Next](
    <MarshalAs(UnmanagedType.U4)> celt As UInteger,
    <MarshalAs(UnmanagedType.LPArray, SizeParamIndex:=0)> rgelt() As STATSTG,
    <MarshalAs(UnmanagedType.U4)> ByRef pceltFetched As UInteger
) As Integer

      Sub Skip(celt As UInteger)
      Sub Reset()
      Sub Clone(ByRef ppenum As IEnumSTATSTG)
   End Interface


   <ComImport(), Guid("0000000B-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
   Public Interface IStorage
      Sub CreateStream()
      Sub OpenStream(<MarshalAs(UnmanagedType.LPWStr)> pwcsName As String,
                   reserved1 As IntPtr,
                   grfMode As Integer,
                   reserved2 As Integer,
                   <Out()> ByRef ppstm As IStream)

      Sub CreateStorage()
      Sub OpenStorage()
      Sub CopyTo()
      Sub MoveElementTo()
      Sub Commit()
      Sub Revert()
      Sub EnumElements(reserved1 As Integer, reserved2 As IntPtr, reserved3 As Integer, <Out()> ByRef ppenum As IEnumSTATSTG)

      Sub DestroyElement()
      Sub RenameElement()
      Sub SetElementTimes()
      Sub SetClass()
      Sub SetStateBits()
      Sub Stat(<Out()> ByRef pstatstg As STATSTG, grfStatFlag As Integer)
   End Interface


   <ComImport(), Guid("0000000C-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
   Public Interface IStream
      Sub Read(<Out()> pv As Byte(), cb As Integer, <Out()> ByRef pcbRead As Integer)
      Sub Write(pv As Byte(), cb As Integer, <Out()> ByRef pcbWritten As Integer)
      Sub Seek(dlibMove As Long, dwOrigin As Integer, <Out()> ByRef plibNewPosition As Long)
      Sub SetSize(libNewSize As Long)
      Sub CopyTo(pstm As IStream, cb As Long, <Out()> ByRef pcbRead As Long, <Out()> ByRef pcbWritten As Long)
      Sub Commit(grfCommitFlags As Integer)
      Sub Revert()
      Sub LockRegion()
      Sub UnlockRegion()
      Sub Stat(<Out()> ByRef pstatstg As STATSTG, grfStatFlag As Integer)
      Sub Clone(<Out()> ByRef ppstm As IStream)
   End Interface


   <DllImport("ole32.dll", CharSet:=CharSet.Unicode)>
   Private Shared Function StgOpenStorage(
       pwcsName As String,
       pstgPriority As IntPtr,
       grfMode As Integer,
       snbExclude As IntPtr,
       reserved As Integer,
       <Out()> ByRef ppstgOpen As IStorage) As Integer
   End Function

   Private Const STGM_READ As Integer = &H0
   Private Const STGM_SHARE_DENY_WRITE As Integer = &H20

   ' --- Public model ---

   Public Class JumpListEntry
      Public Property StreamName As String
      Public Property TargetPath As String
      Public Property Arguments As String
      Public Property Description As String
      Public Property WorkingDirectory As String
      Public Property IconLocation As String
      Public Property LastAccessTime As DateTime?
   End Class

   Public Shared Function ReadJumpList(path As String) As List(Of JumpListEntry)
      Dim storage As IStorage = Nothing
      Dim hr = StgOpenStorage(path,
                              IntPtr.Zero,
                              STGM_READ Or STGM_SHARE_DENY_WRITE,
                              IntPtr.Zero,
                              0,
                              storage)

      If hr <> 0 OrElse storage Is Nothing Then
         Throw New IOException("StgOpenStorage failed, HRESULT=0x" & hr.ToString("X"))
      End If

      ' Enumerate all streams
      Dim streams = EnumerateStreams(storage)

      ' Read DestList (if present)
      Dim destListInfo = New Dictionary(Of String, DateTime?)()
      If streams.Contains("DestList") Then
         destListInfo = ParseDestList(storage)
      End If

      Dim result As New List(Of JumpListEntry)()

      ' For each numbered stream, read as LNK and parse
      For Each sName In streams
         If sName = "DestList" Then Continue For

         ' Jump List numbered streams are typically "1", "2", "3", ...
         If Not IsNumericStreamName(sName) Then Continue For

         Dim lnkBytes = ReadStreamBytes(storage, sName)
         If lnkBytes Is Nothing OrElse lnkBytes.Length = 0 Then Continue For

         Dim linkInfo = parseShellLink.ParseLnkBytes(lnkBytes)

         Dim entry As New JumpListEntry()
         entry.StreamName = sName
         entry.TargetPath = linkInfo.TargetPath
         entry.Arguments = linkInfo.Arguments
         entry.Description = linkInfo.Description
         entry.WorkingDirectory = linkInfo.WorkingDirectory
         entry.IconLocation = linkInfo.IconLocation

         ' Map DestList timestamp if available
         If destListInfo.ContainsKey(sName) Then
            entry.LastAccessTime = destListInfo(sName)
         Else
            entry.LastAccessTime = linkInfo.AccessTime
         End If

         result.Add(entry)
      Next

      Return result
   End Function

   ' --- Helpers ---

   Private Shared Function EnumerateStreams(storage As IStorage) As List(Of String)
      Dim list As New List(Of String)()
      Dim enumStat As IEnumSTATSTG = Nothing

      storage.EnumElements(0, IntPtr.Zero, 0, enumStat)

      Dim fetched As UInteger = 0
      Dim statArray(0) As STATSTG   ' array with 1 element

      While enumStat.[Next](1UI, statArray, fetched) = 0 AndAlso fetched = 1UI
         Dim stat = statArray(0)
         MessageBox.Show($"Found: '{stat.pwcsName}' type={stat.type}") ' TEMP DEBUG
         If stat.type = 2 Then ' STGTY_STREAM
            list.Add(stat.pwcsName)
         End If
      End While

      Return list
   End Function


   Private Shared Function ReadStreamBytes(storage As IStorage, name As String) As Byte()
      Dim stm As IStream = Nothing
      storage.OpenStream(name, IntPtr.Zero, STGM_READ Or STGM_SHARE_DENY_WRITE, 0, stm)
      If stm Is Nothing Then Return Nothing

      Dim stat As New STATSTG()
      stm.Stat(stat, 0)
      Dim size = CInt(stat.cbSize)
      Dim buffer(size - 1) As Byte
      Dim read As Integer
      stm.Read(buffer, size, read)
      If read < size Then
         Array.Resize(buffer, read)
      End If
      Return buffer
   End Function

   Private Shared Function IsNumericStreamName(name As String) As Boolean
      Dim n As Integer
      Return Integer.TryParse(name, n)
   End Function

   ' --- DestList parsing ---

   Private Shared Function ParseDestList(storage As IStorage) As Dictionary(Of String, DateTime?)
      Dim result As New Dictionary(Of String, DateTime?)()

      Dim bytes = ReadStreamBytes(storage, "DestList")
      If bytes Is Nothing OrElse bytes.Length < 32 Then Return result

      Dim offset As Integer = 0

      ' Header:
      ' 0x00: 4 bytes - unknown (version)
      ' 0x04: 4 bytes - number of entries
      ' 0x08: 8 bytes - last used time (FILETIME)
      ' 0x10: 8 bytes - unknown
      ' 0x18: 8 bytes - unknown

      Dim version As UInt32 = BitConverter.ToUInt32(bytes, offset)
      Dim entryCount As UInt32 = BitConverter.ToUInt32(bytes, offset + 4)
      offset += 32 ' skip header

      ' Each entry:
      ' FILETIME (8 bytes) - last access
      ' 4 bytes - unknown
      ' 4 bytes - unknown
      ' 4 bytes - stream ID (Int32)
      ' 4 bytes - unknown
      ' 4 bytes - unknown
      ' 4 bytes - unknown
      ' 4 bytes - unknown
      ' 4 bytes - unknown
      ' 4 bytes - unknown
      ' (structure varies by version; we only care about FILETIME + stream ID)

      For i As Integer = 0 To CInt(entryCount) - 1
         If offset + 32 > bytes.Length Then Exit For

         Dim ft As Long = BitConverter.ToInt64(bytes, offset)
         Dim lastAccess As DateTime? = Nothing
         If ft <> 0 Then
            lastAccess = DateTime.FromFileTimeUtc(ft).ToLocalTime()
         End If

         Dim streamId As Int32 = BitConverter.ToInt32(bytes, offset + 16)
         Dim streamName = streamId.ToString()

         If Not result.ContainsKey(streamName) Then
            result.Add(streamName, lastAccess)
         End If

         ' DestList entry size is typically 0x90 or 0x98 depending on version;
         ' we use a conservative step of 0x90 (144) which works for common builds.
         offset += &H90
      Next

      Return result
   End Function

End Class
