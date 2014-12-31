Imports System.Threading.Tasks

Class MainWindow

    Private Async Sub Button_Click(sender As Object, e As RoutedEventArgs)
        StatusText.Text = "Scanning..."
        ScanEntireDriveButton.IsEnabled = False
        Await Task.Factory.StartNew(AddressOf EntireScan)
        StatusText.Text = "Completed."
        ScanEntireDriveButton.IsEnabled = True
    End Sub

    Private Sub EntireScan()
        Dim drives = My.Computer.FileSystem.Drives.Where(Function(d) Not d.Name.Contains("C"))
        Dim rootFolder As New Folder("Computer")
        For Each drive In drives
            rootFolder.Children.Add(New Folder(drive.RootDirectory, 0))
        Next
        rootFolder.ToXml().Save("Snapshoter_Entire_Hard_Drive_Scan_Reports.xml")
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim taskMethods = GetType(Task).GetMethods()
        If taskMethods.All(Function(m) m.Name <> "Run") Then
            MsgBox("You must have .NET Framework 4.5 or above installed.")
            Application.Current.Shutdown()
        End If
    End Sub
End Class
