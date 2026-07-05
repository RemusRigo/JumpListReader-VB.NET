Imports System
Imports System.IO
Imports System.Collections.Generic
Imports OpenMcdf

Public Class parseJumpList

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
      Dim result As New List(Of JumpListEntry)()

      Using root As RootStorage = RootStorage.OpenRead(path)

         ' Enumerate top-level stream names
         Dim streamNames As New List(Of String)()
         For Each e In root.EnumerateEntries()
            If e.Type = EntryType.Stream Then
               streamNames.Add(e.Name)
            End If
         Next

         Dim destListInfo As New Dictionary(Of String, DateTime?)()
         If streamNames.Contains("DestList") Then
            destListInfo = ParseDestList(root)
         End If

         For Each sName In streamNames
            If sName = "DestList" OrElse sName = "DestListPropertyStore" Then Continue For
            If Not IsNumericStreamName(sName) Then Continue For

            Dim lnkBytes = ReadStreamBytes(root, sName)
            If lnkBytes Is Nothing OrElse lnkBytes.Length = 0 Then Continue For

            Dim linkInfo = parseShellLink.ParseLnkBytes(lnkBytes)

            Dim entry As New JumpListEntry()
            entry.StreamName = sName
            entry.TargetPath = linkInfo.TargetPath
            entry.Arguments = linkInfo.Arguments
            entry.Description = linkInfo.Description
            entry.WorkingDirectory = linkInfo.WorkingDirectory
            entry.IconLocation = linkInfo.IconLocation

            If destListInfo.ContainsKey(sName) Then
               entry.LastAccessTime = destListInfo(sName)
            Else
               entry.LastAccessTime = linkInfo.AccessTime
            End If

            result.Add(entry)
         Next

      End Using

      Return result
   End Function

   ' --- Helpers ---

   Private Shared Function ReadStreamBytes(root As RootStorage, name As String) As Byte()
      Using cfbStream As CfbStream = root.OpenStream(name)
         Dim buffer(CInt(cfbStream.Length) - 1) As Byte
         Dim totalRead As Integer = 0
         While totalRead < buffer.Length
            Dim n = cfbStream.Read(buffer, totalRead, buffer.Length - totalRead)
            If n = 0 Then Exit While
            totalRead += n
         End While
         If totalRead < buffer.Length Then Array.Resize(buffer, totalRead)
         Return buffer
      End Using
   End Function

   Private Shared Function IsNumericStreamName(name As String) As Boolean
      Dim n As Integer
      Return Integer.TryParse(name, n)
   End Function

   ' --- DestList parsing (unchanged from before — this part was never the problem) ---

   Private Shared Function ParseDestList(root As RootStorage) As Dictionary(Of String, DateTime?)
      Dim result As New Dictionary(Of String, DateTime?)()

      Dim bytes = ReadStreamBytes(root, "DestList")
      If bytes Is Nothing OrElse bytes.Length < 32 Then Return result

      Dim formatVersion As Int32 = BitConverter.ToInt32(bytes, 0)
      Dim entryCount As UInt32 = BitConverter.ToUInt32(bytes, 4)

      Dim offset As Integer = 32 ' header is always 32 bytes

      For i As Integer = 0 To CInt(entryCount) - 1
         If offset >= bytes.Length Then Exit For

         Dim entryIdOffset As Integer = offset + 88
         Dim entryIdSize As Integer
         Dim filetimeOffset As Integer = offset + 100
         Dim pathLenOffset As Integer
         Dim entryFixedSize As Integer

         If formatVersion = 1 Then
            ' Windows 7 layout (rare in practice now)
            entryIdSize = 8
            pathLenOffset = offset + 114
            entryFixedSize = 116
         Else
            ' Windows 8 / 8.1 / 10 / 11 layout
            entryIdSize = 4
            pathLenOffset = offset + 128
            entryFixedSize = 130
         End If

         If pathLenOffset + 2 > bytes.Length Then Exit For

         ' --- Entry ID / stream name ---
         Dim streamId As Int64
         If entryIdSize = 4 Then
            streamId = BitConverter.ToInt32(bytes, entryIdOffset)
         Else
            streamId = BitConverter.ToInt64(bytes, entryIdOffset)
         End If
         Dim streamName = streamId.ToString()

         ' --- Last access FILETIME ---
         Dim lastAccess As DateTime? = Nothing
         Dim ft As Long = BitConverter.ToInt64(bytes, filetimeOffset)
         If ft > 0 Then
            Try
               lastAccess = DateTime.FromFileTimeUtc(ft).ToLocalTime()
            Catch ex As ArgumentOutOfRangeException
               lastAccess = Nothing ' corrupted/garbage timestamp - skip, don't crash the whole parse
            End Try
         End If

         If Not result.ContainsKey(streamName) Then
            result.Add(streamName, lastAccess)
         End If

         ' --- Use the variable-length path string to find the NEXT entry's real offset ---
         Dim pathLenChars As UInt16 = BitConverter.ToUInt16(bytes, pathLenOffset)
         Dim entrySize As Integer = entryFixedSize + (pathLenChars * 2) + 4 ' +4 = trailing zero padding

         offset += entrySize
      Next

      Return result
   End Function

End Class