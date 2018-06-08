using Dreambuild.Extensions;
using Dreambuild.Gis.Display;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Dreambuild.Gis.Desktop.Demos.Changdu
{
    /// <summary>
    /// DemoPanel.xaml 的交互逻辑
    /// </summary>
    public partial class DemoPanel : UserControl
    {
        private Color[] _colors = { Colors.YellowGreen, Colors.Orange, Colors.Salmon, Colors.SkyBlue, Colors.Orchid, Colors.PaleGoldenrod, Colors.PaleGreen, Colors.PaleTurquoise, Colors.PaleVioletRed, Colors.PeachPuff, Colors.Peru, Colors.Pink, Colors.DodgerBlue, Colors.SeaGreen, Colors.OrangeRed, Colors.SlateBlue, Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Magenta, Colors.Cyan, Colors.Purple };
        private List<FrameworkElement> _marks = new List<FrameworkElement>();

        public DemoPanel()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            DataManager.Topics.ForEach(x => SelectTopic.Items.Add(x));
            SelectTopic.SelectedIndex = 0;
        }

        private void SelectTopic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectTopic.SelectedItem != null)
            {
                string topic = SelectTopic.SelectedItem.ToString();
                SelectItem.Items.Clear();
                string[] items = DataManager.GetTopicItems(topic);
                items.ForEach(x => SelectItem.Items.Add(x));
                //SelectItem.SelectedIndex = 0;

                LegendPanel.Children.Clear();
                var itemData = items.ToDictionary(x => x, x => DataManager.GetTopicItemData(topic, x));
                MultiConditionTheme mct = new MultiConditionTheme();
                mct.AddCondition(f => itemData.Count(x => x.Value.Contains(f["名称"])) > 1, Colors.LightGray);
                AddLegendItem("(多种)", Colors.LightGray);
                int i = 0;
                itemData.ForEach(x =>
                {
                    mct.AddCondition(f => x.Value.Contains(f["名称"]), _colors[i]);
                    AddLegendItem(x.Key, _colors[i]);
                    i++;
                });
                mct.AddCondition(f => true, Colors.Gray);
                MapControl.Current.FindMapLayers("区域").ForEach(x =>
                {
                    x.ApplyColorTheme(mct);
                    x.ApplyToolTip(f => UIHelper.TitledToolTip(f["名称"], string.Join("\n", itemData.Where(y => y.Value.Contains(f["名称"])).Select(y => y.Key).ToArray())));
                });
                AddMultiItemsMark(itemData);
            }
        }

        private void SelectItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectItem.SelectedItem != null)
            {
                string topic = SelectTopic.SelectedItem.ToString();
                string item = SelectItem.SelectedItem.ToString();
                var data = DataManager.GetTopicItemData(topic, item);
                PredicateTheme pt = new PredicateTheme(f => data.Contains(f["名称"]), Colors.Yellow, Colors.Gray);
                MapControl.Current.FindMapLayers("区域").ForEach(x => x.ApplyColorTheme(pt));
            }
        }

        private void AddLegendItem(string name, Color color)
        {
            StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(3), Height = 20 };
            sp.Children.Add(new Rectangle { Width = 20, Fill = new SolidColorBrush(color) });
            sp.Children.Add(new TextBlock { Text = name, Margin = new Thickness(3, 0, 0, 0), VerticalAlignment = System.Windows.VerticalAlignment.Center });
            LegendPanel.Children.Add(sp);
        }

        private void AddMultiItemsMark(Dictionary<string, string[]> itemData)
        {
            double size = 8000;
            MapControl.Current.FindMapLayers("区域").ForEach(x =>
            {
                _marks.ForEach(y => x.Children.Remove(y));
                _marks.Clear();

                foreach (var feature in x.LayerData.Features)
                {
                    string name = feature["名称"];
                    var items = itemData.Where(y => y.Value.Contains(name)).ToList();
                    if (items.Count > 1)
                    {
                        StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal, Height = size };
                        int i = 0;
                        foreach (var item in itemData)
                        {
                            if (item.Value.Contains(name))
                            {
                                sp.Children.Add(new Rectangle { Width = size, Fill = new SolidColorBrush(_colors[i]) });
                            }
                            i++;
                        }
                        var extents = Geometry.Extents.FromPoints(new Geometry.PointString(feature.GeoData).Points);
                        var center = extents.Center();
                        Canvas.SetLeft(sp, center.X - items.Count * size / 2);
                        Canvas.SetTop(sp, center.Y - size / 2);
                        _marks.Add(sp);
                        x.Children.Add(sp);
                    }
                }
            });
        }
    }
}
