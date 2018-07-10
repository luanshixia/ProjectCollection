using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace TongJi.Drawing.Viewer3D
{
    /// <summary>
    /// MainWindow.xaml code behind.
    /// </summary>
    public partial class MainWindow : Window
    {
        private double _left;
        private double _right;
        private double _top;
        private double _bottom;
        //private double[] _data;
        private int _width;

        public MainWindow()
        {
            this.InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }
        
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Viewport.ZoomExtents();
        }

        public void ShowDemoTerrain(string file)
        {
            var data = this.ReadDAT(file);

            this.Terrain.LoadData(data, _width, _left, _right, _top, _bottom);
            this.Terrain.TextureType = TextureType.Elevation;

            this.Terrain.Update();
        }
        
        public double[] ReadDAT(string source)
        {
            // 读数据
            double yfirst = 0;  //记录第一个读出来的y坐标，如果发生变化，说明第一行数据读取完毕
            _width = 0;
            List<double> databuf = new List<double>();
            using (var sr = new StreamReader(source))
            {
                int index = 0;
                //MinimumZ = double.MaxValue;
                //MaximumZ = double.MinValue;
                while (!sr.EndOfStream)
                {
                    string strRead = sr.ReadLine();
                    if (!string.IsNullOrWhiteSpace(strRead))
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
                return databuf.ToArray();
            }
        }

        private void cboxTexture_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.Terrain.HasData)
            {
                return;
            }
            switch (cboxTexture.SelectedIndex)
            {
                case 0:
                    this.Terrain.TextureType = TextureType.Elevation;
                    break;
                case 1:
                    this.Terrain.TextureType = TextureType.Slope;
                    break;
                default:
                    break;
            }
            this.Terrain.Update();
        }

        private void sliderVM_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderVM.Value >= 0)
            {
                this.Terrain.VerticalMagnification = sliderVM.Value + 1;
            }
            else
            {
                this.Terrain.VerticalMagnification = 1.0 / Math.Abs(sliderVM.Value);
            }
            this.Terrain.Update();
        }
    }
}
