<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmTaskSchedulerReader
   Inherits System.Windows.Forms.Form

   'Form overrides dispose to clean up the component list.
   <System.Diagnostics.DebuggerNonUserCode()>
   Protected Overrides Sub Dispose(disposing As Boolean)
      Try
         If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
         End If
      Finally
         MyBase.Dispose(disposing)
      End Try
   End Sub

   'Required by the Windows Form Designer
   Private components As System.ComponentModel.IContainer

   'NOTE: The following procedure is required by the Windows Form Designer
   'It can be modified using the Windows Form Designer.
   'Do not modify it using the code editor.
   <System.Diagnostics.DebuggerStepThrough()>
   Private Sub InitializeComponent()
      Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmTaskSchedulerReader))
      SplitContainer = New SplitContainer()
      lvTasks = New ListView()
      lvDetails = New ListView()
      pnlOptions = New Panel()
      btnBrowse = New Button()
      txtBoxPath = New TextBox()
      lblPath = New Label()
      btnScan = New Button()
      CType(SplitContainer, ComponentModel.ISupportInitialize).BeginInit()
      SplitContainer.Panel1.SuspendLayout()
      SplitContainer.Panel2.SuspendLayout()
      SplitContainer.SuspendLayout()
      pnlOptions.SuspendLayout()
      SuspendLayout()
      ' 
      ' SplitContainer
      ' 
      SplitContainer.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
      SplitContainer.FixedPanel = FixedPanel.Panel1
      SplitContainer.Location = New Point(0, 0)
      SplitContainer.Name = "SplitContainer"
      ' 
      ' SplitContainer.Panel1
      ' 
      SplitContainer.Panel1.Controls.Add(lvTasks)
      ' 
      ' SplitContainer.Panel2
      ' 
      SplitContainer.Panel2.Controls.Add(lvDetails)
      SplitContainer.Size = New Size(1177, 424)
      SplitContainer.SplitterDistance = 580
      SplitContainer.TabIndex = 4
      ' 
      ' lvTasks
      ' 
      lvTasks.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
      lvTasks.FullRowSelect = True
      lvTasks.Location = New Point(0, 0)
      lvTasks.Name = "lvTasks"
      lvTasks.Size = New Size(580, 424)
      lvTasks.TabIndex = 0
      lvTasks.UseCompatibleStateImageBehavior = False
      lvTasks.View = View.Details
      ' 
      ' lvDetails
      ' 
      lvDetails.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
      lvDetails.FullRowSelect = True
      lvDetails.Location = New Point(0, 0)
      lvDetails.Name = "lvDetails"
      lvDetails.Size = New Size(593, 424)
      lvDetails.TabIndex = 0
      lvDetails.UseCompatibleStateImageBehavior = False
      lvDetails.View = View.Details
      ' 
      ' pnlOptions
      ' 
      pnlOptions.Controls.Add(btnBrowse)
      pnlOptions.Controls.Add(txtBoxPath)
      pnlOptions.Controls.Add(lblPath)
      pnlOptions.Controls.Add(btnScan)
      pnlOptions.Dock = DockStyle.Bottom
      pnlOptions.Location = New Point(0, 430)
      pnlOptions.Name = "pnlOptions"
      pnlOptions.Size = New Size(1177, 51)
      pnlOptions.TabIndex = 5
      ' 
      ' btnBrowse
      ' 
      btnBrowse.Anchor = AnchorStyles.Top Or AnchorStyles.Right
      btnBrowse.Image = CType(resources.GetObject("btnBrowse.Image"), Image)
      btnBrowse.Location = New Point(1029, 21)
      btnBrowse.Name = "btnBrowse"
      btnBrowse.Size = New Size(23, 23)
      btnBrowse.TabIndex = 11
      btnBrowse.UseVisualStyleBackColor = True
      ' 
      ' txtBoxPath
      ' 
      txtBoxPath.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
      txtBoxPath.Location = New Point(43, 21)
      txtBoxPath.Name = "txtBoxPath"
      txtBoxPath.Size = New Size(980, 23)
      txtBoxPath.TabIndex = 6
      ' 
      ' lblPath
      ' 
      lblPath.AutoSize = True
      lblPath.Location = New Point(3, 29)
      lblPath.Name = "lblPath"
      lblPath.Size = New Size(34, 15)
      lblPath.TabIndex = 5
      lblPath.Text = "Path:"
      ' 
      ' btnScan
      ' 
      btnScan.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
      btnScan.Location = New Point(1130, 24)
      btnScan.Name = "btnScan"
      btnScan.Size = New Size(42, 24)
      btnScan.TabIndex = 4
      btnScan.Text = "&Scan"
      btnScan.UseVisualStyleBackColor = True
      ' 
      ' frmTaskSchedulerReader
      ' 
      AutoScaleDimensions = New SizeF(7F, 15F)
      AutoScaleMode = AutoScaleMode.Font
      ClientSize = New Size(1177, 481)
      Controls.Add(pnlOptions)
      Controls.Add(SplitContainer)
      Name = "frmTaskSchedulerReader"
      StartPosition = FormStartPosition.CenterScreen
      Text = "Task Scheduler Reader"
      SplitContainer.Panel1.ResumeLayout(False)
      SplitContainer.Panel2.ResumeLayout(False)
      CType(SplitContainer, ComponentModel.ISupportInitialize).EndInit()
      SplitContainer.ResumeLayout(False)
      pnlOptions.ResumeLayout(False)
      pnlOptions.PerformLayout()
      ResumeLayout(False)
   End Sub
   Friend WithEvents SplitContainer As SplitContainer
   Friend WithEvents lvTasks As ListView
   Friend WithEvents lvDetails As ListView
   Friend WithEvents pnlOptions As Panel
   Friend WithEvents txtBoxPath As TextBox
   Friend WithEvents lblPath As Label
   Friend WithEvents btnScan As Button
   Friend WithEvents btnBrowse As Button

End Class
