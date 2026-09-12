'--------------------------------------------------------------------------------------------------
' Logger class
'    © 2026 Remus Rigo
'       v1.0 2026-06-25
'--------------------------------------------------------------------------------------------------

Imports System.IO
Imports System.Text

Public Enum LogLevel
   Info
   Warning
   [Error]
End Enum

Public Class Logger

   Public Property File As String = "application"
   Public ReadOnly Msg As LogMessage

   Public Sub New(fileName As String)
      Me.File = fileName
      Msg = New LogMessage(Me)
   End Sub

   Friend Sub Write(level As LogLevel, message As String)
      Dim now As DateTime = DateTime.Now
      Dim folder As String = Path.Combine("Logs", now.ToString("yyyy"), now.ToString("MM"), now.ToString("dd"))
      Dim cleanMessage As String = message.Replace(vbCr, " ").Replace(vbLf, " ") ' Remove newlines from message

      If Not Directory.Exists(folder) Then Directory.CreateDirectory(folder)

      Dim filePath As String = Path.Combine(folder, $"{Me.File}.log")
      Dim logLine As String = $"{now:yyyy-MM-dd HH:mm:ss}: {level}: {cleanMessage}"

      SyncLock Me
         IO.File.AppendAllText(filePath, logLine & Environment.NewLine, Encoding.UTF8)
      End SyncLock
   End Sub

End Class

Public Class LogMessage

   Private ReadOnly _parent As Logger

   Public Sub New(parent As Logger)
      _parent = parent
   End Sub

   Public Sub Info(message As String)
      _parent.Write(LogLevel.Info, message)
   End Sub

   Public Sub Warning(message As String)
      _parent.Write(LogLevel.Warning, message)
   End Sub

   Public Sub [Error](message As String)
      _parent.Write(LogLevel.Error, message)
   End Sub

End Class
