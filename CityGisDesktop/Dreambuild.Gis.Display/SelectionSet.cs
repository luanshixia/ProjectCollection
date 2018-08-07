using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows.Media;
using System.Windows.Shapes;

namespace Dreambuild.Gis.Display
{
    /// <summary>
    /// 选择集
    /// </summary>
    public static class SelectionSet // todo: show selection bounding box.
    {
        /// <summary>
        /// 选择集的内容
        /// </summary>
        public static HashSet<IFeature> Contents { get; } = new HashSet<IFeature>();
        /// <summary>
        /// 选择集改变事件
        /// </summary>
        public static event Action SelectionChanged;
        /// <summary>
        /// 引发选择集改变事件
        /// </summary>
        public static void OnSelectionChanged()
        {
            SelectionChanged?.Invoke();
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
            Contents.Clear();
            entities.ToList().ForEach(x => Contents.Add(x));
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
            Contents.Clear();
            Contents.Add(entity);
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
                if (!Contents.Contains(x))
                {
                    Contents.Add(x);
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
            if (!Contents.Contains(entity))
            {
                Contents.Add(entity);
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
                if (Contents.Contains(x))
                {
                    Contents.Remove(x);
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
            Contents.Clear();
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
                        var feature = content.Key;
                        var drawing = content.Value;
                        if (Contents.Contains(feature))
                        {
                            if (FindLayer(feature).GeoType != VectorLayer.GEOTYPE_LINEAR)
                            {
                                (layer as DrawingMapLayer).BringToFront(drawing);
                            }
                            drawing.Pen.Brush = MarkBrush;
                            //_tempDrawings.Add(drawing);
                        }
                    }
                }
                else
                {
                    foreach (var content in layer.Features)
                    {
                        var feature = content.Key;
                        var shape = content.Value;
                        //System.Windows.Controls.Canvas.SetZIndex(shape, 0);
                        if (Contents.Contains(feature))
                        {
                            if (FindLayer(feature).GeoType != VectorLayer.GEOTYPE_LINEAR)
                            {
                                layer.BringToFront(shape);
                            }
                            shape.Stroke = MarkBrush;
                            //_tempShapes.Add(shape);
                        }
                    }
                }
            }
            //UpdateSelectionMark();
        }

        //private static List<Shape> _tempShapes = new List<Shape>();
        //private static List<GeometryDrawing> _tempDrawings = new List<GeometryDrawing>();

        //private static void UpdateSelectionMark()
        //{
        //    _tempShapes.ForEach(s => s.StrokeThickness = 1.0 * MapControl.Current.Scale);
        //    _tempDrawings.ForEach(d => d.Pen.Thickness = 1.0 * MapControl.Current.Scale);
        //}

        /// <summary>
        /// 查找实体对应的显示图形
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>图形，未找到为null</returns>
        public static Shape FindShape(IFeature entity)
        {
            foreach (var mapLayer in MapControl.Current.Layers)
            {
                if (mapLayer.Features.ContainsKey(entity))
                {
                    return mapLayer.Features[entity];
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
            foreach (var mapLayer in MapControl.Current.Layers)
            {
                if (mapLayer.LayerData.Features.Contains(entity))
                {
                    return mapLayer.LayerData;
                }
            }
            return null;
        }
    }

}
