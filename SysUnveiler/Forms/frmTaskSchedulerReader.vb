'--------------------------------------------------------------------------------------------------
' Task Scheduler Reader
'    © 2026 Remus Rigo
'       v1.0.20260911
'--------------------------------------------------------------------------------------------------

Imports System.IO
Imports System.Linq

Public Class frmTaskSchedulerReader

   Private tasksPath As String

   '===============================================================================================
   Private Sub LV_AddItem(name As String, value As String)
      Dim li As New ListViewItem(name)
      li.SubItems.Add(value)
      lvDetails.Items.Add(li)
   End Sub

   '===============================================================================================
   Private Sub frmTaskReader_Load(sender As Object, e As EventArgs) Handles MyBase.Load
      Me.Text = "Task Scheduler Reader v1.0.20260911"

      lvTasks.View = View.Details
      lvTasks.FullRowSelect = True
      lvTasks.Columns.Add("Task", 220)
      lvTasks.Columns.Add("Folder", 260)
      lvTasks.Columns.Add("Author", 150)
      lvTasks.Columns.Add("Enabled", 70)
      lvTasks.Columns.Add("Run As", 150)

      lvDetails.View = View.Details
      lvDetails.FullRowSelect = True
      lvDetails.Columns.Add("Property", 160)
      lvDetails.Columns.Add("Value", 420)

      txtBoxPath.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks")
   End Sub

   '===============================================================================================
   Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
      Using dlgFolderBrowser As New FolderBrowserDialog
         If Directory.Exists(txtBoxPath.Text) Then dlgFolderBrowser.SelectedPath = txtBoxPath.Text
         If dlgFolderBrowser.ShowDialog = DialogResult.OK Then
            txtBoxPath.Text = dlgFolderBrowser.SelectedPath
         End If
      End Using
   End Sub

   '===============================================================================================
   Private Sub btnScan_Click(sender As Object, e As EventArgs) Handles btnScan.Click
      lvTasks.Items.Clear()
      lvDetails.Items.Clear()

      If Not Directory.Exists(txtBoxPath.Text) Then
         MessageBox.Show("Folder not found:" & vbCrLf & txtBoxPath.Text, appTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
         Exit Sub
      End If

      tasksPath = txtBoxPath.Text

      Try
         Directory.GetFiles(tasksPath)
      Catch ex As UnauthorizedAccessException
         MessageBox.Show("Access to '" & tasksPath & "' was denied." & vbCrLf & vbCrLf &
                          "This folder can only be listed by an elevated (Administrator) process. Re-launch TSJob as Administrator to see all scheduled tasks.",
                          appTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
         Exit Sub
      End Try

      Dim scanErrors As New List(Of String)
      Dim files = parseTaskXml.SafeEnumerateFiles(tasksPath, scanErrors)

      For Each f In files
         Dim item As New ListViewItem(Path.GetFileName(f))
         Try
            Dim task = parseTaskXml.ReadTask(f, tasksPath)
            item.SubItems.Add(Path.GetDirectoryName(task.TaskPath))
            item.SubItems.Add(task.Author)
            item.SubItems.Add(If(task.Enabled, "Yes", "No"))
            item.SubItems.Add(task.PrincipalId)
            item.Tag = task
         Catch ex As Exception
            item.SubItems.Add("")
            item.SubItems.Add("")
            item.SubItems.Add("")
            item.SubItems.Add("Parse error: " & ex.Message)
         End Try
         lvTasks.Items.Add(item)
      Next

      lvTasks.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)

      If files.Count = 0 AndAlso scanErrors.Count > 0 Then
         Dim preview = String.Join(vbCrLf, scanErrors.Take(15))
         Dim more = If(scanErrors.Count > 15, vbCrLf & $"... and {scanErrors.Count - 15} more", "")
         MessageBox.Show($"No tasks were found, but {scanErrors.Count} folder(s) could not be scanned:" & vbCrLf & vbCrLf & preview & more,
                          appTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
      End If
   End Sub

   '===============================================================================================
   Private Sub lvTasks_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvTasks.SelectedIndexChanged
      lvDetails.Items.Clear()
      If lvTasks.SelectedItems.Count = 0 Then Exit Sub

      Dim task = TryCast(lvTasks.SelectedItems(0).Tag, ScheduledTask)
      If task Is Nothing Then Exit Sub

      LV_AddItem("Task Path", task.TaskPath)
      LV_AddItem("URI", task.Uri)
      LV_AddItem("Author", task.Author)
      LV_AddItem("Description", task.Description)
      LV_AddItem("Registration Date", task.RegistrationDate)
      If Not String.IsNullOrEmpty(task.Documentation) Then LV_AddItem("Documentation", task.Documentation)
      LV_AddItem("Enabled", If(task.Enabled, "Yes", "No"))
      LV_AddItem("Hidden", If(task.Hidden, "Yes", "No"))
      LV_AddItem("Run As", task.PrincipalId)
      LV_AddItem("Logon Type", task.LogonType)
      LV_AddItem("Run Level", task.RunLevel)
      LV_AddItem("Multiple Instances", task.MultipleInstancesPolicy)
      LV_AddItem("Priority", task.Priority)
      LV_AddItem("Execution Time Limit", task.ExecutionTimeLimit)
      LV_AddItem("Allow Start On Demand", task.AllowStartOnDemand)
      LV_AddItem("Start When Available", task.StartWhenAvailable)
      LV_AddItem("Disallow If On Batteries", task.DisallowStartIfOnBatteries)

      Dim n As Integer = 0
      For Each a In task.Actions
         n += 1
         LV_AddItem($"Action {n} ({a.Type})", a.Command)
         If Not String.IsNullOrEmpty(a.Arguments) Then LV_AddItem("  Arguments", a.Arguments)
         If Not String.IsNullOrEmpty(a.WorkingDirectory) Then LV_AddItem("  Working Directory", a.WorkingDirectory)
      Next

      n = 0
      For Each t In task.Triggers
         n += 1
         LV_AddItem($"Trigger {n} ({t.Type})", t.Description)
         If Not t.Enabled Then LV_AddItem("  Trigger Enabled", "No")
         If Not String.IsNullOrEmpty(t.EndBoundary) Then LV_AddItem("  End Boundary", t.EndBoundary)
      Next
   End Sub

End Class
