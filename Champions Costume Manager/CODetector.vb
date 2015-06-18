Imports System.IO

Public Class CODetector

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        'For Each fld In My.Computer.FileSystem.GetDirectories(My.Computer.FileSystem.SpecialDirectories.ProgramFiles, FileIO.SearchOption.SearchAllSubDirectories)
        '    CheckedListBox1.Items.Add(fld)
        'Next

        'Dim programFiles As New DirectoryInfo("D:\")
        'Dim result As List(Of DirectoryInfo)
        'result = programFiles.GetDirectories("*Champions Online", SearchOption.AllDirectories).ToList()

        Dim allDrives() As DriveInfo = DriveInfo.GetDrives()
        Dim result As List(Of String)

        Dim d As DriveInfo
        For Each d In allDrives
            Console.WriteLine(d.Name)
            Console.WriteLine(d.IsReady)

            If d.IsReady Then
                result = GetDirectories(d.Name)
            End If
        Next



        For Each dir As String In result
            If dir.EndsWith("Champions Online\Live\screenshots") Then
                CheckedListBox1.Items.Add(dir)
            End If

        Next


    End Sub

    Public Function GetDirectories(ByVal path As String) As List(Of String)
        Dim subfolders As New List(Of String)

        For Each subfolder As String In IO.Directory.GetDirectories(path)
            subfolders.Add(subfolder)

            Try
                subfolders.AddRange(GetDirectories(subfolder))
            Catch ex As UnauthorizedAccessException
                'Ignore this folder and move on.
            End Try
        Next subfolder

        Return subfolders
    End Function

End Class