Imports System.Net

Public Class import
    Dim RawPasteURL As String = "http://pastebin.com/raw.php?i="

    'Dim DownloadedCostume As String = ""
    Dim DownloadedCostumeName As String = ""

    Dim TmpFol As String = My.Computer.FileSystem.SpecialDirectories.Temp() & "\CCM\"
    Dim TmpLoc As String = TmpFol & "TMP-Costume.jpg"


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim link As String = RawPasteURL & TextBox1.Text
        DownloadCostume(link)
    End Sub

    Private Sub DownloadCostume(ByVal url As String)
        Dim client As WebClient = New WebClient
        AddHandler client.DownloadProgressChanged, AddressOf client_ProgressChanged
        AddHandler client.DownloadStringCompleted, AddressOf client_DownloadStringCompleted
        client.DownloadStringAsync(New Uri(url))
        Button1.Text = "Download in Progress"
        Button1.Enabled = False
    End Sub
    Private Sub client_ProgressChanged(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs)
        ProgressBar1.Value = e.ProgressPercentage
    End Sub
    Private Sub client_DownloadStringCompleted(ByVal sender As Object, ByVal e As DownloadStringCompletedEventArgs)
        'MessageBox.Show("Download Complete")
        'TextBox2.Text = e.Result
        'DownloadedCostume = e.Result

        If e.Result.Contains("Spam Detection") Then
            If MsgBox("Spam... Please go to the Paste, and enter the Captcha in order to unlock it." & vbNewLine & _
                      "Go to Paste to enter the Captcha now?" & vbNewLine & vbNewLine & _
                      "After you've entered the Captcha, and can see the paste. Feel free to close it, try again here.", MsgBoxStyle.YesNo, "Spam Filter") = MsgBoxResult.Yes Then
                Process.Start("http://pastebin.com/" & TextBox1.Text)
            End If

        Else
            Dim Split As String() = e.Result.Split(vbNewLine)
            
            Dim RawImg As Byte() = Convert.FromBase64String(Split(Split.Count() - 1))
            Console.WriteLine(TmpLoc)

            If My.Computer.FileSystem.DirectoryExists(TmpLoc) = False Then My.Computer.FileSystem.CreateDirectory(TmpFol)
            My.Computer.FileSystem.WriteAllBytes(TmpLoc, RawImg, False)

            PictureBox1.ImageLocation = TmpLoc

            DownloadedCostumeName = Split(Split.Count() - 2)
            DownloadedCostumeName = DownloadedCostumeName.TrimStart(vbLf)

            Button2.Enabled = True
        End If



        Button1.Text = "Download"
        Button1.Enabled = True
    End Sub

    Private Sub import_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        TextBox1.Clear()
        TextBox2.Clear()
        PictureBox1.Image = Nothing
        ProgressBar1.Value = 0
        Button2.Enabled = False

        If My.Computer.Clipboard.ContainsText Then TextBox1.Text = My.Computer.Clipboard.GetText()
    End Sub

    Private Sub Done(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim SaveAs As String = Form1.COFolder & "\" & DownloadedCostumeName

        If My.Computer.FileSystem.FileExists(SaveAs) Then
            Dim Promt As MsgBoxResult = MsgBox("Costume with that name already exists, overwrite?", vbYesNoCancel, "Costume name already exists")
            If Promt = MsgBoxResult.No Then
                'Nothing
            ElseIf Promt = MsgBoxResult.Cancel Then
                Exit Sub
            Else
                My.Computer.FileSystem.MoveFile(TmpLoc, SaveAs, True)
            End If
        Else
            My.Computer.FileSystem.MoveFile(TmpLoc, SaveAs, True)
        End If

        Form1.loadCostumes()
        Me.Close()
    End Sub

    Private Sub Cancel(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Me.Close()
    End Sub
End Class