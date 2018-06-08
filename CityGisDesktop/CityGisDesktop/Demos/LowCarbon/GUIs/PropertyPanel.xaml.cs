using Dreambuild.Extensions;
using Dreambuild.Utils;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Dreambuild.Gis.Desktop.Demos.LowCarbon
{
    /// <summary>
    /// Interaction logic for PropertyPanel.xaml
    /// </summary>
    public partial class PropertyPanel : UserControl, ILanguageSwitcher
    {
        private object[] _objs;

        public PropertyPanel()
        {
            InitializeComponent();
            PropGrid.PropertyValueChanged += PropGrid_PropertyValueChanged;
        }

        void PropGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
        {
            try
            {
                var changedProperty = e.ChangedItem.PropertyDescriptor.Name;
                if (PropGrid.SelectedObject != null && PropGrid.SelectedObjects.Length == 1)
                {
                    var unit = PropGrid.SelectedObject as ComputationUnit;
                    PropertyValueChanged(unit, changedProperty);
                }
                else if (PropGrid.SelectedObjects != null)
                {
                    PropGrid.SelectedObjects.ForEach(x =>
                    {
                        var unit = x as ComputationUnit;
                        PropertyValueChanged(unit, changedProperty);
                    });
                    PropGrid.SelectedObjects = PropGrid.SelectedObjects;
                }

                if (changedProperty == "Type")
                {
                    Demo.ShowTypeTheme();
                }
                PropGrid.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }

        private void PropertyValueChanged(ComputationUnit unit, string changedProperty)
        {
            if (changedProperty == "FAR" || changedProperty == "Levels" || changedProperty == "Type"
                || changedProperty == "Area" || changedProperty == "Levels" || changedProperty == "PerCapitaLivingSpace")
            {
                unit.UpdatePopulation();
            }

            if (unit.Type == BuildingType.Mixed)
            {
                unit.IsValidMixedValue();
                unit.UpdatePopulation();
            }

            unit.UpdateResult();
            unit.UpdateFeature();
        }

        public void SetObjects(params object[] objs)
        {
            _objs = objs;
            string format = LocalizationHelper.GetString("PropertyDetails") + "({0})...";
            EditButton.Content = string.Format(format, _objs.Length);
            switch (_objs.Length)
            {
                case 0:
                    PropGrid.SelectedObject = null;
                    break;
                case 1:
                    PropGrid.SelectedObject = _objs[0];
                    break;
                default:
                    PropGrid.SelectedObjects = _objs;
                    break;
            }
            PropGrid.Refresh();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            string title = LocalizationHelper.GetString("PropertyDetails");
            Dreambuild.Gis.Display.Gui.PropertyWindow(title, _objs, 400, 500, true);
        }

        void ILanguageSwitcher.RefreshLanguage()
        {
            string format = LocalizationHelper.GetString("PropertyDetails") + "({0})...";
            int length = (_objs == null) ? 0 : _objs.Length;
            EditButton.Content = string.Format(format, length);
            PropGrid.Refresh();
        }

        public void SwitchBuildingParcel()
        {
            if (_objs != null)
            {
                foreach (var o in _objs)
                {
                    ComputationUnit unit = o as ComputationUnit;
                    unit.UpdateResult();
                    unit.UpdateFeature();
                }

                PropGrid.Refresh();
            }
        }

        public void RefreshComputation()
        {
            if (_objs != null)
            {
                foreach (var o in _objs)
                {
                    ComputationUnit unit = o as ComputationUnit;
                    unit.UpdatePopulation();
                    unit.UpdateResult();
                    unit.UpdateFeature();
                }

                PropGrid.Refresh();
            }
        }
    }
}
