'--------------------------------------------------------------------------------------------------
' Jump List reader
'    © 2026 Remus Rigo
'       v1.0 2026-07-01
'--------------------------------------------------------------------------------------------------

Imports System.IO
Imports System.Runtime
Imports System.Runtime.InteropServices
Imports System.Windows.Forms.Design.AxImporter
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class frmJLReader

   Private pbLoad As rrProgressBar

   Dim log As New Logger(appName)

   Dim pathAD As String = Path.Combine(Environment.GetEnvironmentVariable("appdata"), "Microsoft\Windows\Recent\AutomaticDestinations")
   Dim pathCD As String = Path.Combine(Environment.GetEnvironmentVariable("appdata"), "Microsoft\Windows\Recent\CustomDestinations")

   '===============================================================================================
   Public Function DarkenColor(c As Color, percent As Integer) As Color
      Dim factor As Double = (100 - percent) / 100.0
      Return Color.FromArgb(c.A, CInt(c.R * factor), CInt(c.G * factor), CInt(c.B * factor))
   End Function

   '===============================================================================================
   ' FormatBytes: Converts a byte count into a human-readable string with appropriate units
   Private Function FormatBytes(b As Long) As String
      If b >= 1073741824L Then
         Return String.Format("{0:F2} GB", b / 1073741824.0R)
      ElseIf b >= 1048576L Then
         Return String.Format("{0:F2} MB", b / 1048576.0R)
      ElseIf b >= 1024L Then
         Return String.Format("{0:F1} KB", b / 1024.0R)
      Else
         Return String.Format("{0} B", b)
      End If
   End Function

   '===============================================================================================
   Private Sub frmJLReader_Load(sender As Object, e As EventArgs) Handles MyBase.Load
      Me.Text = appTitle
      lvJLView.View = View.Details
      lvJLView.FullRowSelect = True
      lvJLView.Columns.Add("File Name", 250)
      lvJLView.Columns.Add("App ID", 100)
      lvJLView.Columns.Add("Size", 100)
      lvJLView.Columns.Add("Modified", 150)

      lvDetails.View = View.Details
      lvDetails.FullRowSelect = True
      lvDetails.Columns.Add("#", 30)
      lvDetails.Columns.Add("Name", 100)
      lvDetails.Columns.Add("Path", 300)
      lvDetails.Columns.Add("Arg", 150)
      lvDetails.Columns.Add("Description", 250)
      lvDetails.Columns.Add("Last Access Time", 150)

      pbLoad = New rrProgressBar()
      pbLoad.Dock = DockStyle.None
      pbLoad.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
      pbLoad.Location = New Point(3, Me.ClientSize.Height - pbLoad.Height - 3)
      pbLoad.Size = New Size(Me.ClientSize.Width - btnScan.Width - 6, 20)
      'pbLoad.BarColor = DarkenColor(tvOptions.BackColor, 15)
      'pbLoad.BarColorDone = DarkenColor(tvOptions.BackColor, 30)
      Me.Controls.Add(pbLoad)


   End Sub

   '========================================================================================================================
   Private Sub btnScan_Click(sender As Object, e As EventArgs) Handles btnScan.Click
      lvJLView.Items.Clear()

      Dim files As New List(Of String)

      ' AutomaticDestinations ---------------------------------------------------------------------
      If chkBoxAD.Checked AndAlso Directory.Exists(pathAD) Then
         'lvJLView.Items.Add(Path.GetFileName(file))
         files.AddRange(Directory.GetFiles(pathAD, "*.automaticDestinations-ms"))
      End If

      ' Custom Destinations -----------------------------------------------------------------------
      If chkBoxCD.Checked AndAlso Directory.Exists(pathCD) Then
         files.AddRange(Directory.GetFiles(pathCD, "*.customDestinations-ms"))
      End If

      pbLoad.Maximum = files.Count
      pbLoad.Value = 0
      Dim grpAD = New ListViewGroup("Automatic Destinations")
      lvJLView.Groups.Add(grpAD)
      Dim grpCD = New ListViewGroup("Custom Destinations")
      lvJLView.Groups.Add(grpCD)

      For Each f In files
         Dim name = Path.GetFileName(f)
         Dim ext As String = Path.GetExtension(f).ToLower()

         Dim item As New ListViewItem(name)
         Dim fi As New FileInfo(f)
         'App ID
         item.SubItems.Add(GetAppByID(Path.GetFileNameWithoutExtension(f)))  ' Use the revised function name
         item.SubItems.Add(FormatBytes(fi.Length))
         item.SubItems.Add(fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"))
         item.Tag = f   ' store full path for next steps
         If ext = ".automaticdestinations-ms" Then
            item.Group = grpAD
         ElseIf ext = ".customdestinations-ms" Then
            item.Group = grpCD
         End If
         lvJLView.Items.Add(item)

         pbLoad.Value += 1
      Next
      'lvJLView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)
      lvJLView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
   End Sub

   Private Sub lvJLView_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvJLView.SelectedIndexChanged
      If lvJLView.SelectedItems.Count = 0 Then Exit Sub

      Dim item = lvJLView.SelectedItems(0)
      Dim jlFile As String = item.Tag.ToString()

      lvDetails.Items.Clear()
      Try
         Dim cnt As Integer = 0
         Dim jlEntries = parseJumpList.ReadJumpList(jlFile)
         For Each jl In jlEntries
            cnt += 1
            Dim li As New ListViewItem(cnt.ToString)
            li.SubItems.Add(jl.StreamName)
            li.SubItems.Add(jl.TargetPath)
            li.SubItems.Add(jl.Arguments)
            li.SubItems.Add(jl.Description)
            li.SubItems.Add(If(jl.LastAccessTime.HasValue, jl.LastAccessTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), ""))
            lvDetails.Items.Add(li)
         Next
      Catch ex As Exception
         log.Msg.Error("Error reading Jump List file: " & jlFile & vbCrLf & ex.Message)
      End Try
      lvDetails.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
   End Sub

   Private Sub lvJLView_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles lvJLView.MouseDoubleClick

   End Sub

   Private Sub lvJLView_KeyDown(sender As Object, e As KeyEventArgs) Handles lvJLView.KeyDown
      ' Ctrl+C to copy selected items to clipboard
      If e.Control AndAlso e.KeyCode = Keys.C Then
         If lvJLView.SelectedItems.Count > 0 Then
            Dim sb As New System.Text.StringBuilder()

            For Each item As ListViewItem In lvJLView.SelectedItems
               Dim fields As New List(Of String)()
               For Each sub_ As ListViewItem.ListViewSubItem In item.SubItems
                  fields.Add(sub_.Text)
               Next
               sb.AppendLine(String.Join(vbTab, fields))
            Next

            Try
               Clipboard.SetText(sb.ToString())
            Catch ex As Exception
               MessageBox.Show("Could not copy to clipboard: " & ex.Message)
            End Try
         End If

         e.Handled = True
         e.SuppressKeyPress = True
      End If
   End Sub
End Class
