'--------------------------------------------------------------------------------------------------
' System Unveiler
'    © 2026 Remus Rigo
'       v1.1.20260911
'--------------------------------------------------------------------------------------------------

Imports SysUnveiler.API

Public Class frmSysUnveiler

   Private Const SYSMENU_ABOUT_ID As UInteger = 1000

   '===============================================================================================
   Protected Overrides Sub OnHandleCreated(e As EventArgs)
      MyBase.OnHandleCreated(e)
      Dim hSysMenu As IntPtr = GetSystemMenu(Me.Handle, False)
      ' Add a separator and then your custom item
      AppendMenu(hSysMenu, MF_SEPARATOR, 0, String.Empty)
      AppendMenu(hSysMenu, MF_STRING, SYSMENU_ABOUT_ID, "About...")
   End Sub

   '===============================================================================================
   Protected Overrides Sub WndProc(ByRef m As Message)
      MyBase.WndProc(m)
      If m.Msg = WM_SYSCOMMAND Then
         If CUInt(m.WParam) = SYSMENU_ABOUT_ID Then
            frmAbout.ShowDialog()
         End If
      End If
   End Sub

   '===============================================================================================
   Private Sub frmSysUnveiler_Load(sender As Object, e As EventArgs) Handles MyBase.Load
      Me.Text = appTitle
      Me.Location = New Point(13, 13)
      lstBoxActions.Items.Clear()
      lstBoxActions.Items.Add("Jump Lists")
   End Sub

   '===============================================================================================
   Private Sub lstBoxActions_DoubleClick(sender As Object, e As EventArgs) Handles lstBoxActions.DoubleClick
      Select Case lstBoxActions.SelectedItem.ToString()
         Case "Jump Lists"
            frmJLReader.Show()
      End Select
   End Sub

End Class