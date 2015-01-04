Public Class AssessForm
    Public f1 As Form1

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim instance As New Assess
        instance.f1 = f1
        instance.Doit(NumericUpDown1.Value)
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Try
            System.Diagnostics.Process.Start(Application.StartupPath & "\Curve.exe")
        Catch ex As Exception
            MessageBox.Show("无法启动绘图组件，请检查您的安装是否完整。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub
End Class