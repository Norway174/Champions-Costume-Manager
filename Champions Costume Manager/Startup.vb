Public Class Startup
    Dim First As Boolean = False

    Private Sub Startup_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        If First = True Then Exit Sub
        Application.DoEvents()
        'System.Threading.Thread.Sleep(50)

        Log("Loading...")
        Log("Checking for updates... Version: " & My.Application.Info.Version.ToString)
        CheckForUpdates()
        Log("Checking for Settings file")
        CheckSettings()
        Log("Checking for Champions Online directory")

        'System.Threading.Thread.Sleep(5000)

        First = True
        'Form1.Show()
        'Me.Hide()
    End Sub

    Sub Log(ByVal text As String)
        TextBox1.AppendText(text & vbNewLine)
    End Sub

    Sub CheckForUpdates()

        Dim ReleasesURL = "https://api.github.com/repos/Norway174/Champions-Costume-Manager/releases"

        Dim webClient As New System.Net.WebClient
        webClient.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0")
        Dim result As String

        Try
            result = webClient.DownloadString(ReleasesURL)
            'result = My.Resources.APIJsonExample
        Catch ex As Exception
            Log("Failed to get updates.")
            Console.WriteLine(ex.Message)
            Exit Sub
        End Try

        Dim tagname As String = "tag_name"
        Dim version As String = ""
        If result.Contains(tagname) Then
            'version = result.Substring(result.IndexOf(tagname) + tagname.Count + 4, result.IndexOf("""," & vbNewLine))

            Dim Split As String() = result.Split(",")
            For Each lne As String In Split
                If lne.Contains(tagname) Then
                    Dim lneSplit As String() = lne.Split("""")
                    version = lneSplit(lneSplit.Count - 2)
                    Exit For
                End If
            Next



            Console.WriteLine(version)
            Log("Latest version: " & version)
            If My.Application.Info.Version.ToString <> version Then
                Log("CCM update avalible!")
                If MsgBox("There is a new update avalible for CCM! Download now?", MsgBoxStyle.YesNo, "Update!") = MsgBoxResult.Yes Then
                    Process.Start("https://github.com/Norway174/Champions-Costume-Manager/releases")
                End If
            Else
                Log("No update required.")
            End If

        Else
            Log("Failed to read update data.")
            Console.WriteLine(result)
        End If

    End Sub

    Sub CheckSettings()
        If My.Computer.FileSystem.DirectoryExists(Form1.SettingsFolder) = False Then
            If MsgBox("No settings file found, do you want to create one now? It will be saved in: " & vbNewLine & Form1.SettingsFolder, MsgBoxStyle.YesNo, "Where to save stuff?") = MsgBoxResult.Yes Then
                My.Computer.FileSystem.CreateDirectory(Form1.SettingsFolder)

                CODetector.ShowDialog()
            End If
        End If
    End Sub
End Class