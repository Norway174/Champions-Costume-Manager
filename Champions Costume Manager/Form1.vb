Public Class Form1

    Dim Debug_folder As String = "D:\SteamLibrary\SteamApps\common\Champions Online\Champions Online\Live\screenshots"
    Public SettingsFolder As String = My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData.Replace("\" & My.Application.Info.Version.ToString, "")
    Public SettingsFile As String = SettingsFolder & "\CCM-Settings.cfg"

    Function COFolder() As String
        Return Debug_folder
    End Function

    Sub StartupProcedure() Handles MyBase.Load
        Startup.Show()
        Startup.Visible = ShowLogToolStripMenuItem.Checked

        Console.WriteLine(SettingsFolder)

        loadCostumes()
    End Sub

    Sub loadCostumes() Handles ReloadToolStripMenuItem.Click
        Status_SelCount.Text = ""

        ImageList1.Images.Clear()
        ListView1.Items.Clear()
        Dim CostumesCount As Integer = 0

        For Each img In My.Computer.FileSystem.GetFiles(Debug_folder)
            If FileIO.FileSystem.GetName(img).StartsWith("Costume") And FileIO.FileSystem.GetName(img).EndsWith(".jpg") Then
                ImageList1.Images.Add(Drawing.Image.FromFile(img))
                ListView1.Items.Add(FileIO.FileSystem.GetName(img).Replace(".jpg", ""), ImageList1.Images.Count - 1)
                ListView1.Items.Item(ImageList1.Images.Count - 1).Tag = img

                CostumesCount = CostumesCount + 1
                Status_CosCount.Text = "Costumes: " & CostumesCount.ToString
            End If
        Next
    End Sub

    Private Sub ListView1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListView1.SelectedIndexChanged
        UpladToolStripMenuItem.Enabled = False

        If ListView1.SelectedItems.Count = 1 Then
            Dim imgLoc As String = ListView1.SelectedItems.Item(0).Tag
            PictureBox1.ImageLocation = imgLoc
            Label1.Hide()
            TextBox1.Text = FileIO.FileSystem.GetName(imgLoc).Replace(".jpg", "")
            UpladToolStripMenuItem.Enabled = True
        ElseIf ListView1.SelectedItems.Count = 0 Then
            Label1.Show()
            PictureBox1.Image = Nothing
            TextBox1.Text = ""
        End If

        If ListView1.SelectedItems.Count > 0 Then
            Status_SelCount.Text = "Sel: " & ListView1.SelectedItems.Count
            SelectedToolStripMenuItem.Text = "Selection: " & ListView1.SelectedItems.Count
            SelectedToolStripMenuItem.Visible = True
        Else
            Status_SelCount.Text = ""
            SelectedToolStripMenuItem.Visible = False
            PictureBox1.Image = Nothing
        End If


    End Sub

    Private Sub ConfirmToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ConfirmToolStripMenuItem.Click
        'DELETES STUFF! CAREFULL!
        For Each cost As ListViewItem In ListView1.SelectedItems

            My.Computer.FileSystem.DeleteFile(cost.Tag)
        Next
        loadCostumes()
    End Sub

    Private Sub UpladToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UpladToolStripMenuItem.Click
        Export.ShowDialog()
    End Sub

    Private Sub ToolStripButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton1.Click
        import.ShowDialog()
    End Sub

    Private Sub ShowLogToolStrip(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowLogToolStripMenuItem.CheckedChanged
        Startup.Visible = ShowLogToolStripMenuItem.Checked
    End Sub
End Class
