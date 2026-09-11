'--------------------------------------------------------------------------------------------------
' parseTaskXml - Task Scheduler task parser
'    © 2026 Remus Rigo
'       v1.0.20260911
'--------------------------------------------------------------------------------------------------

Imports System.IO
Imports System.Xml.Linq
Imports System.Linq

Public Class TaskAction
   Public Property Type As String
   Public Property Command As String
   Public Property Arguments As String
   Public Property WorkingDirectory As String
End Class

Public Class TaskTriggerXml
   Public Property Type As String
   Public Property Enabled As Boolean = True
   Public Property StartBoundary As String
   Public Property EndBoundary As String
   Public Property Description As String
End Class

Public Class ScheduledTask
   Public Property FilePath As String
   Public Property TaskPath As String
   Public Property Uri As String
   Public Property Author As String
   Public Property Description As String
   Public Property RegistrationDate As String
   Public Property Documentation As String
   Public Property Enabled As Boolean = True
   Public Property Hidden As Boolean = False
   Public Property PrincipalId As String
   Public Property LogonType As String
   Public Property RunLevel As String
   Public Property MultipleInstancesPolicy As String
   Public Property ExecutionTimeLimit As String
   Public Property Priority As String
   Public Property AllowStartOnDemand As String
   Public Property StartWhenAvailable As String
   Public Property DisallowStartIfOnBatteries As String
   Public Property Actions As New List(Of TaskAction)
   Public Property Triggers As New List(Of TaskTriggerXml)
End Class

