using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StartUp
{
    public partial class Form1 : Form
    {
        private AcadProductManager apm;

        private static int count = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                apm.AddLaunchInfo(apm.GetAllowedProducts().ToList()[listBox1.SelectedIndex].RegKey);
                System.Diagnostics.Process.Start(apm.GetAllowedProducts().ToList()[listBox1.SelectedIndex].Path + "\\acad.exe");
                timer1.Start();
                this.Hide();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            apm = new AcadProductManager();
            System.IO.File.Delete(FileManager.CurrentFolder + "loaded.tmp");
            foreach (var product in apm.GetAllowedProducts())
            {
                listBox1.Items.Add(product.Name);
            }
            if (listBox1.Items.Count > 0)
            {
                listBox1.SelectedIndex = 0;
            }

            if (FileManager.Config["StartUp"]["Login"] != "Off")
            {
                LoginForm lf = new LoginForm();
                lf.ShowDialog();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(FileManager.CurrentFolder + "loaded.tmp") || count > 20)
            {
                //System.IO.File.Delete(FileManager.CurrentFolder + "loaded.tmp");
                apm.RemoveLaunchInfo(apm.GetAllowedProducts().ToList()[listBox1.SelectedIndex].RegKey);
                this.Close();
            }
            else
            {
                count++;
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button1_Click(null, null);
        }
    }
}
