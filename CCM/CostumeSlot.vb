Public Class CostumeSlot
    Private CostumeLoc As String

    ' Declares the name and type of the property.
    Property CostumeLocation() As String
        ' Retrieves the value of the private variable colBColor.
        Get
            Return CostumeLoc
        End Get
        ' Stores the selected value in the private variable colBColor, and 
        ' updates the background color of the label control lblDisplay.
        Set(ByVal value As String)
            CostumeLoc = value
        End Set

    End Property


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        TextBox2.Text = "Test"
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        If Panel1.Visible Then
            Panel1.Visible = False
            BackColor = SystemColors.Control
        Else
            Panel1.Visible = True
            BackColor = SystemColors.Highlight
        End If

    End Sub

    Private Sub CostumeSlot_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PictureBox1.ImageLocation() = CostumeLoc
    End Sub
End Class
