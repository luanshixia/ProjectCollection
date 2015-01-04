Public Class Assess
    Public pre As XDocument
    Public f1 As Form1
    Public fc As ChoiceForm
    Public n As Integer

    Public Sub RandWord(ByVal n As Integer)
        Me.n = n
        Dim ids As New List(Of Integer)
        Dim rd As New Random
        For i = 1 To n
            ids.Add(rd.NextDouble * xword.Element("Word").Elements.Count)
        Next
        ids.Sort()
        f1.SelectToBasket(Function(x) ids.IndexOf(x.Attribute("i").Value) <> -1) '以前留了这个方法，简单多了
        fc = New ChoiceForm(f1.WorkingSet)
        fc.RadioButton2.Enabled = False
        fc.ShowDialog()
    End Sub

    Public Sub Record()
        If IO.File.Exists(Application.StartupPath & "\Assess.xml") = False Then
            pre = New XDocument()
            pre.Add(New XElement("Data"))
        Else
            pre = XDocument.Load(Application.StartupPath & "\Assess.xml")
        End If
        pre.Element("Data").Add(New XElement("a"))
        Dim work = pre.Element("Data").Elements.Last(Function(x) x.Name = "a")
        work.Add(New XAttribute("day", f1.today))
        work.Add(New XAttribute("date", Date.Now.ToShortDateString))
        work.Add(New XAttribute("quantity", n))
        work.Add(New XAttribute("properrate", fc.rate))
        pre.Save(Application.StartupPath & "\Assess.xml")
    End Sub

    Public Sub Doit(ByVal n As Integer)
        RandWord(n)
        If fc.IsCompelete = True Then
            Record()
        End If
    End Sub

End Class
