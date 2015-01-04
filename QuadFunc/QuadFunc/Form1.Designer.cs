namespace 二次函数
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.函数FToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.新建NToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.已知三点求函数ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.求解ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.判别式ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.对称轴ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.顶点ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.极值ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.范围最值ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.y0的根ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yk的根ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.变换ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.两点式ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.顶点式ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.一般式ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.数值跟踪ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.生成自动报表AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.退出QToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.图形GToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.绘制DToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.定形绘制SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.显示对称轴ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.清除上次绘制LToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.清除已绘制函数图象ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.选项OToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.图形ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助HToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助主题SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Location = new System.Drawing.Point(21, 18);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(200, 200);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(29, 352);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(102, 34);
            this.button1.TabIndex = 1;
            this.button1.Text = "建立函数";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(25, 320);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(185, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "欢迎使用，请建立函数。";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(34, 364);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(71, 22);
            this.textBox1.TabIndex = 3;
            this.textBox1.Visible = false;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(111, 364);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(71, 22);
            this.textBox2.TabIndex = 4;
            this.textBox2.Visible = false;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(188, 364);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(71, 22);
            this.textBox3.TabIndex = 5;
            this.textBox3.Visible = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.函数FToolStripMenuItem,
            this.图形GToolStripMenuItem,
            this.帮助HToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(313, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 函数FToolStripMenuItem
            // 
            this.函数FToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新建NToolStripMenuItem,
            this.已知三点求函数ToolStripMenuItem,
            this.toolStripSeparator2,
            this.求解ToolStripMenuItem,
            this.变换ToolStripMenuItem,
            this.数值跟踪ToolStripMenuItem,
            this.生成自动报表AToolStripMenuItem,
            this.toolStripSeparator1,
            this.退出QToolStripMenuItem});
            this.函数FToolStripMenuItem.Name = "函数FToolStripMenuItem";
            this.函数FToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.函数FToolStripMenuItem.Text = "函数(&F)";
            this.函数FToolStripMenuItem.Click += new System.EventHandler(this.函数FToolStripMenuItem_Click);
            // 
            // 新建NToolStripMenuItem
            // 
            this.新建NToolStripMenuItem.Name = "新建NToolStripMenuItem";
            this.新建NToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.新建NToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.新建NToolStripMenuItem.Text = "新建(&N)...";
            this.新建NToolStripMenuItem.Click += new System.EventHandler(this.新建NToolStripMenuItem_Click);
            // 
            // 已知三点求函数ToolStripMenuItem
            // 
            this.已知三点求函数ToolStripMenuItem.Name = "已知三点求函数ToolStripMenuItem";
            this.已知三点求函数ToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.已知三点求函数ToolStripMenuItem.Text = "已知三点求函数(&T)...";
            this.已知三点求函数ToolStripMenuItem.Click += new System.EventHandler(this.已知三点求函数ToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(185, 6);
            // 
            // 求解ToolStripMenuItem
            // 
            this.求解ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.判别式ToolStripMenuItem,
            this.对称轴ToolStripMenuItem,
            this.顶点ToolStripMenuItem,
            this.极值ToolStripMenuItem,
            this.范围最值ToolStripMenuItem,
            this.y0的根ToolStripMenuItem,
            this.yk的根ToolStripMenuItem});
            this.求解ToolStripMenuItem.Name = "求解ToolStripMenuItem";
            this.求解ToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.求解ToolStripMenuItem.Text = "求解";
            // 
            // 判别式ToolStripMenuItem
            // 
            this.判别式ToolStripMenuItem.Name = "判别式ToolStripMenuItem";
            this.判别式ToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.判别式ToolStripMenuItem.Text = "判别式";
            this.判别式ToolStripMenuItem.Click += new System.EventHandler(this.判别式ToolStripMenuItem_Click);
            // 
            // 对称轴ToolStripMenuItem
            // 
            this.对称轴ToolStripMenuItem.Name = "对称轴ToolStripMenuItem";
            this.对称轴ToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.对称轴ToolStripMenuItem.Text = "对称轴";
            this.对称轴ToolStripMenuItem.Click += new System.EventHandler(this.对称轴ToolStripMenuItem_Click);
            // 
            // 顶点ToolStripMenuItem
            // 
            this.顶点ToolStripMenuItem.Name = "顶点ToolStripMenuItem";
            this.顶点ToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.顶点ToolStripMenuItem.Text = "顶点";
            this.顶点ToolStripMenuItem.Click += new System.EventHandler(this.顶点ToolStripMenuItem_Click);
            // 
            // 极值ToolStripMenuItem
            // 
            this.极值ToolStripMenuItem.Name = "极值ToolStripMenuItem";
            this.极值ToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.极值ToolStripMenuItem.Text = "极值";
            this.极值ToolStripMenuItem.Click += new System.EventHandler(this.极值ToolStripMenuItem_Click);
            // 
            // 范围最值ToolStripMenuItem
            // 
            this.范围最值ToolStripMenuItem.Name = "范围最值ToolStripMenuItem";
            this.范围最值ToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.范围最值ToolStripMenuItem.Text = "范围最值...";
            this.范围最值ToolStripMenuItem.Click += new System.EventHandler(this.范围最值ToolStripMenuItem_Click);
            // 
            // y0的根ToolStripMenuItem
            // 
            this.y0的根ToolStripMenuItem.Name = "y0的根ToolStripMenuItem";
            this.y0的根ToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.y0的根ToolStripMenuItem.Text = "y = 0 的根";
            this.y0的根ToolStripMenuItem.Click += new System.EventHandler(this.y0的根ToolStripMenuItem_Click);
            // 
            // yk的根ToolStripMenuItem
            // 
            this.yk的根ToolStripMenuItem.Name = "yk的根ToolStripMenuItem";
            this.yk的根ToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.yk的根ToolStripMenuItem.Text = "y = k 的根...";
            this.yk的根ToolStripMenuItem.Click += new System.EventHandler(this.yk的根ToolStripMenuItem_Click);
            // 
            // 变换ToolStripMenuItem
            // 
            this.变换ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.两点式ToolStripMenuItem,
            this.顶点式ToolStripMenuItem,
            this.一般式ToolStripMenuItem});
            this.变换ToolStripMenuItem.Name = "变换ToolStripMenuItem";
            this.变换ToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.变换ToolStripMenuItem.Text = "变换";
            // 
            // 两点式ToolStripMenuItem
            // 
            this.两点式ToolStripMenuItem.Name = "两点式ToolStripMenuItem";
            this.两点式ToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.两点式ToolStripMenuItem.Text = "两点式(分解)";
            this.两点式ToolStripMenuItem.Click += new System.EventHandler(this.两点式ToolStripMenuItem_Click);
            // 
            // 顶点式ToolStripMenuItem
            // 
            this.顶点式ToolStripMenuItem.Name = "顶点式ToolStripMenuItem";
            this.顶点式ToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.顶点式ToolStripMenuItem.Text = "顶点式(配方)";
            this.顶点式ToolStripMenuItem.Click += new System.EventHandler(this.顶点式ToolStripMenuItem_Click);
            // 
            // 一般式ToolStripMenuItem
            // 
            this.一般式ToolStripMenuItem.Name = "一般式ToolStripMenuItem";
            this.一般式ToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.一般式ToolStripMenuItem.Text = "一般式(展开)";
            this.一般式ToolStripMenuItem.Click += new System.EventHandler(this.一般式ToolStripMenuItem_Click);
            // 
            // 数值跟踪ToolStripMenuItem
            // 
            this.数值跟踪ToolStripMenuItem.Name = "数值跟踪ToolStripMenuItem";
            this.数值跟踪ToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.数值跟踪ToolStripMenuItem.Text = "数值跟踪(&C)";
            this.数值跟踪ToolStripMenuItem.Click += new System.EventHandler(this.数值跟踪ToolStripMenuItem_Click);
            // 
            // 生成自动报表AToolStripMenuItem
            // 
            this.生成自动报表AToolStripMenuItem.Name = "生成自动报表AToolStripMenuItem";
            this.生成自动报表AToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.生成自动报表AToolStripMenuItem.Text = "生成自动报表(&A)...";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(185, 6);
            // 
            // 退出QToolStripMenuItem
            // 
            this.退出QToolStripMenuItem.Name = "退出QToolStripMenuItem";
            this.退出QToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.退出QToolStripMenuItem.Text = "退出(&X)";
            this.退出QToolStripMenuItem.Click += new System.EventHandler(this.退出QToolStripMenuItem_Click);
            // 
            // 图形GToolStripMenuItem
            // 
            this.图形GToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.绘制DToolStripMenuItem,
            this.定形绘制SToolStripMenuItem,
            this.显示对称轴ToolStripMenuItem,
            this.toolStripSeparator3,
            this.清除上次绘制LToolStripMenuItem,
            this.清除已绘制函数图象ToolStripMenuItem,
            this.toolStripSeparator4,
            this.选项OToolStripMenuItem,
            this.图形ToolStripMenuItem});
            this.图形GToolStripMenuItem.Name = "图形GToolStripMenuItem";
            this.图形GToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.图形GToolStripMenuItem.Text = "图形(&G)";
            // 
            // 绘制DToolStripMenuItem
            // 
            this.绘制DToolStripMenuItem.Name = "绘制DToolStripMenuItem";
            this.绘制DToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.绘制DToolStripMenuItem.Text = "定轴绘制(&X)";
            this.绘制DToolStripMenuItem.Click += new System.EventHandler(this.绘制DToolStripMenuItem_Click);
            // 
            // 定形绘制SToolStripMenuItem
            // 
            this.定形绘制SToolStripMenuItem.Name = "定形绘制SToolStripMenuItem";
            this.定形绘制SToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.定形绘制SToolStripMenuItem.Text = "定形绘制(&S)";
            // 
            // 显示对称轴ToolStripMenuItem
            // 
            this.显示对称轴ToolStripMenuItem.Checked = true;
            this.显示对称轴ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.显示对称轴ToolStripMenuItem.Name = "显示对称轴ToolStripMenuItem";
            this.显示对称轴ToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.显示对称轴ToolStripMenuItem.Text = "显示对称轴";
            this.显示对称轴ToolStripMenuItem.Click += new System.EventHandler(this.显示对称轴ToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(160, 6);
            // 
            // 清除上次绘制LToolStripMenuItem
            // 
            this.清除上次绘制LToolStripMenuItem.Name = "清除上次绘制LToolStripMenuItem";
            this.清除上次绘制LToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.清除上次绘制LToolStripMenuItem.Text = "清除上次绘制(&L)";
            this.清除上次绘制LToolStripMenuItem.Click += new System.EventHandler(this.清除上次绘制LToolStripMenuItem_Click);
            // 
            // 清除已绘制函数图象ToolStripMenuItem
            // 
            this.清除已绘制函数图象ToolStripMenuItem.Name = "清除已绘制函数图象ToolStripMenuItem";
            this.清除已绘制函数图象ToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.清除已绘制函数图象ToolStripMenuItem.Text = "清除所有绘制(&C)";
            this.清除已绘制函数图象ToolStripMenuItem.Click += new System.EventHandler(this.清除已绘制函数图象ToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(160, 6);
            // 
            // 选项OToolStripMenuItem
            // 
            this.选项OToolStripMenuItem.Name = "选项OToolStripMenuItem";
            this.选项OToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.选项OToolStripMenuItem.Text = "选项(&O)...";
            // 
            // 图形ToolStripMenuItem
            // 
            this.图形ToolStripMenuItem.Name = "图形ToolStripMenuItem";
            this.图形ToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.图形ToolStripMenuItem.Text = "图形另存为...";
            this.图形ToolStripMenuItem.Click += new System.EventHandler(this.图形ToolStripMenuItem_Click);
            // 
            // 帮助HToolStripMenuItem
            // 
            this.帮助HToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.帮助主题SToolStripMenuItem,
            this.关于AToolStripMenuItem});
            this.帮助HToolStripMenuItem.Name = "帮助HToolStripMenuItem";
            this.帮助HToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.帮助HToolStripMenuItem.Text = "帮助(&H)";
            // 
            // 帮助主题SToolStripMenuItem
            // 
            this.帮助主题SToolStripMenuItem.Name = "帮助主题SToolStripMenuItem";
            this.帮助主题SToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.帮助主题SToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.帮助主题SToolStripMenuItem.Text = "帮助主题(&S)";
            this.帮助主题SToolStripMenuItem.Click += new System.EventHandler(this.帮助主题SToolStripMenuItem_Click);
            // 
            // 关于AToolStripMenuItem
            // 
            this.关于AToolStripMenuItem.Name = "关于AToolStripMenuItem";
            this.关于AToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.关于AToolStripMenuItem.Text = "关于(&A)...";
            this.关于AToolStripMenuItem.Click += new System.EventHandler(this.关于AToolStripMenuItem_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.DecimalPlaces = 3;
            this.numericUpDown1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numericUpDown1.Location = new System.Drawing.Point(155, 67);
            this.numericUpDown1.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(80, 22);
            this.numericUpDown1.TabIndex = 20;
            this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(21, 67);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Minimum = -100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(128, 45);
            this.trackBar1.TabIndex = 19;
            this.trackBar1.TickFrequency = 10;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(29, 39);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(255, 263);
            this.tabControl1.TabIndex = 25;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage2.Controls.Add(this.pictureBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(247, 236);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "函数图形";
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.trackBar1);
            this.tabPage1.Controls.Add(this.numericUpDown1);
            this.tabPage1.Location = new System.Drawing.Point(4, 23);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(247, 236);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "数值跟踪";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(34, 172);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 14);
            this.label4.TabIndex = 23;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 137);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 14);
            this.label3.TabIndex = 22;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 14);
            this.label2.TabIndex = 21;
            this.label2.Text = "定义自变量 x 的值。";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Title = "保存函数图形";
            this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(313, 405);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "二次函数";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 函数FToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 新建NToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 生成自动报表AToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 求解ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 判别式ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 对称轴ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 顶点ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 极值ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 范围最值ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 变换ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 两点式ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 顶点式ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 一般式ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem y0的根ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出QToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 图形GToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 绘制DToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 帮助HToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 帮助主题SToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 关于AToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem yk的根ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 已知三点求函数ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 数值跟踪ToolStripMenuItem;
        public System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem 定形绘制SToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem 清除上次绘制LToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 清除已绘制函数图象ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 显示对称轴ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem 选项OToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 图形ToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

