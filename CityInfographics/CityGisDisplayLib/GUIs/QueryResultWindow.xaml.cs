using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TongJi.Gis.Display
{
    /// <summary>
    /// QueryResultWindow.xaml 的交互逻辑
    /// </summary>
    public partial class QueryResultWindow : Window
    {
        public QueryResultWindow()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public void SetData(IEnumerable<IFeature> features)
        {
            dataGrid1.Columns.Clear();
            dataGrid1.ItemsSource = features;
            dataGrid1.AutoGenerateColumns = false;

            var columns = features.SelectMany(f => f.Properties.Keys).Distinct(StringComparer.OrdinalIgnoreCase);
            dataGrid1.Columns.Add(new DataGridTextColumn { Header = "FeatId", Binding = new System.Windows.Data.Binding("FeatId") }); 
            foreach (string col in columns)
            {
                dataGrid1.Columns.Add(new DataGridTextColumn { Header = col , Binding = new System.Windows.Data.Binding(string.Format("Properties[{0}]", col)) }); 
            }
        }
    }
}
