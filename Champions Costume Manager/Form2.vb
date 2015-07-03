Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Diagnostics
Imports System.Windows.Forms
Imports Microsoft.Win32
Imports System.IO
Imports System.Windows.Media.Imaging
Imports System.Collections
Imports System.Globalization
Imports System.Reflection

Namespace cynagecocrt
    Partial Public Class MainForm
        Inherits Form
        Private ImageData As Byte()
        Private OriginalCostumeFileName As String
        Private CurrentFileDate As Double
        Private CostumeFilenameFakeDate As String
        Private CurrentAccountName As String
        Private CurrentCostumeName As String
        Private AccountNameStart As Integer
        Private CostumeNameStart As Integer
        Private AccountNameLength As Integer
        Private CostumeNameLength As Integer
        Private StartOfNameStuff As Integer
        Private EndOfNameStuff As Integer
        Private StartOfMainLengthStuff As Integer
        Private MainLengthValue As Integer
        Private HexCostumeArray As String()
        Private CostumeImage As Image
        Private TmpImage As Bitmap
        Private ServerTypeList As ArrayList
        Private CurrentSelectedCostumeIndex As Integer
        Private version As String
        Private SortColumnID As Integer = -1

        Public Sub New()
            InitializeComponent()
            InitApp()
        End Sub

        Private Sub InitApp()
            version = "0.9a"
            GetCOFolder()
            Dim FolderFound As Boolean = VerifyCOFolder()
            While FolderFound = False
                MessageBox.Show("I'm sorry, that doesn't appear to be a Champions Online installation folder. In the next window, please browse to its location.", "Folder Not Found", MessageBoxButtons.OK, MessageBoxIcon.[Error])
                ManualLocateFolder()
                FolderFound = VerifyCOFolder()
            End While

            GetServerList()
            cmb_Server.Text = "Live"
            CurrentSelectedCostumeIndex = 0
            Dim TargetServerType As String = "Live"
            If Properties.Settings.[Default].ServerType.ToString() <> "" Then
                TargetServerType = Properties.Settings.[Default].ServerType.ToString()
            End If

            cmb_Gender.Text = "Both"
            txt_CostumeNameInTailor.Text = ""
            CostumeFilenameFakeDate = ""

            Dim hasServerType As Boolean = False
            For Each ServerType As String In cmb_Server.Items
                If TargetServerType = ServerType Then
                    hasServerType = True
                    Exit For
                End If
            Next

            If Not hasServerType Then
                TargetServerType = cmb_Server.Items(0).ToString()
            End If

            cmb_Server.Text = TargetServerType

            If Properties.Settings.[Default].GenderType.ToString() <> "" Then
                cmb_Gender.Text = Properties.Settings.[Default].GenderType.ToString()
            End If


            If Properties.Settings.[Default].FilenameWidth <> 0 Then
                lvw_CostumeList.Columns(0).Width = Properties.Settings.[Default].FilenameWidth
            End If

            If Properties.Settings.[Default].DatecolumnWidth <> 0 Then
                lvw_CostumeList.Columns(1).Width = Properties.Settings.[Default].DatecolumnWidth
            End If

            If Properties.Settings.[Default].height <> 0 Then
                Me.Height = Properties.Settings.[Default].height
            End If

            If Properties.Settings.[Default].width <> 0 Then
                Me.Width = Properties.Settings.[Default].width
            End If

            If Properties.Settings.[Default].xPos <> 0 AndAlso Properties.Settings.[Default].yPos <> 0 Then
                Me.Location = New Point(Properties.Settings.[Default].xPos, Properties.Settings.[Default].yPos)
            Else
                Me.StartPosition = FormStartPosition.CenterScreen
            End If

            cbx_CopyFilename.Checked = Properties.Settings.[Default].CopyFileFromName
            cbx_Date.Checked = Properties.Settings.[Default].PreserveDate
            InitUI()
        End Sub

        Private Sub GetServerList()
            ServerTypeList = New ArrayList()
            cmb_Server.Items.Clear()


            Dim LiveFolder As String = Properties.Settings.[Default].COFolder.ToString() + "\Live\screenshots"
            If Directory.Exists(LiveFolder) Then
                ServerTypeList.Add("Live")
            End If

            Dim PlaytestFolder As String = Properties.Settings.[Default].COFolder.ToString() + "\Playtest\screenshots"
            If Directory.Exists(PlaytestFolder) Then
                ServerTypeList.Add("Test")
            End If

            For Each ServerType As String In ServerTypeList
                cmb_Server.Items.Add(ServerType)
            Next
        End Sub

        Private Sub InitUI()
            Me.Text = Convert.ToString("Champions Online Costume Renaming Tool v") & version
            Dim ThisAssembly As Assembly = Assembly.GetExecutingAssembly()
            Dim CostumeOptionsImgStream As Stream = ThisAssembly.GetManifestResourceStream("cynagecocrt.TextLabels.CostumeOptions.png")
            Dim CostumeOptionsImg As New Bitmap(CostumeOptionsImgStream)
            Dim pbx_C As New PictureBox()
            pbx_C.Location = New Point(10, 10)
            pbx_C.Height = 23
            pbx_C.Width = 218

            Dim p1 As SplitterPanel = splitContainer3.Panel1
            p1.Controls.Add(pbx_C)

            pbx_C.Image = CostumeOptionsImg

            Dim FilterImgStream As Stream = ThisAssembly.GetManifestResourceStream("cynagecocrt.TextLabels.Filter.png")
            Dim FilterImg As New Bitmap(FilterImgStream)
            Dim pbx_F As New PictureBox()
            pbx_F.Location = New Point(10, 10)
            pbx_F.Height = 23
            pbx_F.Width = 77

            Dim p2 As SplitterPanel = splitContainer4.Panel1
            p2.Controls.Add(pbx_F)

            pbx_F.Image = FilterImg

            ' lst_CostumeList.Columns[0].Width = lst_CostumeList.Width - 25;

            'lst_CostumeList.Columns[0].Width = lst_CostumeList.Columns[0].Width - 2;
            ' Stream myStream = myAssembly.GetManifestResourceStream( "MyNamespace.SubFolder.MyImage.bmp" );

        End Sub

        Private Sub GetCOFolder()
            Dim softwareKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software")
            Dim CrypticKey As RegistryKey = softwareKey.OpenSubKey("Cryptic")
            Dim FoundFolder As Boolean = False
            If CrypticKey.ToString() <> "" Then
                Dim COKey As RegistryKey = CrypticKey.OpenSubKey("Champions Online")
                If COKey.ToString() <> "" Then
                    If COKey.GetValue("InstallLocation") IsNot Nothing Then
                        Dim CrypticInstallFolder As String = COKey.GetValue("InstallLocation").ToString() + "\Champions Online"
                        If CrypticInstallFolder <> "" AndAlso Directory.Exists(CrypticInstallFolder) Then
                            Properties.Settings.[Default].COFolder = CrypticInstallFolder
                            FoundFolder = True
                        End If
                    End If
                End If
            End If

            If Not FoundFolder Then
                MessageBox.Show("I'm sorry, but I couldn't locate your Champions Online installation folder. In the next window, please browse to its location.", "Folder Not Found", MessageBoxButtons.OK, MessageBoxIcon.[Error])
                ManualLocateFolder()
            End If
        End Sub

        Private Function VerifyCOFolder() As Boolean
            Dim CurrentFolder As String = ""
            If Properties.Settings.[Default].COFolder.ToString() <> "" Then
                CurrentFolder = Properties.Settings.[Default].COFolder.ToString()
            Else
                CurrentFolder = "C:/"
            End If

            Debug.WriteLine(CurrentFolder)
            Dim di As New DirectoryInfo(CurrentFolder)
            Dim SubfolderList As DirectoryInfo() = di.GetDirectories()
            Dim FolderType As String = ""
            For i As Integer = 0 To SubfolderList.Length - 1
                Dim ThisDir As DirectoryInfo = SubfolderList(i)
                Debug.WriteLine(ThisDir.Name)
                Dim found As Boolean = False
                Select Case ThisDir.Name
                    Case "Champions Online"
                        FolderType = "OfficialRoot"
                        found = True
                        Exit Select
                    Case "Live"
                        FolderType = "COFolder"
                        found = True
                        Exit Select
                    Case Else
                        Exit Select
                End Select

                If found Then
                    Exit For
                End If
            Next

            Dim RetVal As Boolean = False
            Select Case FolderType
                Case "OfficialRoot"
                    Debug.WriteLine("Cryptic Root. Setting folder to: " + di.FullName + "\Champions Online")
                    Properties.Settings.[Default].COFolder = di.FullName + "\Champions Online"
                    RetVal = True
                    Exit Select
                Case "COFolder"
                    Debug.WriteLine("Setting folder to: " + di.FullName)
                    Properties.Settings.[Default].COFolder = di.FullName
                    RetVal = True
                    Exit Select
                Case Else
                    Debug.WriteLine("Not real CO Folder")
                    Exit Select
            End Select
            'Properties.Settings.Default.

            Return RetVal
        End Function

        Private Sub ManualLocateFolder()
            If dlg_selectFolder.ShowDialog() = DialogResult.OK Then
                Properties.Settings.[Default].COFolder = dlg_selectFolder.SelectedPath.ToString()
            End If
        End Sub

        Private Sub LoadCostumeList(ByVal sender As Object, ByVal e As EventArgs)
            pbx_costume.Image = Nothing
            If CostumeImage IsNot Nothing Then
                CostumeImage.Dispose()
            End If
            Dim Folder As String = GetFolderName()
            Debug.WriteLine("Loading " + Folder.ToString() + " Costume List")
            If Folder = "" Then
                Return
            End If
            Dim ScreenshotPath As String = Folder.ToString()
            If Not Directory.Exists(ScreenshotPath) Then
                MessageBox.Show("Sorry, but I cannot find the screenshots folder for that server (" + Folder.ToString() + ")." & vbLf & "Are you sure you have saved costumes?")
                Return
            End If

            CleanFileList()

            Dim di As New DirectoryInfo(ScreenshotPath)
            Dim AllFiles As FileInfo() = di.GetFiles("Costume_*.jpg")
            For Each ThisFile As FileInfo In AllFiles
                Dim AddItem As Boolean = True
                If cmb_Gender.Text <> "Both" Then
                    Dim g As String() = ExtractDataFromImage(di.FullName + "\" + ThisFile.Name, New String() {"47", "65", "6E", "64", "65", "72", _
                     "3A"}, 1, 89)

                    Dim target As String = "61"
                    If cmb_Gender.Text = "Female" Then
                        target = "65"
                    End If

                    If target <> g(0) Then
                        AddItem = False
                    End If
                End If

                If txt_Filter.Text <> "" Then
                    Dim contains As Boolean = ThisFile.Name.IndexOf(txt_Filter.Text, StringComparison.OrdinalIgnoreCase) >= 0
                    If Not contains Then
                        AddItem = False
                    End If
                End If

                If AddItem Then
                    Dim li As New ListViewItem()
                    li.Text = ThisFile.Name
                    li.SubItems.Add(ThisFile.LastWriteTime.ToShortDateString() + " " + ThisFile.LastWriteTime.ToShortTimeString())
                    lvw_CostumeList.Items.Add(li)
                End If
            Next

            If lvw_CostumeList.Items.Count > CurrentSelectedCostumeIndex Then
                lvw_CostumeList.TopItem = lvw_CostumeList.Items(CurrentSelectedCostumeIndex)
            End If
            txt_currentAction.Text = "Number of Costumes: " + lvw_CostumeList.Items.Count.ToString()
        End Sub

        Private Function GetFolderName() As String
            Dim Folder As New StringBuilder()
            Folder.Append(Properties.Settings.[Default].COFolder.ToString())
            Select Case cmb_Server.Text
                Case "Live"
                    Folder.Append("\Live")
                    Exit Select
                Case "Test"
                    Folder.Append("\Playtest")
                    Exit Select
                Case Else
                    MessageBox.Show("Sorry, I don't know where to find costumes for that servertype")
                    Return ""
            End Select

            Folder.Append("\Screenshots")
            Return Folder.ToString()
        End Function

        Private Sub CleanFileList()
            lvw_CostumeList.Items.Clear()
        End Sub

        Private Sub LoadCostume(ByVal sender As Object, ByVal e As EventArgs)
            If lvw_CostumeList.SelectedIndices.Count > 0 Then
                ' Let's load the image first..
                pbx_costume.Image = Nothing

                CurrentSelectedCostumeIndex = lvw_CostumeList.SelectedIndices(0)

                Dim iColl As ListView.SelectedListViewItemCollection = lvw_CostumeList.SelectedItems
                If iColl.Count = 0 Then
                    Return
                End If
                OriginalCostumeFileName = lvw_CostumeList.SelectedItems(0).Text.ToString()
                Dim Folder As String = GetFolderName()
                If Folder = "" Then
                    Return
                End If
                Dim ImagePath As String = Convert.ToString(Folder & Convert.ToString("\")) & OriginalCostumeFileName
                Try
                    CostumeImage = Image.FromFile(ImagePath)
                    TmpImage = New Bitmap(CostumeImage)
                    CostumeImage.Dispose()
                    pbx_costume.Image = Image.FromHbitmap(TmpImage.GetHbitmap())
                Catch e2 As Exception
                    MessageBox.Show("Sorry, but the image for this costume could not be loaded. You may still be able to edit the costume name, however.")
                End Try

                ' Now let's drag the metadata out of the image..
                CurrentAccountName = ""
                CurrentCostumeName = ""
                Dim DateString As String = ""
                AccountNameStart = 0
                CostumeNameStart = 0
                AccountNameLength = 0
                CostumeNameLength = 0

                ImageData = ReadImageAsBinaryData(ImagePath)
                HexCostumeArray = BitConverter.ToString(ImageData).Split("-"c)
                Dim foundCount As Integer = 0
                Dim found As Boolean = False

                For i As Integer = 0 To HexCostumeArray.Length - 1
                    found = False
                    If i + 5 < HexCostumeArray.Length Then
                        Dim char1 As String = HexCostumeArray(i)
                        Dim char2 As String = HexCostumeArray(i + 1)
                        Dim char3 As String = HexCostumeArray(i + 2)
                        Dim char4 As String = HexCostumeArray(i + 3)

                        If char1 = "FF" AndAlso char2 = "ED" Then
                            StartOfMainLengthStuff = i + 2
                            Dim hexMainLength As String = HexCostumeArray(i + 2) + HexCostumeArray(i + 3)
                            'Debug.WriteLine(HexCostumeArray[i + 2] + HexCostumeArray[i + 3]);
                            'Debug.WriteLine("Main length: " + MainLengthValue);
                            MainLengthValue = Integer.Parse(HexCostumeArray(i + 2) + HexCostumeArray(i + 3), System.Globalization.NumberStyles.AllowHexSpecifier)
                        End If

                        If char1 = "1C" AndAlso char2 = "02" AndAlso char3 = "78" AndAlso char4 = "00" Then
                            foundCount += 1
                            found = True
                        End If

                        If foundCount = 1 AndAlso found Then
                            StartOfNameStuff = i
                            AccountNameStart = i + 5
                            AccountNameLength = Integer.Parse(HexCostumeArray(i + 4), System.Globalization.NumberStyles.AllowHexSpecifier)
                        End If

                        If foundCount = 2 AndAlso found Then
                            CostumeNameStart = i + 5

                            CostumeNameLength = Integer.Parse(HexCostumeArray(i + 4), System.Globalization.NumberStyles.AllowHexSpecifier)
                        End If
                        If foundCount = 3 AndAlso found Then
                            EndOfNameStuff = i
                            Exit For
                        End If
                    End If
                Next

                'Debug.WriteLine("Number of separators: " + foundCount.ToString());
                If foundCount = 2 Then
                    '
                    '                     * Single tag costume, we can edit it, but not put it back the way it was.
                    '                     * Also, single tag costume means we have to reshuffle our 2nd indexes.
                    '                     

                    EndOfNameStuff = CostumeNameStart - 5
                    CostumeNameStart = 0

                    CostumeNameLength = 0
                End If

                If AccountNameStart <> 0 Then
                    Dim AccountNameEnd As Integer = AccountNameStart + AccountNameLength
                    Dim AccountNameBytestream As Byte() = New Byte(AccountNameLength - 1) {}
                    Dim bCount As Integer = 0
                    For i As Integer = AccountNameStart To AccountNameEnd - 1
                        Dim b As Byte = Convert.ToByte(HexCostumeArray(i), 16)
                        AccountNameBytestream(bCount) = b
                        bCount += 1
                    Next

                    CurrentAccountName = ASCIIEncoding.ASCII.GetString(AccountNameBytestream)
                End If

                If CostumeNameStart <> 0 Then
                    Dim CostumeNameEnd As Integer = CostumeNameStart + CostumeNameLength
                    Dim CostumeNameBytestream As Byte() = New Byte(CostumeNameLength - 1) {}
                    Dim bCount As Integer = 0
                    For i As Integer = CostumeNameStart To CostumeNameEnd - 1
                        Dim b As Byte = Convert.ToByte(HexCostumeArray(i), 16)
                        CostumeNameBytestream(bCount) = b
                        bCount += 1
                    Next

                    CurrentCostumeName = ASCIIEncoding.ASCII.GetString(CostumeNameBytestream)
                End If

                Dim tmp As New Stack(Of String)(OriginalCostumeFileName.Split("_"c))
                Dim timestamp As String = tmp.Pop().Replace(".jpg", "")
                CostumeFilenameFakeDate = timestamp
                Dim aTmpName As String() = tmp.ToArray(Of String)()
                Dim sTmpName As String = String.Join("_", aTmpName)

                Dim tmp2 As New Queue(Of String)(sTmpName.Split("_"c))
                Dim hastimestamp As Boolean = True
                Try
                    CurrentFileDate = Double.Parse(timestamp)
                Catch
                    Debug.WriteLine("No Timestamp")
                    CostumeFilenameFakeDate = ""
                    hastimestamp = False
                End Try

                aTmpName = tmp.ToArray(Of String)()
                Array.Reverse(aTmpName)
                sTmpName = String.Join("_", aTmpName)

                ' And finally, the date stuff
                If hastimestamp Then
                    DateString = ConvertDate(CurrentFileDate)
                Else
                    sTmpName += Convert.ToString("_") & timestamp
                    DateString = ""
                End If

                txt_CostumeName.Text = CurrentCostumeName
                txt_AccountName.Text = CurrentAccountName
                sTmpName = sTmpName.Replace("Costume_", "")
                txt_FileName.Text = sTmpName.Replace("Costume", "")

                If cbx_CopyFilename.Checked Then
                    UpdateTailorName(Nothing, Nothing)
                End If
                If hastimestamp Then
                    txt_CostumeNameInTailor.Text = Convert.ToString((CurrentAccountName & CurrentCostumeName) + " ") & DateString
                Else
                    txt_CostumeNameInTailor.Text = CurrentAccountName & CurrentCostumeName
                End If
            End If
        End Sub

        Private Function ConvertDate(ByVal timestamp As Double) As String
            Dim dateTime As DateTime = New System.DateTime(1970, 1, 1, 0, 0, 0, _
             0)
            dateTime = dateTime.AddSeconds(timestamp)
            dateTime = dateTime.AddSeconds(946684800)
            ' Cryptic Epoch of Jan 1, 2000
            Dim TextDate As String = dateTime.ToString("yyyy-MM-dd hh:mm:ss")
            Return TextDate

        End Function

        Private Sub WriteCostume(ByVal sender As Object, ByVal e As EventArgs)

            ' Scary! Writing Costumes!
            '
            '             * File structure is:
            '             * STUFF_BEFORE_INITIAL_LENGTH_BITS
            '             * FFED
            '             * LENGTH BYTES(2)
            '             * STUFF_INBETWEEN
            '             * 1C 02 78 00 bb
            '             * NAME_STUFF
            '             * 1C 02 78 00 10
            '             * REST_OF_FILE
            '             * 
            '             * startOfNameStuff is the index that the first 1C lives at
            '             * endOfNameStuff is the index of the byte 1 -before- the last 1C lives at.
            '             



            '
            '            Debug.WriteLine("Name stuff starts at: " + StartOfNameStuff.ToString());
            '            for (int i = StartOfNameStuff; i < EndOfNameStuff; i++)
            '            {
            '                string hexbyte = HexCostumeArray[i];
            '                Debug.Write(hexbyte + " ");
            '            }
            '            Debug.WriteLine("Name stuff ends at: " + EndOfNameStuff.ToString());
            '            


            Dim NewImageHexArray As New ArrayList()

            '    Debug.WriteLine("Initial Length bits live at: " + StartOfMainLengthStuff.ToString());
            For i As Integer = StartOfMainLengthStuff To StartOfMainLengthStuff + 1
                '  Debug.Write(hexbyte + " ");
                Dim hexbyte As String = HexCostumeArray(i)
            Next


            ' Write pre-name stuff
            For i As Integer = 0 To StartOfNameStuff - 1
                Dim hexbyte As String = HexCostumeArray(i)
                NewImageHexArray.Add(hexbyte)
            Next

            ' Write first name
            If txt_AccountName.Text <> "" Then
                ' Write pre-name separator
                NewImageHexArray = AddSeparator(NewImageHexArray)
                NewImageHexArray = AddItem(NewImageHexArray, txt_AccountName.Text)
            End If


            If txt_CostumeName.Text <> "" Then
                NewImageHexArray = AddSeparator(NewImageHexArray)
                NewImageHexArray = AddItem(NewImageHexArray, txt_CostumeName.Text)
            End If

            ' Write post-name stuff
            For i As Integer = EndOfNameStuff To HexCostumeArray.Length - 1
                Dim hexbyte As String = HexCostumeArray(i)
                NewImageHexArray.Add(hexbyte)
            Next

            ' Now find FF DB
            Dim ffdbLocation As Integer = 0

            For i As Integer = 0 To NewImageHexArray.Count - 1
                If NewImageHexArray(i).ToString() = "FF" AndAlso NewImageHexArray(i + 1).ToString() = "DB" Then
                    ffdbLocation = i
                    Exit For
                End If
            Next

            Dim numbytesused As Integer = ffdbLocation - StartOfMainLengthStuff
            '    Debug.WriteLine("Offset bytes to write: " + numbytesused.ToString());
            Dim hexbytesused As String = Convert.ToString(numbytesused, 16).ToUpper().PadLeft(4, "0"c)
            '    Debug.WriteLine("Bytes to write in hex: " + hexbytesused);

            '    Debug.WriteLine("Original first length hex pair: " + NewImageHexArray[StartOfMainLengthStuff].ToString());
            '    Debug.WriteLine("Original second length hex pair: " + NewImageHexArray[StartOfMainLengthStuff + 1].ToString());

            Dim newFirstPair As String = hexbytesused(0).ToString() + hexbytesused(1).ToString()
            Dim newSecondPair As String = hexbytesused(2).ToString() + hexbytesused(3).ToString()

            '     Debug.WriteLine("New first length hex pair: " + newFirstPair);
            '     Debug.WriteLine("New second length hex pair: " + newSecondPair);

            NewImageHexArray(StartOfMainLengthStuff) = newFirstPair
            NewImageHexArray(StartOfMainLengthStuff + 1) = newSecondPair

            '    Debug.WriteLine("array list length: " + NewImageHexArray.Count.ToString());
            '    Debug.WriteLine("original image size: " + HexCostumeArray.Length.ToString());
            Dim newImage As Byte() = New Byte(NewImageHexArray.Count - 1) {}
            For i As Integer = 0 To NewImageHexArray.Count - 1
                Dim b As Byte = Convert.ToByte(NewImageHexArray(i).ToString(), 16)
                newImage(i) = b
            Next

            Dim CostumeFileName As String = "Costume_" + txt_FileName.Text
            If cbx_Date.Checked Then
                CostumeFileName += "_" + CostumeFilenameFakeDate.ToString()
            End If

            CostumeFileName += ".jpg"

            CostumeImage.Dispose()
            pbx_costume.Image = Nothing
            'string ImagePath = "E:\\tmp.jpg";
            Dim Folder As String = GetFolderName()
            If Folder = "" Then
                Return
            End If
            Dim ImagePath As String = Convert.ToString(Folder & Convert.ToString("\")) & CostumeFileName

            '
            '             * Okay, so we should do some intelligent name checking here.
            '             * 
            '             * If the new name is the same as the old name, we're replacing the same file
            '             * and we don't ask.
            '             * 
            '             * BUT.. If it's different _AND_ a file exists, then we're obviously trying to overwrite a different costume.
            '             * So we better ask.
            '            


            Dim deleteCurrentCostumeFileName As Boolean = True
            If CostumeFileName = OriginalCostumeFileName Then
                Debug.WriteLine("A")
                deleteCurrentCostumeFileName = False
            Else
                Debug.WriteLine("B")
                If File.Exists(ImagePath) Then
                    Debug.WriteLine("C")
                    For i As Integer = 2 To 999999

                        Dim NewFileName As String = ImagePath.Replace(".jpg", "") + " (" + i.ToString() + ").jpg"
                        Debug.WriteLine(Convert.ToString("Checking for ") & NewFileName)
                        If Not File.Exists(NewFileName) Then
                            Debug.WriteLine(NewFileName & Convert.ToString(" not found!!"))
                            ImagePath = NewFileName
                            Exit For
                        End If
                    Next

                End If
            End If

            Debug.WriteLine(Convert.ToString("CostumeFileName: ") & CostumeFileName)
            Debug.WriteLine(Convert.ToString("CurrentCostumeFileName: ") & OriginalCostumeFileName)
            Debug.WriteLine(Convert.ToString("ImagePath: ") & ImagePath)
            If File.Exists(ImagePath) Then
                File.Delete(ImagePath)
            End If

            Dim fOut As New FileStream(ImagePath, FileMode.Create)
            fOut.Write(newImage, 0, newImage.Length)
            fOut.Close()


            If deleteCurrentCostumeFileName Then
                ' unload image
                Try

                    Debug.WriteLine(Convert.ToString((Convert.ToString("trying to delete ") & Folder) + "\") & OriginalCostumeFileName)
                    File.Delete(Convert.ToString(Folder & Convert.ToString("\")) & OriginalCostumeFileName)
                Catch e3 As Exception
                End Try
            End If

            cleanup()
            LoadCostumeList(Nothing, Nothing)
        End Sub

        Private Sub cleanup()
            OriginalCostumeFileName = ""
            CurrentFileDate = 0
            CostumeFilenameFakeDate = ""
            CurrentAccountName = ""
            CurrentCostumeName = ""
            AccountNameStart = 0
            CostumeNameStart = 0
            AccountNameLength = 0
            CostumeNameLength = 0
            StartOfNameStuff = 0
            EndOfNameStuff = 0
            txt_AccountName.Text = ""
            txt_CostumeName.Text = ""
            txt_CostumeNameInTailor.Text = ""
            txt_currentAction.Text = ""
            txt_FileName.Text = ""
        End Sub

        Private Function AddSeparator(ByVal arr As ArrayList) As ArrayList
            arr.Add("1C")
            arr.Add("02")
            arr.Add("78")
            arr.Add("00")
            Return arr
        End Function

        Private Function AddItem(ByVal arr As ArrayList, ByVal s As String) As ArrayList
            Dim strlen As Integer = s.Length
            Dim hexLength As String = Convert.ToString(strlen, 16).ToUpper().PadLeft(2, "0"c)
            Debug.WriteLine(Convert.ToString((s & Convert.ToString(" is length ")) + strlen.ToString() + " which is hex: ") & hexLength)
            arr.Add(hexLength)
            Dim AccountNameToBeWrittenInBytes As Byte() = ASCIIEncoding.UTF8.GetBytes(s)
            Debug.WriteLine(BitConverter.ToString(AccountNameToBeWrittenInBytes))
            Dim hex As String() = BitConverter.ToString(AccountNameToBeWrittenInBytes).Split("-"c)
            For Each h As String In hex
                arr.Add(h)
            Next
            Return arr
        End Function

        Private Function ReadImageAsBinaryData(ByVal path As String) As Byte()
            Debug.WriteLine(Convert.ToString("ReadImageAsBinaryData called from LoadCostume for ") & path)
            Dim fStream As New FileStream(path, FileMode.Open, FileAccess.Read)

            Dim length As Integer = CInt(fStream.Length)
            Dim ImageData As Byte() = New Byte(length - 1) {}
            Dim count As Integer
            Dim sum As Integer = 0

            While (InlineAssignHelper(count, fStream.Read(ImageData, sum, length - sum))) > 0
                sum += count
            End While

            fStream.Close()
            Debug.WriteLine("Closing image stream after reading image")

            Return ImageData
        End Function

        Private Sub UpdateTailorName(ByVal sender As Object, ByVal e As EventArgs)
            txt_CostumeNameInTailor.Text = txt_AccountName.Text + txt_CostumeName.Text

            ' update the saved file name
            ' If we do this before we check whether or not to add the date.. 
            ' ... we don't have to remove the date to display it in this box
            ' (which doesn't show the stupid cryptic timestamp, like it doesn't show the Costume_ prefix)
            If cbx_CopyFilename.Checked Then
                txt_FileName.Text = txt_CostumeNameInTailor.Text
            End If

            ' Now add date, if we a) want to, and b) actually have a date to add.
            If cbx_Date.Checked AndAlso CostumeFilenameFakeDate <> "" Then
                txt_CostumeNameInTailor.Text += Convert.ToString(" ") & ConvertDate(Double.Parse(CostumeFilenameFakeDate.ToString()))
            End If

        End Sub

        Private Sub CheckForDisclaimer(ByVal sender As Object, ByVal e As EventArgs)
            If Properties.Settings.[Default].hasRun = False Then
                Dim disclaimer As New StringBuilder()
                disclaimer.AppendLine("This program is provided as-is, with no warranties of any kind. By")
                disclaimer.AppendLine("using this program you agree to not hold the author(s) responsible")
                disclaimer.AppendLine("for any loss of data and information that may arise. It is recommended")
                disclaimer.AppendLine("that you back up your costume files before using this program. If")
                disclaimer.AppendLine("you do not agree to these terms, press 'Cancel' below and the program")
                disclaimer.AppendLine("will close")
                disclaimer.AppendLine("")
                disclaimer.AppendLine("Would you like to back up your costume files now?")
                Dim dr As DialogResult = MessageBox.Show(disclaimer.ToString(), "Disclaimer", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning)
                If dr = DialogResult.Cancel Then
                    Application.[Exit]()
                End If

                If dr = DialogResult.Yes Then
                    BackupFiles()
                End If
                Properties.Settings.[Default].hasRun = True
            End If
        End Sub

        Private Sub BackupFiles()
            GetCOFolder()
            Dim LiveFolder As String = Properties.Settings.[Default].COFolder.ToString() + "\Live\screenshots"
            If Directory.Exists(LiveFolder) Then
                txt_currentAction.Text = "Backing up live server costumes. Please wait.."
                If Directory.Exists(Properties.Settings.[Default].COFolder.ToString() + "\COCRT\Live") Then
                    Directory.Delete(Properties.Settings.[Default].COFolder.ToString() + "\COCRT\Live", True)
                End If
                Directory.CreateDirectory(Properties.Settings.[Default].COFolder.ToString() + "\COCRT\Live")
                Dim di As New DirectoryInfo(LiveFolder)
                Dim FileList As FileInfo() = di.GetFiles()
                For Each file As FileInfo In FileList
                    file.CopyTo(Properties.Settings.[Default].COFolder.ToString() + "\COCRT\Live\" + file.Name)
                    UpdateCurrentAction("Copying " + file.Name)
                Next
            End If

            Dim TestFolder As String = Properties.Settings.[Default].COFolder.ToString() + "\PlayTest\screenshots"
            If Directory.Exists(TestFolder) Then
                txt_currentAction.Text = "Backing up test server costumes. Please wait.."

                If Directory.Exists(Properties.Settings.[Default].COFolder.ToString() + "\COCRT\PlayTest") Then
                    Directory.Delete(Properties.Settings.[Default].COFolder.ToString() + "\COCRT\PlayTest", True)
                End If
                Directory.CreateDirectory(Properties.Settings.[Default].COFolder.ToString() + "\COCRT\PlayTest")

                If Not Directory.Exists(Properties.Settings.[Default].COFolder.ToString() + "\COCRT\PlayTest") Then
                    Debug.WriteLine("Directory not created")
                    Directory.CreateDirectory(Properties.Settings.[Default].COFolder.ToString() + "\COCRT\PlayTest")
                End If
                Dim di As New DirectoryInfo(TestFolder)
                Dim FileList As FileInfo() = di.GetFiles()
                For Each file As FileInfo In FileList
                    file.CopyTo(Properties.Settings.[Default].COFolder.ToString() + "\COCRT\PlayTest\" + file.Name)
                    UpdateCurrentAction("Copying " + file.Name)
                Next
            End If
            UpdateCurrentAction("")
        End Sub

        Private Sub UpdateCurrentAction(ByVal s As String)
            txt_currentAction.Text = s
            Application.DoEvents()
        End Sub

        Private Sub ShowAccountNameWarning(ByVal sender As Object, ByVal e As EventArgs)
            ' We don't need this anymore, but I'll leave it in here for now ;)
            Return
            '
            '            if (Properties.Settings.Default.ShowAccountNameWarning)
            '            {
            '                AccountNameWarning ac = new AccountNameWarning();
            '                ac.ShowDialog();
            '            }

        End Sub

        Private Sub DoBackup(ByVal sender As Object, ByVal e As EventArgs)
            BackupFiles()
        End Sub

        Private Sub CloseApplication(ByVal sender As Object, ByVal e As EventArgs)
            Application.[Exit]()
        End Sub

        Private Function ExtractDataFromImage(ByVal imagePath As String, ByVal preSequence As String(), ByVal length As Integer, ByVal offset As Integer) As String()
            Debug.WriteLine(Convert.ToString("ExtractDataFromImage called by LoadCostumeList for ") & imagePath)
            Dim fi As New FileInfo(imagePath)

            Dim hexData As String()

            Dim sr As New FileStream(imagePath, FileMode.Open)
            Dim buffer As Byte() = New Byte(preSequence.Length - 1) {}
            Dim target As Byte() = New Byte(length - 1) {}
            Dim container As Byte() = New Byte(fi.Length - 1) {}
            Dim i As Integer = 0

            Dim foundGender As Boolean = False

            If offset > 0 Then
                While i < fi.Length - preSequence.Length
                    Dim b As Integer = sr.ReadByte()
                    If i > offset Then
                        Array.Copy(container, CLng(i - preSequence.Length), buffer, 0, CLng(preSequence.Length))
                        Dim thisSection As String() = BitConverter.ToString(buffer).Split("-"c)
                        If TestArrays(thisSection, preSequence) Then
                            ' Debug.WriteLine("Found gender (q) at " + i);
                            foundGender = True
                            sr.Read(target, 0, length)
                            Exit While
                        End If
                    End If
                    container(i) = CByte(b)
                    i += 1
                End While
            End If

            If Not foundGender Then
                While i < fi.Length - preSequence.Length
                    Dim b As Integer = sr.ReadByte()
                    If i >= preSequence.Length Then
                        Array.Copy(container, CLng(i - preSequence.Length), buffer, 0, CLng(preSequence.Length))
                        Dim thisSection As String() = BitConverter.ToString(buffer).Split("-"c)
                        If TestArrays(thisSection, preSequence) Then
                            sr.Read(target, 0, length)
                            Exit While
                        End If
                    End If
                    container(i) = CByte(b)
                    i += 1
                End While
            End If

            sr.Close()
            Debug.WriteLine("Closing file after ExtractDataFromImage")

            hexData = BitConverter.ToString(target).Split("-"c)
            Return hexData
        End Function

        Private Function TestArrays(ByVal s1 As String(), ByVal s2 As String()) As Boolean
            Dim found As Boolean = True
            For j As Integer = 0 To s1.Length - 1
                If s1(j) <> s2(j) Then
                    Return False
                End If
            Next
            Return found
        End Function

        Private Sub DoGenderCheck(ByVal sender As Object, ByVal e As EventArgs)
            CurrentSelectedCostumeIndex = 0
            If pbx_costume.Image IsNot Nothing Then
                pbx_costume.Image.Dispose()
            End If
            LoadCostumeList("FromDropDown", Nothing)
        End Sub

        Private Sub DeleteCostume(ByVal sender As Object, ByVal e As EventArgs)
            Dim CostumeToDelete As String = lvw_CostumeList.SelectedItems(0).Text
            Dim dr As DialogResult = MessageBox.Show((Convert.ToString("You are about to delete this costume (") & CostumeToDelete) + "). Are you sure?", "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            If dr = DialogResult.OK Then
                pbx_costume.Image = Nothing
                CostumeImage.Dispose()
                Dim Folder As String = GetFolderName()
                Dim FileName As String = Convert.ToString(Folder & Convert.ToString("//")) & CostumeToDelete
                If File.Exists(FileName) Then
                    File.Delete(FileName)
                End If

                LoadCostumeList(Nothing, Nothing)
            End If
        End Sub

        Private Sub DoResizeActions(ByVal sender As Object, ByVal e As EventArgs)
            '  lvw_CostumeList.Columns[0].Width = lvw_CostumeList.Width - 25;
        End Sub

        Private Sub DoFilter(ByVal sender As Object, ByVal e As EventArgs)
            LoadCostumeList(Nothing, Nothing)
        End Sub

        Private Sub SaveSettings(ByVal sender As Object, ByVal e As FormClosingEventArgs)
            Debug.WriteLine("saving colum0 width as : " + Me.Left)
            Properties.Settings.[Default].FilenameWidth = lvw_CostumeList.Columns(0).Width
            Properties.Settings.[Default].DatecolumnWidth = lvw_CostumeList.Columns(1).Width
            Properties.Settings.[Default].CopyFileFromName = cbx_CopyFilename.Checked
            Properties.Settings.[Default].PreserveDate = cbx_Date.Checked
            Properties.Settings.[Default].GenderType = cmb_Gender.Text
            Properties.Settings.[Default].ServerType = cmb_Server.Text
            Properties.Settings.[Default].xPos = Me.Location.X
            Properties.Settings.[Default].yPos = Me.Location.Y
            Properties.Settings.[Default].width = Me.Width
            Properties.Settings.[Default].height = Me.Height
            Properties.Settings.[Default].Save()
        End Sub

        Private Sub SortColumn(ByVal sender As Object, ByVal e As ColumnClickEventArgs)
            If e.Column <> SortColumnID Then
                Debug.WriteLine("New column, setting id")
                SortColumnID = e.Column
                lvw_CostumeList.Sorting = SortOrder.Ascending
            Else
                If lvw_CostumeList.Sorting = SortOrder.Ascending Then
                    Debug.WriteLine("setting to descending sort")
                    lvw_CostumeList.Sorting = SortOrder.Descending
                Else
                    lvw_CostumeList.Sorting = SortOrder.Ascending
                End If
            End If
            lvw_CostumeList.Sort()
            lvw_CostumeList.ListViewItemSorter = New ListViewItemComparer(e.Column, lvw_CostumeList.Sorting)
        End Sub
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
            target = value
            Return value
        End Function
    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================

End Class