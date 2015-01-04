Public Class Report
    Public message As String
    Public status As Integer()
    Public f As Form

    Private Sub Report_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        TextBox1.Text = message
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.Close()
        f.Close()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        If SaveFileDialog2.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim a As New IO.StreamWriter(SaveFileDialog2.FileName)
            a.Write(TextBox1.Text)
            a.Close()
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim xd As New XDocument
            Dim a As New XElement("Word")
            Dim q = xword.Element("Word").Elements.Where(Function(x) status(Convert.ToInt32(x.Attribute("i").Value)) = 2)
            For Each i In q
                Dim bi As New XElement("w" & i.Attribute("i").Value)
                bi.Add(New XAttribute("i", i.Attribute("i").Value))
                bi.Add(New XAttribute("w", i.Attribute("w").Value))
                bi.Add(New XAttribute("m", i.Attribute("m").Value))
                a.Add(bi)
            Next
            xd.Add(a)
            xd.Save(SaveFileDialog1.FileName)
        End If
    End Sub
End Class