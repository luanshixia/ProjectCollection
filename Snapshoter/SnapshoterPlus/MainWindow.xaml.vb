Class MainWindow

    Public Shared Property Current As MainWindow

    Public Sub New()

        InitializeComponent()

        Current = Me
    End Sub

    Private Async Sub ButtonStart_Click(sender As Object, e As RoutedEventArgs)
        Await Me.Start()
    End Sub

    Private Async Function Start() As Task
        Dim sm As New SnapshotManager(Me.PathTestBox.Text)
        Dim records = sm.GetEmptyRecords()
        Me.TheList.ItemsSource = records

        Await Task.WhenAll(records.Select(Function(r) r.StartSum()))
    End Function

End Class
