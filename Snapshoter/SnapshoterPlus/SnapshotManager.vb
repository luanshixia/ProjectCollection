Imports System.IO
Imports System.Collections.ObjectModel
Imports System.ComponentModel

Public Class SnapshotManager
    Public Property Path As DirectoryInfo

    Public Sub New(path As String)
        Me.Path = New DirectoryInfo(path)
    End Sub

    Public Shared Function GetDirectorySize(dir As DirectoryInfo) As Long
        Dim subDirs = dir.GetDirectories()
        Dim sumSubDirSize = subDirs.Sum(Function(x) GetDirectorySize(x))
        Dim files = dir.GetFiles()
        Dim sumFileSize = files.Sum(Function(x) x.Length)
        Return sumSubDirSize + sumFileSize
    End Function

    Public Shared Function GetSizeString(size As Long) As String
        If size < 1024 Then
            Return size & " Byte"
        ElseIf size < 1024 * 1024 Then
            Return CInt(size / 1024) & " KB"
        Else
            Return CInt(size / 1024 / 1024) & " MB"
        End If
    End Function

    Public Function GetRecords() As List(Of Record)
        Dim subDirs = Path.GetDirectories()
        Dim subDirRecords = subDirs.Select(Function(x) New Record(x) With {.Size = GetDirectorySize(x)})
        Dim files = Path.GetFiles()
        Dim fileRecords = files.Select(Function(x) New Record(x))
        Return subDirRecords.Concat(fileRecords).ToList()
    End Function

    Public Function GetEmptyRecords() As ObservableCollection(Of Record)
        Dim subDirs = Path.GetDirectories()
        Dim subDirRecords = subDirs.Select(Function(x) New Record(x))
        Dim files = Path.GetFiles()
        Dim fileRecords = files.Select(Function(x) New Record(x))
        Return New ObservableCollection(Of Record)(subDirRecords.Concat(fileRecords).ToList())
    End Function

    Public Sub Test()
        For i = 0 To 100
            Dim func = Function(x) x * i
        Next
    End Sub
End Class

Public Class Record
    Implements INotifyPropertyChanged
    Public Property DF As String
    Public Property Name As String
    Private sizeValue As Long
    Private propertyChangeCount As Long
    Private calculationFinished As Boolean
    Private dirInfo As DirectoryInfo

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Sub UpdateProperty(Optional finishCalculation As Boolean = False)
        Me.calculationFinished = finishCalculation
        NotifyPropertyChanged("Size")
        NotifyPropertyChanged("SizeString")
    End Sub

    Public Property Size As Long
        Get
            Return sizeValue
        End Get
        Set(value As Long)
            sizeValue = value
            propertyChangeCount += 1
            If propertyChangeCount Mod 200 = 0 Then
                UpdateProperty()
            End If
        End Set
    End Property

    Public ReadOnly Property SizeString0 As String
        Get
            If Size < 1024 Then
                Return Size & " Byte"
            ElseIf Size < 1024 * 1024 Then
                Return CInt(Size / 1024) & " KB"
            Else
                Return CInt(Size / 1024 / 1024) & " MB"
            End If
        End Get
    End Property

    Public ReadOnly Property SizeString As String
        Get
            Return IIf(calculationFinished, SizeString0, "(...) " & SizeString0)
        End Get
    End Property

    Public Sub New(dir As DirectoryInfo)
        DF = "D"
        Name = dir.Name
        Size = 0
        dirInfo = dir
    End Sub

    Public Sub New(file As FileInfo)
        DF = "F"
        Name = file.Name
        Size = file.Length
    End Sub

    Public Async Function SumSize() As Task(Of Long)
        If dirInfo IsNot Nothing Then
            Return Await GetDirectorySize(dirInfo)
        Else
            Return Size
        End If
    End Function

    Public Async Function StartSum() As Task
        Me.Size = Await Me.SumSize()
        Me.UpdateProperty(finishCalculation:=True)
    End Function

    Public Async Function GetDirectorySize(dir As DirectoryInfo) As Task(Of Long)
        Return Await Task.Run(Async Function()
                                  Dim subDirs = dir.GetDirectories()
                                  Dim sumSubDirSize = 0L
                                  For Each subDir In subDirs
                                      Dim size = Await GetDirectorySize(subDir)
                                      sumSubDirSize += size
                                  Next
                                  Dim files = dir.GetFiles()
                                  Dim sumFileSize = files.Sum(Function(x) x.Length)
                                  Size = Size + sumFileSize ' 大小累计只计算文件，不计算目录，否则会重复
                                  Return sumSubDirSize + sumFileSize
                              End Function)
    End Function
End Class