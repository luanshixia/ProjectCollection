Imports System.IO
Imports System.Collections.ObjectModel
Imports System.ComponentModel

Public Class SnapshotManager
    Public Property Path As DirectoryInfo

    Public Sub New(path As String)
        Me.Path = New DirectoryInfo(path)
    End Sub

    Public Function GetEmptyRecords() As ObservableCollection(Of Record)
        Dim subDirs = Path.GetDirectories()
        Dim subDirRecords = subDirs.Select(Function(x) New Record(x))
        Dim files = Path.GetFiles()
        Dim fileRecords = files.Select(Function(x) New Record(x))
        Return New ObservableCollection(Of Record)(subDirRecords.Concat(fileRecords).ToList())
    End Function
End Class

Public Class Record
    Implements INotifyPropertyChanged
    Public Property Type As String
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
        Me.NotifyPropertyChanged(NameOf(Size))
        Me.NotifyPropertyChanged(NameOf(SizeString))
    End Sub

    Public Property Size As Long
        Get
            Return Me.sizeValue
        End Get
        Set(value As Long)
            Me.sizeValue = value
            Me.propertyChangeCount += 1
            If Me.propertyChangeCount Mod 200 = 0 Then
                Me.UpdateProperty()
            End If
        End Set
    End Property

    Public ReadOnly Property SizeString0 As String
        Get
            If Me.Size < 1024 Then
                Return Me.Size & " Byte"
            ElseIf Me.Size < 1024 * 1024 Then
                Return CInt(Me.Size / 1024) & " KB"
            Else
                Return CInt(Me.Size / 1024 / 1024) & " MB"
            End If
        End Get
    End Property

    Public ReadOnly Property SizeString As String
        Get
            Return IIf(Me.calculationFinished, Me.SizeString0, "(...) " & Me.SizeString0)
        End Get
    End Property

    Public Sub New(dir As DirectoryInfo)
        Me.Type = "D"
        Me.Name = dir.Name
        Me.Size = 0
        Me.dirInfo = dir
    End Sub

    Public Sub New(file As FileInfo)
        Me.Type = "F"
        Me.Name = file.Name
        Me.Size = file.Length
    End Sub

    Public Async Function SumSize() As Task(Of Long)
        If Me.dirInfo IsNot Nothing Then
            Return Await Me.GetDirectorySize(dirInfo)
        Else
            Return Me.Size
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
                                      Dim size = Await Me.GetDirectorySize(subDir)
                                      sumSubDirSize += size
                                  Next
                                  Dim files = dir.GetFiles()
                                  Dim sumFileSize = files.Sum(Function(x) x.Length)
                                  Size = Size + sumFileSize ' To avoid duplicates: Only file size accumulates; dirs excluded.
                                  Return sumSubDirSize + sumFileSize
                              End Function)
    End Function
End Class