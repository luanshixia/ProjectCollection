Imports System.IO

Public Interface Node
    Property Name As String
    Property Attributes As String
    Function ToXml() As XElement
End Interface

Public Class Folder
    Implements Node
    Public Property Children As List(Of Node)
    Public Property Name As String Implements Node.Name
    Public Property Attributes As String Implements Node.Attributes
    Public Property Depth As Integer
    Public Shared Property MaxSearchDepth As Integer = 4
    Public Shared Property MaxDisplayChildren As Integer = 50
    Public Shared Property IncludeHidden As Boolean = False

    Sub New(name As String)
        Me.Name = name
        Children = New List(Of Node)
    End Sub

    Sub New(dir As DirectoryInfo, depth As Integer)
        Name = dir.Name
        Attributes = dir.Attributes.ToString()
        Me.Depth = depth
        Dim cond1 = If(IncludeHidden, True, If(depth = 0, True, ((dir.Attributes And FileAttributes.Hidden) <> FileAttributes.Hidden) And ((dir.Attributes And FileAttributes.System) <> FileAttributes.System)))
        Dim cond2 = depth < MaxSearchDepth
        If (cond1 And cond2) Then
            Children = New List(Of Node)
            Try
                dir.GetDirectories().ToList().ForEach(Sub(d) Children.Add(New Folder(d, depth + 1)))
                dir.GetFiles().ToList().ForEach(Sub(f) Children.Add(New File(f)))
            Catch ex As Exception

            End Try
        End If
    End Sub

    Public Function ToXml() As XElement Implements Node.ToXml
        Dim excludeExtensions() As String = {"jpg", "png", "gif"}
        Dim xe = <Folder Name=<%= Name %>></Folder>
        If Children IsNot Nothing Then
            Dim children1 = Children.Where(Function(c) excludeExtensions.All(Function(e) Not c.Name.EndsWith(e, StringComparison.OrdinalIgnoreCase))).ToList()
            children1.Take(MaxDisplayChildren).ToList().ForEach(Sub(n) xe.Add(n.ToXml()))
            If ((Children.Count - children1.Count) Or (children1.Count > MaxDisplayChildren)) Then
                xe.Add(<Etc Total=<%= Children.Count %>/>)
            End If
        End If
        Return xe
    End Function
End Class

Public Class File
    Implements Node
    Public Property Name As String Implements Node.Name
    Public Property Attributes As String Implements Node.Attributes
    Public Property Size As String
    Public Property Created As String
    Public Property Modified As String

    Sub New(file As FileInfo)
        Name = file.Name
        Attributes = file.Attributes.ToString()
        Size = GetMegabyteString(file.Length)
        Created = file.CreationTime.ToShortDateString()
        Modified = file.LastWriteTime.ToShortDateString()
    End Sub

    Public Function ToXml() As XElement Implements Node.ToXml
        Return <File Name=<%= Name %> Size=<%= Size %> Created=<%= Created %> Modified=<%= Modified %>/>
    End Function

    Public Shared Function GetMegabyteString(ByVal bytes As Long) As String
        Dim megabytes = bytes / 1024 / 1024
        Return megabytes.ToString("0.00") & "MB"
    End Function
End Class


