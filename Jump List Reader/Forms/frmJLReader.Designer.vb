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
      lvJLView = New ListView()
      chkBoxAD = New CheckBox()
      chkBoxCD = New CheckBox()
      btnScan = New Button()
      SuspendLayout()
      ' 
      ' lvJLView
      ' 
      lvJLView.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
      lvJLView.Location = New Point(1, 26)
      lvJLView.Name = "lvJLView"
      lvJLView.Size = New Size(797, 392)
      lvJLView.TabIndex = 0
      lvJLView.UseCompatibleStateImageBehavior = False
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
      btnScan.Location = New Point(756, 427)
      btnScan.Name = "btnScan"
      btnScan.Size = New Size(42, 20)
      btnScan.TabIndex = 3
      btnScan.Text = "&Scan"
      btnScan.UseVisualStyleBackColor = True
      ' 
      ' frmJLReader
      ' 
      AutoScaleDimensions = New SizeF(7F, 15F)
      AutoScaleMode = AutoScaleMode.Font
      ClientSize = New Size(800, 450)
      Controls.Add(btnScan)
      Controls.Add(chkBoxCD)
      Controls.Add(chkBoxAD)
      Controls.Add(lvJLView)
      Name = "frmJLReader"
      Text = "Form1"
      ResumeLayout(False)
      PerformLayout()
   End Sub

   Friend WithEvents lvJLView As ListView
   Protected WithEvents chkBoxAD As CheckBox
   Protected WithEvents chkBoxCD As CheckBox
   Friend WithEvents btnScan As Button

End Class
