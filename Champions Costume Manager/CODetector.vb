Imports System.IO

Public Class CODetector

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        'For Each fld In My.Computer.FileSystem.GetDirectories(My.Computer.FileSystem.SpecialDirectories.ProgramFiles, FileIO.SearchOption.SearchAllSubDirectories)
        '    CheckedListBox1.Items.Add(fld)
        'Next

        'Dim programFiles As New DirectoryInfo("D:\")
        'Dim result As List(Of DirectoryInfo)
        'result = programFiles.GetDirectories("*Champions Online", SearchOption.AllDirectories).ToList()

        'Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))

        Button2.Enabled = False
        BackgroundWorker1.RunWorkerAsync()
        





    End Sub

    Public Function GetDirectories(ByVal path As String) As List(Of String)
        Dim subfolders As New List(Of String)

        For Each subfolder As String In IO.Directory.GetDirectories(path)
            If subfolder.Contains("C:\Windows") Then Continue For
            If subfolder.Contains("C:\Users") Then Continue For

            subfolders.Add(subfolder)
            TextBox1.Text = subfolder
            'Application.DoEvents()

            Try
                subfolders.AddRange(GetDirectories(subfolder))
            Catch ex As Exception
                'Ignore this folder and move on.
                'Console.WriteLine(ex.Message)
            End Try
        Next subfolder

        Return subfolders
    End Function

    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        CheckForIllegalCrossThreadCalls = False
        'Label1.Visible = True

        Dim allDrives() As DriveInfo = DriveInfo.GetDrives()
        Dim result As List(Of String) = Nothing

        Dim d As DriveInfo
        For Each d In allDrives
            Console.WriteLine(d.Name)
            Console.WriteLine(d.IsReady)

            'Label1.Text = "Scanning Drive: " & d.Name

            If d.IsReady Then

                result = GetDirectories(d.Name)

            End If
        Next

        'result = GetDirectories(result)

        If result.Count <> 0 Then
            For Each dir As String In result

                If dir.EndsWith("Champions Online\Live\screenshots") Then
                    CheckedListBox1.Items.Add(dir, True)
                End If

            Next
        End If

        TextBox1.Text = "All done!"
        Button2.Enabled = True

    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        FolderBrowserDialog1.Description = "Select your Champions Online Costume folder." & vbNewLine & _
            "Usually that folder ends with, ""Champions Online\Live\screenshots"""
        FolderBrowserDialog1.ShowDialog()
        Dim fld As String = FolderBrowserDialog1.SelectedPath

        If fld = "" Then Exit Sub
        CheckedListBox1.Items.Add(fld, True)
    End Sub

    Private Sub CheckedListBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckedListBox1.SelectedIndexChanged
        If CheckedListBox1.SelectedItems.Count > 0 Then
            Button4.Enabled = True
        Else
            Button4.Enabled = False
        End If
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        CheckedListBox1.Items.RemoveAt(CheckedListBox1.SelectedIndex)
        CheckedListBox1.Select()
    End Sub

    Private Sub UpdateDoneBtn(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        If CheckedListBox1.CheckedItems.Count > 0 Then
            Button1.Enabled = True
        Else
            Button1.Enabled = False
        End If
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Me.Close()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim txt As String = ""
        Dim first As Boolean = True
        For Each inst As String In CheckedListBox1.CheckedItems
            If first = False Then txt = txt & vbNewLine
            txt = txt & inst

            first = False
        Next
        My.Computer.FileSystem.WriteAllText(Form1.InstancesFile, txt, False)
        'Console.WriteLine(Form1.InstancesFile)

        Form1.loadInstances()
        Me.Close()
    End Sub

    Private Sub CODetector_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckedListBox1.Items.Clear()

        If My.Computer.FileSystem.FileExists(Form1.InstancesFile) Then
            Dim Instances As String() = My.Computer.FileSystem.ReadAllText(Form1.InstancesFile).Split(vbNewLine)
            'costumeFolder = Instances(0)

            If Instances.Count > 0 Then
                For Each Int As String In Instances
                    'TODO: Add the rest of the instances to a list and let the user switch between them.
                    'ToolStripComboBox2.Items.Add(Int)
                    CheckedListBox1.Items.Add(Int, True)
                Next
                'ToolStripComboBox2.SelectedIndex = 0
            End If



        End If
    End Sub
End Class