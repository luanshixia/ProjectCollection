Public Class Form1
    Private g As Graphics
    Private ox, oy As Integer '表示原点的位置
    Private xscale, yscale As Double '表示坐标轴一个单位的显示宽度
    Private MouseDownPositionX, MouseDownPositionY As Integer '拖动时记录按下鼠标的位置
    Private MousePositionX, MousePositionY As Integer
    Private MouseDownOx, MouseDownOy As Integer '拖动时记录原点的位置
    Delegate Function curve(ByVal x As Double) As Double '委托，表示要绘图的函数
    Private PresentShowingCurve As curve = Function(x) Math.Cos(x / 2) * 2 '委托对象，表示当前图形函数，初始预设使用Lambda表达式。Lambda不需要AddressOf引导
    Event ScaleChanged() '简单事件，没有事件参数，表示xscale/yscale的改变
    Private lag As Lagrange
    Private IsNearPoint As Boolean = False
    Private PointNear As Point
    Private cur As New Cursor("Delete.ico")
    Private Tips() As String = { _
    "数值分析学习演示软件", _
    "按住鼠标右键可以拖动坐标系", _
    "指向既有点，出现红色捕捉圆圈，可以单击删除点", _
    "双击可以缩放坐标系，左键放大，右键缩小", _
    "支持存储和读取", _
    "2008王阳软件"}

    '绘制坐标轴
    Private Sub DrawAxes()
        If oy < PictureBox1.Height And oy > 0 Then
            g.DrawLine(Pens.Gray, 0, oy, PictureBox1.Width, oy)
            Dim n0, nn As Integer
            n0 = DisplayToMath(0, 0).X - 1
            nn = DisplayToMath(PictureBox1.Width, 0).X + 1
            For n As Integer = n0 To nn
                Dim m As Integer = MathToDisplay(n, 0).X
                If m < PictureBox1.Width And m > 0 And n Mod 20 / xscale = 0 Then
                    g.DrawLine(Pens.Gray, m, oy, m, oy - 4)
                    If xscale >= 10 Then
                        g.DrawString(n.ToString, New Font("Arial", 8), Brushes.Black, m - 5, oy)
                    ElseIf n Mod 40 / xscale = 0 Then
                        g.DrawString(n.ToString, New Font("Arial", 8), Brushes.Black, m - 5, oy)
                    End If
                End If
            Next
        End If
        If ox < PictureBox1.Width And ox > 0 Then
            g.DrawLine(Pens.Gray, ox, 0, ox, PictureBox1.Height)
            Dim n0, nn As Integer
            nn = DisplayToMath(0, 0).Y + 1
            n0 = DisplayToMath(0, PictureBox1.Height).Y - 1
            For n As Integer = n0 To nn
                Dim m As Integer = MathToDisplay(0, n).Y
                If m < PictureBox1.Height And m > 0 And n Mod 20 / xscale = 0 Then
                    g.DrawLine(Pens.Gray, ox, m, ox + 4, m)
                    If xscale >= 10 Then
                        g.DrawString(n.ToString, New Font("Arial", 8), Brushes.Black, ox + 5, m - 5)
                    ElseIf n Mod 40 / xscale = 0 Then
                        g.DrawString(n.ToString, New Font("Arial", 8), Brushes.Black, ox + 5, m - 5)
                    End If
                End If
            Next
            'Dim y = oy
            'While y > 0
            '    g.DrawLine(Pens.Gray, ox, y, ox + 4, y)
            '    y -= xscale
            'End While
        End If
    End Sub
    '数学坐标转换为显示坐标。刚开始没有约束，导致大比例显示运算溢出，后改为这样
    Private Function MathToDisplay(ByVal x As Single, ByVal y As Single) As Point
        Dim x1, y1 As Integer
        If ox + x * xscale <= PictureBox1.Width And ox + x * xscale >= 0 Then
            x1 = ox + x * xscale
        ElseIf ox + x * xscale > PictureBox1.Width Then
            x1 = PictureBox1.Width
        Else
            x1 = -1
        End If
        If oy - y * yscale <= PictureBox1.Height And oy - y * yscale >= 0 Then
            y1 = oy - y * yscale
        ElseIf oy - y * yscale > PictureBox1.Height Then
            y1 = PictureBox1.Height
        Else
            y1 = -10 '节点比较大，-1的话还会显示一点
        End If
        Return New Point(x1, y1)
    End Function
    '显示坐标转换为数学坐标
    Private Function DisplayToMath(ByVal x As Integer, ByVal y As Integer) As PointF
        Dim x1 As Single = (x - ox) / xscale
        Dim y1 As Single = (oy - y) / yscale
        Return New PointF(x1, y1)
    End Function

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ResetAxes()
        Me.Icon = My.Resources.Program
        'Dim curimage As Image = Image.FromFile("Delete.ico")
        'Dim curgraphic As Graphics = Graphics.FromImage(curimage)
        'cur.Draw(curgraphic, New Rectangle(0, 0, 20, 10))
    End Sub

    Private Sub PictureBox1_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseClick
        If e.Button = Windows.Forms.MouseButtons.Left And CheckBox1.Checked Then
            If IsNearPoint Then
                DeletePoint()
            Else
                lag.points.Add(DisplayToMath(e.X, e.Y))
            End If
        End If
        If lag IsNot Nothing Then
            PresentShowingCurve = AddressOf lag.Lag
            PictureBox1.Invalidate()
        End If
    End Sub
    '接近已存在点时，捕捉，删除点，此功能在后期加入，一举调试成功。得益于整体架构！
    'Public Sub DeletePoint(ByVal x As Integer, ByVal y As Integer, ByRef deleted As Boolean)
    '    Dim c = From p In lag.points Select pd = MathToDisplay(p.X, p.Y) 'LINQ to Object
    '    For Each pd In c
    '        If (x - pd.X) ^ 2 + (y - pd.Y) ^ 2 <= 9 Then
    '            Me.Cursor = cur
    '            Dim s = DisplayToMath(pd.X, pd.Y)
    '            lag.points.Remove(s)
    '            deleted = True
    '            Exit For
    '        End If
    '    Next
    'End Sub
    '下面是改进后的实现方式，因为牵涉到突出显示捕捉点
    Public Sub DeletePoint()
        Dim s = DisplayToMath(PointNear.X, PointNear.Y)
        lag.points.Remove(s)
    End Sub
    Public Sub IsNear(ByVal x As Integer, ByVal y As Integer)
        IsNearPoint = False
        Dim c = From p In lag.points Select pd = MathToDisplay(p.X, p.Y) 'LINQ to Object
        For Each pd In c
            If (x - pd.X) ^ 2 + (y - pd.Y) ^ 2 <= 100 Then
                IsNearPoint = True
                PointNear = pd
                Me.Cursor = cur
                Exit For
            End If
        Next
    End Sub
    '双击缩放
    Private Sub PictureBox1_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDoubleClick
        Select Case e.Button
            Case Windows.Forms.MouseButtons.Left
                If xscale < 160 Then
                    xscale *= 2
                    yscale *= 2
                End If
            Case Windows.Forms.MouseButtons.Right
                If xscale > 0.1 Then
                    xscale /= 2
                    yscale /= 2
                End If
        End Select
        PictureBox1.Invalidate()
        RaiseEvent ScaleChanged()
    End Sub
    '右键拖动记录信息
    Private Sub PictureBox1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Right Then
            Me.Cursor = Cursors.SizeAll
            MouseDownPositionX = e.X
            MouseDownPositionY = e.Y
            MouseDownOx = ox
            MouseDownOy = oy
        End If
    End Sub

    Private Sub PictureBox1_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox1.MouseLeave
        Me.Cursor = Cursors.Default
    End Sub
    '右键拖动实现
    Private Sub PictureBox1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseMove
        MousePositionX = e.X
        MousePositionY = e.Y
        If e.Button = Windows.Forms.MouseButtons.Right Then
            Dim xx = e.X - MouseDownPositionX
            Dim yy = e.Y - MouseDownPositionY
            ox = MouseDownOx + xx
            oy = MouseDownOy + yy
        ElseIf e.Button = Windows.Forms.MouseButtons.None Then
            'If lag IsNot Nothing Then
            '    PresentShowingCurve = AddressOf lag.Lag
            '    PictureBox1.Invalidate()
            'End If  ’留在这里，将来实现预览功能
            If CheckBox1.Checked Then
                Me.Cursor = Cursors.Cross
            End If
            If lag IsNot Nothing Then
                IsNear(e.X, e.Y)
            End If
        End If
        PictureBox1.Invalidate()
    End Sub
    '松开鼠标恢复指针
    Private Sub PictureBox1_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseUp
        Me.Cursor = Cursors.Default
    End Sub
    '绘制鼠标位置文本
    Private Sub DrawMousePosition()
        If CheckBox1.Checked Then
            Dim s As String = String.Format("({0:#.00}, {1:#.00})", DisplayToMath(MousePositionX, 0).X, DisplayToMath(0, MousePositionY).Y)
            g.DrawString(s, New Font("Arial", 8), Brushes.Black, MousePositionX, MousePositionY)
            'PictureBox1.Invalidate()
        End If
    End Sub
    '绘制插值已知点
    Private Sub DrawPoint()
        If lag IsNot Nothing Then
            For Each p As PointF In lag.points
                g.FillEllipse(Brushes.Red, MathToDisplay(p.X, 0).X - 3, MathToDisplay(0, p.Y).Y - 3, 6, 6)
            Next
            Dim c = From p In lag.points Select pd = MathToDisplay(p.X, p.Y) 'LINQ to Object
            'PictureBox1.Invalidate()
        End If
    End Sub
    Public Sub DrawNearPoint()
        If IsNearPoint Then
            Dim pen1 As New Pen(Color.Red, 3)
            g.DrawEllipse(pen1, PointNear.X - 10, PointNear.Y - 10, 20, 20)
        End If
    End Sub
    '绘图
    Private Sub PictureBox1_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles PictureBox1.Paint
        g = e.Graphics
        DrawAxes()
        DrawCurve(PresentShowingCurve)
        DrawMousePosition()
        DrawPoint()
        DrawNearPoint()
    End Sub
    '恢复坐标轴初始状态
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        ResetAxes()
        RaiseEvent ScaleChanged()
    End Sub
    '被Form_Load和Button1_Click调用
    Private Sub ResetAxes()
        ox = PictureBox1.Width / 2
        oy = PictureBox1.Height / 2
        xscale = 20
        yscale = 20
        PictureBox1.Invalidate()
    End Sub
    '绘制函数图形
    Private Sub DrawCurve(ByVal f As curve)
        Dim x() As Double = New Double(PictureBox1.Width) {}
        Dim y() As Integer = New Integer(PictureBox1.Width) {}
        '原本有一个大的Try语句，但解决不了实际问题，改为现在的形式
        For i As Integer = 0 To PictureBox1.Width - 1
            x(i) = DisplayToMath(i, 0).X
            Dim fval As Double
            Try
                fval = f(x(i))
                y(i) = MathToDisplay(0, fval).Y
            Catch ex As Exception
                y(i) = 0 '貌似不是这里溢出
            End Try
        Next
        Dim s As String = ""
        Dim a = 0
        For i As Integer = 1 To PictureBox1.Width - 1
            Try
                g.DrawLine(Pens.Orange, i - 1, y(i - 1), i, y(i))
            Catch ex As Exception
                s += String.Format("{0}   {1}" & vbCrLf, y(i - 1), y(i))
                a = 1
            End Try
        Next
        If a = 1 Then
            MessageBox.Show(s)
        End If
    End Sub
    '卷动方式改变比例
    Private Sub TrackBar1_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar1.Scroll
        xscale = Math.Pow(2, TrackBar1.Value) * 10
        yscale = xscale
        PictureBox1.Invalidate()
        RaiseEvent ScaleChanged()
    End Sub
    '事件处理
    Private Sub Scale_Changed() Handles Me.ScaleChanged
        TrackBar1.Value = Math.Log10(xscale / 10) / Math.Log10(2)
        NumericUpDown1.Value = xscale / 20
    End Sub
    '输入方式改变比例
    Private Sub NumericUpDown1_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NumericUpDown1.ValueChanged
        xscale = 20 * NumericUpDown1.Value
        yscale = xscale
        PictureBox1.Invalidate()
        RaiseEvent ScaleChanged()
    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            CheckBox1.Text = "完成"
            If lag Is Nothing Then
                lag = New Lagrange
                PresentShowingCurve = AddressOf lag.Lag
                PictureBox1.Invalidate()
            End If
            Button3.Enabled = False
            Button4.Enabled = False
        Else
            CheckBox1.Text = "开始绘制点"
            Button3.Enabled = True
            Button4.Enabled = True
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        lag = New Lagrange
        PresentShowingCurve = AddressOf lag.Lag
        PictureBox1.Invalidate()
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Static i As Integer = 0
        StatusStrip1.Items(0).Text = Tips(i)
        i += 1
        If i >= Tips.Length Then
            i = 0
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            lag.Save(SaveFileDialog1.FileName)
        End If
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            lag = New Lagrange
            PresentShowingCurve = AddressOf lag.Lag
            lag.Load(OpenFileDialog1.FileName)
            PictureBox1.Invalidate()
        End If
    End Sub
End Class
