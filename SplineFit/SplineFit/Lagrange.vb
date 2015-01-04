Public Class Lagrange
    Public points As New List(Of PointF) '泛型List，保存数学点
    'Lagrange插值函数
    Public Function Lag(ByVal x As Double) As Double
        Dim n As Integer = points.Count
        Dim sum As Double = 0
        For i As Integer = 0 To n - 1
            Dim prod As Double = 1
            For j As Integer = 0 To i - 1
                prod *= (x - points(j).X) / (points(i).X - points(j).X)
            Next
            For j As Integer = i + 1 To n - 1
                prod *= (x - points(j).X) / (points(i).X - points(j).X)
            Next
            sum += prod * points(i).Y
        Next
        Return sum
    End Function
    Public Sub Save(ByVal path As String)
        Dim s As New XDocument
        Dim lag As New XElement("lag")
        Dim n As Integer = points.Count
        Dim p() As XElement
        p = New XElement(n) {}
        For i As Integer = 0 To n - 1
            p(i) = New XElement("p" & i.ToString, New XAttribute("x", points(i).X), New XAttribute("y", points(i).Y))
            lag.Add(p(i))
        Next
        s.Add(lag)
        MessageBox.Show(s.ToString, "保存的数据")
        s.Save(path)
    End Sub
    Public Sub Load(ByVal path As String)
        points.Clear()
        Dim s As XDocument = XDocument.Load(path)
        MessageBox.Show(s.ToString, "读入的数据")
        Dim elag = s.Element("lag")
        Dim ps = From p In elag.Elements Select p
        For Each p In ps
            points.Add(New PointF(p.Attribute("x"), p.Attribute("y")))
        Next
    End Sub
End Class