Public Class parseTaskXml

   Private Shared ReadOnly ns As XNamespace = "http://schemas.microsoft.com/windows/2004/02/mit/task"

   '===============================================================================================
   ' SafeEnumerateFiles: recursive file scan that skips folders it cannot access instead of aborting.
   ' errors, when supplied, receives one "folder: exception message" entry per skipped folder so the
   ' caller can tell "nothing found" apart from "everything was inaccessible" (e.g. explicit Deny ACEs
   ' on individual Task Scheduler subfolders that still block an elevated Administrators token).
   Public Shared Function SafeEnumerateFiles(root As String, Optional errors As List(Of String) = Nothing) As List(Of String)
      Dim result As New List(Of String)
      Dim stack As New Stack(Of String)
      stack.Push(root)
      Dim dirsScanned As Integer = 0

      While stack.Count > 0
         Dim dir = stack.Pop()
         dirsScanned += 1
         Try
            For Each subDir In Directory.GetDirectories(dir)
               stack.Push(subDir)
            Next
         Catch ex As Exception
            errors?.Add($"{dir}: {ex.GetType().Name} - {ex.Message}")
         End Try
         Try
            For Each f In Directory.GetFiles(dir)
               result.Add(f)
            Next
         Catch ex As Exception
            errors?.Add($"{dir}: {ex.GetType().Name} - {ex.Message}")
         End Try
      End While

      Return result
   End Function

   '===============================================================================================
   Public Shared Function ReadTask(taskFilePath As String, tasksRoot As String) As ScheduledTask
      ' Task files are XML declared as UTF-16; XDocument auto-detects encoding from the BOM/prolog
      Dim doc = XDocument.Load(taskFilePath)
      Dim root = doc.Root
      If root Is Nothing OrElse root.Name.LocalName <> "Task" Then
         Throw New InvalidDataException("Not a Task Scheduler task definition")
      End If

      Dim t As New ScheduledTask()
      t.FilePath = taskFilePath
      t.TaskPath = "\" & Path.GetRelativePath(tasksRoot, taskFilePath)

      Dim reg = root.Element(ns + "RegistrationInfo")
      If reg IsNot Nothing Then
         t.Uri = Val(reg, "URI")
         t.Author = Val(reg, "Author")
         t.Description = Val(reg, "Description")
         t.RegistrationDate = Val(reg, "Date")
         t.Documentation = Val(reg, "Documentation")
      End If

      Dim principal = root.Element(ns + "Principals")?.Elements(ns + "Principal")?.FirstOrDefault()
      If principal IsNot Nothing Then
         t.PrincipalId = If(Val(principal, "UserId"), Val(principal, "GroupId"))
         t.LogonType = Val(principal, "LogonType")
         t.RunLevel = Val(principal, "RunLevel")
      End If

      Dim settings = root.Element(ns + "Settings")
      If settings IsNot Nothing Then
         t.Enabled = ValBool(settings, "Enabled", True)
         t.Hidden = ValBool(settings, "Hidden", False)
         t.MultipleInstancesPolicy = Val(settings, "MultipleInstancesPolicy")
         t.ExecutionTimeLimit = Val(settings, "ExecutionTimeLimit")
         t.Priority = Val(settings, "Priority")
         t.AllowStartOnDemand = Val(settings, "AllowStartOnDemand")
         t.StartWhenAvailable = Val(settings, "StartWhenAvailable")
         t.DisallowStartIfOnBatteries = Val(settings, "DisallowStartIfOnBatteries")
      End If

      Dim actionsEl = root.Element(ns + "Actions")
      If actionsEl IsNot Nothing Then
         For Each a In actionsEl.Elements()
            Dim action As New TaskAction()
            action.Type = a.Name.LocalName
            Select Case action.Type
               Case "Exec"
                  action.Command = Val(a, "Command")
                  action.Arguments = Val(a, "Arguments")
                  action.WorkingDirectory = Val(a, "WorkingDirectory")
               Case "ComHandler"
                  action.Command = Val(a, "ClassId")
                  action.Arguments = Val(a, "Data")
               Case "SendEmail"
                  action.Command = Val(a, "Subject")
                  action.Arguments = "To: " & Val(a, "To")
               Case "ShowMessage"
                  action.Command = Val(a, "Title")
                  action.Arguments = Val(a, "Body")
               Case Else
                  action.Command = a.Value
            End Select
            t.Actions.Add(action)
         Next
      End If

      Dim triggersEl = root.Element(ns + "Triggers")
      If triggersEl IsNot Nothing Then
         For Each trig In triggersEl.Elements()
            Dim tt As New TaskTriggerXml()
            tt.Type = trig.Name.LocalName
            tt.Enabled = ValBool(trig, "Enabled", True)
            tt.StartBoundary = Val(trig, "StartBoundary")
            tt.EndBoundary = Val(trig, "EndBoundary")
            tt.Description = DescribeTrigger(trig)
            t.Triggers.Add(tt)
         Next
      End If

      Return t
   End Function

   '===============================================================================================
   Private Shared Function Val(parent As XElement, name As String) As String
      Return parent.Element(ns + name)?.Value
   End Function

   Private Shared Function ValBool(parent As XElement, name As String, defaultValue As Boolean) As Boolean
      Dim s = Val(parent, name)
      If String.IsNullOrEmpty(s) Then Return defaultValue
      Dim result As Boolean
      If Boolean.TryParse(s, result) Then Return result
      Return defaultValue
   End Function

   Private Shared Function ChildNames(el As XElement) As String
      Dim names = el.Elements().Select(Function(e) e.Name.LocalName).ToList()
      Return If(names.Count > 0, String.Join(", ", names), "(none)")
   End Function

   Private Shared Function Repetition(el As XElement) As String
      Dim rep = el.Element(ns + "Repetition")
      If rep Is Nothing Then Return ""
      Dim interval = Val(rep, "Interval")
      Dim duration = Val(rep, "Duration")
      If String.IsNullOrEmpty(interval) Then Return ""
      Return $" (repeats every {interval}{If(String.IsNullOrEmpty(duration), "", " for " & duration)})"
   End Function

   '===============================================================================================
   ' DescribeTrigger: builds a human readable one-line summary of a trigger's schedule
   Public Shared Function DescribeTrigger(el As XElement) As String
      Dim startBoundary = Val(el, "StartBoundary")
      Dim startText = If(String.IsNullOrEmpty(startBoundary), "", $" starting {startBoundary}")

      Select Case el.Name.LocalName
         Case "TimeTrigger"
            Return $"Once{startText}{Repetition(el)}"

         Case "CalendarTrigger"
            Return DescribeCalendarTrigger(el) & startText & Repetition(el)

         Case "BootTrigger"
            Return "At system startup" & Repetition(el)

         Case "LogonTrigger"
            Dim userId = Val(el, "UserId")
            Return "At logon" & If(String.IsNullOrEmpty(userId), "", $" ({userId})") & Repetition(el)

         Case "EventTrigger"
            Return "On event" & Repetition(el)

         Case "IdleTrigger"
            Return "On idle" & Repetition(el)

         Case "RegistrationTrigger"
            Return "At task creation/registration" & Repetition(el)

         Case "SessionStateChangeTrigger"
            Dim stateChange = Val(el, "StateChange")
            Return "On session state change" & If(String.IsNullOrEmpty(stateChange), "", $": {stateChange}")

         Case Else
            Return el.Name.LocalName
      End Select
   End Function

   '===============================================================================================
   Private Shared Function DescribeCalendarTrigger(calEl As XElement) As String
      Dim byDay = calEl.Element(ns + "ScheduleByDay")
      Dim byWeek = calEl.Element(ns + "ScheduleByWeek")
      Dim byMonth = calEl.Element(ns + "ScheduleByMonth")
      Dim byMonthDow = calEl.Element(ns + "ScheduleByMonthDayOfWeek")

      If byDay IsNot Nothing Then
         Dim interval = If(Val(byDay, "DaysInterval"), "1")
         Return $"Daily, every {interval} day(s)"

      ElseIf byWeek IsNot Nothing Then
         Dim interval = If(Val(byWeek, "WeeksInterval"), "1")
         Dim daysEl = byWeek.Element(ns + "DaysOfWeek")
         Dim days = If(daysEl IsNot Nothing, ChildNames(daysEl), "(none)")
         Return $"Weekly, every {interval} week(s) on {days}"

      ElseIf byMonth IsNot Nothing Then
         Dim domEl = byMonth.Element(ns + "DaysOfMonth")
         Dim days = If(domEl IsNot Nothing, String.Join(", ", domEl.Elements(ns + "Day").Select(Function(d) d.Value)), "(none)")
         Dim monthsEl = byMonth.Element(ns + "Months")
         Dim months = If(monthsEl IsNot Nothing, ChildNames(monthsEl), "(none)")
         Return $"Monthly, on day(s) {days} of {months}"

      ElseIf byMonthDow IsNot Nothing Then
         Dim weeksEl = byMonthDow.Element(ns + "Weeks")
         Dim weeks = If(weeksEl IsNot Nothing, ChildNames(weeksEl), "(none)")
         Dim daysEl = byMonthDow.Element(ns + "DaysOfWeek")
         Dim days = If(daysEl IsNot Nothing, ChildNames(daysEl), "(none)")
         Dim monthsEl = byMonthDow.Element(ns + "Months")
         Dim months = If(monthsEl IsNot Nothing, ChildNames(monthsEl), "(none)")
         Return $"Monthly, on the {weeks} {days} of {months}"

      Else
         Return "Calendar trigger"
      End If
   End Function

End Class
