Imports System.IO

Public Class Form1

    Private Extensions() As String
    Private Depth As Integer

    Private Sub btnDo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDo.Click
        Dim rootDir As New DirectoryInfo(txtPath.Text)
        Extensions = txtExtension.Text.Split(",")
        Depth = nudDepth.Value
        Dim xd As New XDocument
        Dim xe = <Root Path=<%= txtPath.Text %> Extensions=<%= txtExtension.Text %> Depth=<%= nudDepth.Value %>></Root>
        xd.Add(xe)
        If CheckBox1.Checked Then
            SearchDirCompact(rootDir, xe, 0)
        Else
            SearchDir(rootDir, xe, 0)
        End If
        xd.Save(txtPath.Text & String.Format("\Snapshot {0}.xml", Date.Now.ToShortDateString().Replace("/", "-")))
    End Sub

    Public Sub SearchDir(ByVal dir As DirectoryInfo, ByVal xe As XElement, ByVal i As Integer)
        If i > Depth Then
            Return
        End If
        Dim dirs = dir.GetDirectories()
        For Each d In dirs
            Dim xeDir = <Folder Name=<%= d.Name %>></Folder>
            xe.Add(xeDir)
            SearchDir(d, xeDir, i + 1)
        Next
        Dim files = dir.GetFiles()
        For Each f In files
            If Extensions.Length = 1 AndAlso Extensions(0) = "" OrElse Extensions.Contains(f.Extension.Trim(".")) Then
                Dim xeFile = <File Name=<%= f.Name %> Size=<%= GetMegabyteString(f.Length) %> Created=<%= f.CreationTime.ToShortDateString %>></File>
                xe.Add(xeFile)
            End If
        Next
    End Sub

    Public Function SearchDirCompact(ByVal dir As DirectoryInfo, ByVal xe As XElement, ByVal i As Integer) As Boolean
        If i > Depth Then
            Return False
        End If
        Dim indicator = False
        Dim dirs = dir.GetDirectories()
        For Each d In dirs
            Dim xeDir = <Folder Name=<%= d.Name %>></Folder>
            If SearchDirCompact(d, xeDir, i + 1) = True Then
                xe.Add(xeDir)
                indicator = True
            End If
        Next
        Dim files = dir.GetFiles()
        For Each f In files
            If Extensions.Length = 1 AndAlso Extensions(0) = "" OrElse Extensions.Contains(f.Extension.Trim(".")) Then
                Dim xeFile = <File Name=<%= f.Name %> Size=<%= GetMegabyteString(f.Length) %> Created=<%= f.CreationTime.ToShortDateString %>></File>
                xe.Add(xeFile)
                indicator = True
            End If
        Next
        Return indicator
    End Function

    Public Function GetMegabyteString(ByVal bytes As Long) As String
        Dim megabytes = bytes / 1024 / 1024
        Return megabytes.ToString("0.00") & "MB"
    End Function

    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
        Dim folderDlg As New FolderBrowserDialog()
        folderDlg.ShowNewFolderButton = False
        folderDlg.RootFolder = Environment.SpecialFolder.Desktop
        folderDlg.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        If folderDlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
            txtPath.Text = folderDlg.SelectedPath
        End If
    End Sub
End Class
