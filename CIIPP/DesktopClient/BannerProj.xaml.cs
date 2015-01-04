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

using CIIPP;

namespace DesktopClient
{
    /// <summary>
    /// BannerProj.xaml 的交互逻辑
    /// </summary>
    public partial class BannerProj : UserControl
    {
        public static BannerProj Current { get; private set; }

        public BannerProj()
        {
            InitializeComponent();

            Current = this;
        }

        public void UpdateProjList()
        {
            ProjList.ItemsSource = DocumentManager.CurrentDocument.Projects.Select(x =>
            {
                var paper = new ThumbnailControl { Width = 90, Height = 60 };
                paper.Paper.Children.Add(new TextBlock { Text = x.P1A, FontSize = 11, TextWrapping = TextWrapping.Wrap, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(5) });
                paper.MouseLeftButtonUp += (sender, e) =>
                {
                    var id = DocumentManager.CurrentDocument.Projects.IndexOf(x);
                    NavigationManager.Navigate("RichProjectPage.xaml", "id=" + id);
                    if (RichProjectPage.Current != null)
                    {
                        RichProjectPage.Current.SetProj(id);
                    }
                };
                paper.MouseRightButtonUp += (sender, e) =>
                {
                    paper.ContextMenu = BuildContextMenu(new Dictionary<string, Action>
                    {
                        { "删除项目", () => 
                            {
                                if (MessageBox.Show("确实要删除该项目吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question) ==  MessageBoxResult.Yes)
                                {
                                    DocumentManager.CurrentDocument.Projects.RemoveAt(DocumentManager.CurrentDocument.Projects.IndexOf(x));                
                                    UpdateProjList();
                                }
                            } 
                        }
                    });
                };
                return paper;
            });
            ProjList.ReadyAnimation();
        }

        public static ContextMenu BuildContextMenu(Dictionary<string, Action> items)
        {
            ContextMenu menu = new ContextMenu();
            foreach (var item in items)
            {
                MenuItem mi = new MenuItem { Header = item.Key };
                mi.Click += (sender, e) => item.Value();
                menu.Items.Add(mi);
            }
            return menu;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //this.UpdateProjList();
        }
    }
}
