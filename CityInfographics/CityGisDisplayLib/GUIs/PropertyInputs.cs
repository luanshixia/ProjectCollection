using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TongJi.Gis.Display
{
    public partial class PropertyInputs : Form
    {
        public PropertyInputs()
        {
            InitializeComponent();
        }

        public object PropertyObject
        {
            get
            {
                return propertyGrid1.SelectedObject;
            }
            set
            {
                propertyGrid1.SelectedObject = value;
            }
        }
    }
}
