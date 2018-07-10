using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace TongJi.Drawing.Viewer3D
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public void ExecutableStart(object sender, StartupEventArgs e)
        {
            string arg;
            if (e.Args.Length > 0)
            {
                arg = e.Args[0];
            }
            else
            {
                arg = "DemoTerrain.dat";
            }
            MainWindow w = new MainWindow();
            w.ShowDemoTerrain(arg);
            w.Show();
        }
    }
}
