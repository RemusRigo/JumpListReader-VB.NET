<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSysUnveiler
   Inherits System.Windows.Forms.Form

   'Form overrides dispose to clean up the component list.
   <System.Diagnostics.DebuggerNonUserCode()> _
   Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
   <System.Diagnostics.DebuggerStepThrough()> _
   Private Sub InitializeComponent()
      lstBoxActions = New ListBox()
      SuspendLayout()
      ' 
      ' lstBoxActions
      ' 
      lstBoxActions.BackColor = SystemColors.ActiveBorder
      lstBoxActions.Dock = DockStyle.Fill
      lstBoxActions.Font = New Font("Segoe UI", 9F, FontStyle.Bold)
      lstBoxActions.ForeColor = SystemColors.ActiveCaptionText
      lstBoxActions.FormattingEnabled = True
      lstBoxActions.Location = New Point(0, 0)
      lstBoxActions.Name = "lstBoxActions"
      lstBoxActions.Size = New Size(284, 386)
      lstBoxActions.TabIndex = 0
      ' 
      ' frmSysUnveiler
      ' 
      AutoScaleDimensions = New SizeF(7F, 15F)
      AutoScaleMode = AutoScaleMode.Font
      ClientSize = New Size(284, 386)
      Controls.Add(lstBoxActions)
      MaximizeBox = False
      MinimizeBox = False
      Name = "frmSysUnveiler"
      StartPosition = FormStartPosition.Manual
      Text = "SysUnveiler"
      ResumeLayout(False)
   End Sub

   Friend WithEvents lstBoxActions As ListBox
End Class
