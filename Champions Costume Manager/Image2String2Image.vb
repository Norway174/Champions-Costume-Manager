Imports System.IO
Imports System.Drawing.Imaging

Public Class Image2String2Image

    '******************************************
    '*                                        *
    '*         THIS IS NOT A PART OF          *
    '*             THE PROGRAM                *
    '*                                        *
    '******************************************

    Dim szResult As String = ""
    Dim img As Image = Nothing
    Public Function ConvertImageToString(ByVal value As Image) As String

        If value Is Nothing Then Return ""

        Using ms As New MemoryStream
            value.Save(ms, ImageFormat.Jpeg)
            ms.Flush()
            ms.Position = 0
            Dim buffer = ms.ToArray
            'szResult = System.Text.Encoding.ASCII.GetString(buffer)
            szResult = Convert.ToBase64String(buffer)
        End Using

        Return szResult

    End Function

    Public Function ConvertStringToImage(ByVal imageEncodedString As String)


        ' Convert Base64 String to byte[]
        Dim imageBytes As Byte() = Convert.FromBase64String(imageEncodedString)
        Dim ms As New MemoryStream(imageBytes, 0, imageBytes.Length)

        ' Convert byte[] to Image
        ms.Write(imageBytes, 0, imageBytes.Length)
        Dim img As Image = Image.FromStream(ms, True)
        Return ms


        'If String.IsNullOrEmpty(imageEncodedString) Then Return Nothing

        'Dim buffer = Convert.FromBase64String(imageEncodedString)
        'Dim mem As New MemoryStream


        'Using mem

        '    mem.Position = 0

        '    img = Image.FromStream(mem)
        'End Using

        'Return img
    End Function

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        'Dim BackUpClipboard As String = My.Computer.Clipboard.GetText
        'SendKeys.Send("{PRTSC}")
        'PictureBox1.Image = My.Computer.Clipboard.GetImage

        Dim value As Image
        value = Image.FromFile(TextBox4.Text)
        TextBox1.Text = ConvertImageToString(value)
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim imageEncode As String
        imageEncode = TextBox1.Text
        'PictureBox2.Image = ConvertStringToImage(imageEncode)

        'My.Computer.FileSystem.WriteAllText("C:\Users\Willy\Desktop\Costume-save3.jpg", ConvertStringToImage(imageEncode).ToString, False)

        Dim image As Image = image.FromStream(ConvertStringToImage(imageEncode))
        image.Save("C:\Users\Willy\Desktop\Costume-save3.jpg", Imaging.ImageFormat.Jpeg)

    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        PictureBox1.ImageLocation = TextBox4.Text

        'BackgroundWorker1.RunWorkerAsync()
        'TextBox2.Text = System.Drawing.Imaging.


    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click

        'Dim img As Image = Image.FromFile(TextBox4.Text)
        'img.Save("C:\Users\Willy\Desktop\Costume-save3.jpg", Imaging.ImageFormat.Jpeg)


        ConvertFileFromBase64("C:\Users\Willy\Desktop\Costume-save3.jpg", ConvertFileToBase64(TextBox4.Text))
        'Dim pic As Image
        'pic = PictureBox1.Image
        'SaveFileDialog1.ShowDialog()
        'pic.Save(SaveFileDialog1.FileName)
    End Sub

    Public Function ConvertFileToBase64(ByVal fileName As String) As String
        Return Convert.ToBase64String(System.IO.File.ReadAllBytes(fileName))
    End Function
    Public Sub ConvertFileFromBase64(ByVal fileName As String, ByVal Base64 As String)
        System.IO.File.WriteAllBytes(fileName, Convert.FromBase64String(Base64))
    End Sub


    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim allByte() As Byte = My.Computer.FileSystem.ReadAllBytes(TextBox4.Text)

        Dim count As Integer = 1
        For Each bty As Byte In allByte
            TextBox2.AppendText(TextBox2.Text & count & " " & bty.ToString & vbNewLine)
            count += 1
        Next
    End Sub

    Private Sub Image2String2Image_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Control.CheckForIllegalCrossThreadCalls = False
    End Sub
End Class