using Dreambuild.Extensions;
using Dreambuild.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Dreambuild.Gis.Display
{
    /// <summary>
    /// Map layer panel.
    /// </summary>
    public class MapLayerPanelControl : UserControl, ILanguageSwitcher
    {
        private readonly TreeView _mapLayerList = new TreeView
        {
            Height = 200,
            BorderBrush = new SolidColorBrush(Colors.LightGray)
        };

        public MapLayerPanelControl()
        {
            var layoutRoot = new StackPanel();
            layoutRoot.Children.Add(_mapLayerList);
            this.Content = layoutRoot;
            this.Margin = new Thickness(5);
        }

        public void Update()
        {
            _mapLayerList.Items.Clear();
            foreach (var map in MapControl.Current.MapAndLayers)
            {
                var mapNode = new TreeViewItem
                {
                    Header = GetMapNodeHeader(map.Item1)
                };

                foreach (var layer in map.Item2)
                {
                    mapNode.Items.Add(new TreeViewItem
                    {
                        Header = GetLayerNodeHeader(layer)
                    });
                }

                _mapLayerList.Items.Add(mapNode);
            }
        }

        private FrameworkElement GetLayerNodeHeader(MapLayer layer)
        {
            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            var checkBox = new CheckBox
            {
                Content = layer.LayerData.Name,
                IsChecked = layer.Visibility == Visibility.Visible
            };

            checkBox.Checked += (sender, e) => LayerVisible(layer, true);
            checkBox.Unchecked += (sender, e) => LayerVisible(layer, false);

            var slider = new Slider
            {
                Minimum = 0,
                Maximum = 1,
                Width = 40,
                Margin = new Thickness(10, 0, 0, 0)
            };

            slider.SetBinding(
                dp: Slider.ValueProperty,
                binding: new Binding("Opacity")
                {
                    Source = layer,
                    Mode = BindingMode.TwoWay
                });

            stackPanel.Children.Add(checkBox);
            stackPanel.Children.Add(slider);
            return stackPanel;
        }

        private FrameworkElement GetMapNodeHeader(Map map)
        {
            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            var mapAndLayers = MapControl.Current.MapAndLayers;

            var mapTitle = LocalizationHelper.CurrentLocale == Locales.ZH_CN
                ? "地图 {0}"
                : "Map {0}";

            var mapNumber = mapAndLayers
                .Select(mapLayersPair => mapLayersPair.Item1)
                .ToList().IndexOf(map) + 1;

            var checkBox = new CheckBox
            {
                IsChecked = true,
                Content = string.IsNullOrEmpty(map.Name)
                    ? string.Format("地图 {0}", mapNumber)
                    : map.Name
            };

            var mapLayers = mapAndLayers.First(mapLayersPair => mapLayersPair.Item1 == map).Item2;
            checkBox.Checked += (sender, e) => mapLayers.ForEach(mapLayer => LayerVisible(mapLayer, true));
            checkBox.Unchecked += (sender, e) => mapLayers.ForEach(mapLayer => LayerVisible(mapLayer, false));
            stackPanel.Children.Add(checkBox);
            return stackPanel;
        }

        private static void LayerVisible(MapLayer layer, bool visible)
        {
            var visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            layer.Visibility = visibility;
            layer.LabelLayer.Visibility = visibility;
        }

        void ILanguageSwitcher.RefreshLanguage()
        {
            this.Update();
        }
    }

    /// <summary>
    /// Feature panel.
    /// </summary>
    public class FeaturePanelControl : UserControl
    {
        public const string None = "None";
        public const string LandUse = "Land use";
        public const string FloorAreaRatio = "Floor-area ratio"; // aka Plot ratio
        public const string SiteCoverageRatio = "Site coverage ratio";
        public const string GreenRate = "Green rate";
        public const string BuildingHeightLimit = "Building height limit";

        private readonly ComboBox _comboBox = new ComboBox
        {
            BorderBrush = new SolidColorBrush(Colors.LightGray),
            ItemsSource = new[] { None, LandUse, FloorAreaRatio, SiteCoverageRatio, GreenRate, BuildingHeightLimit },
            SelectedIndex = 0
        };

        private readonly CheckBox _checkBox = new CheckBox
        {
            Content = "Legend",
            IsChecked = true,
            Visibility = Visibility.Collapsed
        };

        private readonly StackPanel _vLayout = new StackPanel();

        private Dictionary<string, Color> _dictColor = new Dictionary<string, Color>();

        public FeaturePanelControl()
        {
            var layoutRoot = new StackPanel();
            this.Content = layoutRoot;
            this.Margin = new Thickness(5);

            _comboBox.SelectionChanged += ComboBoxCallback;
            layoutRoot.Children.Add(_comboBox);

            _checkBox.Click += CheckBoxCallback;
            layoutRoot.Children.Add(_checkBox);

            layoutRoot.Children.Add(_vLayout);
        }

        private static double GetPropMinValue(MapLayer layer, string propertyName)
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

        private static double GetPropMaxValue(MapLayer layer, string propertyName)
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
                _vLayout.Visibility = Visibility.Collapsed;
            }
            else
            {
                _vLayout.Visibility = Visibility.Visible;
            }

            _vLayout.Children.Clear();
            foreach (var element in _dictColor)
            {
                var rect = new Rectangle
                {
                    Width = 12,
                    Height = 12,
                    Fill = new SolidColorBrush(element.Value)
                };

                var textBlock = new TextBlock
                {
                    Text = element.Key
                };

                var hLayout = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0.5, 0.5, 0.5, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                };

                hLayout.Children.Add(rect);
                hLayout.Children.Add(textBlock);
                _vLayout.Children.Add(hLayout);
            }
        }

        private void CheckBoxCallback(object sender, EventArgs e)
        {
            if (_checkBox.IsChecked == true)
            {
                _vLayout.Visibility = Visibility.Visible;
            }
            else
            {
                _vLayout.Visibility = Visibility.Collapsed;
            }
        }

        private void ComboBoxCallback(object sender, EventArgs e)
        {
            if (!MapControl.Current.Layers.Any(mapLayer => mapLayer.LayerData.Name == WellknownLayerNames.Parcel))
            {
                return;
            }

            var parcelLayer = MapControl.Current.Layers.First(mapLayer => mapLayer.LayerData.Name == WellknownLayerNames.Parcel);
            string currentItem = _comboBox.SelectedItem as string;
            IColorTheme parcelTheme = null;
            if (currentItem == None)
            {
                _vLayout.Children.Clear();

                _checkBox.Visibility = Visibility.Collapsed;
                parcelTheme = null;
            }
            else if (currentItem == LandUse)
            {
                _checkBox.Visibility = Visibility.Visible;

                var usageTheme = new ParcelUsageTheme();
                parcelTheme = usageTheme;
                _dictColor = usageTheme.DictColor;
                ShowLegend();
            }
            else if (currentItem == FloorAreaRatio || currentItem == SiteCoverageRatio
                || currentItem == GreenRate || currentItem == BuildingHeightLimit)
            {
                _checkBox.Visibility = Visibility.Visible;

                var gradientTheme = new BiColorGradientTheme
                {
                    Property = currentItem,
                    MinValue = GetPropMinValue(parcelLayer, currentItem),
                    MaxValue = GetPropMaxValue(parcelLayer, currentItem)
                };

                parcelTheme = gradientTheme;
                _dictColor = GetGradientDictColor(gradientTheme);
                ShowLegend();
            }

            if (parcelTheme != null) // mod 20120725
            {
                parcelLayer.ApplyColorTheme(parcelTheme);
                if (parcelTheme is IDataVisualizationTheme)
                {
                    string prop = (parcelTheme as IDataVisualizationTheme).Property;
                    parcelLayer.ApplyToolTip(feature => UIHelper.TitledToolTip(title: prop, content: feature[prop]));
                }
                else if (parcelTheme is ParcelUsageTheme)
                {
                    parcelLayer.ApplyToolTip(feature => UIHelper.TitledToolTip(
                        title: feature[WellknownPropertyNames.LandUseCode],
                        content: ParcelColorCfg.GetUsageByCode(feature[WellknownPropertyNames.LandUseCode])));
                }
            }
            else
            {
                parcelLayer.ClearColorTheme();
                parcelLayer.ApplyToolTip(feature => null);
            }
        }

        private static Dictionary<string, Color> GetGradientDictColor(BiColorGradientTheme gradientTheme)
        {
            var result = new Dictionary<string, Color>();
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

                result.Add(range, gradientTheme.GetColorByValue(minValue + index * interval));
            }

            return result;
        }
    }

    /// <summary>
    /// Property panel.
    /// </summary>
    public class PropertyPanelControl : UserControl
    {
        private readonly Grid _propertyGrid = new Grid
        {
            VerticalAlignment = VerticalAlignment.Top
        };

        private readonly ComboBox _layerComboBox = new ComboBox
        {
            BorderBrush = new SolidColorBrush(Colors.LightGray)
        };

        private List<IFeature> _features = new List<IFeature>();

        public string CurrentLayer
        {
            get
            {
                if (_layerComboBox.SelectedItem != null)
                {
                    return _layerComboBox.SelectedItem.ToString().Split(' ')[0];
                }
                else
                {
                    return "<all>";
                }
            }
        }

        public PropertyPanelControl()
        {
            var layoutRoot = new StackPanel();
            var firstRow = new DockPanel();
            var showAllButton = new Button
            {
                Width = 50,
                Content = "Details",
                BorderBrush = new SolidColorBrush(Colors.LightGray)
            };

            DockPanel.SetDock(showAllButton, Dock.Right);
            showAllButton.Click += ButtonCallback;
            firstRow.Children.Add(showAllButton);
            _layerComboBox.SelectionChanged += LayerComboBox_SelectionChanged;
            firstRow.Children.Add(_layerComboBox);
            layoutRoot.Children.Add(firstRow);

            _propertyGrid.ColumnDefinitions.Add(new ColumnDefinition());
            _propertyGrid.ColumnDefinitions.Add(new ColumnDefinition());

            var scrollViewer = new ScrollViewer
            {
                Height = 200,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            scrollViewer.Content = _propertyGrid;
            layoutRoot.Children.Add(scrollViewer);

            this.Content = layoutRoot;
            this.Margin = new Thickness(5);
        }

        void LayerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_layerComboBox.SelectedItem != null)
            {
                ReadyGrid();
            }
        }

        public void Update(IEnumerable<IFeature> features)
        {
            _features = features.ToList();
            _layerComboBox.Items.Clear();
            if (_features.Count > 0)
            {
                var layerNames = _features
                    .GroupBy(feature => SelectionSet.FindLayer(feature))
                    .Select(group => string.Format("{0} ({1})", group.Key.Name, group.Count()))
                    .ToList();

                layerNames.Add(string.Format("<all> ({0})", _features.Count));
                layerNames.ForEach(layerName => _layerComboBox.Items.Add(layerName));
                _layerComboBox.SelectedIndex = 0;
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
            if (this.CurrentLayer != "<all>")
            {
                var featuresInLayer = _features
                    .Where(feature => SelectionSet.FindLayer(feature).Name == this.CurrentLayer)
                    .ToList();

                var propFeature = GetPropertyRepresentation(featuresInLayer);
                this.ShowProperties(propFeature);
            }
            else
            {
                _propertyGrid.Children.Clear();
            }
        }

        private static IFeature GetPropertyRepresentation(List<IFeature> features)
        {
            var dummyFeature = new Feature();
            if (features.Count > 0)
            {
                var props = features[0].Properties.Keys.ToList();
                foreach (string prop in props)
                {
                    if (features.Select(feature => feature[prop]).Distinct().Count() <= 1)
                    {
                        dummyFeature.Properties.Add(prop, features[0][prop]);
                    }
                    else
                    {
                        dummyFeature.Properties.Add(prop, string.Empty);
                    }
                }
            }
            return dummyFeature;
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

                var textName = new TextBlock
                {
                    Text = property.Key,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 0, 0),
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 96, 96, 96))
                };

                var textVal = new TextBox
                {
                    Text = property.Value,
                    Height = 20,
                    BorderBrush = new SolidColorBrush(Colors.LightGray)
                };

                textVal.TextChanged += (s, args) => _features
                    .Where(feature => SelectionSet.FindLayer(feature).Name == this.CurrentLayer)
                    .ForEach(feature => feature[prop.Key] = textVal.Text); // newly 20120514

                var tipName = new ToolTip
                {
                    Content = property.Key
                };
                ToolTipService.SetToolTip(textName, tipName);

                if (property.Value != string.Empty)
                {
                    var tipVal = new ToolTip
                    {
                        Content = property.Value
                    };
                    ToolTipService.SetToolTip(textVal, tipVal);
                }

                var border = new Border
                {
                    Child = textName,
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(0.25, 0.25, 0.25, 0.25),
                    Background = row % 2 == 0
                        ? new SolidColorBrush(Colors.White)
                        : new SolidColorBrush(new Color { A = 255, R = 240, G = 240, B = 240 })
                };

                _propertyGrid.Children.Add(border);
                _propertyGrid.Children.Add(textVal);

                Grid.SetRow(border, row);
                Grid.SetColumn(border, 0);
                Grid.SetRow(textVal, row);
                Grid.SetColumn(textVal, 1);

                row++;
            }
        }

        private void ButtonCallback(object sender, EventArgs e)
        {
            if (this.CurrentLayer != "<all>")
            {
                var qrw = new QueryResultWindow();
                qrw.SetData(_features
                    .Where(feature => SelectionSet.FindLayer(feature).Name == this.CurrentLayer)
                    .ToList());

                qrw.ShowDialog();
            }
        }
    }

    /// <summary>
    /// Search panel.
    /// </summary>
    public class SearchPanelControl : UserControl
    {
        private readonly TextBox _searchTextBox = new TextBox();
        private readonly ListBox _list = new ListBox
        {
            Height = 200,
            BorderBrush = new SolidColorBrush(Colors.LightGray)
        };

        private List<IFeature> _findResults = new List<IFeature>();

        private readonly ComboBox _cbbLayers = new ComboBox
        {
            Width = 80,
            BorderBrush = new SolidColorBrush(Colors.LightGray)
        };

        private readonly ComboBox _cbbProps = new ComboBox
        {
            Width = 80,
            BorderBrush = new SolidColorBrush(Colors.LightGray)
        };

        private readonly ComboBox _cbbOperations = new ComboBox
        {
            ItemsSource = new[]
            {
                "= equals",
                "≠ not equals",
                "> greater than",
                "< less than"
            },
            Width = 82,
            BorderBrush = new SolidColorBrush(Colors.LightGray)
        };

        private readonly TextBox _txtParam = new TextBox
        {
            Width = 140,
            BorderBrush = new SolidColorBrush(Colors.LightGray)
        };

        public SearchPanelControl()
        {
            var layoutRootNew = new StackPanel();
            var layoutRoot = new DockPanel();
            var toolTip = new ToolTip();

            var findButton = new Button
            {
                Content = "Find",
                Width = 50,
                BorderBrush = new SolidColorBrush(Colors.LightGray)
            };

            var doQueryButton = new Button
            {
                Content = "Query",
                Width = 52,
                BorderBrush = new SolidColorBrush(Colors.LightGray)
            };

            var resetQueryButton = new Button
            {
                Content = "Reset",
                Width = 50,
                BorderBrush = new SolidColorBrush(Colors.LightGray)
            };

            var queryControls = new WrapPanel();

            this.Content = layoutRootNew;
            this.Margin = new Thickness(5);

            findButton.Click += new RoutedEventHandler(_button_Click);
            _searchTextBox.KeyUp += new KeyEventHandler(_text_KeyUp);
            _list.SelectionChanged += new SelectionChangedEventHandler(_list_SelectionChanged);

            toolTip.Content = "Type in a keyword.";
            ToolTipService.SetToolTip(_searchTextBox, toolTip);

            DockPanel.SetDock(findButton, Dock.Right);
            layoutRoot.Children.Add(findButton);
            layoutRoot.Children.Add(_searchTextBox);
            layoutRootNew.Children.Add(layoutRoot);

            _cbbLayers.DropDownClosed += (s, args) => ReadyProps();
            queryControls.Children.Add(_cbbLayers);
            queryControls.Children.Add(_cbbProps);
            queryControls.Children.Add(_cbbOperations);
            queryControls.Children.Add(_txtParam);

            doQueryButton.Click += new RoutedEventHandler(_btnDoQuery_Click);
            resetQueryButton.Click += new RoutedEventHandler(_btnResetQuery_Click);
            queryControls.Children.Add(doQueryButton);
            queryControls.Children.Add(resetQueryButton);
            layoutRootNew.Children.Add(queryControls);

            layoutRootNew.Children.Add(_list);
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
            var operation = (DataQueryOperation)_cbbOperations.SelectedIndex;
            string param = _txtParam.Text;

            var theme = new PredicateTheme(
                feature => MapQueryServices.FeatureSelector(feature, prop, operation, param),
                Colors.Yellow,
                Colors.Gray);

            MapControl.Current.Layers
                .First(mapLayer => mapLayer.LayerData.Name == layerName)
                .ApplyColorTheme(theme);

            _findResults = MapDataManager.LatestMap
                .Layers[layerName]
                .QueryFeatures(prop, operation, param)
                .ToList();

            _list.Items.Clear();
            foreach (var find in _findResults)
            {
                _list.Items.Add(new TextBlock { Text = string.Format("Feature {0}", find.FeatId) });
            }
        }

        public void ResetQuery()
        {
            MapControl.Current.Layers.ForEach(mapLayer => mapLayer.ClearColorTheme());

            _cbbLayers.Items.Clear();
            MapControl.Current.AllMaps
                .SelectMany(map => map.Layers)
                .Select(layer => layer.Name)
                .Distinct()
                .ForEach(layerName => _cbbLayers.Items.Add(layerName));

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
                string layerName = _cbbLayers.SelectedItem.ToString();
                if (MapControl.Current.AllMaps
                    .SelectMany(map => map.Layers)
                    .Any(layer => layer.Name == layerName))
                {
                    var features = MapControl.Current.AllMaps
                        .SelectMany(map => map.Layers)
                        .First(layer => layer.Name == layerName)
                        .Features;

                    if (features.Count > 0)
                    {
                        features[0].Properties.ForEach(pair => _cbbProps.Items.Add(pair.Key));
                    }
                }
            }
        }

        void _list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_list.SelectedIndex > -1 && _list.SelectedIndex < _findResults.Count)
            {
                var feature = _findResults[_list.SelectedIndex];
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
            if (string.IsNullOrEmpty(_searchTextBox.Text))
            {
                _list.Items.Clear();
            }
            else
            {
                //MapDataManager.FindAndMarkFeatures(searchText);
                _findResults = MapDataManager.LatestMap.FindFeatures(_searchTextBox.Text).ToList();
                _list.Items.Clear();
                foreach (var find in _findResults)
                {
                    _list.Items.Add(new TextBlock
                    {
                        Text = string.Format("Feature {0}", find.FeatId)
                    });
                }
            }
        }
    }

    /// <summary>
    /// UI helpers.
    /// </summary>
    public static class UIHelper
    {
        public static ToolTip TitledToolTip(string title, string content)
        {
            var stackPanel = new StackPanel();

            stackPanel.Children.Add(new TextBlock
            {
                Text = title,
                Margin = new Thickness(10),
                FontWeight = FontWeights.Bold
            });

            stackPanel.Children.Add(new TextBlock
            {
                Text = content,
                Margin = new Thickness(10, 0, 10, 10)
            });

            return new ToolTip { Content = stackPanel };
        }
    }
}
