Public Class Startup

    Private Sub Startup_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Application.DoEvents()
        Log("Loading...")
        Log("Checking for updates... Version: " & My.Application.Info.Version.ToString)
        CheckForUpdates()
        Log("Checking for Settings file")
        CheckSettings()
        Log("Checking for Champions Online directory")

        'Form1.Show()
    End Sub

    Sub Log(ByVal text As String)
        TextBox1.AppendText(text & vbNewLine)
    End Sub

    Sub CheckForUpdates()
        'TODO
    End Sub

    Sub CheckSettings()
        'TODO
    End Sub
End Class