Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Public Class rrProgressBar
   Inherits Control

   Private _value As Integer = 0
   Private _maximum As Integer = 100

   <Category("Behavior")>
   <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
   Public Property Maximum As Integer
      Get
         Return _maximum
      End Get
      Set(value As Integer)
         If value < 1 Then value = 1
         _maximum = value
         If _value > value Then _value = value
         Me.Invalidate()
      End Set
   End Property

   <Category("Behavior")>
   <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
   Public Property Value As Integer
      Get
         Return _value
      End Get
      Set(value As Integer)
         If value < 0 Then value = 0
         If value > _maximum Then value = _maximum
         _value = value
         Me.Invalidate()
      End Set
   End Property

   <Category("Appearance")>
   <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
   Public Property BarColor As Color = ColorTranslator.FromWin32(&HD07800)

   <Category("Appearance")>
   <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
   Public Property BarColorDone As Color = ColorTranslator.FromWin32(&H50B000)

   <Category("Appearance")>
   <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
   Public Property BorderColor As Color = ColorTranslator.FromWin32(&HAAAAAA)

   <Category("Appearance")>
   <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
   Public Property BackgroundColor As Color = SystemColors.Window

   Public Sub New()
      Me.DoubleBuffered = True
      Me.SetStyle(ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw, True)
      Me.Height = 20
   End Sub

   Protected Overrides Sub OnPaint(e As PaintEventArgs)
      MyBase.OnPaint(e)

      Dim g = e.Graphics
      Dim rect = Me.ClientRectangle

      ' Background
      Using bg As New SolidBrush(BackgroundColor)
         g.FillRectangle(bg, rect)
      End Using

      ' Progress fill
      If _value > 0 Then
         Dim fillWidth = CInt(rect.Width * (_value / _maximum))
         Dim fillRect = New Rectangle(rect.Left, rect.Top, fillWidth, rect.Height)

         Dim fillColor = If(_value >= _maximum, BarColorDone, BarColor)

         Using br As New SolidBrush(fillColor)
            g.FillRectangle(br, fillRect)
         End Using
      End If

      ' Border
      Using pen As New Pen(BorderColor)
         g.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1)
      End Using

      ' Percentage text
      Dim percentText = CInt((_value / _maximum) * 100) & "%"
      TextRenderer.DrawText(g, percentText, Me.Font, rect, Color.Black,
                              TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
   End Sub

End Class

