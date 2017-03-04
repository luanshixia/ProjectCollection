using Dreambuild.Gis.Desktop.Demos.LowCarbon.GUIs;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace Dreambuild.Gis.Desktop.Demos.LowCarbon
{
    internal class EnergySavingEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context,
            System.IServiceProvider provider, object value)
        {
            EnergySavingRatio ratio = value as EnergySavingRatio;

            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                var ratioParameterWindow = new EnergySavingEditorWindow(ratio) { Owner = MainWindow.Current };
                ratioParameterWindow.ShowDialog();

                if (ratioParameterWindow.IsOk)
                {
                    value = ratioParameterWindow.Standard;
                }
            }

            return value;
        }
    }
}
