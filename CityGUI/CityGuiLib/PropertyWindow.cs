using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

namespace TongJi.City
{
    public partial class PropertyWindow : DockContent
    {
        private static PropertyWindow _current;
        public static PropertyWindow Current { get { return _current; } }

        public PropertyWindow()
        {
            InitializeComponent();

            _current = this;
        }

        private void PropertyWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
