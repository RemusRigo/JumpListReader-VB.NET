'--------------------------------------------------------------------------------------------------
' Messages - Constants for Windows messages
'
'   © 2026 Remus Rigo
'      v1.0.20260820
'--------------------------------------------------------------------------------------------------

Imports System.Runtime.InteropServices

Namespace API
   Module Messages

      '--------------------------------------------------------------------------------------------------
      ' Constants: Window Styles

      ''' <summary>The window has a thin-line border</summary>
      Public Const WS_BORDER As Integer = &H800000

      ''' <summary>The window has a title bar (includes WS_BORDER)</summary>
      Public Const WS_CAPTION As Integer = &HC00000

      ''' <summary> The window is a child window. Cannot be used with WS_POPUP</summary>
      Public Const WS_CHILD As Integer = &H40000000

      ''' <summary>Alias for WS_CHILD.</summary>
      Public Const WS_CHILDWINDOW As Integer = WS_CHILD

      ''' <summary>Excludes child windows from clipping regions.</summary>
      Public Const WS_CLIPCHILDREN As Integer = &H2000000

      ''' <summary>Clips sibling windows during drawing operations.</summary>
      Public Const WS_CLIPSIBLINGS As Integer = &H4000000

      ''' <summary>Disables the window.</summary>
      Public Const WS_DISABLED As Integer = &H8000000

      ''' <summary>The window has a dialog-style frame.</summary>
      Public Const WS_DLGFRAME As Integer = &H400000

      ''' <summary>The window is the first control in a group.</summary>
      Public Const WS_GROUP As Integer = &H20000

      ''' <summary>The window has a horizontal scroll bar.</summary>
      Public Const WS_HSCROLL As Integer = &H100000

      ''' <summary>The window is initially minimized.</summary>
      Public Const WS_ICONIC As Integer = &H20000000

      ''' <summary>The window is initially maximized.</summary>
      Public Const WS_MAXIMIZE As Integer = &H1000000

      ''' <summary>The window has a maximize button.</summary>
      Public Const WS_MAXIMIZEBOX As Integer = &H10000

      ''' <summary>The window is initially minimized.</summary>
      Public Const WS_MINIMIZE As Integer = &H20000000

      ''' <summary>The window has a minimize button.</summary>
      Public Const WS_MINIMIZEBOX As Integer = &H20000

      ''' <summary>A standard overlapped window (default).</summary>
      Public Const WS_OVERLAPPED As Integer = &H0

      ''' <summary>The window is a pop-up window. Cannot be used with WS_CHILD.</summary>
      Public Const WS_POPUP As UInteger = &H80000000UI

      ''' <summary>The window has a sizing border.</summary>
      Public Const WS_SIZEBOX As Integer = &H40000

      ''' <summary>The window has a system menu.</summary>
      Public Const WS_SYSMENU As Integer = &H80000

      ''' <summary>The control can receive keyboard focus via TAB.</summary>
      Public Const WS_TABSTOP As Integer = &H10000

      ''' <summary>Alias for WS_SIZEBOX.</summary>
      Public Const WS_THICKFRAME As Integer = WS_SIZEBOX

      ''' <summary>Alias for WS_OVERLAPPED.</summary>
      Public Const WS_TILED As Integer = WS_OVERLAPPED

      ''' <summary>The window is initially visible.</summary>
      Public Const WS_VISIBLE As Integer = &H10000000

      ''' <summary>The window has a vertical scroll bar.</summary>
      Public Const WS_VSCROLL As Integer = &H200000


      '--------------------------------------------------------------------------------------------------
      ' Constants: Extended Window Styles

      ''' <summary>The window accepts drag-drop files.</summary>
      Public Const WS_EX_ACCEPTFILES As Integer = &H10

      ''' <summary>Forces a top-level window onto the taskbar when the window is visible.</summary>
      Public Const WS_EX_APPWINDOW As Integer = &H40000

      ''' <summary>The window has a border with a sunken edge.</summary>
      Public Const WS_EX_CLIENTEDGE As Integer = &H200

      ''' <summary>Paints all descendants of a window in bottom-to-top painting order using double-buffering.</summary>
      Public Const WS_EX_COMPOSITED As Integer = &H2000000

      ''' <summary>The title bar includes a question mark. Cannot be used with maximize/minimize buttons.</summary>
      Public Const WS_EX_CONTEXTHELP As Integer = &H400

      ''' <summary>The window itself contains child windows that should take part in dialog box navigation.</summary>
      Public Const WS_EX_CONTROLPARENT As Integer = &H10000

      ''' <summary>The window has a double border.</summary>
      Public Const WS_EX_DLGMODALFRAME As Integer = &H1

      ''' <summary>The window is a layered window. Required for transparency/alpha effects.</summary>
      Public Const WS_EX_LAYERED As Integer = &H80000

      ''' <summary>If the shell language is Hebrew/Arabic, the horizontal origin is on the right edge.</summary>
      Public Const WS_EX_LAYOUTRTL As Integer = &H400000

      ''' <summary>The window has generic left-aligned properties. This is the default.</summary>
      Public Const WS_EX_LEFT As Integer = &H0

      ''' <summary>The vertical scroll bar is to the left of the client area (RTL languages).</summary>
      Public Const WS_EX_LEFTSCROLLBAR As Integer = &H4000

      ''' <summary>The window text is displayed using left-to-right reading-order properties.</summary>
      Public Const WS_EX_LTRREADING As Integer = &H0

      ''' <summary>The window is a MDI child window.</summary>
      Public Const WS_EX_MDICHILD As Integer = &H40

      ''' <summary>A top-level window that does not become the foreground window when clicked.</summary>
      Public Const WS_EX_NOACTIVATE As Integer = &H8000000

      ''' <summary>The window does not pass its window layout to its child windows.</summary>
      Public Const WS_EX_NOINHERITLAYOUT As Integer = &H100000

      ''' <summary>The child window does not send the WM_PARENTNOTIFY message to its parent.</summary>
      Public Const WS_EX_NOPARENTNOTIFY As Integer = &H4

      ''' <summary>The window does not render to a redirection surface. Use for Acrylic/Composition.</summary>
      Public Const WS_EX_NOREDIRECTIONBITMAP As Integer = &H200000

      ''' <summary>The window has generic "right-aligned" properties (RTL languages).</summary>
      Public Const WS_EX_RIGHT As Integer = &H1000

      ''' <summary>The vertical scroll bar is to the right of the client area. This is the default.</summary>
      Public Const WS_EX_RIGHTSCROLLBAR As Integer = &H0

      ''' <summary>The window text is displayed using right-to-left reading-order properties.</summary>
      Public Const WS_EX_RTLREADING As Integer = &H2000

      ''' <summary>The window has a 3D border style for items that do not accept user input.</summary>
      Public Const WS_EX_STATICEDGE As Integer = &H20000

      ''' <summary>The window is intended to be used as a floating toolbar (no taskbar icon).</summary>
      Public Const WS_EX_TOOLWINDOW As Integer = &H80

      ''' <summary>The window should be placed above all non-topmost windows.</summary>
      Public Const WS_EX_TOPMOST As Integer = &H8

      ''' <summary>The window should not be painted until siblings beneath it are painted.</summary>
      Public Const WS_EX_TRANSPARENT As Integer = &H20

      ''' <summary>The window has a border with a raised edge.</summary>
      Public Const WS_EX_WINDOWEDGE As Integer = &H3C

      ' --- Combined Styles ---

      ''' <summary>Combines WS_EX_WINDOWEDGE and WS_EX_CLIENTEDGE.</summary>
      Public Const WS_EX_OVERLAPPEDWINDOW As Integer = (WS_EX_WINDOWEDGE Or WS_EX_CLIENTEDGE)

      ''' <summary>Combines WS_EX_WINDOWEDGE, WS_EX_TOOLWINDOW, and WS_EX_TOPMOST.</summary>
      Public Const WS_EX_PALETTEWINDOW As Integer = (WS_EX_WINDOWEDGE Or WS_EX_TOOLWINDOW Or WS_EX_TOPMOST)

      '--------------------------------------------------------------------------------------------------
      ' Window Message Constants

      Public Const WM_SYSCOMMAND As Integer = &H112

      '--------------------------------------------------------------------------------------------------
      ' Menu flags
      Public Const MF_STRING As UInteger = &H0
      Public Const MF_SEPARATOR As UInteger = &H800

      '--------------------------------------------------------------------------------------------------

      Friend Const SWP_NOSIZE As UInteger = &H1
      Friend Const SWP_NOMOVE As UInteger = &H2
      Friend Const SWP_NOACTIVATE As UInteger = &H10
      Friend Const SWP_SHOWWINDOW As UInteger = &H40

   End Module

End Namespace