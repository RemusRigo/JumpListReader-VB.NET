<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmJumpListReader
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
      Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmJumpListReader))
      SplitContainer = New SplitContainer()
      lvJLView = New ListView()
      lvDetails = New ListView()
      pnlOptions = New Panel()
      btnBrowseCD = New Button()
      btnBrowseAD = New Button()
      txtBoxCD = New TextBox()
      txtBoxAD = New TextBox()
      chkBoxCD = New CheckBox()
      chkBoxAD = New CheckBox()
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
      SplitContainer.Panel1.Controls.Add(lvJLView)
      ' 
      ' SplitContainer.Panel2
      ' 
      SplitContainer.Panel2.Controls.Add(lvDetails)
      SplitContainer.Size = New Size(1234, 412)
      SplitContainer.SplitterDistance = 625
      SplitContainer.TabIndex = 4
      ' 
      ' lvJLView
      ' 
      lvJLView.Dock = DockStyle.Fill
      lvJLView.FullRowSelect = True
      lvJLView.Location = New Point(0, 0)
      lvJLView.Name = "lvJLView"
      lvJLView.Size = New Size(625, 412)
      lvJLView.TabIndex = 1
      lvJLView.UseCompatibleStateImageBehavior = False
      lvJLView.View = View.Details
      ' 
      ' lvDetails
      ' 
      lvDetails.Dock = DockStyle.Fill
      lvDetails.FullRowSelect = True
      lvDetails.Location = New Point(0, 0)
      lvDetails.Name = "lvDetails"
      lvDetails.Size = New Size(605, 412)
      lvDetails.TabIndex = 0
      lvDetails.UseCompatibleStateImageBehavior = False
      lvDetails.View = View.Details
      ' 
      ' pnlOptions
      ' 
      pnlOptions.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
      pnlOptions.BorderStyle = BorderStyle.FixedSingle
      pnlOptions.Controls.Add(btnBrowseCD)
      pnlOptions.Controls.Add(btnBrowseAD)
      pnlOptions.Controls.Add(txtBoxCD)
      pnlOptions.Controls.Add(txtBoxAD)
      pnlOptions.Controls.Add(chkBoxCD)
      pnlOptions.Controls.Add(chkBoxAD)
      pnlOptions.Controls.Add(btnScan)
      pnlOptions.Location = New Point(0, 416)
      pnlOptions.Name = "pnlOptions"
      pnlOptions.Size = New Size(1234, 65)
      pnlOptions.TabIndex = 5
      ' 
      ' btnBrowseCD
      ' 
      btnBrowseCD.Image = CType(resources.GetObject("btnBrowseCD.Image"), Image)
      btnBrowseCD.Location = New Point(1109, 37)
      btnBrowseCD.Name = "btnBrowseCD"
      btnBrowseCD.Size = New Size(23, 23)
      btnBrowseCD.TabIndex = 10
      btnBrowseCD.UseVisualStyleBackColor = True
      ' 
      ' btnBrowseAD
      ' 
      btnBrowseAD.Image = CType(resources.GetObject("btnBrowseAD.Image"), Image)
      btnBrowseAD.Location = New Point(1109, 7)
      btnBrowseAD.Name = "btnBrowseAD"
      btnBrowseAD.Size = New Size(23, 23)
      btnBrowseAD.TabIndex = 9
      btnBrowseAD.UseVisualStyleBackColor = True
      ' 
      ' txtBoxCD
      ' 
      txtBoxCD.Location = New Point(159, 36)
      txtBoxCD.Name = "txtBoxCD"
      txtBoxCD.Size = New Size(944, 23)
      txtBoxCD.TabIndex = 8
      ' 
      ' txtBoxAD
      ' 
      txtBoxAD.Location = New Point(159, 7)
      txtBoxAD.Name = "txtBoxAD"
      txtBoxAD.Size = New Size(944, 23)
      txtBoxAD.TabIndex = 7
      ' 
      ' chkBoxCD
      ' 
      chkBoxCD.AutoSize = True
      chkBoxCD.Checked = True
      chkBoxCD.CheckState = CheckState.Checked
      chkBoxCD.Location = New Point(3, 40)
      chkBoxCD.Name = "chkBoxCD"
      chkBoxCD.Size = New Size(136, 19)
      chkBoxCD.TabIndex = 6
      chkBoxCD.Text = "Custom Destinations"
      chkBoxCD.UseVisualStyleBackColor = True
      ' 
      ' chkBoxAD
      ' 
      chkBoxAD.AutoSize = True
      chkBoxAD.Checked = True
      chkBoxAD.CheckState = CheckState.Checked
      chkBoxAD.Location = New Point(3, 9)
      chkBoxAD.Name = "chkBoxAD"
      chkBoxAD.Size = New Size(150, 19)
      chkBoxAD.TabIndex = 5
      chkBoxAD.Text = "Automatic Destinations"
      chkBoxAD.UseVisualStyleBackColor = True
      ' 
      ' btnScan
      ' 
      btnScan.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
      btnScan.Location = New Point(1187, 40)
      btnScan.Name = "btnScan"
      btnScan.Size = New Size(42, 20)
      btnScan.TabIndex = 4
      btnScan.Text = "&Scan"
      btnScan.UseVisualStyleBackColor = True
      ' 
      ' frmJumpListReader
      ' 
      AutoScaleDimensions = New SizeF(7F, 15F)
      AutoScaleMode = AutoScaleMode.Font
      ClientSize = New Size(1234, 481)
      Controls.Add(pnlOptions)
      Controls.Add(SplitContainer)
      Icon = CType(resources.GetObject("$this.Icon"), Icon)
      Name = "frmJumpListReader"
      StartPosition = FormStartPosition.CenterScreen
      Text = "Jump List Reader"
      SplitContainer.Panel1.ResumeLayout(False)
      SplitContainer.Panel2.ResumeLayout(False)
      CType(SplitContainer, ComponentModel.ISupportInitialize).EndInit()
      SplitContainer.ResumeLayout(False)
      pnlOptions.ResumeLayout(False)
      pnlOptions.PerformLayout()
      ResumeLayout(False)
   End Sub
   Friend WithEvents SplitContainer As SplitContainer
   Friend WithEvents lvJLView As ListView
   Friend WithEvents lvDetails As ListView
   Friend WithEvents pnlOptions As Panel
   Friend WithEvents txtBoxCD As TextBox
   Friend WithEvents txtBoxAD As TextBox
   Protected WithEvents chkBoxCD As CheckBox
   Protected WithEvents chkBoxAD As CheckBox
   Friend WithEvents btnScan As Button
   Friend WithEvents btnBrowseCD As Button
   Friend WithEvents btnBrowseAD As Button

End Class
