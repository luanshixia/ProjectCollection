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
    /// Interaction logic for Gallery.xaml
    /// </summary>
    public partial class Gallery : UserControl
    {
        private ActionManager _config = new ActionManager();

        public Gallery()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var root = new TreeNode
                {
                    Name = "绘图工具库",
                    Children = _config.Groups.Select(g =>
                        new TreeNode
                        {
                            Name = g,
                            Children = _config.Entries[g].Select(t =>
                                new TreeNode
                                {
                                    Name = t.Name,
                                    Data = t,
                                    ToolTipEnabled = true
                                }).ToList()
                        }).ToList()
                };
            Tree.ItemsSource = new List<TreeNode> { root };
            (Tree.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem).IsExpanded = true;
        }

        private void Tree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (e.ChangedButton == MouseButton.Left)
            //{
            //    Ok();
            //}
        }

        private void Ok()
        {
            if (Tree.SelectedItem != null)
            {
                var node = Tree.SelectedItem as TreeNode;
                var action = node.Data as ActionConfigEntry;
                if (action != null)
                {
                    var command = action.Command;
                    AutoCADCommands.Interaction.SetActiveDocFocus();
                    AutoCADCommands.Interaction.StartCommand(command + " ");
                }
            }
        }

        private void Tree_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Ok();
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GalleryWindow gw = new GalleryWindow();
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModalWindow(gw);
        }
    }

    public class TreeNode
    {
        public string Name { get; set; }
        public List<TreeNode> Children { get; set; }
        public object Data { get; set; }
        public bool ToolTipEnabled { get; set; }
    }
}
