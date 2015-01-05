using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;
using System.Windows.Shapes;

namespace TongJi.Gis.Display
{
    /// <summary>
    /// 选择集
    /// </summary>
    public static class SelectionSet
    {
        private static HashSet<IFeature> _contents = new HashSet<IFeature>();
        /// <summary>
        /// 选择集的内容
        /// </summary>
        public static HashSet<IFeature> Contents { get { return _contents; } }

        /// <summary>
        /// 选择集改变事件
        /// </summary>
        public static event Action SelectionChanged;
        /// <summary>
        /// 引发选择集改变事件
        /// </summary>
        public static void OnSelectionChanged()
        {
            if (SelectionChanged != null)
            {
                SelectionChanged();
            }
        }

        private static Brush _markBrush = Brushes.Red;
        /// <summary>
        /// 获取或设置选择集标记的笔刷
        /// </summary>
        public static Brush MarkBrush
        {
            get
            {
                return _markBrush;
            }
            set
            {
                _markBrush = value;
                MarkSelection();
            }
        }

        /// <summary>
        /// 设置选择集标记笔刷颜色RGB
        /// </summary>
        /// <param name="r">R分量</param>
        /// <param name="g">G分量</param>
        /// <param name="b">B分量</param>
        public static void markcolor(byte r, byte g, byte b)
        {
            // ss.markcolor(255,255,0)

            MarkBrush = new SolidColorBrush(Color.FromArgb(255, r, g, b));
        }

        /// <summary>
        /// 设置选择集内容
        /// </summary>
        /// <param name="entities">实体数组</param>
        public static void Select(IFeature[] entities)
        {
            UnmarkSelection();
            _contents.Clear();
            entities.ToList().ForEach(x => _contents.Add(x));
            MarkSelection();
            OnSelectionChanged();
        }

        /// <summary>
        /// 设置选择集内容
        /// </summary>
        /// <param name="entity">实体</param>
        public static void Select(IFeature entity)
        {
            UnmarkSelection();
            _contents.Clear();
            _contents.Add(entity);
            MarkSelection();
            OnSelectionChanged();
        }

        /// <summary>
        /// 添加到选择集
        /// </summary>
        /// <param name="entities">实体数组</param>
        public static void AddSelection(IFeature[] entities)
        {
            UnmarkSelection();
            entities.ToList().ForEach(x =>
            {
                if (!_contents.Contains(x))
                {
                    _contents.Add(x);
                }
            });
            MarkSelection();
            OnSelectionChanged();
        }

        /// <summary>
        /// 添加到选择集
        /// </summary>
        /// <param name="entity">实体</param>
        public static void AddSelection(IFeature entity)
        {
            UnmarkSelection();
            if (!_contents.Contains(entity))
            {
                _contents.Add(entity);
            }
            MarkSelection();
            OnSelectionChanged();
        }

        /// <summary>
        /// 从选择集减去
        /// </summary>
        /// <param name="entities">实体数组</param>
        public static void SubtractSelection(IFeature[] entities)
        {
            UnmarkSelection();
            entities.ToList().ForEach(x =>
            {
                if (_contents.Contains(x))
                {
                    _contents.Remove(x);
                }
            });
            MarkSelection();
            OnSelectionChanged();
        }

        /// <summary>
        /// 清空选择集
        /// </summary>
        public static void ClearSelection()
        {
            UnmarkSelection();
            _contents.Clear();
            OnSelectionChanged();
        }

        private static void UnmarkSelection() // mod 20130226
        {
            foreach (var layer in MapControl.Current.Layers)
            {
                if (layer is DrawingMapLayer)
                {
                    foreach (var content in (layer as DrawingMapLayer).Features)
                    {
                        var drawing = content.Value;
                        drawing.Pen.Brush = layer.LayerStyle.Stroke;
                    }
                }
                else
                {
                    foreach (var f in layer.Features)
                    {
                        var shape = f.Value;
                        shape.Stroke = layer.LayerStyle.Stroke;
                    }
                }
            }
        }

        private static void MarkSelection() // mod 20130226
        {
            foreach (var layer in MapControl.Current.Layers)
            {
                if (layer is DrawingMapLayer)
                {
                    foreach (var content in (layer as DrawingMapLayer).Features)
                    {
                        var f = content.Key;
                        var drawing = content.Value;
                        if (_contents.Contains(f))
                        {
                            if (FindLayer(f).GeoType != "2")
                            {
                                (layer as DrawingMapLayer).BringToFront(drawing);
                            }
                            drawing.Pen.Brush = MarkBrush;
                        }
                    }
                }
                else
                {
                    foreach (var content in layer.Features)
                    {
                        var f = content.Key;
                        var shape = content.Value;
                        System.Windows.Controls.Canvas.SetZIndex(shape, 0);
                        if (_contents.Contains(f))
                        {
                            if (FindLayer(f).GeoType != "2")
                            {
                                System.Windows.Controls.Canvas.SetZIndex(shape, 10);
                            }
                            shape.Stroke = MarkBrush;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 查找实体对应的显示图形
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>图形，未找到为null</returns>
        public static Shape FindShape(IFeature entity)
        {
            foreach (var layer in MapControl.Current.Layers)
            {
                if (layer.Features.ContainsKey(entity))
                {
                    return layer.Features[entity];
                }
            }
            return null;
        }

        /// <summary>
        /// 查找实体所在的图层
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>图层，未找到为null</returns>
        public static VectorLayer FindLayer(IFeature entity) // mod 20130403
        {
            foreach (var layer in MapControl.Current.Layers)
            {
                if (layer.LayerData.Features.Contains(entity))
                {
                    return layer.LayerData;
                }
            }
            return null;
        }
    }

}
