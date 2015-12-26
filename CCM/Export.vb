Imports System.Net
Imports System.Collections.Specialized
Imports System.Text
Imports System.Runtime.Serialization.Formatters.Binary
Imports Newtonsoft.Json

Public Class Export

    Dim ImageFile As String = ""
    Dim resultError As String = ""

    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start("https://paste.ee/")
    End Sub

    Private Sub Export_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, Button4.Click
        ComboBox1.SelectedIndex = 0

        TabControl1.SelectTab(0)

        Me.Height = 201

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

        Dim JSComstume As New JSON_Costume()

        JSComstume.CCM = "Champions Costume Manager by Norway174"
        JSComstume.CCM_Version = My.Application.Info.Version.ToString
        JSComstume.Costume_Name = imageNAME
        JSComstume.Costume_Data = imageRAW

        Dim output As String = JsonConvert.SerializeObject(JSComstume, formatting:=1)

        Dim result As String = Post(output, imageNAME)
        If result.Contains("Bad API request") Then
            TabControl1.SelectTab(2)
            resultError = result
        ElseIf result.ToLower.Contains("error") Then
            TabControl1.SelectTab(2)
            resultError = result
        ElseIf result = "False" Then
            TabControl1.SelectTab(2)
            resultError = "1: Failed to connect"
        Else
            TabControl1.SelectTab(1)



            'Dim FormatTxt = result.TrimStart("{""key"":""")
            'FormatTxt = FormatTxt.TrimEnd("""}""")

            Dim JsonRaw As JSON_Result = JsonConvert.DeserializeObject(Of JSON_Result)(result)
            'Dim JsonResult As JSON_ID = JsonConvert.DeserializeObject(Of JSON_ID)(JsonRaw.Paste)

            TextBox1.Text = JsonRaw.Paste.ID
        End If


    End Sub

    Public Class JSON_ID
        Public ID As String
    End Class
    Public Class JSON_Result
        Public Paste As JSON_ID
    End Class

    Public Class JSON_Costume
        Public CCM As String
        Public CCM_Version As String
        Public Costume_Name As String
        Public Costume_Data As String
    End Class

    Private Sub CopyCodeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyCodeToolStripMenuItem.Click
        My.Computer.Clipboard.SetText(TextBox1.Text)
    End Sub

    Private Sub TextBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.Click
        TextBox1.SelectAll()
    End Sub




    'Private Const post_url As String = "http://pastebin.com/api/api_post.php"
    'Private Const dev_key As String = "a1778a16bb14a8305b0782299c9bb33a"

    'Private Const post_url As String = "http://hastebin.com/documents"

    Private Const post_url As String = "http://paste.ee/api"

    Public Function Post(ByVal paste_code As String,
                         ByVal paste_name As String) As String


        Dim Data As New NameValueCollection()

        'PASTEBIN:
        'Data.Add("api_dev_key", dev_key)
        'Data.Add("api_option", "paste")
        'Data.Add("api_paste_code", paste_code)
        'Data.Add("api_paste_name", paste_name)
        'Data.Add("api_paste_private", ComboBox2.SelectedIndex)  '0=public 1=unlisted 2=private
        'Data.Add("api_paste_expire_date", SelectedExpire)
        'Data.Add("api_paste_format", "json")

        'HASTEBIN
        'Dim Data As String = paste_code

        'PASTE.EE
        Data.Add("key", "526ca40c65ee650265008e61ab8ccd11")
        Data.Add("description", paste_name)
        Data.Add("paste", paste_code)
        Data.Add("format", "json")
        Data.Add("expire", SelectedExpire)

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
                Return "0" 'Never
            Case 1
                Return "10" '10 Minutes
            Case 2
                Return "60" '1 Hour
            Case 3
                Return "1440" '1 Day
            Case 4
                Return "10080" '1 Week
            Case 5
                Return "20160" '3 Weeks
            Case 6
                Return "43800" '1 Month
            'Case 7
            '    Return "views; " & NumericUpDown1.Value
            Case Else
                Return "0" 'Default to never
        End Select
    End Function

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedItem = "Set download limit" Then
            Label2.Visible = True
            NumericUpDown1.Visible = True
        Else
            Label2.Visible = False
            NumericUpDown1.Visible = False
        End If
    End Sub
End Class