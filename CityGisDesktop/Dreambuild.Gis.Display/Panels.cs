using Dreambuild.Extensions;
using Dreambuild.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Dreambuild.Gis.Display
{
    /// <summary>
    /// 地图图层面板
    /// </summary>
    public class MapLayerPanelControl : UserControl, ILanguageSwitcher
    {
        private StackPanel _layoutRoot;
        private Slider _opacitySlider;
        private TreeView _mapLayerList;
        private List<Tuple<Map, List<MapLayer>>> _mapAndLayers;


        public MapLayerPanelControl()
        {
            _layoutRoot = new StackPanel();
            _opacitySlider = new Slider { Minimum = 0, Maximum = 1, Value = 1 };
            _mapLayerList = new TreeView { Height = 200, BorderBrush = new SolidColorBrush(Colors.LightGray) };
            _layoutRoot.Children.Add(_mapLayerList);
            this.Content = _layoutRoot;
            this.Margin = new Thickness(5);
        }

        public void Update()
        {
            _mapLayerList.Items.Clear();
            _mapAndLayers = MapControl.Current.MapAndLayers;
            foreach (var map in _mapAndLayers)
            {
                TreeViewItem mapNode = new TreeViewItem();
                mapNode.Header = GetMapNodeHeader(map.Item1);
                foreach (var layer in map.Item2)
                {
                    TreeViewItem layerNode = new TreeViewItem();
                    layerNode.Header = GetLayerNodeHeader(layer);
                    mapNode.Items.Add(layerNode);
                }
                _mapLayerList.Items.Add(mapNode);
            }
        }

        private FrameworkElement GetLayerNodeHeader(MapLayer layer)
        {
            StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal };
            CheckBox cb = new CheckBox { Content = layer.LayerData.Name };
            cb.IsChecked = layer.Visibility == System.Windows.Visibility.Visible;
            cb.Checked += (sender, e) => LayerVisible(layer, true);
            cb.Unchecked += (sender, e) => LayerVisible(layer, false);
            Slider slider = new Slider { Minimum = 0, Maximum = 1, Width = 40, Margin = new Thickness(10, 0, 0, 0) };
            slider.SetBinding(Slider.ValueProperty, new System.Windows.Data.Binding("Opacity") { Source = layer, Mode = System.Windows.Data.BindingMode.TwoWay });
            sp.Children.Add(cb);
            sp.Children.Add(slider);
            return sp;
        }

        void ILanguageSwitcher.RefreshLanguage()
        {
            Update();
        }

        private FrameworkElement GetMapNodeHeader(Map map)
        {
            StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal };
            //添加语言切换判断
            CheckBox cb;
            if (LocalizationHelper.CurrentLocale == Locales.ZH_CN)
            {
                cb = new CheckBox { Content = string.IsNullOrEmpty(map.Name) ? string.Format("地图 {0}", _mapAndLayers.Select(x => x.Item1).ToList().IndexOf(map) + 1) : map.Name };
            }
            else
            {
                cb = new CheckBox { Content = string.IsNullOrEmpty(map.Name) ? string.Format("Map {0}", _mapAndLayers.Select(x => x.Item1).ToList().IndexOf(map) + 1) : map.Name };
            }
            cb.IsChecked = true;
            var layers = _mapAndLayers.First(x => x.Item1 == map).Item2;
            cb.Checked += (sender, e) => layers.ForEach(layer => LayerVisible(layer, true));
            cb.Unchecked += (sender, e) => layers.ForEach(layer => LayerVisible(layer, false));
            sp.Children.Add(cb);
            return sp;
        }

        private static void LayerVisible(MapLayer layer, bool visible)
        {
            Visibility v = visible ? Visibility.Visible : Visibility.Collapsed;
            layer.Visibility = v;
            layer.LabelLayer.Visibility = v;
        }
    }

    /// <summary>
    /// 专题面板
    /// </summary>
    public class FeaturePanelControl : UserControl
    {
        private ComboBox _comboBox = new ComboBox { BorderBrush = new SolidColorBrush(Colors.LightGray) };
        private CheckBox _checkBox = new CheckBox();
        private StackPanel _layoutRoot;
        private StackPanel _vLayout;
        public IColorTheme ParcelTheme { get; private set; }

        private Dictionary<string, Color> _dictColor = new Dictionary<string, Color>();
        public string None; //无
        public string LandCharacter;//用地性质
        public string PlotRatio;//容积率
        public string BuildingDensity;//建筑密度
        public string GreenSpaceRatio;//绿地率
        public string BuildingMax;//建筑限高
        public void Update()
        {
            _comboBox.Items.Clear();
            LandCharacter = LocalizationHelper.GetString("LandCharacter");
            _comboBox.Items.Add(LandCharacter);
        }
        public FeaturePanelControl()
        {
            _layoutRoot = new StackPanel();
            this.Content = _layoutRoot;
            this.Margin = new Thickness(5);

            _comboBox.Items.Add("无");
            _comboBox.Items.Add(LandCharacter);
            _comboBox.Items.Add("容积率");
            _comboBox.Items.Add("建筑密度");
            _comboBox.Items.Add("绿地率");
            _comboBox.Items.Add("建筑限高");
            _comboBox.SelectedIndex = 0;
            _comboBox.SelectionChanged += comboboxCallback;
            _layoutRoot.Children.Add(_comboBox);

            _checkBox.Content = "图例";
            _checkBox.IsChecked = true;
            _checkBox.Visibility = System.Windows.Visibility.Collapsed;
            _checkBox.Click += checkboxCallback;
            _layoutRoot.Children.Add(_checkBox);

            _vLayout = new StackPanel();
            _layoutRoot.Children.Add(_vLayout);
        }

        private double GetPropMinValue(MapLayer layer, string propertyName)
        {
            double min = 0;
            foreach (var feature in layer.LayerData.Features)
            {
                foreach (var property in feature.Properties)
                {
                    if (property.Key == propertyName && property.Value != string.Empty)
                    {
                        double propertyVal = Convert.ToDouble(property.Value);
                        min = (min < propertyVal) ? min : propertyVal;
                    }
                }
            }
            return min;
        }

        private double GetPropMaxValue(MapLayer layer, string propertyName)
        {
            double max = 0;
            foreach (var feature in layer.LayerData.Features)
            {
                foreach (var property in feature.Properties)
                {
                    if (property.Key == propertyName && property.Value != string.Empty)
                    {
                        double propertyVal = Convert.ToDouble(property.Value);
                        max = (max > propertyVal) ? max : propertyVal;
                    }
                }
            }
            return max;
        }

        private void ShowLegend()
        {
            if (_checkBox.IsChecked == false)
            {
                _vLayout.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                _vLayout.Visibility = System.Windows.Visibility.Visible;
            }

            _vLayout.Children.Clear();
            foreach (var element in _dictColor)
            {
                Rectangle rect = new Rectangle();
                rect.Width = 12;
                rect.Height = 12;
                rect.Fill = new SolidColorBrush(element.Value);

                TextBlock textB = new TextBlock();
                textB.Text = element.Key;

                StackPanel hLayout = new StackPanel();
                hLayout.Orientation = Orientation.Horizontal;
                hLayout.Margin = new Thickness(0.5, 0.5, 0.5, 0);
                hLayout.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                hLayout.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                hLayout.Children.Add(rect);
                hLayout.Children.Add(textB);
                _vLayout.Children.Add(hLayout);
            }
        }

        private void checkboxCallback(object sender, EventArgs e)
        {
            if (_checkBox.IsChecked == true)
            {
                _vLayout.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                _vLayout.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void comboboxCallback(object sender, EventArgs e)
        {
            if (!MapControl.Current.Layers.Any(x => x.LayerData.Name == "地块"))
            {
                return;
            }

            MapLayer parcelLayer = MapControl.Current.Layers.First(x => x.LayerData.Name == "地块");
            string currentItem = _comboBox.SelectedItem as string;
            if (currentItem == "无")
            {
                _vLayout.Children.Clear();

                _checkBox.Visibility = System.Windows.Visibility.Collapsed;
                ParcelTheme = null;
            }
            else if (currentItem == "用地性质")
            {
                _checkBox.Visibility = System.Windows.Visibility.Visible;

                ParcelUsageTheme usageTheme = new ParcelUsageTheme();
                ParcelTheme = usageTheme;
                _dictColor = usageTheme.DictColor;
                ShowLegend();
            }
            else if (currentItem == "容积率" || currentItem == "建筑密度" || currentItem == "绿地率" || currentItem == "建筑限高")
            {
                _checkBox.Visibility = System.Windows.Visibility.Visible;

                var gradientTheme = new BiColorGradientTheme();
                gradientTheme.Property = currentItem;
                gradientTheme.MinValue = GetPropMinValue(parcelLayer, currentItem);
                gradientTheme.MaxValue = GetPropMaxValue(parcelLayer, currentItem);
                ParcelTheme = gradientTheme;
                _dictColor = GetGradientDictColor(gradientTheme);
                ShowLegend();
            }
            if (ParcelTheme != null) // mod 20120725
            {
                parcelLayer.ApplyColorTheme(ParcelTheme);
                if (ParcelTheme is IDataVisualizationTheme)
                {
                    string prop = (ParcelTheme as IDataVisualizationTheme).Property;
                    parcelLayer.ApplyToolTip(f => UIHelper.TitledToolTip(prop, f[prop]));
                }
                else if (ParcelTheme is ParcelUsageTheme)
                {
                    parcelLayer.ApplyToolTip(f => UIHelper.TitledToolTip(f["用地代码"], ParcelColorCfg.GetUsageByCode(f["用地代码"])));
                }
            }
            else
            {
                parcelLayer.ClearColorTheme();
                parcelLayer.ApplyToolTip(f => null);
            }
        }

        private Dictionary<string, Color> GetGradientDictColor(BiColorGradientTheme gradientTheme)
        {
            Dictionary<string, Color> result = new Dictionary<string, Color>();
            double minValue = gradientTheme.MinValue;
            double maxValue = gradientTheme.MaxValue;

            double interval = (maxValue - minValue) / 5;
            double leftVal = 0;
            double rightVal = 0;
            if (interval == 0)
            {
                interval = 1;
            }
            for (int index = 0; index <= 4; index++)
            {
                leftVal = minValue + interval * index;
                rightVal = minValue + interval * (index + 1);
                string range = string.Format(" {0}~{1}", leftVal.ToString(), rightVal.ToString());

                Color color = new Color();
                color = gradientTheme.GetColorByValue(minValue + index * interval);

                result.Add(range, color);
            }
            return result;
        }
    }

    /// <summary>
    /// 属性面板
    /// </summary>
    public class PropertyPanelControl : UserControl
    {
        private Grid _propertyGrid = new Grid { VerticalAlignment = VerticalAlignment.Top };
        private ScrollViewer _scroll = new ScrollViewer { Height = 200, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
        private StackPanel _layoutRoot = new StackPanel();
        private DockPanel _firstRow = new DockPanel();
        private ComboBox _cbbAll = new ComboBox { BorderBrush = new SolidColorBrush(Colors.LightGray) };
        private Button _btnShowAll = new Button { Width = 50, Content = "详细", BorderBrush = new SolidColorBrush(Colors.LightGray) };

        private List<IFeature> _features = new List<IFeature>();

        public string CurrentLayer
        {
            get
            {
                if (_cbbAll.SelectedItem != null)
                {
                    return _cbbAll.SelectedItem.ToString().Split(' ')[0];
                }
                else
                {
                    return "<全部>";
                }
            }
        }

        public PropertyPanelControl()
        {
            DockPanel.SetDock(_btnShowAll, Dock.Right);
            _btnShowAll.Click += buttonCallback;
            _firstRow.Children.Add(_btnShowAll);
            _cbbAll.SelectionChanged += new SelectionChangedEventHandler(_cbbAll_SelectionChanged);
            _firstRow.Children.Add(_cbbAll);
            _layoutRoot.Children.Add(_firstRow);

            _propertyGrid.ColumnDefinitions.Add(new ColumnDefinition());
            _propertyGrid.ColumnDefinitions.Add(new ColumnDefinition());
            _scroll.Content = _propertyGrid;
            _layoutRoot.Children.Add(_scroll);

            this.Content = _layoutRoot;
            this.Margin = new Thickness(5);
        }

        void _cbbAll_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_cbbAll.SelectedItem != null)
            {
                ReadyGrid();
            }
        }

        public void Update(IEnumerable<IFeature> features)
        {
            _features = features.ToList();
            _cbbAll.Items.Clear();
            if (_features.Count > 0)
            {
                var layers = _features.GroupBy(x => SelectionSet.FindLayer(x)).Select(x => string.Format("{0} ({1})", x.Key.Name, x.Count())).ToList();
                layers.Add(string.Format("<全部> ({0})", _features.Count));
                layers.ForEach(x => _cbbAll.Items.Add(x));
                _cbbAll.SelectedIndex = 0;
            }
            else
            {
                _propertyGrid.Children.Clear();
            }
        }

        public void Update()
        {
            Update(SelectionSet.Contents);
        }

        private void ReadyGrid()
        {
            if (CurrentLayer != "<全部>")
            {
                var featuresInLayer = _features.Where(x => SelectionSet.FindLayer(x).Name == CurrentLayer).ToList();
                var propFeature = GetPropPresentation(featuresInLayer);
                this.ShowProperties(propFeature);
            }
            else
            {
                _propertyGrid.Children.Clear();
            }
        }

        private static IFeature GetPropPresentation(List<IFeature> features)
        {
            Feature f = new Feature();
            if (features.Count > 0)
            {
                List<string> props = features[0].Properties.Keys.ToList();
                foreach (string prop in props)
                {
                    if (features.Select(x => x[prop]).Distinct().Count() <= 1)
                    {
                        f.Properties.Add(prop, features[0][prop]);
                    }
                    else
                    {
                        f.Properties.Add(prop, string.Empty);
                    }
                }
            }
            return f;
        }

        private void ShowProperties(IFeature f)
        {
            int row = 0;
            _propertyGrid.Children.Clear();
            _propertyGrid.RowDefinitions.Clear();
            foreach (var property in f.Properties)
            {
                var prop = property;
                _propertyGrid.RowDefinitions.Add(new RowDefinition());

                var textName = new TextBlock { Text = property.Key, VerticalAlignment = System.Windows.VerticalAlignment.Center, Margin = new Thickness(5, 0, 0, 0), Foreground = new SolidColorBrush(Color.FromArgb(255, 96, 96, 96)) };
                var textVal = new TextBox { Text = property.Value, Height = 20, BorderBrush = new SolidColorBrush(Colors.LightGray) };
                textVal.TextChanged += (s, args) => _features.Where(x => SelectionSet.FindLayer(x).Name == CurrentLayer).ForEach(x => x[prop.Key] = textVal.Text); // newly 20120514

                ToolTip tipName = new ToolTip();
                tipName.Content = property.Key;
                ToolTipService.SetToolTip(textName, tipName);

                ToolTip tipVal = new ToolTip();
                if (property.Value != string.Empty)
                {
                    tipVal.Content = property.Value;
                    ToolTipService.SetToolTip(textVal, tipVal);
                }

                Border border = new Border();
                border.Child = textName;
                border.BorderBrush = new SolidColorBrush(Colors.Black);
                border.BorderThickness = new Thickness(0.25, 0.25, 0.25, 0.25);
                border.Background = row % 2 == 0 ? new SolidColorBrush(Colors.White) : new SolidColorBrush(new Color { A = 255, R = 240, G = 240, B = 240 });

                _propertyGrid.Children.Add(border);
                _propertyGrid.Children.Add(textVal);

                Grid.SetRow(border, row);
                Grid.SetColumn(border, 0);
                Grid.SetRow(textVal, row);
                Grid.SetColumn(textVal, 1);

                row++;
            }
        }

        private void buttonCallback(object sender, EventArgs e)
        {
            if (CurrentLayer != "<全部>")
            {
                QueryResultWindow qrw = new QueryResultWindow();
                qrw.SetData(_features.Where(x => SelectionSet.FindLayer(x).Name == CurrentLayer).ToList());
                qrw.ShowDialog();
            }
        }
    }

    /// <summary>
    /// 查找面板
    /// </summary>
    public class SearchPanelControl : UserControl
    {
        private StackPanel _layoutRootNew = new StackPanel();
        private DockPanel _layoutRoot = new DockPanel();
        private TextBox _text = new TextBox();
        private ToolTip _tip = new ToolTip();
        private Button _button = new Button { Width = 50, BorderBrush = new SolidColorBrush(Colors.LightGray) };
        private ListBox _list = new ListBox { Height = 200, BorderBrush = new SolidColorBrush(Colors.LightGray) };
        private List<IFeature> _findResults = new List<IFeature>();

        private ComboBox _cbbLayers = new ComboBox { Width = 80, BorderBrush = new SolidColorBrush(Colors.LightGray) };
        private ComboBox _cbbProps = new ComboBox { Width = 80, BorderBrush = new SolidColorBrush(Colors.LightGray) };
        private ComboBox _cbbOperations = new ComboBox { Width = 82, BorderBrush = new SolidColorBrush(Colors.LightGray) };
        private TextBox _txtParam = new TextBox { Width = 140, BorderBrush = new SolidColorBrush(Colors.LightGray) };
        private Button _btnDoQuery = new Button { Content = "查询", Width = 52, BorderBrush = new SolidColorBrush(Colors.LightGray) };
        private Button _btnResetQuery = new Button { Content = "复位", Width = 50, BorderBrush = new SolidColorBrush(Colors.LightGray) };
        private WrapPanel _queryControls = new WrapPanel();

        public SearchPanelControl()
        {
            this.Content = _layoutRootNew;
            this.Margin = new Thickness(5);

            _button.Content = "查找";
            _button.Click += new RoutedEventHandler(_button_Click);
            _text.KeyUp += new KeyEventHandler(_text_KeyUp);
            _list.SelectionChanged += new SelectionChangedEventHandler(_list_SelectionChanged);

            _tip.Content = "输入关键字以查找";
            ToolTipService.SetToolTip(_text, _tip);

            DockPanel.SetDock(_button, Dock.Right);
            _layoutRoot.Children.Add(_button);
            _layoutRoot.Children.Add(_text);
            _layoutRootNew.Children.Add(_layoutRoot);

            _cbbLayers.DropDownClosed += (s, args) => ReadyProps();
            _queryControls.Children.Add(_cbbLayers);
            _queryControls.Children.Add(_cbbProps);
            _cbbOperations.Items.Add("= 等于");
            _cbbOperations.Items.Add("≠ 不等于");
            _cbbOperations.Items.Add("> 大于");
            _cbbOperations.Items.Add("< 小于");
            _queryControls.Children.Add(_cbbOperations);
            _queryControls.Children.Add(_txtParam);
            _btnDoQuery.Click += new RoutedEventHandler(_btnDoQuery_Click);
            _btnResetQuery.Click += new RoutedEventHandler(_btnResetQuery_Click);
            _queryControls.Children.Add(_btnDoQuery);
            _queryControls.Children.Add(_btnResetQuery);
            _layoutRootNew.Children.Add(_queryControls);

            _layoutRootNew.Children.Add(_list);
        }

        void _btnResetQuery_Click(object sender, RoutedEventArgs e)
        {
            ResetQuery();
        }

        void _btnDoQuery_Click(object sender, RoutedEventArgs e)
        {
            DoQuery();
        }

        private void DoQuery()
        {
            if (_cbbLayers.SelectedItem == null || _cbbProps.SelectedItem == null || _cbbOperations.SelectedItem == null)
            {
                return;
            }

            string layerName = _cbbLayers.SelectedItem.ToString();
            string prop = _cbbProps.SelectedItem.ToString();
            DataQueryOperation operation = (DataQueryOperation)_cbbOperations.SelectedIndex;
            string param = _txtParam.Text;

            var theme = new PredicateTheme(f => MapQueryServices.FeatureSelector(f, prop, operation, param), Colors.Yellow, Colors.Gray);
            MapControl.Current.Layers.First(x => x.LayerData.Name == layerName).ApplyColorTheme(theme);
            _findResults = MapDataManager.LatestMap.Layers[layerName].QueryFeatures(prop, operation, param).ToList();
            _list.Items.Clear();
            foreach (var find in _findResults)
            {
                _list.Items.Add(new TextBlock { Text = string.Format("Feature {0}", find.FeatId) });
            }
        }

        public void ResetQuery()
        {
            MapControl.Current.Layers.ForEach(x => x.ClearColorTheme());

            _cbbLayers.Items.Clear();
            MapControl.Current.AllMaps.SelectMany(x => x.Layers).Select(x => x.Name).Distinct().ForEach(x => _cbbLayers.Items.Add(x));
            _cbbProps.Items.Clear();
            _cbbOperations.SelectedIndex = -1;
            _txtParam.Text = string.Empty;

            _list.Items.Clear();
        }

        private void ReadyProps()
        {
            _cbbProps.Items.Clear();
            if (_cbbLayers.SelectedItem != null)
            {
                string layer = _cbbLayers.SelectedItem.ToString();
                if (MapControl.Current.AllMaps.SelectMany(x => x.Layers).Any(x => x.Name == layer))
                {
                    var features = MapControl.Current.AllMaps.SelectMany(x => x.Layers).First(x => x.Name == layer).Features;
                    if (features.Count > 0)
                    {
                        features[0].Properties.ForEach(x => _cbbProps.Items.Add(x.Key));
                    }
                }
            }
        }

        void _list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_list.SelectedIndex > -1 && _list.SelectedIndex < _findResults.Count)
            {
                IFeature feature = _findResults[_list.SelectedIndex];
                MapControl.Current.Zoom(new Geometry.PointString(feature.GeoData).GetExtents());
                SelectionSet.Select(feature);
            }
            else
            {
                SelectionSet.ClearSelection();
            }
        }

        void _button_Click(object sender, RoutedEventArgs e)
        {
            DoSearch();
        }

        void _text_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DoSearch();
            }
        }

        void DoSearch()
        {
            string searchText = _text.Text;
            if (searchText == string.Empty)
            {
                _list.Items.Clear();
            }
            else
            {
                //MapDataManager.FindAndMarkFeatures(searchText);
                _findResults = MapDataManager.LatestMap.FindFeatures(searchText).ToList();
                _list.Items.Clear();
                foreach (var find in _findResults)
                {
                    _list.Items.Add(new TextBlock { Text = string.Format("Feature {0}", find.FeatId) });
                }
            }
        }
    }

    /// <summary>
    /// 界面助手
    /// </summary>
    public static class UIHelper
    {
        public static ToolTip TitledToolTip(string title, string content)
        {
            StackPanel sp = new StackPanel();
            sp.Children.Add(new TextBlock { Text = title, Margin = new Thickness(10), FontWeight = FontWeights.Bold });
            sp.Children.Add(new TextBlock { Text = content, Margin = new Thickness(10, 0, 10, 10) });
            return new ToolTip { Content = sp };
        }
    }
}
