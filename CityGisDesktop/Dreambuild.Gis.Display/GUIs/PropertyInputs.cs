using System.Windows.Forms;

namespace Dreambuild.Gis.Display
{
    public partial class PropertyInputs : Form
    {
        public PropertyInputs()
        {
            InitializeComponent();
        }

        public void SetData(object data)
        {
            if (data is object[])
            {
                propertyGrid1.SelectedObjects = data as object[];
            }
            else
            {
                propertyGrid1.SelectedObject = data;
            }
        }

        public void SetDetailData(object[] data)
        {
            propertyGrid1.SelectedObject = data;
        }
    }
}
