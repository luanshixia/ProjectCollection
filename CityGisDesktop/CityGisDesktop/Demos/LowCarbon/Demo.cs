using Dreambuild.Gis.Display;
using Dreambuild.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace Dreambuild.Gis.Desktop.Demos.LowCarbon
{
    public class Demo : DemoBase, ILanguageSwitcher
    {
        private DemoPanel _demoPanel = null;
        private readonly PropertyPanel _propPanel = new PropertyPanel();
        public static ComputationUnitType CaculateType { get; set; }

        public Demo()
        {
            Name = "C-Smart";
            Description = "一个城市规划低碳分析程序。";
        }

        public override void OnLoad()
        {
            base.OnLoad();
            _demoPanel = new DemoPanel(_propPanel);
            MainWindow.Current.HidePanel("属性", "专题地图", "查询", "IronPython Console", "调试与测试");
            MainWindow.Current.AddPanel("Function", _demoPanel, 0);
            MainWindow.Current.AddPanel("Property", _propPanel, 1);
            MainWindow.Current.Title = "C-Smart 1.0";

            (this as ILanguageSwitcher).RefreshLanguage();

            MainWindow.Current.BeforeMapShow += Current_BeforeMapShow;
            MainWindow.Current.AfterMapShow += Current_AfterMapShow;
            MapDataManager.MapDataChanged += MapDataManager_MapDataChanged;
            SelectionSet.SelectionChanged += SelectionSet_SelectionChanged;
        }

        void ILanguageSwitcher.RefreshLanguage()
        {
            RefreshToolsMenu();
            (_propPanel as ILanguageSwitcher).RefreshLanguage();
        }

        private void RefreshToolsMenu()
        {
            MainWindow.Current.ToolsMenu.Items.Clear();
            this.GetDemoTools().ForEach(t =>
            {
                MenuItem mi = new MenuItem { Header = LocalizationHelper.GetString(t.Item1) };
                mi.Click += t.Item2;
                mi.IsEnabled = ((MapDataManager.LatestMap != null) && (MapDataManager.LatestMap.Layers.Count > 0));
                MainWindow.Current.ToolsMenu.Items.Add(mi);
            });
        }

        public void RefreshComputation()
        {
            _propPanel.RefreshComputation();
        }

        #region Event handlers

        void Current_AfterMapShow()
        {
            //MapControl.Current.FindMapLayers("地块").ForEach(x => x.ApplyColorTheme(new ParcelUsageTheme()));
            ShowTypeTheme();
        }

        void Current_BeforeMapShow()
        {
            PreProcessMap();
        }

        void MapDataManager_MapDataChanged()
        {
            MapDataManager.LatestMap.BeforeSave += LatestMap_BeforeSave;
            _propPanel.SetObjects();
            RefreshToolsMenu();
        }

        void LatestMap_BeforeSave(object sender, EventArgs e)
        {
            (sender as Map).Layers.ForEach(layer =>
            {
                layer.Features.ForEach(f =>
                {
                    ComputationUnit.Get(f).UpdateFeature();
                });
            });
        }

        void SelectionSet_SelectionChanged()
        {
            if (SelectionSet.Contents.Count > 1)
            {
                _propPanel.SetObjects(SelectionSet.Contents.Select(f => ComputationUnit.Get(f)).ToArray());
            }
            else if (SelectionSet.Contents.Count > 0)
            {
                _propPanel.SetObjects(ComputationUnit.Get(SelectionSet.Contents.First()));
            }
            else
            {
                _propPanel.SetObjects();
            }
        }

        private void PreProcessMap() // newly 20130506 // mod 20140620
        {
            if (!MapDataManager.LatestMap.Layers.Any(l => l.Name == MainDemo.LAYER_DRAW_POINT))
            {
                MapDataManager.LatestMap.Layers.Add(new VectorLayer(MainDemo.LAYER_DRAW_POINT, VectorLayer.GEOTYPE_POINT));
                MapDataManager.LatestMap.Layers.Add(new VectorLayer(MainDemo.LAYER_DRAW_POLY, VectorLayer.GEOTYPE_REGION));
            }

            MapDataManager.LatestMap.Layers.ForEach(layer => layer.Features.ForEach(f => InitializeFeature(f, layer.Name)));
        }

        private void InitializeFeature(IFeature f, string layerName)
        {
            if (string.IsNullOrWhiteSpace(f["layerName"]))
            {
                f["layerName"] = layerName;
            }
            if (string.IsNullOrWhiteSpace(f["far"]))
            {
                f["far"] = "1.0";
            }
            if (string.IsNullOrWhiteSpace(f["levels"]))
            {
                f["levels"] = "5";
            }
            if (string.IsNullOrWhiteSpace(f["escType"]))
            {
                f["escType"] = Parameter.Base.Standard.EnergyType.ToString();
            }
            if (string.IsNullOrWhiteSpace(f["escValue"]))
            {
                f["escValue"] = Parameter.Base.Standard.TempValue.ToString();
            }
            if (string.IsNullOrWhiteSpace(f["perResi"]))
            {
                f["perResi"] = Parameter.Trip.PerCapitaResidencial.ToString();
            }
            if (string.IsNullOrWhiteSpace(f["perOffic"]))
            {
                f["perOffic"] = Parameter.Trip.PerCapitaOffice.ToString();
            }
            if (string.IsNullOrWhiteSpace(f["perIndu"]))
            {
                f["perIndu"] = Parameter.Trip.PerCapitaIndustry.ToString();
            }
            ComputationUnit.Attach(f);
        }

        public static void ShowTypeTheme()
        {
            MapControl.Current.Layers.ForEach(x => x.ApplyColorTheme(new LcTypeTheme()));
        }

        #endregion

        #region DemoTool actions

        [DemoTool("ExportCSV")]
        public void ExportCsv()
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog { Filter = "Comma Separated Values (*.csv)|*.csv" };
            if (sfd.ShowDialog() == true)
            {
                var model = new LcComputation(MapDataManager.LatestMap, Demo.CaculateType);
                var exporter = new Dreambuild.IO.CsvExporter<ComputationUnit>(model.AllUnits, new List<string>
                {
                    "Name",
                    "Type",
                    "GreenField",
                    "Area",
                    "Floor",
                    "Populations",
                    "CarbonProduction.Material",
                    "CarbonProduction.Construction",
                    "CarbonProduction.Maintenance",
                    "CarbonProduction.Maintenance.AirConditioning",
                    "CarbonProduction.Maintenance.Lighting",
                    "CarbonProduction.Maintenance.Equipment",
                    "CarbonProduction.Maintenance.Heating",
                    "CarbonProduction.Recycle",
                    "GreenFieldConsumption"
                });
                exporter.Headers.AddRange(new List<string> 
                {
                    "名称",
                    "类型",
                    "绿地类型",
                    "面积",
                    "建筑面积",
                    "人口",
                    "建材准备",
                    "建造过程",
                    "运行维护",
                    "空调",
                    "照明",
                    "设备",
                    "供暖",
                    "拆毁回收",
                    "绿地碳汇"
                });
                exporter.ShowHeaders = true;
                exporter.Export(sfd.FileName);
                MainDemo.TryOpenFile(sfd.FileName);
            }
        }

        #endregion
    }

    /**
     * Please keep adequate abstraction of the following types.
     * 
     */

    public class LcColorTheme : MultiColorGradientTheme
    {
        private Color _zeroColor = Colors.DarkGray;
        private Func<double, ComputationUnit, double> _selector = (x, cu) => x;

        public LcColorTheme(string property, Func<double, ComputationUnit, double> selector = null)
            : base(property, 0, 100)
        {
            Stops.Clear();
            AddStop(0.0, Colors.Green);
            AddStop(0.5, Colors.Yellow);
            AddStop(1.0, Colors.Red);

            if (selector != null)
            {
                _selector = selector;
            }
        }

        public override double GetValue(IFeature f)
        {
            var cu = ComputationUnit.Get(f);
            return GetCuPropertyValue(cu);
        }

        public void DetermineValueRange(List<ComputationUnit> cus)
        {
            MinValue = cus.Any() ? cus.Min(cu => GetCuPropertyValue(cu)) : 0;
            MaxValue = cus.Any() ? cus.Max(cu => GetCuPropertyValue(cu)) : 0;
        }

        public List<Tuple<string, string>> BuildLegend(int count, bool zeroAtEnd)
        {
            double seg = 1.0 / count;
            List<Tuple<string, string>> result = Enumerable.Range(0, count + 1)
                .Select(i => Tuple.Create(i * seg, (i + 1) * seg))
                .Select(x => Tuple.Create(GetLabelString(x.Item1, x.Item2), GetColorString(GetColorOfPos(x.Item1))))
                .ToList();

            if (zeroAtEnd)
            {
                result.Add(Tuple.Create("0.0", GetColorString(_zeroColor)));
            }
            else
            {
                result.Insert(0, Tuple.Create("0.0", GetColorString(_zeroColor)));
            }
            return result;
        }

        private double GetValueOfPos(double pos)
        {
            return MinValue + pos * (MaxValue - MinValue);
        }

        private Color GetColorOfPos(double pos)
        {
            return base.GetColorByValue(GetValueOfPos(pos));
        }

        private string GetColorString(Color color)
        {
            return string.Format("rgb({0},{1},{2})", color.R, color.G, color.B);
        }

        private string GetLabelString(double pos1, double pos2)
        {
            return string.Format("{0:N1} - {1:N1}", GetValueOfPos(pos1), GetValueOfPos(pos2));
        }

        private double GetCuPropertyValue(ComputationUnit cu)
        {
            double result = cu.GetPropertyValue(Property)
               .TryToString()
               .TryParseToDouble();

            return _selector(result, cu);
        }

        public override Color GetColorByValue(double value)
        {
            if (value == 0.0)
            {
                return _zeroColor;
            }
            else
            {
                return base.GetColorByValue(value);
            }
        }
    }

    public class LcTypeTheme : MultiConditionTheme
    {
        public Dictionary<string, Color> DictColor { get; private set; }

        public LcTypeTheme()
        {
            var dict = new Dictionary<string, Color>
            {
                { "Office", Colors.Magenta }, 
                { "Residencial", Colors.Yellow }, 
                { "Hotel", Colors.SkyBlue }, 
                { "Retail", Colors.Red }, 
                { "Education", Colors.Chocolate }, 
                { "GreenField", Colors.Green }, 
                { "Industry", Colors.DimGray }, 
                { "Other", Colors.LightGray }, 
                { "Mixed", Colors.LightBlue }, 
                { "Unknown", Colors.LightCoral }, 
            };
            var name = new Dictionary<string, string>
            {
                { "Office", "办公" }, 
                { "Residencial", "居住" }, 
                { "Hotel", "旅馆" }, 
                { "Retail", "商业" }, 
                { "Education", "教育" }, 
                { "GreenField", "绿地" }, 
                { "Industry", "工业" }, 
                { "Other", "其他" }, 
                { "Mixed", "混合" }, 
                { "Unknown", "未知" }, 
            };
            //英文版添加
            var nameE = new Dictionary<string, string>
            {
                { "Office", "Office" }, 
                { "Residencial", "Residencial" }, 
                { "Hotel", "Hotel" }, 
                { "Retail", "Retail" }, 
                { "Education", "Education" }, 
                { "GreenField", "GreenField" }, 
                { "Industry", "Industry" }, 
                { "Other", "Other" }, 
                { "Mixed", "Mixed" }, 
                { "Unknown", "Unknown" }, 
            };
            DictColor = new Dictionary<string, Color>();
            foreach (var entry in dict)
            {
                if (LocalizationHelper.CurrentLocale == Locales.ZH_CN)
                {
                    DictColor[name[entry.Key]] = entry.Value;
                }
                else
                {
                    DictColor[nameE[entry.Key]] = entry.Value;
                }
                this.AddCondition(f => ComputationUnit.Get(f).Type.ToString() == entry.Key, entry.Value);
            }
        }

        public List<Tuple<string, string>> BuildLegend()
        {
            return DictColor.Select(x => Tuple.Create(x.Key, GetColorString(x.Value))).ToList();
        }

        private string GetColorString(Color color)
        {
            return string.Format("rgb({0},{1},{2})", color.R, color.G, color.B);
        }
    }
}
