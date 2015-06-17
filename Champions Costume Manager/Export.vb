Imports System.Net
Imports System.Collections.Specialized
Imports System.Text
Imports System.Runtime.Serialization.Formatters.Binary

Public Class Export

    Dim ImageFile As String = ""
    Dim resultError As String = ""

    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start("http://pastebin.com/")
    End Sub

    Private Sub Export_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, Button4.Click
        ComboBox1.SelectedIndex = 0
        ComboBox2.SelectedIndex = 1

        TabControl1.SelectTab(0)

        Me.Height = 191

        Dim imgLoc As String = Form1.ListView1.SelectedItems.Item(0).Tag
        PictureBox1.ImageLocation = imgLoc

        ImageFile = imgLoc
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click, Button3.Click, Button5.Click
        Me.Close()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        'UPLOAD THE COSTUME HERE! THIS IS THE MAIN PRUPOSE OF THIS STUB...

        Dim imageRAW As String = Convert.ToBase64String(System.IO.File.ReadAllBytes(ImageFile))
        Dim imageNAME As String = My.Computer.FileSystem.GetName(ImageFile)

        Dim msg As String = "[CCM] Champions Costume Manager by Norway174" & vbNewLine & _
            "Version: " & My.Application.Info.Version.ToString & vbNewLine & _
            imageNAME & vbNewLine & _
            imageRAW.ToString

        Dim result As String = Post(msg, imageNAME)
        If result.Contains("Bad API request") Then
            TabControl1.SelectTab(2)
            resultError = result
        ElseIf result = "False" Then
            TabControl1.SelectTab(2)
            resultError = "1: Failed to connect"
        Else
            TabControl1.SelectTab(1)

            TextBox1.Text = result.Substring(result.LastIndexOf("/") + 1)
        End If


    End Sub

    Shared Function Serialize(ByVal data As Object) As Byte()
        If TypeOf data Is Byte() Then Return data
        Using M As New IO.MemoryStream : Dim F As New BinaryFormatter : F.Serialize(M, data) : Return M.ToArray() : End Using
    End Function

    <Serializable()> Class Costume
        Public First As String
        Sub New(ByVal _first As String)

            First = _first

        End Sub
    End Class



        Private Sub CopyCodeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyCodeToolStripMenuItem.Click
            My.Computer.Clipboard.SetText(TextBox1.Text)
        End Sub

        Private Sub TextBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.Click
            TextBox1.SelectAll()
        End Sub




        Private Const post_url As String = "http://pastebin.com/api/api_post.php"
        Private Const dev_key As String = "a1778a16bb14a8305b0782299c9bb33a"

        Public Function Post(ByVal paste_code As String, _
                             ByVal paste_name As String) As String


            Dim Data As New NameValueCollection()
            Data.Add("api_dev_key", dev_key)
            Data.Add("api_option", "paste")
            Data.Add("api_paste_code", paste_code)
            Data.Add("api_paste_name", paste_name)
            Data.Add("api_paste_private", ComboBox2.SelectedIndex)  '0=public 1=unlisted 2=private
            Data.Add("api_paste_expire_date", SelectedExpire)

            Dim PasteURL As String = "False"
            Using Client As New WebClient()
                Dim Response As String = Encoding.UTF8.GetString(Client.UploadValues(post_url, Data))

                PasteURL = Response

            End Using

            Return PasteURL
        End Function

        Public Function SelectedExpire() As String
            Select Case ComboBox1.SelectedIndex

                Case 0
                    Return "N"
                Case 1
                    Return "10M"
                Case 2
                    Return "1H"
                Case 3
                    Return "1D"
                Case 4
                    Return "1W"
                Case 5
                    Return "2W"
                Case 6
                    Return "1M"
                Case Else
                    Return "N"
            End Select
        End Function

    End Class