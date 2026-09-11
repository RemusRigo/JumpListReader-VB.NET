Imports System
Imports System.IO
Imports System.Collections.Generic
Imports System.Globalization
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

   ' Compound File Binary (OLE2) magic - marks an *.automaticDestinations-ms file.
   Private Shared ReadOnly CfbMagic As Byte() =
      New Byte() {&HD0, &HCF, &H11, &HE0, &HA1, &HB1, &H1A, &HE1}

   ' Start of an embedded Shell Link: HeaderSize (0x0000004C) followed by CLSID_ShellLink.
   ' Used to locate each shortcut inside a *.customDestinations-ms file.
   Private Shared ReadOnly ShellLinkSignature As Byte() =
      New Byte() {&H4C, 0, 0, 0,
                  &H1, &H14, &H2, 0, 0, 0, 0, 0, &HC0, 0, 0, 0, 0, 0, 0, &H46}

   ''' <summary>
   ''' Reads a Windows Jump List. Automatically handles both
   ''' *.automaticDestinations-ms (OLE compound file) and
   ''' *.customDestinations-ms (concatenated .lnk blobs).
   ''' </summary>
   Public Shared Function ReadJumpList(path As String) As List(Of JumpListEntry)
      If IsCompoundFile(path) Then
         Return ReadAutomaticDestinations(path)
      Else
         Return ReadCustomDestinations(path)
      End If
   End Function

   ' =====================================================================
   '  AutomaticDestinations  (OLE compound file)
   ' =====================================================================

   Private Shared Function ReadAutomaticDestinations(path As String) As List(Of JumpListEntry)
      Dim result As New List(Of JumpListEntry)()

      Using root As RootStorage = RootStorage.OpenRead(path)

         ' Enumerate top-level stream names
         Dim streamNames As New List(Of String)()
         For Each e In root.EnumerateEntries()
            If e.Type = EntryType.Stream Then
               streamNames.Add(e.Name)
            End If
         Next

         Dim destListInfo As New Dictionary(Of String, DateTime?)(StringComparer.OrdinalIgnoreCase)
         Dim destListOrder As New List(Of String)()
         If streamNames.Contains("DestList") Then
            destListInfo = ParseDestList(root, destListOrder)
         End If

         ' Collect the numeric (hex-named) shortcut streams.
         Dim linkStreams As New List(Of String)()
         For Each sName In streamNames
            If sName = "DestList" OrElse sName = "DestListPropertyStore" Then Continue For
            Dim dummy As Long
            If Not TryParseStreamName(sName, dummy) Then Continue For
            linkStreams.Add(sName)
         Next

         ' Present them in DestList (MRU) order when we have it, otherwise by id.
         linkStreams.Sort(Function(a, b) StreamOrderKey(a, destListOrder).CompareTo(StreamOrderKey(b, destListOrder)))

         For Each sName In linkStreams
            Dim lnkBytes = ReadStreamBytes(root, sName)
            If lnkBytes Is Nothing OrElse lnkBytes.Length = 0 Then Continue For

            Dim linkInfo As parseShellLink.ShellLinkInfo
            Try
               linkInfo = parseShellLink.ParseLnkBytes(lnkBytes)
            Catch ex As Exception
               ' A single corrupt stream must not abort the whole jump list.
               Continue For
            End Try

            Dim entry As New JumpListEntry()
            entry.StreamName = sName
            entry.TargetPath = linkInfo.TargetPath
            entry.Arguments = linkInfo.Arguments
            entry.Description = linkInfo.Description
            entry.WorkingDirectory = linkInfo.WorkingDirectory
            entry.IconLocation = linkInfo.IconLocation

            If destListInfo.ContainsKey(sName) AndAlso destListInfo(sName).HasValue Then
               entry.LastAccessTime = destListInfo(sName)
            Else
               entry.LastAccessTime = linkInfo.AccessTime
            End If

            result.Add(entry)
         Next

      End Using

      Return result
   End Function

   ' =====================================================================
   '  CustomDestinations  (header + concatenated .lnk blobs)
   ' =====================================================================

   Private Shared Function ReadCustomDestinations(path As String) As List(Of JumpListEntry)
      Dim result As New List(Of JumpListEntry)()

      Dim data = File.ReadAllBytes(path)
      Dim offsets = FindSignatureOffsets(data, ShellLinkSignature)

      For idx As Integer = 0 To offsets.Count - 1
         Dim startPos As Integer = offsets(idx)
         Dim endPos As Integer = If(idx + 1 < offsets.Count, offsets(idx + 1), data.Length)

         Dim length As Integer = endPos - startPos
         If length <= 0 Then Continue For

         Dim slice(length - 1) As Byte
         Array.Copy(data, startPos, slice, 0, length)

         Dim linkInfo As parseShellLink.ShellLinkInfo
         Try
            linkInfo = parseShellLink.ParseLnkBytes(slice)
         Catch ex As Exception
            Continue For
         End Try

         ' An empty shell link is a task-list separator - nothing to show.
         If String.IsNullOrWhiteSpace(linkInfo.TargetPath) AndAlso
            String.IsNullOrWhiteSpace(linkInfo.Arguments) AndAlso
            String.IsNullOrWhiteSpace(linkInfo.Description) Then
            Continue For
         End If

         Dim entry As New JumpListEntry()
         entry.StreamName = (idx + 1).ToString()
         entry.TargetPath = linkInfo.TargetPath
         entry.Arguments = linkInfo.Arguments
         entry.Description = linkInfo.Description
         entry.WorkingDirectory = linkInfo.WorkingDirectory
         entry.IconLocation = linkInfo.IconLocation
         ' CustomDestinations has no DestList; the shortcut's own timestamps are all we have.
         entry.LastAccessTime = If(linkInfo.WriteTime, linkInfo.AccessTime)

         result.Add(entry)
      Next

      Return result
   End Function

   ' --- Helpers ---

   Private Shared Function IsCompoundFile(path As String) As Boolean
      Try
         Dim sig(CfbMagic.Length - 1) As Byte
         Using fs As New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            If fs.Read(sig, 0, sig.Length) < sig.Length Then Return False
         End Using
         For i As Integer = 0 To CfbMagic.Length - 1
            If sig(i) <> CfbMagic(i) Then Return False
         Next
         Return True
      Catch
         Return False
      End Try
   End Function

   Private Shared Function FindSignatureOffsets(data As Byte(), sig As Byte()) As List(Of Integer)
      Dim hits As New List(Of Integer)()
      If data Is Nothing OrElse sig Is Nothing OrElse sig.Length = 0 Then Return hits

      Dim lastStart As Integer = data.Length - sig.Length
      Dim i As Integer = 0
      While i <= lastStart
         Dim match As Boolean = True
         For j As Integer = 0 To sig.Length - 1
            If data(i + j) <> sig(j) Then
               match = False
               Exit For
            End If
         Next
         If match Then
            hits.Add(i)
            i += sig.Length
         Else
            i += 1
         End If
      End While

      Return hits
   End Function

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

   ' Stream names are the DestList entry id formatted as hexadecimal (1, 2 ... a, b ... 1f).
   Private Shared Function TryParseStreamName(name As String, ByRef value As Long) As Boolean
      Return Long.TryParse(name, NumberStyles.HexNumber, CultureInfo.InvariantCulture, value)
   End Function

   Private Shared Function StreamOrderKey(name As String, destListOrder As List(Of String)) As Integer
      Dim pos As Integer = destListOrder.FindIndex(Function(s) String.Equals(s, name, StringComparison.OrdinalIgnoreCase))
      If pos >= 0 Then Return pos

      ' Not in DestList - push to the end, ordered by numeric id.
      Dim id As Long
      If TryParseStreamName(name, id) Then
         Return destListOrder.Count + CInt(Math.Min(id, Integer.MaxValue - destListOrder.Count - 1))
      End If
      Return Integer.MaxValue
   End Function

   ' --- DestList parsing ---

   Private Shared Function ParseDestList(root As RootStorage, order As List(Of String)) As Dictionary(Of String, DateTime?)
      Dim result As New Dictionary(Of String, DateTime?)(StringComparer.OrdinalIgnoreCase)

      Dim bytes = ReadStreamBytes(root, "DestList")
      If bytes Is Nothing OrElse bytes.Length < 32 Then Return result

      Dim formatVersion As Int32 = BitConverter.ToInt32(bytes, 0)
      Dim entryCount As UInt32 = BitConverter.ToUInt32(bytes, 4)

      Dim offset As Integer = 32 ' header is always 32 bytes

      ' Entry-number is always a 4-byte value at +88; last-access FILETIME is at +100.
      ' Only the fixed-part length (and therefore the path-length field offset) changes
      ' between the Windows 7 (v1) and Windows 8+/10/11 (v3, v4) layouts.
      Dim entryIdOffset As Integer
      Dim filetimeOffset As Integer
      Dim pathLenOffset As Integer
      Dim entryFixedSize As Integer
      Dim trailingPad As Integer

      For i As Integer = 0 To CInt(entryCount) - 1
         If offset >= bytes.Length Then Exit For

         entryIdOffset = offset + 88
         filetimeOffset = offset + 100

         If formatVersion <= 1 Then
            pathLenOffset = offset + 112
            entryFixedSize = 114
            trailingPad = 0
         Else
            pathLenOffset = offset + 128
            entryFixedSize = 130
            trailingPad = 4
         End If

         If pathLenOffset + 2 > bytes.Length Then Exit For
         If filetimeOffset + 8 > bytes.Length Then Exit For

         ' --- Entry ID / stream name (hex, matching the CFB stream names) ---
         Dim streamId As UInt32 = BitConverter.ToUInt32(bytes, entryIdOffset)
         Dim streamName As String = streamId.ToString("x", CultureInfo.InvariantCulture)

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
            order.Add(streamName)
         End If

         ' --- Use the variable-length path string to find the NEXT entry's real offset ---
         Dim pathLenChars As UInt16 = BitConverter.ToUInt16(bytes, pathLenOffset)
         Dim entrySize As Integer = entryFixedSize + (pathLenChars * 2) + trailingPad

         If entrySize <= 0 Then Exit For
         offset += entrySize
      Next

      Return result
   End Function

End Class
