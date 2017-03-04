using Dreambuild.Gis.Display;
using Dreambuild.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Dreambuild.Gis.Desktop.Demos.LowCarbon
{
    /// <summary>
    /// DemoPanel.xaml 的交互逻辑
    /// </summary>
    public partial class DemoPanel : UserControl
    {
        private PropertyPanel _propertyPanel = null;
        private bool _initialized = false;

        public DemoPanel(PropertyPanel propertyPanel)
        {
            InitializeComponent();
            _propertyPanel = propertyPanel;
            Demo.CaculateType = ComputationUnitType.Parcel;
            _initialized = true;
        }

        private void RecaculatePopulation()
        {
            List<ComputationUnit> UnitList = MapDataManager.LatestMap.Layers.SelectMany(layer => layer.Features.Select(ComputationUnit.Get)).ToList();

            foreach (var unit in UnitList)
            {
                unit.UpdatePopulation();
                unit.UpdateResult();
                unit.UpdateFeature();
            }
        }

        private void CaculateType_Set(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
                return;

            var button = sender as RadioButton;
            var CaculateType = button.Name.ToString();
            if (CaculateType == "RB_Land")
            {
                Demo.CaculateType = ComputationUnitType.Parcel;
            }
            else if (CaculateType == "RB_Architecture")
            {
                Demo.CaculateType = ComputationUnitType.Building;
            }
            else
            {
                throw new Exception("Unknown Caculate Type[" + CaculateType + "]");
            }

            if (_propertyPanel != null)
            {
                string msg = LocalizationHelper.GetString("CulatePopulation");
                if (MessageBox.Show(msg, "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    RecaculatePopulation();
                }
                _propertyPanel.SwitchBuildingParcel();
            }
        }

        private void Button_Result(object sender, RoutedEventArgs e)
        {
            try
            {
                var model = new LcComputation(MapDataManager.LatestMap, Demo.CaculateType);
                var result = new ResultSummary
                {
                    Count = model.AllUnits.Count,
                    Area = model.AllUnits.Sum(x => x.Area),
                    Green = model.GetGreenCarbonConsumption(),
                    Building = model.GetBuildingCarbonProduction(),
                    Trip = model.GetTripCarbonProduction()
                };

                string SumResult = LocalizationHelper.GetString("RS:SumResult");
                Gui.PropertyWindow(SumResult, result, 300, 400);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }

        private void Button_Open(object sender, RoutedEventArgs e)
        {
            MainWindow.Current.OpenMap();
        }

        private void Button_BaseParams(object sender, RoutedEventArgs e)
        {
            BaseParams baseParams = Parameter.Base;
            //Gui.PropertyInputs("基础参数", Parameter.Base, 350, 400);
            var baseParameterWindow = new BaseParamsWindow { Owner = MainWindow.Current };
            baseParameterWindow.ShowDialog();
        }

        private void Button_BuildingParams(object sender, RoutedEventArgs e)
        {
            //Gui.PropertyInputs("建筑参数", Parameter.Building, 350, 400);
            var buildingParamsWindow = new BuildingParamsWindow { Owner = MainWindow.Current };
            buildingParamsWindow.ShowDialog();
        }

        private void Button_TripParams(object sender, RoutedEventArgs e)
        {
            var pw = new ParametersWindow { Owner = MainWindow.Current };
            pw.ShowDialog();
        }

        private void Button_GreenParams(object sender, RoutedEventArgs e)
        {
            //Gui.PropertyInputs("绿地参数", Parameter.Green, 400, 400);
            var greenParamsWindow = new GreenParamsWindow { Owner = MainWindow.Current };
            greenParamsWindow.ShowDialog();
        }

        private void Button_Visualization(object sender, RoutedEventArgs e)
        {
            var dict = new Dictionary<string, string>();
            dict["1. 建材准备"] = "CarbonProduction.Material";
            dict["2. 建造过程"] = "CarbonProduction.Construction";
            dict["3. 运行维护"] = "CarbonProduction.Maintenance";
            dict["3.1 空调"] = "CarbonProduction.Maintenance.AirConditioning";
            dict["3.2 照明"] = "CarbonProduction.Maintenance.Lighting";
            dict["3.3 设备"] = "CarbonProduction.Maintenance.Equipment";
            dict["3.4 供暖"] = "CarbonProduction.Maintenance.Heating";
            dict["4. 拆毁回收"] = "CarbonProduction.Recycle";
            dict["5. 绿地碳汇"] = "GreenFieldConsumption";
            dict["6. 功能类型"] = "Type";
            var options = dict.Keys.ToArray();
            var optionsE = dict.Values.ToArray();
            Tuple<string, int> opt;
            bool isDensity = false;
            string caption;
            string title;
            string url = GetFileUrl("Web/legend.html");
            if (LocalizationHelper.CurrentLocale == Locales.ZH_CN)
            {
                opt = GetOption2("选择项目", options);
                caption = "图例";
                title = "图例";
            }
            else
            {
                opt = GetOption2("Choose project", optionsE);
                caption = "Legend";
                title = "Legend";
            }
            if (opt.Item1 == "Cancel")
            {
                return;
            }
            if (opt.Item1 == "B")
            {
                isDensity = true;
                title += " (kg/m2)";
            }
            else
            {
                if (opt.Item2 != 9)
                {
                    title += " (kg)";
                }
            }
            var model = new LcComputation(MapDataManager.LatestMap, Demo.CaculateType);
            var path = dict[options[opt.Item2]];
            if (path == "Type")
            {
                var theme = new LcTypeTheme();
                MapControl.Current.Layers.ForEach(x => x.ApplyColorTheme(theme));
                var items = theme.BuildLegend().Select(x => new { text = x.Item1, color = x.Item2 }).ToArray();
                var data = new { title, items };
                System.IO.File.WriteAllText("Web/data/legend.json", Newtonsoft.Json.JsonConvert.SerializeObject(data));
            }
            else
            {
                var theme = isDensity ? new LcColorTheme(path, (x, cu) => x / cu.Area) : new LcColorTheme(path);
                theme.DetermineValueRange(model.AllUnits);
                MapControl.Current.Layers
                    .Where(l => l.LayerData.GeoType == VectorLayer.GEOTYPE_REGION)
                    .ForEach(x => x.ApplyColorTheme(theme));
                var items = theme.BuildLegend(5, opt.Item2 == 7).Select(x => new { text = x.Item1, color = x.Item2 }).ToArray();
                var data = new { title, items };
                System.IO.File.WriteAllText("Web/data/legend.json", Newtonsoft.Json.JsonConvert.SerializeObject(data));
            }
            Gui.WebBrowser(caption, url, 450, 400);
        }

        private static string GetFileUrl(string relativeUrl)
        {
            return string.Format("file://{0}/{1}", Environment.CurrentDirectory, relativeUrl);
        }

        public static Tuple<string, int> GetOption2(string tip, params string[] options)
        {
            string title = LocalizationHelper.GetString("Tip2:Title");

            Window window = new Window { Width = 400, SizeToContent = SizeToContent.Height, WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen, ShowInTaskbar = false, WindowStyle = WindowStyle.ToolWindow, Title = title };
            window.Owner = Application.Current.MainWindow;
            Canvas sp = new Canvas { Height = options.Count() * 30 };
            TextBlock tb = new TextBlock { Text = (tip == string.Empty ? LocalizationHelper.GetString("Tip2:Choose") : tip), TextWrapping = TextWrapping.Wrap };
            sp.Children.Add(tb);
            Tuple<string, int> result = Tuple.Create("Cancel", 0);
            int top = 30;
            var txts = options.Select((x, i) =>
            {
                string txt = x;
                if (txt.Contains("CarbonProduction."))
                {
                    txt = txt.Replace("CarbonProduction.", "");
                }
                TextBox textBox = new TextBox { Text = txt, Background = new SolidColorBrush(Colors.Orange), IsReadOnly = true, Width = 200 };
                Canvas.SetLeft(textBox, 10);
                Canvas.SetTop(textBox, i * 25 + top);
                return textBox;
            }).ToList();
            txts.ForEach(x => sp.Children.Add(x));

            var btnSet1 = options.Select((x, i) =>
            {
                if (i != options.Length - 1)
                {
                    Button btn = new Button { Content = LocalizationHelper.GetString("Tip2:Summation"), Tag = i, Width = 80 };
                    Canvas.SetLeft(btn, 220);
                    Canvas.SetTop(btn, i * 25 + top);
                    return btn;
                }
                else
                {
                    Button btn = new Button { Content = LocalizationHelper.GetString("Tip2:Show"), Tag = i, Width = 80 };
                    Canvas.SetLeft(btn, 220);
                    Canvas.SetTop(btn, i * 25 + top);
                    return btn;
                }
            }).ToList();
            btnSet1.ForEach(x => sp.Children.Add(x));
            btnSet1.ForEach(x => x.Click += (s, args) => { result = Tuple.Create("A", (int)x.Tag); window.DialogResult = true; });

            var btnSet2 = options.Select((x, i) =>
            {
                if (i != options.Length - 1)
                {
                    Button btn = new Button
                    {
                        Content = LocalizationHelper.GetString("Tip2:Density"),
                        Tag = i,
                        Width = 60
                    };
                    Canvas.SetLeft(btn, 305);
                    Canvas.SetTop(btn, i * 25 + top);
                    return btn;
                }
                else
                {
                    Button btn = new Button
                    {
                        Content = LocalizationHelper.GetString("Tip2:Show"),
                        Tag = i,
                        Width = 60,
                        Visibility = System.Windows.Visibility.Hidden
                    };
                    Canvas.SetLeft(btn, 305);
                    Canvas.SetTop(btn, i * 25 + top);
                    return btn;
                }
            }).ToList();
            btnSet2.ForEach(x => sp.Children.Add(x));
            btnSet2.ForEach(x => x.Click += (s, args) => { result = Tuple.Create("B", (int)x.Tag); window.DialogResult = true; });

            window.Content = sp;
            window.ShowDialog();
            return result;
        }
    }
}
