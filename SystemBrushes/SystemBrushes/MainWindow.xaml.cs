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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SystemBrushes
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var brush in GetSystemBrushes())
            {
                root.Items.Add(new ListItem(brush.Item1, brush.Item2));
            }
        }

        private IEnumerable<Tuple<Color, string>> GetSystemBrushes()
        {
            var props = typeof(Brushes).GetProperties();
            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(SolidColorBrush))
                {
                    yield return new Tuple<Color, string>((prop.GetValue(null, null) as SolidColorBrush).Color, prop.Name);
                }
            }
        }

        private void root_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (ListItem item in root.Items)
            {
                if (item == root.SelectedItem)
                {
                    item.SetHighlight();
                }
                else
                {
                    item.CancelHighlight();
                }
            }
        }
    }
}
