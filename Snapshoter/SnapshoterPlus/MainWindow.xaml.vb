Class MainWindow

    Public Shared Property Current As MainWindow
    'Private records As System.Collections.ObjectModel.ObservableCollection(Of Record)

    Public Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        Current = Me
    End Sub

    Private Async Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        Await Start()
    End Sub

    Private Async Function Start() As Task
        Dim sm As New SnapshotManager(TextBox1.Text)
        Dim records = sm.GetEmptyRecords()
        TheList.ItemsSource = records

        Await Task.WhenAll(records.Select(Function(r) r.StartSum()))
    End Function

End Class
