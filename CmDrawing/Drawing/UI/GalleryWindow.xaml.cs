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

namespace TongJi.Drawing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GalleryWindow : Window
    {
        private ActionManager _config = new ActionManager();

        public GalleryWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GroupList.ItemsSource = _config.Groups;
            GroupList.SelectedIndex = 0;
        }

        private void GroupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupList.SelectedItem != null)
            {
                string group = GroupList.SelectedItem.ToString();
                var features = _config.Entries[group];
                FeatureList.ItemsSource = features;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Ok();
        }

        private void FeatureList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Ok();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Ok()
        {
            if (FeatureList.SelectedItem != null)
            {
                var command = (FeatureList.SelectedItem as ActionConfigEntry).Command;
                this.DialogResult = true;
                AutoCADCommands.Interaction.StartCommand(command + " ");
            }
        }
    }
}
