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

   Public Function DarkenColor(c As Color, percent As Integer) As Color
      Dim factor As Double = (100 - percent) / 100.0
      Return Color.FromArgb(c.A, CInt(c.R * factor), CInt(c.G * factor), CInt(c.B * factor))
   End Function

   Private Sub frmJLReader_Load(sender As Object, e As EventArgs) Handles MyBase.Load
      Me.Text = appTitle
      lvJLView.View = View.Details
      lvJLView.FullRowSelect = True
      lvJLView.Columns.Add("File Name", 250)
      lvJLView.Columns.Add("Full Path", 500)

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
         item.SubItems.Add(f)
         item.Tag = f   ' store full path for next steps
         If ext = ".automaticdestinations-ms" Then
            item.Group = grpAD
         ElseIf ext = ".customdestinations-ms" Then
            item.Group = grpCD
         End If
         lvJLView.Items.Add(item)

         pbLoad.Value += 1
      Next
      lvJLView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
   End Sub

End Class
