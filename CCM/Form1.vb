Imports System.IO
Imports System.Net
Imports Microsoft.Win32
Imports Newtonsoft.Json



''' <summary>
''' TODO:
''' - Uninstall button / remove settings file (DONE!)
''' - Import page, + functionality (DONE!)
''' - Export page, + functionality (DONE!)
''' - Move costumes between folders (DONE!)
''' - Simple delete options of one or multiple (DONE!)
''' 
''' TODO v2:
''' - Auto-update via GitHub. Need atleast one release, and more data to map JSON
''' - Save Export Info to JSON, associate with the correct costume
''' 
''' ADVANCED TODO:
''' - Display proper tailor costume names
''' - Rename costumes
''' </summary>
''' 



Public Class Form1

    'DECLARATIONS
    Public ReadOnly appFolder As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\CCM"
    Public ReadOnly settingsFile As String = appFolder & "\CCM-Settings.cfg"

    Dim FirstRun As Boolean = False
    Private Searching As Boolean = False

    Dim COFolder As String = "D:\SteamLibrary\SteamApps\common\Champions Online\Champions Online\Live\screenshots"

    'Dim int As Integer = 0


#Region "Misc subs"

    Private Sub startUp() Handles MyBase.Load
        'PART ONE: Check Settings
        CheckSettings()

        'PART TWO: Load Settings
        'Check down for LoadSettings, which happens on MyBase.Shown

        'PART THREE: Load up the costumes
        'AddCostumes() - Moved to Load Settings!

        'NO PART FOUR?


        'WTF!!?
        'TabControl1.ItemSize = New Size(0, 1)
        'TabControl1.SizeMode = TabSizeMode.Fixed

        'SplitContainer1.Panel2Collapsed = True
    End Sub

    Sub ShowHideFoldersDropDown() Handles TabControl1.TabIndexChanged, TabControl1.Click, Me.Shown
        If TabControl1.SelectedIndex = 0 Or TabControl1.SelectedIndex = 1 Then
            SelectInstallFolder_Dropdown_Panel.Show()
        Else
            SelectInstallFolder_Dropdown_Panel.Hide()
        End If
    End Sub

    Sub FixMoveNCopyPanels() Handles Me.Load
        MoveOrCopy_ParentPanel.Dock = 5

        MoveOrCopy_ParentPanel.Visible = False
        MoveGroupBox6()
    End Sub

    Sub MoveGroupBox6() Handles Me.Resize
        MoveOrCopy_GroupBox.Top = (MoveOrCopy_ParentPanel.Height - MoveOrCopy_GroupBox.Height) / 2
        MoveOrCopy_GroupBox.Left = (MoveOrCopy_ParentPanel.Width - MoveOrCopy_GroupBox.Width) / 2
    End Sub



#End Region

#Region "MainForm"

    Sub AddCostumes() Handles ComboBox1.SelectedIndexChanged ', SearchBox_Text.TextChanged
        COFolder = ComboBox1.SelectedItem
        ListView1.Clear()
        ImageList1.Images.Clear()

        For Each img In My.Computer.FileSystem.GetFiles(COFolder, FileIO.SearchOption.SearchTopLevelOnly, "*.jpg")
            'Ctme = New CostumeSlot
            'Ctme.CostumeLocation = fle
            'FlowLayoutPanel1.Controls.Add(Ctme)

            Dim SearchString As String = ""
            If Searching Then
                If SearchBox_Text.TextLength <> 0 Then
                    SearchString = SearchBox_Text.Text
                End If
            End If

            If FileIO.FileSystem.GetName(img).StartsWith("Costume_") And FileIO.FileSystem.GetName(img).EndsWith(".jpg") _
                And FileIO.FileSystem.GetName(img).Contains(SearchString) Then

                ImageList1.Images.Add(Drawing.Image.FromFile(img))
                ListView1.Items.Add(FileIO.FileSystem.GetName(img).Replace(".jpg", ""), ImageList1.Images.Count - 1)
                ListView1.Items.Item(ImageList1.Images.Count - 1).Tag = img
            End If

        Next
        ToolStripStatusLabel1.Text = "Loaded costumes: " & ListView1.Items.Count
        ToolStripStatusLabel1.Visible = True
    End Sub

    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged
        ToolStripStatusLabel2.Visible = True
        ToolStripStatusLabel2.Text = "- Selected: " & ListView1.SelectedItems.Count

        If ListView1.SelectedItems.Count = 1 Then
            Dim imgLoc As String = ListView1.SelectedItems.Item(0).Tag
            PictureBox1.ImageLocation = imgLoc
            'SplitContainer1.Panel2Collapsed = False
        ElseIf ListView1.SelectedItems.Count = 0 Then
            'SplitContainer1.Panel2Collapsed = True
            PictureBox1.Image = Nothing
            ToolStripStatusLabel2.Visible = False
        End If

        If ListView1.SelectedItems.Count > 0 Then

        Else

        End If
    End Sub

#Region "Costume Management: Export, Delete, Move & Copy"

    Sub ExportCostume() Handles ExportCostumeToolStripMenuItem.Click, ExportCostumeToolStripMenuItem1.Click
        Export.ShowDialog()
    End Sub

    Sub DeleteCostumes() Handles ConfirmToolStripMenuItem.Click, ConfirmToolStripMenuItem1.Click
        Dim SlcCos As String = ListView1.SelectedItems.Count
        Dim CostumePlural As String = " costumes"
        If ListView1.SelectedItems.Count = 1 Then CostumePlural = " costume"
        SlcCos = SlcCos & CostumePlural

        Dim StringBuilder = "Are you sure you wish to delete " & SlcCos & " permanently?" & vbNewLine &
            vbNewLine & "This action CAN NOT be undone!"

        If MsgBox(StringBuilder, MsgBoxStyle.YesNo, "WARNING!") = MsgBoxResult.Yes Then
            For Each itm In ListView1.SelectedItems

                PictureBox1.Image = Nothing

                Try
                    My.Computer.FileSystem.DeleteFile(itm.Tag, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                Catch ex As Exception
                    MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Error")
                    Exit For
                End Try


                ListView1.Items.Remove(itm)
            Next
            MsgBox(SlcCos & " deleted permanently.")
        End If


    End Sub

    Sub MoveCostume() Handles MoveToolStripMenuItem.Click, MoveToolStripMenuItem1.Click
        MoveOrCopy("Move")
    End Sub

    Sub CopyCostume() Handles CopyToolStripMenuItem.Click, CopyToolStripMenuItem1.Click
        MoveOrCopy("Copy")
    End Sub

    Sub MoveOrCopy(ByVal Action)
        MoveOrCopy_GroupBox.Text = Action
        MoveOrCopy_Btn_Accept.Text = Action

        MoveOrCopy_ChListBox.Items.Clear()

        MoveOrCopy_ParentPanel.Visible = True

        For Each fld In ComboBox1.Items
            If fld = ComboBox1.SelectedItem Then
                'Skip it
            Else
                MoveOrCopy_ChListBox.Items.Add(fld)
            End If
        Next

        MoveOrCopy_Btn_Accept.Tag = Action
    End Sub

    Sub MoveOrCopy_End() Handles MoveOrCopy_Btn_Cancel.Click
        MoveOrCopy_ParentPanel.Visible = False
        MoveOrCopy_ChListBox.Items.Clear()
        MoveOrCopy_Btn_Accept.Tag = ""
        MoveOrCopy_Btn_Accept.Enabled = False
    End Sub

    Sub MoveOrCopyCheckedChanged() Handles MoveOrCopy_ChListBox.SelectedIndexChanged
        If MoveOrCopy_ChListBox.CheckedIndices.Count = 0 Then
            MoveOrCopy_Btn_Accept.Enabled = False
        Else
            MoveOrCopy_Btn_Accept.Enabled = True
        End If
    End Sub

    Sub MoveOrCopy() Handles MoveOrCopy_Btn_Accept.Click
        For Each itm In ListView1.SelectedItems
            For Each dest In MoveOrCopy_ChListBox.CheckedItems

                If MoveOrCopy_Btn_Accept.Tag = "Move" Then

                    Dim refinedDest As String = dest & "\" & FileIO.FileSystem.GetName(itm.Tag)
                    My.Computer.FileSystem.MoveFile(itm.tag, refinedDest, True)

                    ListView1.Items.Remove(itm)

                ElseIf MoveOrCopy_Btn_Accept.Tag = "Copy" Then

                    Dim refinedDest As String = dest & "\" & FileIO.FileSystem.GetName(itm.Tag)
                    My.Computer.FileSystem.CopyFile(itm.tag, refinedDest, True)

                Else
                    'This one should never occur. But if it does happen, just do nothing and close the window.
                End If
            Next
        Next

        MoveOrCopy_End()
    End Sub

#End Region

#Region "SearchBox"
    Private Sub FistTimeFocusfix() Handles Me.Shown
        ListView1.Focus()
        SearchBox_Panel.Hide()
    End Sub

    Private Sub AddCostumes(sender As Object, e As EventArgs) Handles SearchBox_Text.TextChanged, ComboBox1.SelectedIndexChanged

    End Sub

    'Private Sub SearchBox_GotFocus() Handles SearchBox_Text.GotFocus
    '    If SearchBox_Text.Text = "Search for costume..." Then
    '        SearchBox_Text.Text = ""
    '        SearchBox_Text.ForeColor = Color.Black
    '        Searching = True
    '    End If
    'End Sub
    'Private Sub SearchBox1_LostFocus() Handles SearchBox_Text.LostFocus
    '    If SearchBox_Text.Text = "" Then
    '        SearchBox_Text.Text = "Search for costume..."
    '        SearchBox_Text.ForeColor = Color.LightGray
    '        Searching = False
    '        AddCostumes()
    '    End If
    'End Sub

    'Sub ClearBtn() Handles SearchBox_ClearBtn.Click
    '    SearchBox_Text.Text = ""
    '    SearchBox_Text.Focus()
    '    SearchBox_GotFocus()
    'End Sub

    'Sub ShowHideSearch() Handles TabControl1.TabIndexChanged, TabControl1.Click
    '    If TabControl1.SelectedIndex = 0 Then
    '        SearchBox_Panel.Show()
    '    Else
    '        SearchBox_Panel.Hide()
    '    End If
    'End Sub

    'Sub Search() Handles SearchBox_Text.TextChanged

    'End Sub
#End Region

#End Region

#Region "Import"

    Dim RawPasteURL As String = "https://paste.ee/r/" '"http://pastebin.com/raw.php?i="

    'Dim DownloadedCostume As String = ""
    Dim DownloadedCostumeName As String = ""

    Dim TmpFol As String = appFolder & "\tmp\"
    Dim TmpLoc As String = TmpFol & "tmpCostume.jpg"


    Private Sub ImportCostume() Handles Import_Btn_Download.Click
        Dim link As String = RawPasteURL & Import_TextBox_CostumeCode.Text
        DownloadCostume(link)
    End Sub

    Private Sub DownloadCostume(ByVal url As String)
        Dim client As WebClient = New WebClient
        AddHandler client.DownloadProgressChanged, AddressOf client_ProgressChanged
        AddHandler client.DownloadStringCompleted, AddressOf client_DownloadStringCompleted
        client.DownloadStringAsync(New Uri(url))
        Import_Btn_Download.Text = "Download in Progress"
        Import_Btn_Download.Enabled = False
    End Sub
    Private Sub client_ProgressChanged(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs)
        Import_ProgressBar.Value = e.ProgressPercentage
    End Sub
    Private Sub client_DownloadStringCompleted(ByVal sender As Object, ByVal e As DownloadStringCompletedEventArgs)
        'MessageBox.Show("Download Complete")
        'TextBox2.Text = e.Result
        'DownloadedCostume = e.Result

        'If e.Error.Message = Nothing Then
        '    MsgBox(e.Error.Message, MsgBoxStyle.Exclamation, "An Error as occured!")
        '    Button1.Enabled = True
        '    Button1.Text = "Download"
        '    Exit Sub
        'End If

        If e.Result.Contains("Spam Detection") Then
            If MsgBox("Spam... Please go to the Paste, and enter the Captcha in order to unlock it." & vbNewLine &
                      "Go to Paste to enter the Captcha now?" & vbNewLine & vbNewLine &
                      "After you've entered the Captcha, and can see the paste. Feel free to close it, try again here.", MsgBoxStyle.YesNo, "Spam Filter") = MsgBoxResult.Yes Then
                Process.Start("http://pastebin.com/" & Import_TextBox_CostumeCode.Text)
            End If
            'ElseIf e.Result.Contains("404") Then
            '    MsgBox(e.Error.Message, MsgBoxStyle.Exclamation, "An Error as occured!")
            '    Button1.Enabled = True
            '    Button1.Text = "Download"
            '    Exit Sub
        Else
            'Dim Split As String() = e.Result.Split(vbNewLine)

            'Console.WriteLine(TmpLoc)

            Dim deserializedCostume As JSON_Costume = JsonConvert.DeserializeObject(Of JSON_Costume)(e.Result)

            Dim RawImg As Byte() = Convert.FromBase64String(deserializedCostume.Costume_Data)

            If My.Computer.FileSystem.DirectoryExists(TmpLoc) = False Then My.Computer.FileSystem.CreateDirectory(TmpFol)
            My.Computer.FileSystem.WriteAllBytes(TmpLoc, RawImg, False)

            Import_Label_PictureBox_Preview.Visible = False
            Import_PictureBox.ImageLocation = TmpLoc

            DownloadedCostumeName = deserializedCostume.Costume_Name

            'DownloadedCostumeName = Split(Split.Count() - 2)
            'DownloadedCostumeName = DownloadedCostumeName.TrimStart(vbLf)

            'If My.Computer.FileSystem.DirectoryExists(Form1.COFolder) Then
            '    Button2.Enabled = True
            'End If

        End If

        Import_Btn_Download.Text = "Download"
        Import_Btn_Download.Enabled = True

        Import_SaveToActiveFolder.Enabled = True
    End Sub

    Public Class JSON_Costume
        Public CCM As String
        Public CCM_Version As String
        Public Costume_Name As String
        Public Costume_Data As String
    End Class

    Private Sub import_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TabPage_Import.Click
        Import_TextBox_CostumeCode.Clear()
        Import_TextBox2_Hidden.Clear()
        Import_PictureBox.Image = Nothing
        Import_Label_PictureBox_Preview.Visible = True
        Import_ProgressBar.Value = 0
        Import_SaveToActiveFolder.Enabled = False

        If My.Computer.Clipboard.ContainsText Then Import_TextBox_CostumeCode.Text = My.Computer.Clipboard.GetText()
    End Sub

    Private Sub Done(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Import_SaveToActiveFolder.Click
        Dim SaveAs As String = ComboBox1.SelectedItem.ToString & "\" & DownloadedCostumeName

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

        AddCostumes()
        If My.Computer.FileSystem.DirectoryExists(TmpFol) Then My.Computer.FileSystem.DeleteDirectory(TmpFol, FileIO.DeleteDirectoryOption.DeleteAllContents)
    End Sub

    Private Sub Cancel(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If My.Computer.FileSystem.DirectoryExists(TmpFol) Then My.Computer.FileSystem.DeleteDirectory(TmpFol, FileIO.DeleteDirectoryOption.DeleteAllContents)

        Import_TextBox_CostumeCode.Clear()
        Import_TextBox2_Hidden.Clear()
        Import_PictureBox.Image = Nothing
        Import_Label_PictureBox_Preview.Visible = True
        Import_ProgressBar.Value = 0
        Import_SaveToActiveFolder.Enabled = False

        If My.Computer.Clipboard.ContainsText Then Import_TextBox_CostumeCode.Text = My.Computer.Clipboard.GetText()
    End Sub

    Private Sub ImportSaveAs() Handles Import_SaveAs.Click
        SaveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        SaveFileDialog1.FileName = DownloadedCostumeName
        SaveFileDialog1.ShowDialog()

        If SaveFileDialog1.FileName = Nothing Then Exit Sub
        My.Computer.FileSystem.CopyFile(TmpLoc, SaveFileDialog1.FileName, True)
    End Sub

#End Region

#Region "Settings"

    Sub CheckSettings()
        If My.Computer.FileSystem.FileExists(settingsFile) = False Then
            If MsgBox("CCM had detected this is your first time running this program. Do you wish to set up your settings?" & vbNewLine &
                      "Note: If you press no, the program will exit.",
                   MsgBoxStyle.YesNo, "First time start-up") = MsgBoxResult.No Then
                Application.Exit()
            Else
                TabControl1.SelectedTab = TabPage_Settings
                FirstRun = True
            End If
        End If
    End Sub

    Sub LoadSettings() Handles MyBase.Shown
        'Versions
        Update_TextBox_Installed.Text = My.Application.Info.Version.ToString
        Update_TextBox_Online.Text = My.Application.Info.Version.ToString 'PLACEFOLDER FOR NOW
        'TODO: Check up againts online version on GitHub.

        'Champions Install Folders
        If FirstRun Then
            If GetCOFolderFromRegistry() <> "" Then
                InstallFolder_ListBox.Items.Add(GetCOFolderFromRegistry)
                InstallFolder_AddReg.Enabled = False
            End If
        Else
            Dim output As String = ""
            Dim deserializedProduct As settingsJSON = JsonConvert.DeserializeObject(Of settingsJSON)(My.Computer.FileSystem.ReadAllText(settingsFile))

            For Each itm In deserializedProduct.InstallFolders
                If itm = GetCOFolderFromRegistry() Then InstallFolder_AddReg.Enabled = False
                InstallFolder_ListBox.Items.Add(itm)
                ComboBox1.Items.Add(itm)
            Next
            ComboBox1.SelectedIndex = 0
        End If



        'Delete settings file
    End Sub

    Sub SaveCoFolderSettings() Handles InstallFolder_SaveBtn.Click
        Dim settingsJSON As New settingsJSON()

        settingsJSON.InstallFolders = InstallFolder_ListBox.Items.Cast(Of String).ToArray

        Dim output As String = JsonConvert.SerializeObject(settingsJSON, formatting:=1)
        'Debug.Write(output)

        CheckIfSettingsFolderExists()
        My.Computer.FileSystem.WriteAllText(settingsFile, output, False)

        'Deserialize and update the dropbox and such from the JSON
        Dim SelectedFolder As Integer = ComboBox1.SelectedIndex
        InstallFolder_ListBox.Items.Clear()
        ComboBox1.Items.Clear()

        Dim deserializedProduct As settingsJSON = JsonConvert.DeserializeObject(Of settingsJSON)(My.Computer.FileSystem.ReadAllText(settingsFile))

        For Each itm In deserializedProduct.InstallFolders
            If itm = GetCOFolderFromRegistry() Then InstallFolder_AddReg.Enabled = False
            InstallFolder_ListBox.Items.Add(itm)
            ComboBox1.Items.Add(itm)
        Next
        If ComboBox1.Items.Count - 1 >= SelectedFolder Then
            ComboBox1.SelectedIndex = SelectedFolder
        ElseIf ComboBox1.Items.Count = 0
            'No items to select. Why? Prevent this?
        Else
            ComboBox1.SelectedIndex = 0
        End If

    End Sub

    Sub CheckIfSettingsFolderExists()
        If My.Computer.FileSystem.DirectoryExists(appFolder) = False Then
            My.Computer.FileSystem.CreateDirectory(appFolder)
        End If
    End Sub

    Private Class settingsJSON
        Property InstallFolders As String()
    End Class

    Public Function GetCOFolderFromRegistry()
        Dim softwareKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software")
        Dim CrypticKey As RegistryKey = softwareKey.OpenSubKey("Cryptic")
        Dim FoundFolder As Boolean = False
        If CrypticKey.ToString() <> "" Then
            Dim COKey As RegistryKey = CrypticKey.OpenSubKey("Champions Online")
            If COKey.ToString() <> "" Then
                If COKey.GetValue("InstallLocation") IsNot Nothing Then
                    Dim CrypticInstallFolder As String = COKey.GetValue("InstallLocation").ToString() & "\Champions Online\Live\screenshots"
                    If CrypticInstallFolder <> "" AndAlso Directory.Exists(CrypticInstallFolder) Then
                        Return CrypticInstallFolder
                        Exit Function
                    End If
                End If
            End If
        End If

        Return ""
    End Function

#Region "Folder Manager"


    Sub AddManualFolder() Handles InstallFolder_Manual_Button.Click
        InstallFolder_ListBox.Items.Add(InstallFolder_Manual_TextBox.Text)
    End Sub

    Sub AddRegFolder() Handles InstallFolder_AddReg.Click
        InstallFolder_ListBox.Items.Add(GetCOFolderFromRegistry)
        InstallFolder_AddReg.Enabled = False
    End Sub

    Sub DeleteFolder() Handles InstallFolder_RemoveBtn.Click
        If InstallFolder_ListBox.SelectedItem = GetCOFolderFromRegistry() Then InstallFolder_AddReg.Enabled = True

        InstallFolder_ListBox.Items.RemoveAt(InstallFolder_ListBox.SelectedIndex)
    End Sub

    Sub InstallFolderMoveUp() Handles InstallFolder_UpBtn.Click
        'Make sure our item is not the first one on the list.
        If InstallFolder_ListBox.SelectedIndex > 0 Then
            Dim I = InstallFolder_ListBox.SelectedIndex - 1
            InstallFolder_ListBox.Items.Insert(I, InstallFolder_ListBox.SelectedItem)
            InstallFolder_ListBox.Items.RemoveAt(InstallFolder_ListBox.SelectedIndex)
            InstallFolder_ListBox.SelectedIndex = I
        End If
    End Sub

    Sub InstallGolderMoveDown() Handles InstallFolder_DownBtn.Click
        'Make sure our item is not the last one on the list.
        If InstallFolder_ListBox.SelectedIndex < InstallFolder_ListBox.Items.Count - 1 Then
            'Insert places items above the index you supply, since we want
            'to move it down the list we have to do + 2
            Dim I = InstallFolder_ListBox.SelectedIndex + 2
            InstallFolder_ListBox.Items.Insert(I, InstallFolder_ListBox.SelectedItem)
            InstallFolder_ListBox.Items.RemoveAt(InstallFolder_ListBox.SelectedIndex)
            InstallFolder_ListBox.SelectedIndex = I - 1
        End If
    End Sub

#End Region

#Region "DeleteSettings / Uninstall"
    Sub RemoveSettingsFile() Handles ResetCCMBtn.Click
        Dim stringBuilder As String
        stringBuilder = "Are you sure you wish to remove CCM's settings file?" & vbNewLine & vbNewLine
        stringBuilder = stringBuilder & "By selecting yes, CCM will delete it's own settings file." & vbNewLine
        stringBuilder = stringBuilder & "Then you can freely delete CCM's exe to completly remove CCM." & vbNewLine
        stringBuilder = stringBuilder & "Or you can run CCM again to generate a new settings file from scratch."

        If MsgBox(stringBuilder, MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            'Delete stuff
            My.Computer.FileSystem.DeleteDirectory(appFolder, FileIO.UIOption.AllDialogs, FileIO.RecycleOption.DeletePermanently)
        End If
    End Sub

    'Private Sub ComboBox1_DropDown(sender As Object, e As EventArgs)
    '    For i As Integer = 0 To 19
    '        Dim item As New RadListDataItem("Item " + i.ToString())
    '        Me.radDropDownList1.Items.Add(item)
    '    Next
    '    ' assuming you have standard size items (16px height) then set min height based on #items x 16 + a little padding
    '    Me.radDropDownList1.DropDownMinSize = New Size(0, (Me.radDropDownList1.Items.Count * 16) + 2)
    'End Sub

    Private Sub AdjustWidthComboBox_DropDown(sender As Object, e As System.EventArgs) Handles ComboBox1.DropDown
        Dim senderComboBox As ComboBox = DirectCast(sender, ComboBox)
        Dim width As Integer = senderComboBox.DropDownWidth
        Dim g As Graphics = senderComboBox.CreateGraphics()
        Dim font As Font = senderComboBox.Font
        Dim vertScrollBarWidth As Integer = If((senderComboBox.Items.Count > senderComboBox.MaxDropDownItems), SystemInformation.VerticalScrollBarWidth, 0)

        Dim newWidth As Integer
        For Each s As String In DirectCast(sender, ComboBox).Items
            newWidth = CInt(g.MeasureString(s, font).Width) + vertScrollBarWidth
            If width < newWidth Then
                width = newWidth
            End If
        Next
        senderComboBox.DropDownWidth = width
    End Sub

#End Region

#End Region

End Class
