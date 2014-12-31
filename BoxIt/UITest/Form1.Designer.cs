namespace UITest
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbClear = new System.Windows.Forms.RadioButton();
            this.rbBox = new System.Windows.Forms.RadioButton();
            this.rbPorter = new System.Windows.Forms.RadioButton();
            this.cbCreateCell = new System.Windows.Forms.CheckBox();
            this.rbHomeSpace = new System.Windows.Forms.RadioButton();
            this.rbSpace = new System.Windows.Forms.RadioButton();
            this.rbWall = new System.Windows.Forms.RadioButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.游戏GToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.新建NToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.选择关卡ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出QToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.求解SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助HToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Location = new System.Drawing.Point(12, 87);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(284, 289);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbClear);
            this.groupBox1.Controls.Add(this.rbBox);
            this.groupBox1.Controls.Add(this.rbPorter);
            this.groupBox1.Controls.Add(this.cbCreateCell);
            this.groupBox1.Controls.Add(this.rbHomeSpace);
            this.groupBox1.Controls.Add(this.rbSpace);
            this.groupBox1.Controls.Add(this.rbWall);
            this.groupBox1.Location = new System.Drawing.Point(303, 87);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(114, 185);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "创建";
            // 
            // rbClear
            // 
            this.rbClear.AutoSize = true;
            this.rbClear.Location = new System.Drawing.Point(18, 123);
            this.rbClear.Name = "rbClear";
            this.rbClear.Size = new System.Drawing.Size(47, 16);
            this.rbClear.TabIndex = 6;
            this.rbClear.Text = "清除";
            this.rbClear.UseVisualStyleBackColor = true;
            // 
            // rbBox
            // 
            this.rbBox.AutoSize = true;
            this.rbBox.Location = new System.Drawing.Point(18, 102);
            this.rbBox.Name = "rbBox";
            this.rbBox.Size = new System.Drawing.Size(47, 16);
            this.rbBox.TabIndex = 5;
            this.rbBox.Text = "箱子";
            this.rbBox.UseVisualStyleBackColor = true;
            // 
            // rbPorter
            // 
            this.rbPorter.AutoSize = true;
            this.rbPorter.Location = new System.Drawing.Point(18, 82);
            this.rbPorter.Name = "rbPorter";
            this.rbPorter.Size = new System.Drawing.Size(59, 16);
            this.rbPorter.TabIndex = 4;
            this.rbPorter.Text = "搬运工";
            this.rbPorter.UseVisualStyleBackColor = true;
            // 
            // cbCreateCell
            // 
            this.cbCreateCell.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbCreateCell.AutoSize = true;
            this.cbCreateCell.Location = new System.Drawing.Point(18, 150);
            this.cbCreateCell.Name = "cbCreateCell";
            this.cbCreateCell.Size = new System.Drawing.Size(39, 22);
            this.cbCreateCell.TabIndex = 3;
            this.cbCreateCell.Text = "创建";
            this.cbCreateCell.UseVisualStyleBackColor = true;
            // 
            // rbHomeSpace
            // 
            this.rbHomeSpace.AutoSize = true;
            this.rbHomeSpace.Location = new System.Drawing.Point(18, 62);
            this.rbHomeSpace.Name = "rbHomeSpace";
            this.rbHomeSpace.Size = new System.Drawing.Size(59, 16);
            this.rbHomeSpace.TabIndex = 2;
            this.rbHomeSpace.Text = "目标格";
            this.rbHomeSpace.UseVisualStyleBackColor = true;
            // 
            // rbSpace
            // 
            this.rbSpace.AutoSize = true;
            this.rbSpace.Location = new System.Drawing.Point(18, 42);
            this.rbSpace.Name = "rbSpace";
            this.rbSpace.Size = new System.Drawing.Size(59, 16);
            this.rbSpace.TabIndex = 1;
            this.rbSpace.Text = "普通格";
            this.rbSpace.UseVisualStyleBackColor = true;
            // 
            // rbWall
            // 
            this.rbWall.AutoSize = true;
            this.rbWall.Checked = true;
            this.rbWall.Location = new System.Drawing.Point(18, 21);
            this.rbWall.Name = "rbWall";
            this.rbWall.Size = new System.Drawing.Size(35, 16);
            this.rbWall.TabIndex = 0;
            this.rbWall.TabStop = true;
            this.rbWall.Text = "墙";
            this.rbWall.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.游戏GToolStripMenuItem,
            this.求解SToolStripMenuItem,
            this.帮助HToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(429, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 游戏GToolStripMenuItem
            // 
            this.游戏GToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新建NToolStripMenuItem,
            this.保存SToolStripMenuItem,
            this.选择关卡ToolStripMenuItem,
            this.退出QToolStripMenuItem});
            this.游戏GToolStripMenuItem.Name = "游戏GToolStripMenuItem";
            this.游戏GToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.游戏GToolStripMenuItem.Text = "游戏(&G)";
            // 
            // 新建NToolStripMenuItem
            // 
            this.新建NToolStripMenuItem.Name = "新建NToolStripMenuItem";
            this.新建NToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.新建NToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.新建NToolStripMenuItem.Text = "新建(&N)...";
            this.新建NToolStripMenuItem.Click += new System.EventHandler(this.新建NToolStripMenuItem_Click);
            // 
            // 保存SToolStripMenuItem
            // 
            this.保存SToolStripMenuItem.Name = "保存SToolStripMenuItem";
            this.保存SToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.保存SToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.保存SToolStripMenuItem.Text = "保存(&S)...";
            this.保存SToolStripMenuItem.Click += new System.EventHandler(this.保存SToolStripMenuItem_Click);
            // 
            // 选择关卡ToolStripMenuItem
            // 
            this.选择关卡ToolStripMenuItem.Name = "选择关卡ToolStripMenuItem";
            this.选择关卡ToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.选择关卡ToolStripMenuItem.Text = "选择关卡...";
            this.选择关卡ToolStripMenuItem.Click += new System.EventHandler(this.选择关卡ToolStripMenuItem_Click);
            // 
            // 退出QToolStripMenuItem
            // 
            this.退出QToolStripMenuItem.Name = "退出QToolStripMenuItem";
            this.退出QToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.退出QToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.退出QToolStripMenuItem.Text = "退出(&Q)";
            this.退出QToolStripMenuItem.Click += new System.EventHandler(this.退出QToolStripMenuItem_Click);
            // 
            // 求解SToolStripMenuItem
            // 
            this.求解SToolStripMenuItem.Name = "求解SToolStripMenuItem";
            this.求解SToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.求解SToolStripMenuItem.Text = "求解(&S)";
            // 
            // 帮助HToolStripMenuItem
            // 
            this.帮助HToolStripMenuItem.Name = "帮助HToolStripMenuItem";
            this.帮助HToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.帮助HToolStripMenuItem.Text = "帮助(&H)";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "推箱子关卡（*.boxlevel）|*.boxlevel";
            this.saveFileDialog1.Title = "保存关卡";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "推箱子关卡（*.boxlevel）|*.boxlevel";
            this.openFileDialog1.Title = "选择关卡";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(429, 409);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BoxIt";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbHomeSpace;
        private System.Windows.Forms.RadioButton rbSpace;
        private System.Windows.Forms.RadioButton rbWall;
        private System.Windows.Forms.CheckBox cbCreateCell;
        private System.Windows.Forms.RadioButton rbBox;
        private System.Windows.Forms.RadioButton rbPorter;
        private System.Windows.Forms.RadioButton rbClear;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 游戏GToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 求解SToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 帮助HToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 新建NToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出QToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 保存SToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 选择关卡ToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}

