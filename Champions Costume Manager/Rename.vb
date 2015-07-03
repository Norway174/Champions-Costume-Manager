Public Class Rename

    Dim ImageFile As String = ""

    Private Sub Rename_ClientSizeChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.ClientSizeChanged
        If Me.Size.Height <> 463 Then
            Me.Height = 463
        End If
    End Sub


    Private Sub Rename_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        PictureBox1.Image = Nothing
        If Form1.ListView1.SelectedItems.Count = 0 Then
            MsgBox("No costume selected!", MsgBoxStyle.Information)
            Me.Close()
            Exit Sub
        End If
        Dim imgLoc As String = Form1.ListView1.SelectedItems.Item(0).Tag
        PictureBox1.ImageLocation = imgLoc

        ImageFile = imgLoc



    End Sub
End Class