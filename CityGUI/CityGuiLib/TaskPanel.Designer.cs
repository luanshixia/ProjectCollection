namespace TongJi.City
{
    partial class TaskPanel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nudLevels = new System.Windows.Forms.NumericUpDown();
            this.cbGradient = new System.Windows.Forms.CheckBox();
            this.btnDftClr = new System.Windows.Forms.Button();
            this.btnDtValue = new System.Windows.Forms.Button();
            this.txtMax = new System.Windows.Forms.TextBox();
            this.btnMaxClr = new System.Windows.Forms.Button();
            this.txtMin = new System.Windows.Forms.TextBox();
            this.btnMinClr = new System.Windows.Forms.Button();
            this.cbShowGridLine = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numGridSize = new System.Windows.Forms.NumericUpDown();
            this.cbShowSpot = new System.Windows.Forms.CheckBox();
            this.cbShowRoads = new System.Windows.Forms.CheckBox();
            this.cbShowParcels = new System.Windows.Forms.CheckBox();
            this.cbbAddEnt = new System.Windows.Forms.ComboBox();
            this.cbbValueMode = new System.Windows.Forms.ComboBox();
            this.cbShowResult = new System.Windows.Forms.CheckBox();
            this.cbbAggreType = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbbCountRoads = new System.Windows.Forms.CheckBox();
            this.cbbCountParcels = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLevels)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGridSize)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.nudLevels);
            this.groupBox1.Controls.Add(this.cbGradient);
            this.groupBox1.Controls.Add(this.btnDftClr);
            this.groupBox1.Controls.Add(this.btnDtValue);
            this.groupBox1.Controls.Add(this.txtMax);
            this.groupBox1.Controls.Add(this.btnMaxClr);
            this.groupBox1.Controls.Add(this.txtMin);
            this.groupBox1.Controls.Add(this.btnMinClr);
            this.groupBox1.Location = new System.Drawing.Point(18, 159);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(112, 120);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Color mapping";
            // 
            // nudLevels
            // 
            this.nudLevels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudLevels.Location = new System.Drawing.Point(66, 96);
            this.nudLevels.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudLevels.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudLevels.Name = "nudLevels";
            this.nudLevels.Size = new System.Drawing.Size(41, 20);
            this.nudLevels.TabIndex = 6;
            this.nudLevels.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudLevels.ValueChanged += new System.EventHandler(this.nudLevels_ValueChanged);
            // 
            // cbGradient
            // 
            this.cbGradient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbGradient.AutoSize = true;
            this.cbGradient.Checked = true;
            this.cbGradient.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGradient.Location = new System.Drawing.Point(-4, 98);
            this.cbGradient.Name = "cbGradient";
            this.cbGradient.Size = new System.Drawing.Size(72, 16);
            this.cbGradient.TabIndex = 5;
            this.cbGradient.Text = "Gradient";
            this.cbGradient.UseVisualStyleBackColor = true;
            this.cbGradient.CheckedChanged += new System.EventHandler(this.cbGradient_CheckedChanged);
            // 
            // btnDftClr
            // 
            this.btnDftClr.Location = new System.Drawing.Point(4, 69);
            this.btnDftClr.Name = "btnDftClr";
            this.btnDftClr.Size = new System.Drawing.Size(36, 23);
            this.btnDftClr.TabIndex = 4;
            this.btnDftClr.Text = "GtoR";
            this.btnDftClr.UseVisualStyleBackColor = true;
            this.btnDftClr.Click += new System.EventHandler(this.btnDftClr_Click);
            // 
            // btnDtValue
            // 
            this.btnDtValue.Location = new System.Drawing.Point(44, 69);
            this.btnDtValue.Name = "btnDtValue";
            this.btnDtValue.Size = new System.Drawing.Size(61, 23);
            this.btnDtValue.TabIndex = 3;
            this.btnDtValue.Text = "Detect";
            this.btnDtValue.UseVisualStyleBackColor = true;
            // 
            // txtMax
            // 
            this.txtMax.Location = new System.Drawing.Point(44, 44);
            this.txtMax.Name = "txtMax";
            this.txtMax.Size = new System.Drawing.Size(61, 20);
            this.txtMax.TabIndex = 2;
            this.txtMax.Text = "3000";
            this.txtMax.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMax_KeyPress);
            this.txtMax.Validating += new System.ComponentModel.CancelEventHandler(this.txtMax_Validating);
            // 
            // btnMaxClr
            // 
            this.btnMaxClr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnMaxClr.Location = new System.Drawing.Point(4, 43);
            this.btnMaxClr.Name = "btnMaxClr";
            this.btnMaxClr.Size = new System.Drawing.Size(36, 23);
            this.btnMaxClr.TabIndex = 1;
            this.btnMaxClr.Text = "Max";
            this.btnMaxClr.UseVisualStyleBackColor = false;
            this.btnMaxClr.Click += new System.EventHandler(this.btnMinClr_Click);
            // 
            // txtMin
            // 
            this.txtMin.Location = new System.Drawing.Point(44, 18);
            this.txtMin.Name = "txtMin";
            this.txtMin.Size = new System.Drawing.Size(61, 20);
            this.txtMin.TabIndex = 2;
            this.txtMin.Text = "0";
            this.txtMin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMax_KeyPress);
            this.txtMin.Validating += new System.ComponentModel.CancelEventHandler(this.txtMin_Validating);
            // 
            // btnMinClr
            // 
            this.btnMinClr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnMinClr.Location = new System.Drawing.Point(4, 17);
            this.btnMinClr.Name = "btnMinClr";
            this.btnMinClr.Size = new System.Drawing.Size(36, 23);
            this.btnMinClr.TabIndex = 1;
            this.btnMinClr.Text = "Min";
            this.btnMinClr.UseVisualStyleBackColor = false;
            this.btnMinClr.Click += new System.EventHandler(this.btnMinClr_Click);
            // 
            // cbShowGridLine
            // 
            this.cbShowGridLine.AutoSize = true;
            this.cbShowGridLine.Location = new System.Drawing.Point(18, 136);
            this.cbShowGridLine.Name = "cbShowGridLine";
            this.cbShowGridLine.Size = new System.Drawing.Size(89, 16);
            this.cbShowGridLine.TabIndex = 20;
            this.cbShowGridLine.Text = "Show grid line";
            this.cbShowGridLine.UseVisualStyleBackColor = true;
            this.cbShowGridLine.CheckedChanged += new System.EventHandler(this.cbShowGridLine_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 112);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 12);
            this.label1.TabIndex = 19;
            this.label1.Text = "Grid size";
            // 
            // numGridSize
            // 
            this.numGridSize.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numGridSize.Location = new System.Drawing.Point(63, 110);
            this.numGridSize.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numGridSize.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numGridSize.Name = "numGridSize";
            this.numGridSize.Size = new System.Drawing.Size(67, 20);
            this.numGridSize.TabIndex = 18;
            this.numGridSize.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numGridSize.ValueChanged += new System.EventHandler(this.numGridSize_ValueChanged);
            // 
            // cbShowSpot
            // 
            this.cbShowSpot.AutoSize = true;
            this.cbShowSpot.Checked = true;
            this.cbShowSpot.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowSpot.Location = new System.Drawing.Point(18, 44);
            this.cbShowSpot.Name = "cbShowSpot";
            this.cbShowSpot.Size = new System.Drawing.Size(76, 16);
            this.cbShowSpot.TabIndex = 17;
            this.cbShowSpot.Text = "Show spots";
            this.cbShowSpot.UseVisualStyleBackColor = true;
            this.cbShowSpot.CheckedChanged += new System.EventHandler(this.cbShowSpot_CheckedChanged);
            // 
            // cbShowRoads
            // 
            this.cbShowRoads.AutoSize = true;
            this.cbShowRoads.Checked = true;
            this.cbShowRoads.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowRoads.Location = new System.Drawing.Point(18, 12);
            this.cbShowRoads.Name = "cbShowRoads";
            this.cbShowRoads.Size = new System.Drawing.Size(78, 16);
            this.cbShowRoads.TabIndex = 16;
            this.cbShowRoads.Text = "Show roads";
            this.cbShowRoads.UseVisualStyleBackColor = true;
            this.cbShowRoads.CheckedChanged += new System.EventHandler(this.cbShowRoads_CheckedChanged);
            // 
            // cbShowParcels
            // 
            this.cbShowParcels.AutoSize = true;
            this.cbShowParcels.Checked = true;
            this.cbShowParcels.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowParcels.Location = new System.Drawing.Point(18, 28);
            this.cbShowParcels.Name = "cbShowParcels";
            this.cbShowParcels.Size = new System.Drawing.Size(84, 16);
            this.cbShowParcels.TabIndex = 15;
            this.cbShowParcels.Text = "Show parcels";
            this.cbShowParcels.UseVisualStyleBackColor = true;
            this.cbShowParcels.CheckedChanged += new System.EventHandler(this.cbShowParcels_CheckedChanged);
            // 
            // cbbAddEnt
            // 
            this.cbbAddEnt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbAddEnt.FormattingEnabled = true;
            this.cbbAddEnt.Items.AddRange(new object[] {
            "<Add entity>",
            "Retail",
            "Transit",
            "CrossCityTransportation",
            "Medical",
            "Education",
            "Pollution",
            "ZeroOneSpot"});
            this.cbbAddEnt.Location = new System.Drawing.Point(19, 285);
            this.cbbAddEnt.Name = "cbbAddEnt";
            this.cbbAddEnt.Size = new System.Drawing.Size(111, 20);
            this.cbbAddEnt.TabIndex = 13;
            this.cbbAddEnt.SelectedIndexChanged += new System.EventHandler(this.cbbAddEnt_SelectedIndexChanged);
            // 
            // cbbValueMode
            // 
            this.cbbValueMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbValueMode.FormattingEnabled = true;
            this.cbbValueMode.Items.AddRange(new object[] {
            "Parcel price",
            "Grid price",
            "Voronoi"});
            this.cbbValueMode.Location = new System.Drawing.Point(18, 82);
            this.cbbValueMode.Name = "cbbValueMode";
            this.cbbValueMode.Size = new System.Drawing.Size(111, 20);
            this.cbbValueMode.TabIndex = 14;
            this.cbbValueMode.SelectedIndexChanged += new System.EventHandler(this.cbbValueMode_SelectedIndexChanged);
            // 
            // cbShowResult
            // 
            this.cbShowResult.AutoSize = true;
            this.cbShowResult.Checked = true;
            this.cbShowResult.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowResult.Location = new System.Drawing.Point(18, 60);
            this.cbShowResult.Name = "cbShowResult";
            this.cbShowResult.Size = new System.Drawing.Size(77, 16);
            this.cbShowResult.TabIndex = 12;
            this.cbShowResult.Text = "Show result";
            this.cbShowResult.UseVisualStyleBackColor = true;
            this.cbShowResult.CheckedChanged += new System.EventHandler(this.cbShowResult_CheckedChanged);
            // 
            // cbbAggreType
            // 
            this.cbbAggreType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbAggreType.FormattingEnabled = true;
            this.cbbAggreType.Items.AddRange(new object[] {
            "Sum",
            "Minimum",
            "Maximum"});
            this.cbbAggreType.Location = new System.Drawing.Point(19, 311);
            this.cbbAggreType.Name = "cbbAggreType";
            this.cbbAggreType.Size = new System.Drawing.Size(111, 20);
            this.cbbAggreType.TabIndex = 22;
            this.cbbAggreType.SelectedIndexChanged += new System.EventHandler(this.cbbAggreType_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(22, 390);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(107, 22);
            this.panel1.TabIndex = 23;
            // 
            // cbbCountRoads
            // 
            this.cbbCountRoads.AutoSize = true;
            this.cbbCountRoads.Location = new System.Drawing.Point(19, 337);
            this.cbbCountRoads.Name = "cbbCountRoads";
            this.cbbCountRoads.Size = new System.Drawing.Size(109, 16);
            this.cbbCountRoads.TabIndex = 24;
            this.cbbCountRoads.Text = "Count road factors";
            this.cbbCountRoads.UseVisualStyleBackColor = true;
            this.cbbCountRoads.CheckedChanged += new System.EventHandler(this.cbbCountRoads_CheckedChanged);
            // 
            // cbbCountParcels
            // 
            this.cbbCountParcels.AutoSize = true;
            this.cbbCountParcels.Location = new System.Drawing.Point(19, 352);
            this.cbbCountParcels.Name = "cbbCountParcels";
            this.cbbCountParcels.Size = new System.Drawing.Size(115, 16);
            this.cbbCountParcels.TabIndex = 24;
            this.cbbCountParcels.Text = "Count parcel factors";
            this.cbbCountParcels.UseVisualStyleBackColor = true;
            this.cbbCountParcels.CheckedChanged += new System.EventHandler(this.cbbCountParcels_CheckedChanged);
            // 
            // TaskPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(206, 450);
            this.Controls.Add(this.cbbCountParcels);
            this.Controls.Add(this.cbbCountRoads);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cbbAggreType);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cbShowGridLine);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numGridSize);
            this.Controls.Add(this.cbShowSpot);
            this.Controls.Add(this.cbShowRoads);
            this.Controls.Add(this.cbShowParcels);
            this.Controls.Add(this.cbbAddEnt);
            this.Controls.Add(this.cbbValueMode);
            this.Controls.Add(this.cbShowResult);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.Font = new System.Drawing.Font("Tahoma", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TaskPanel";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
            this.ShowIcon = false;
            this.Text = "TaskPanel";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TaskPanel_FormClosing);
            this.Load += new System.EventHandler(this.TaskPanel_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLevels)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGridSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.NumericUpDown nudLevels;
        public System.Windows.Forms.CheckBox cbGradient;
        public System.Windows.Forms.Button btnDftClr;
        public System.Windows.Forms.Button btnDtValue;
        public System.Windows.Forms.TextBox txtMax;
        public System.Windows.Forms.Button btnMaxClr;
        public System.Windows.Forms.TextBox txtMin;
        public System.Windows.Forms.Button btnMinClr;
        public System.Windows.Forms.CheckBox cbShowGridLine;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.NumericUpDown numGridSize;
        public System.Windows.Forms.CheckBox cbShowSpot;
        public System.Windows.Forms.CheckBox cbShowRoads;
        public System.Windows.Forms.CheckBox cbShowParcels;
        public System.Windows.Forms.ComboBox cbbAddEnt;
        public System.Windows.Forms.ComboBox cbbValueMode;
        public System.Windows.Forms.CheckBox cbShowResult;
        public System.Windows.Forms.ComboBox cbbAggreType;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.CheckBox cbbCountRoads;
        public System.Windows.Forms.CheckBox cbbCountParcels;
    }
}