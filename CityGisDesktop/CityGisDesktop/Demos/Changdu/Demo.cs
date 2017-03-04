using Dreambuild.Gis.Display;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Dreambuild.Gis.Desktop.Demos.Changdu
{
    public class Demo : DemoBase
    {
        public Demo()
        {
            Name = "昌都演示";
            Description = "昌都地区现状分析演示程序。";
        }

        public override void OnLoad()
        {
            base.OnLoad();

            MainWindow.Current.HideMenu();
            MainWindow.Current.HidePanel("图层", "专题地图", "调试与测试", "IronPython Console");
            MainWindow.Current.AddPanel("功能", new DemoPanel(), 0);

            this.GetDemoTools().ForEach(t =>
            {
                MenuItem mi = new MenuItem { Header = t.Item1 };
                mi.Click += t.Item2;
                MainWindow.Current.ToolsMenu.Items.Add(mi);
            });

            MapDataManager.Open("Demos\\Changdu\\昌都.ciml");
            MapControl.Current.FindMapLayers("区域").ForEach(x => x.ApplyToolTip(f => UIHelper.TitledToolTip(f["名称"], string.Empty)));
        }
    }

    public static class DataManager
    {
        public const string DataPath = "Demos\\Changdu\\Data\\";
        public static List<string> Topics { get; private set; }

        static DataManager()
        {
            Topics = System.IO.Directory.GetFiles(DataPath).Select(f => System.IO.Path.GetFileNameWithoutExtension(f)).ToList();
        }

        public static string[][] GetTopicData(string topic)
        {
            return System.IO.File.ReadAllLines(DataPath + topic + ".csv", Encoding.Default).Select(x => x.Split(',')).ToArray();
        }

        public static string[] GetTopicItems(string topic)
        {
            return GetTopicData(topic)[1].Skip(2).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        }

        public static string[] GetTopicItemData(string topic, string item)
        {
            var data = GetTopicData(topic);
            var items = GetTopicItems(topic);
            int index = items.ToList().IndexOf(item) + 2;
            return data.Skip(2).Where(x => !string.IsNullOrWhiteSpace(x[index])).Select(x => x[1]).Distinct().ToArray();
        }
    }
}
