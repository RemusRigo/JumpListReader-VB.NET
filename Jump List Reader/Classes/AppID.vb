Public Class AppID

   Private Shared ReadOnly KnownApps As Dictionary(Of String, String)

   Shared Sub New()
      KnownApps = New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

      KnownApps.Add("1b4dd67f29cb1962", "Windows Explorer")
      KnownApps.Add("f01b4d95cf55d32a", "Quick Access")
      KnownApps.Add("5f7b5f1e01b83767", "Notepad")
      KnownApps.Add("3c5c0763d0a1f86a", "WordPad")
      KnownApps.Add("9f4b0f3f0c0e0e0b", "Windows Terminal")
      KnownApps.Add("a7bd71699cd38d1f", "PowerShell")
      KnownApps.Add("b2f7f2f0b0d1c0e0", "PowerShell ISE")
      KnownApps.Add("f4e57c4b203c0d0e", "Remote Desktop Connection")
      KnownApps.Add("3c5c0763d0a1f86b", "Snip & Sketch")
      KnownApps.Add("3c5c0763d0a1f86c", "Snipping Tool")
      KnownApps.Add("9b9cdc69c1c24e2b", "Microsoft Word")
      KnownApps.Add("8ac3b9b6c0d1c0e0", "Google Chrome")
      KnownApps.Add("3d6be802d0a1f86a", "Mozilla Firefox")
      KnownApps.Add("2c9c7b1c0d1c0e0b", "Microsoft Edge")
   End Sub

   Public Shared Function GetAppByID(appID As String, Optional fallbackExe As String = Nothing) As String
      If KnownApps.ContainsKey(appID) Then
         Return KnownApps(appID)
      End If

      If Not String.IsNullOrEmpty(fallbackExe) Then
         Return fallbackExe
      End If

      Return "Unknown Application"
   End Function

End Class
