using Dreambuild.Extensions;
using Dreambuild.Gis.Display;
using System.Windows.Controls;

namespace Dreambuild.Gis.Desktop.Demos.TibetEarthquake
{
    public class Demo : DemoBase
    {
        public Demo()
        {
            Name = "西藏防灾";
            Description = "西藏自治区地震点分布统计演示程序。";
        }

        public override void OnLoad()
        {
            base.OnLoad();

            this.GetDemoTools().ForEach(t =>
            {
                MenuItem mi = new MenuItem { Header = t.Item1 };
                mi.Click += t.Item2;
                MainWindow.Current.ToolsMenu.Items.Add(mi);
            });

            PythonConsole.Current.SetStaticVariable("sd", typeof(SpotDiagram));
        }

        [DemoTool("打开控制器")]
        public void OpenController()
        {
            PythonPlayer player = new PythonPlayer("Demos\\TibetEarthquake\\Player\\");
            player.Show();
        }
    }
}
