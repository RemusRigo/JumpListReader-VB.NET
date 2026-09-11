Imports System
Imports System.IO
Imports System.Collections.Generic
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

   ' Single-byte fallback encoding for the (rare) ANSI strings in legacy shortcuts.
   ' Latin-1 maps every byte 1:1 to U+0000-U+00FF, so it never loses data the way
   ' UTF-8 (which Encoding.Default now is) does.
   Private Shared ReadOnly AnsiFallback As Encoding = Encoding.Latin1

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
      Dim pidlPath As String = Nothing
      If info.Flags.HasFlag(LinkFlags.HasLinkTargetIDList) Then
         Dim idListSize As UInt16 = BitConverter.ToUInt16(data, offset)
         offset += 2

         Try
            pidlPath = ParsePidlPath(data, offset, idListSize)
         Catch
            pidlPath = Nothing
         End Try

         ' IDList is a sequence of ITEMID structures ending with a 0x0000 terminator.
         offset += idListSize
      End If

      ' --- LinkInfo (optional) ---
      If info.Flags.HasFlag(LinkFlags.HasLinkInfo) Then
         Dim linkInfoSize As UInt32 = BitConverter.ToUInt32(data, offset)
         Dim linkInfoOffset As Integer = offset
         offset += CInt(linkInfoSize)

         Dim localPath = ParseLinkInfo_LocalPath(data, linkInfoOffset)
         If Not String.IsNullOrEmpty(localPath) Then
            info.TargetPath = localPath
         End If
      End If

      ' Fall back to the shell item id list (URLs, UWP apps, virtual locations).
      If String.IsNullOrEmpty(info.TargetPath) AndAlso Not String.IsNullOrEmpty(pidlPath) Then
         info.TargetPath = pidlPath
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
      Try
         Return DateTime.FromFileTimeUtc(fileTime).ToLocalTime()
      Catch
         Return Nothing
      End Try
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
         s = AnsiFallback.GetString(data, offset, byteCount)
      End If

      offset += byteCount
      Return s.TrimEnd(ChrW(0))
   End Function

   ' =====================================================================
   '  LinkInfo - local base path (prefers the Unicode copy when present)
   ' =====================================================================

   Private Shared Function ParseLinkInfo_LocalPath(data As Byte(), offset As Integer) As String
      Try
         Dim linkInfoHeaderSize As UInt32 = BitConverter.ToUInt32(data, offset + 4)

         Dim localBasePathOffset As UInt32 = BitConverter.ToUInt32(data, offset + 16)
         Dim commonPathSuffixOffset As UInt32 = BitConverter.ToUInt32(data, offset + 24)

         ' The optional Unicode offsets are only present with an extended header.
         Dim hasUnicode As Boolean = linkInfoHeaderSize >= &H24UI
         Dim localBasePathOffsetUnicode As UInt32 = 0
         Dim commonPathSuffixOffsetUnicode As UInt32 = 0
         If hasUnicode Then
            localBasePathOffsetUnicode = BitConverter.ToUInt32(data, offset + 28)
            commonPathSuffixOffsetUnicode = BitConverter.ToUInt32(data, offset + 32)
         End If

         Dim localBasePath As String = Nothing
         If hasUnicode AndAlso localBasePathOffsetUnicode <> 0UI Then
            localBasePath = ReadNullTerminatedUnicode(data, offset + CInt(localBasePathOffsetUnicode))
         ElseIf localBasePathOffset <> 0UI Then
            localBasePath = ReadNullTerminatedAnsi(data, offset + CInt(localBasePathOffset))
         End If

         Dim commonSuffix As String = Nothing
         If hasUnicode AndAlso localBasePathOffsetUnicode <> 0UI Then
            If commonPathSuffixOffsetUnicode <> 0UI Then
               commonSuffix = ReadNullTerminatedUnicode(data, offset + CInt(commonPathSuffixOffsetUnicode))
            End If
         ElseIf commonPathSuffixOffset <> 0UI Then
            commonSuffix = ReadNullTerminatedAnsi(data, offset + CInt(commonPathSuffixOffset))
         End If

         If String.IsNullOrEmpty(localBasePath) Then Return Nothing

         If String.IsNullOrEmpty(commonSuffix) Then Return localBasePath

         If localBasePath.EndsWith("\"c) OrElse commonSuffix.StartsWith("\"c) Then
            Return localBasePath & commonSuffix
         End If
         Return localBasePath & "\" & commonSuffix
      Catch
         Return Nothing
      End Try
   End Function

   Private Shared Function ReadNullTerminatedAnsi(data As Byte(), offset As Integer) As String
      If offset < 0 OrElse offset >= data.Length Then Return String.Empty
      Dim start = offset
      While offset < data.Length AndAlso data(offset) <> 0
         offset += 1
      End While
      Dim len = offset - start
      If len <= 0 Then Return String.Empty
      Return AnsiFallback.GetString(data, start, len)
   End Function

   Private Shared Function ReadNullTerminatedUnicode(data As Byte(), offset As Integer) As String
      If offset < 0 OrElse offset + 1 >= data.Length Then Return String.Empty
      Dim start = offset
      While offset + 1 < data.Length AndAlso Not (data(offset) = 0 AndAlso data(offset + 1) = 0)
         offset += 2
      End While
      Dim len = offset - start
      If len <= 0 Then Return String.Empty
      Return Encoding.Unicode.GetString(data, start, len)
   End Function

   ' =====================================================================
   '  LinkTargetIDList (PIDL) - best-effort path / URL extraction
   ' =====================================================================

   Private Shared ReadOnly Beef0004Sig As Byte() = New Byte() {&H4, &H0, &HEF, &HBE}

   Private Shared ReadOnly InvalidNameChars As Char() =
      New Char() {"<"c, ">"c, ":"c, """"c, "/"c, "|"c, "?"c, "*"c, "\"c, Chr(0)}

   Private Shared Function ParsePidlPath(data As Byte(), start As Integer, idListSize As Integer) As String
      Dim segments As New List(Of String)()
      Dim drive As String = Nothing

      Dim [end] As Integer = Math.Min(data.Length, start + idListSize)
      Dim pos As Integer = start

      While pos + 2 <= [end]
         Dim itemSize As Integer = BitConverter.ToUInt16(data, pos)
         If itemSize < 3 Then Exit While                 ' 0x0000 terminator or corrupt
         If pos + itemSize > [end] Then Exit While

         Dim classType As Byte = data(pos + 2)

         If classType = &H61 Then
            ' URI shell item - the whole target is a URL / protocol string.
            Dim uri = ReadUriItem(data, pos, itemSize)
            If Not String.IsNullOrEmpty(uri) Then Return uri

         ElseIf (classType And &H70) = &H20 Then
            ' Volume / drive item: ANSI drive string right after the class byte.
            Dim d = ReadNullTerminatedAnsi(data, pos + 3)
            If d.Length >= 2 AndAlso d.Length <= 4 AndAlso Char.IsLetter(d(0)) AndAlso d(1) = ":"c Then
               drive = d.TrimEnd("\"c)
            End If

         ElseIf (classType And &H70) = &H30 Then
            ' File / folder entry.
            Dim nm = ReadFileEntryName(data, pos, itemSize)
            If Not String.IsNullOrEmpty(nm) Then segments.Add(nm)
         End If
         ' classType &H1F (root / GUID folders) and anything else contribute no path text.

         pos += itemSize
      End While

      If segments.Count = 0 AndAlso String.IsNullOrEmpty(drive) Then Return Nothing

      Dim sb As New StringBuilder()
      If Not String.IsNullOrEmpty(drive) Then
         sb.Append(drive).Append("\"c)
      End If
      sb.Append(String.Join("\", segments))
      Return sb.ToString()
   End Function

   Private Shared Function ReadUriItem(data As Byte(), itemStart As Integer, itemSize As Integer) As String
      ' [size:2][0x61][flags:1][unknown:4][uri string...]
      Dim strStart As Integer = itemStart + 8
      Dim limit As Integer = Math.Min(data.Length, itemStart + itemSize)
      If strStart >= limit Then Return Nothing

      Dim value As String
      If strStart + 1 < limit AndAlso data(strStart) <> 0 AndAlso data(strStart + 1) = 0 Then
         value = ReadNullTerminatedUnicode(data, strStart)
      Else
         value = ReadNullTerminatedAnsi(data, strStart)
      End If

      value = value.Trim()
      If value.Length < 3 Then Return Nothing
      Return value
   End Function

   Private Shared Function ReadFileEntryName(data As Byte(), itemStart As Integer, itemSize As Integer) As String
      Dim itemEnd As Integer = Math.Min(data.Length, itemStart + itemSize)

      ' The full long name lives in the BEEF0004 extension block as a UTF-16 string.
      Dim sigPos As Integer = IndexOfBytes(data, Beef0004Sig, itemStart, itemEnd)
      If sigPos >= itemStart + 4 Then
         Dim blockStart As Integer = sigPos - 4
         Dim blockSize As Integer = BitConverter.ToUInt16(data, blockStart)
         Dim blockEnd As Integer = Math.Min(itemEnd, blockStart + Math.Max(blockSize, 10))

         ' Version-independent: take the longest plausible UTF-16 name in the block.
         Dim best As String = Nothing
         Dim s As Integer = blockStart + 8
         While s + 3 < blockEnd
            Dim candidate = ReadNullTerminatedUnicode(data, s)
            If IsPlausibleName(candidate) Then
               If best Is Nothing OrElse candidate.Length > best.Length Then best = candidate
               s += (candidate.Length + 1) * 2
            Else
               s += 2
            End If
         End While
         If best IsNot Nothing Then Return best
      End If

      ' Fall back to the primary (short) name in the fixed part of the item.
      Dim primaryAnsi = ReadNullTerminatedAnsi(data, itemStart + 14)
      If IsPlausibleName(primaryAnsi) Then Return primaryAnsi

      Dim primaryUnicode = ReadNullTerminatedUnicode(data, itemStart + 14)
      If IsPlausibleName(primaryUnicode) Then Return primaryUnicode

      Return Nothing
   End Function

   Private Shared Function IsPlausibleName(s As String) As Boolean
      If String.IsNullOrEmpty(s) OrElse s.Length > 255 Then Return False

      Dim hasReal As Boolean = False
      For Each c In s
         If c < " "c Then Return False
         If Array.IndexOf(InvalidNameChars, c) >= 0 Then Return False
         ' Reject the private-use / half-surrogate junk produced by a misaligned read.
         If c >= ChrW(&HD800) AndAlso c <= ChrW(&HF8FF) Then Return False
         If Not Char.IsWhiteSpace(c) Then hasReal = True
      Next
      Return hasReal
   End Function

   Private Shared Function IndexOfBytes(data As Byte(), pattern As Byte(), fromIndex As Integer, toIndex As Integer) As Integer
      Dim last As Integer = Math.Min(toIndex, data.Length) - pattern.Length
      For i As Integer = Math.Max(0, fromIndex) To last
         Dim ok As Boolean = True
         For j As Integer = 0 To pattern.Length - 1
            If data(i + j) <> pattern(j) Then
               ok = False
               Exit For
            End If
         Next
         If ok Then Return i
      Next
      Return -1
   End Function

End Class
