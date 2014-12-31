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
        For Each r In records
            r.Size = Await r.StartSum()
            r.UpdateProperty()
        Next

        'TheList.Items.Clear()
        'For Each r In records
        '    r.Size = Await r.StartSum()
        '    TheList.Items.Add(r)
        '    'Await r.StartSum()
        '    'Update()
        'Next
    End Function

    Private Async Function Test() As Task(Of Long)
        Await Task.Run(Sub()
                           Dim a = ""
                           For i = 0 To 9999
                               a += "a"
                           Next
                       End Sub)
        Return 100
    End Function

End Class
