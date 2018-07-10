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
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;

namespace TongJi.Drawing.Viewer3D
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private double _left;
        private double _right;
        private double _top;
        private double _bottom;
        private double[] _data;
        private int _width;
        public Terrain3DView Terrain 
        {
            get
            {
                return terrain;
            }
        }
        public MainWindow()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }
        
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            view1.ZoomExtents();
        }

        public void ShowDemoTerrain(string file)
        {
            ReadDAT(file);

            Terrain.LoadData(_data, _width, _left, _right, _top, _bottom);
            Terrain.TextureType = TextureType.Elevation;

            //Point3D[] RDPts = new Point3D[3];
            //RDPts[0] = new Point3D(484834, 152844, 720);
            //RDPts[1] = new Point3D(486854, 150060, 538);
            //RDPts[2] = new Point3D(484773, 142960, 198);
            //Terrain.RoadColor = Colors.Gray;
            //Terrain.AddRoad(RDPts, 100);
            //RDPts[0].X += 3000;
            //RDPts[2].X += 3000;
            //Terrain.AddRoad(RDPts, 100);

            Terrain.Update();
        }
        
        public void ReadDAT(string source)
        {
            // 读数据
            double yfirst = 0;  //记录第一个读出来的y坐标，如果发生变化，说明第一行数据读取完毕
            _width = 0;
            List<double> databuf = new List<double>();
            StreamReader sr = new StreamReader(source);
            int index = 0;
            //MinimumZ = double.MaxValue;
            //MaximumZ = double.MinValue;
            while (!sr.EndOfStream)
            {
                string strRead = sr.ReadLine();
                if (strRead != "")
                {
                    string[] strs = strRead.Split(' ');
                    double z = double.Parse(strs[2]);
                    databuf.Add(z);
                    // 取第一个点和最后一个点的坐标值
                    if (index == 0)
                    {
                        yfirst = double.Parse(strs[1]);
                        Point pt = new Point(double.Parse(strs[0]), double.Parse(strs[1]));
                        _left = pt.X;
                        _bottom = pt.Y;
                    }
                    if (sr.EndOfStream)
                    {
                        Point pt = new Point(double.Parse(strs[0]), double.Parse(strs[1]));
                        _right = pt.X;
                        _top = pt.Y;
                    }
                    // y值发生变化，记录下列数
                    if (double.Parse(strs[1]) != yfirst && _width == 0)
                    {
                        _width = index;
                    }
                }
                index++;
            }
           // Height = databuf.Count / Width;
            _data = databuf.ToArray();
            sr.Close();
        }

        private void cboxTexture_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!terrain.HasData)
            {
                return;
            }
            switch (cboxTexture.SelectedIndex)
            {
                case 0:
                    terrain.TextureType = TextureType.Elevation;
                    break;
                case 1:
                    terrain.TextureType = TextureType.Slope;
                    break;
                default:
                    break;
            }
            terrain.Update();
        }

        private void sliderVM_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderVM.Value >= 0)
            {
                terrain.VerticalMagnification = sliderVM.Value + 1;
            }
            else
            {
                terrain.VerticalMagnification = 1.0 / Math.Abs(sliderVM.Value);
            }
            terrain.Update();
        }
    }
}
