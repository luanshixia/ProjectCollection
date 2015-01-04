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
    /// TileProject.xaml 的交互逻辑
    /// </summary>
    public partial class TileProject : UserControl
    {
        public TileProject()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var tiles = DocumentManager.CurrentDocument.Projects.Select(x =>
            {
                var button = new ProjectButton { ButtonText = x.P1A };
                button.MouseLeftButtonUp += (s, arg) =>
                {
                    var id = DocumentManager.CurrentDocument.Projects.IndexOf(x);
                    NavigationManager.Navigate("RichProjectPage.xaml", "id=" + id);
                    if (RichProjectPage.Current != null)
                    {
                        RichProjectPage.Current.SetProj(id);
                    }
                };
                return button;
            }).ToList<FrameworkElement>();
            ImageButton btnNewProj = new ImageButton { NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "add_proj.png", UriKind.Relative)), Width = 200, Height = 150, Cursor = Cursors.Hand, ToolTip = "新建项目" };
            btnNewProj.Click += (s, arg) =>
            {
                NewProj np = new NewProj { Owner = MainWindow.Current };
                if (np.ShowDialog() == true)
                {
                    DocumentManager.CurrentDocument.Projects.Add(new ProjectStatistics(np.txtProjName.Text, np.txtProjLocation.Text));
                    this.UserControl_Loaded(null, null);
                }
            };
            tiles.Add(btnNewProj);
            tileGridControl1.Tiles = tiles;
        }
    }
}
