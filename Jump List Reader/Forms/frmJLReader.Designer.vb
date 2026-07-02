<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmJLReader
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
      chkBoxAD = New CheckBox()
      chkBoxCD = New CheckBox()
      btnScan = New Button()
      SplitContainer = New SplitContainer()
      lvJLView = New ListView()
      CType(SplitContainer, ComponentModel.ISupportInitialize).BeginInit()
      SplitContainer.Panel1.SuspendLayout()
      SplitContainer.SuspendLayout()
      SuspendLayout()
      ' 
      ' chkBoxAD
      ' 
      chkBoxAD.AutoSize = True
      chkBoxAD.Location = New Point(1, 1)
      chkBoxAD.Name = "chkBoxAD"
      chkBoxAD.Size = New Size(150, 19)
      chkBoxAD.TabIndex = 1
      chkBoxAD.Text = "Automatic Destinations"
      chkBoxAD.UseVisualStyleBackColor = True
      ' 
      ' chkBoxCD
      ' 
      chkBoxCD.AutoSize = True
      chkBoxCD.Location = New Point(160, 1)
      chkBoxCD.Name = "chkBoxCD"
      chkBoxCD.Size = New Size(136, 19)
      chkBoxCD.TabIndex = 2
      chkBoxCD.Text = "Custom Destinations"
      chkBoxCD.UseVisualStyleBackColor = True
      ' 
      ' btnScan
      ' 
      btnScan.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
      btnScan.Location = New Point(988, 428)
      btnScan.Name = "btnScan"
      btnScan.Size = New Size(42, 20)
      btnScan.TabIndex = 3
      btnScan.Text = "&Scan"
      btnScan.UseVisualStyleBackColor = True
      ' 
      ' SplitContainer
      ' 
      SplitContainer.Location = New Point(1, 20)
      SplitContainer.Name = "SplitContainer"
      ' 
      ' SplitContainer.Panel1
      ' 
      SplitContainer.Panel1.Controls.Add(lvJLView)
      SplitContainer.Size = New Size(1027, 404)
      SplitContainer.SplitterDistance = 580
      SplitContainer.TabIndex = 4
      ' 
      ' lvJLView
      ' 
      lvJLView.Dock = DockStyle.Fill
      lvJLView.Location = New Point(0, 0)
      lvJLView.Name = "lvJLView"
      lvJLView.Size = New Size(580, 404)
      lvJLView.TabIndex = 1
      lvJLView.UseCompatibleStateImageBehavior = False
      ' 
      ' frmJLReader
      ' 
      AutoScaleDimensions = New SizeF(7F, 15F)
      AutoScaleMode = AutoScaleMode.Font
      ClientSize = New Size(1032, 451)
      Controls.Add(SplitContainer)
      Controls.Add(btnScan)
      Controls.Add(chkBoxCD)
      Controls.Add(chkBoxAD)
      Name = "frmJLReader"
      Text = "Jump List Reader"
      SplitContainer.Panel1.ResumeLayout(False)
      CType(SplitContainer, ComponentModel.ISupportInitialize).EndInit()
      SplitContainer.ResumeLayout(False)
      ResumeLayout(False)
      PerformLayout()
   End Sub
   Protected WithEvents chkBoxAD As CheckBox
   Protected WithEvents chkBoxCD As CheckBox
   Friend WithEvents btnScan As Button
   Friend WithEvents SplitContainer As SplitContainer
   Friend WithEvents lvJLView As ListView

End Class
