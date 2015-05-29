using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using AutoCADCommands;

using Dreambuild.Gis;
using Dreambuild.Gis.Formats;

namespace TongJi.Drawing
{
    public static class EntityManager
    {
        public static Dictionary<string, EntityDefinition> Entities { get; private set; }

        static EntityManager()
        {
            var path = App.CurrentFolder + "\\Resources\\EntityConfig.csv";
            Entities = System.IO.File.ReadAllLines(path, Encoding.Default)
                .Where(line => !string.IsNullOrEmpty(line))
                .Select(line =>
                {
                    var values = line.Split(',');
                    return new EntityDefinition
                    {
                        EntityID = values[0],
                        EntityCode = values[1],
                        EntityName = values[2],
                        EntityType = values[3],
                        Layer = values[4],
                        ColorIndex = values[5].TryParseToInt32(),
                        Linetype = values[6],
                        ConstantWidth = values[7].TryParseToDouble(),
                        BlockName = values[8],
                        TextStyle = values[9],
                        TextHeight = values[10].TryParseToDouble(2.5),
                        TextXScale = values[11].TryParseToDouble(1),
                        TextItalicAngle = values[12].TryParseToDouble(),
                        AskForHeight = values[13].ToLower() == "true", // fixed 20140708
                        HeightRange = values[14].Replace("(", "").Replace(")", "")
                    };
                }).ToDictionary(x => x.EntityID, x => x);
        }

        public static double GetGlobalScaleFactor()
        {
            var scale = CustomDictionary.GetValue(DictName.CmDrawing, KeyName.GlobalScale);
            var factor = scale.Replace("1:", "").TryParseToDouble() / 1000;
            return factor == 0 ? 1 : factor;
        }

        [Obsolete]
        public static ObjectId AddEntity(string entityID, Func<ObjectId> method)
        {
            var id = method();
            if (!id.IsNull)
            {
                var textStyleId = DbHelper.GetTextStyleId(GetTextStyleName(entityID));
                id.QOpenForWrite<Entity>(ent =>
                {
                    SetEntityStyles(ent, entityID);
                    SetEntityTextStyle(ent, textStyleId);
                });
            }
            return id;
        }

        public static IEnumerable<ObjectId> SetStyles(this IEnumerable<ObjectId> ids, string entityID, bool keepWidth = false)
        {
            if (ids != null)
            {
                var textStyleId = DbHelper.GetTextStyleId(GetTextStyleName(entityID));
                ids.QForEach<Entity>(ent =>
                {
                    try
                    {
                        SetEntityStyles(ent, entityID, keepWidth);
                        SetEntityTextStyle(ent, textStyleId);
                    }
                    catch (Exception e)
                    {
                        Interaction.WriteLine(e.Message);
                    }
                });
            }
            return ids;
        }

        public static ObjectId SetStyles(this ObjectId id, string entityID, bool keepWidth = false)
        {
            if (!id.IsNull) // fixed 20140708
            {
                var textStyleId = DbHelper.GetTextStyleId(GetTextStyleName(entityID));
                id.QOpenForWrite<Entity>(ent =>
                {
                    SetEntityStyles(ent, entityID, keepWidth);
                    SetEntityTextStyle(ent, textStyleId);
                });
            }
            return id;
        }

        public static ObjectId AddPoint()
        {
            var position = Interaction.GetPoint("\n指定点");
            if (position.IsNull())
            {
                return ObjectId.Null;
            }
            return Draw.Point(position);
        }

        public static Func<ObjectId> AddBlock(string entityID, Point3d? position = null, double rotation = 0, bool align = false)
        {
            return () =>
            {
                var def = EntityManager.Entities[entityID];
                var scale = EntityManager.GetGlobalScaleFactor();
                if (position == null)
                {
                    position = Interaction.GetPoint("\n指定插入点");
                    if (position.Value.IsNull())
                    {
                        return ObjectId.Null;
                    }
                }
                if (align)
                {
                    var end = Interaction.GetLineEndPoint("\n指定旋转方向，或直接退出", position.Value);
                    if (!end.IsNull())
                    {
                        rotation = Math.Atan2(end.Y - position.Value.Y, end.X - position.Value.X);
                    }
                }
                return Draw.Insert(def.BlockName, position.Value, rotation, scale);
            };
        }

        public static Func<List<ObjectId>> AddText(string entityID, string text = null, Point3d? pos = null, bool align = false, bool alignToEnd = true)
        {
            return () =>
            {
                var def = EntityManager.Entities[entityID];
                var scale = EntityManager.GetGlobalScaleFactor();
                var height = def.TextHeight;
                if (def.AskForHeight)
                {
                    if (def.HeightRange.Contains("|"))
                    {
                        var heights = def.HeightRange.Split('|').Select(x => Convert.ToDouble(x)).ToArray(); // mod 20140709
                        var input = height;
                        while (true)
                        {
                            input = Interaction.GetValue(string.Format("\n指定字高（{0}）", def.HeightRange), height);
                            if (double.IsNaN(input))
                            {
                                return null;
                            }
                            else if (!heights.Contains(input))
                            {
                                Interaction.WriteLine("输入无效。");
                            }
                            else
                            {
                                break;
                            }
                        }
                        height = input;
                    }
                    else if (def.HeightRange.Contains("-"))
                    {
                        var bounds = def.HeightRange.Split('-').Select(x => Convert.ToDouble(x)).ToArray();
                        var input = height;
                        while (true)
                        {
                            input = Interaction.GetValue(string.Format("\n指定字高（{0} - {1}）", bounds[0], bounds[1]), height);
                            if (double.IsNaN(input))
                            {
                                return null;
                            }
                            else if (input > bounds[1] || input < bounds[0])
                            {
                                Interaction.WriteLine("输入无效。");
                            }
                            else
                            {
                                break;
                            }
                        }
                        height = input;
                    }
                }
                height *= scale;
                if (text == null)
                {
                    text = Interaction.GetString("\n指定文字");
                    if (text == null)
                    {
                        return null;
                    }
                }
                if (pos == null)
                {
                    pos = Interaction.GetPoint("\n指定插入点");
                    if (pos.Value.IsNull())
                    {
                        return null;
                    }
                }
                if (align)
                {
                    var end = Interaction.GetLineEndPoint("\n指定排列线终点，或直接退出", pos.Value);
                    if (end.IsNull())
                    {
                        return Draw.Text(text, height, pos.Value).WrapInList();
                    }
                    else
                    {
                        if (alignToEnd)
                        {
                            return AlignText(text, height, pos.Value, end);
                        }
                        else
                        {
                            var angle = Math.Atan2(end.Y - pos.Value.Y, end.X - pos.Value.X);
                            return Draw.Text(text, height, pos.Value, angle).WrapInList();
                        }
                    }
                }
                else
                {
                    return Draw.Text(text, height, pos.Value).WrapInList();
                }
            };
        }

        private static List<ObjectId> AlignText(string text, double height, Point3d start, Point3d end)
        {
            if (text.Length < 2)
            {
                return new List<ObjectId>();
            }
            var chars = text.Select(x => x.ToString()).ToList();
            var angle = Math.Atan2(end.Y - start.Y, end.X - start.X);
            var div = 1.0 / (chars.Count - 1);
            var texts = Enumerable.Range(0, chars.Count).Select(i => NoDraw.Text(chars[i], height, LerpPoint(start, end, i * div), angle)).ToArray();
            return texts.AddToCurrentSpace().ToList();
        }

        private static Point3d LerpPoint(Point3d p1, Point3d p2, double param)
        {
            return p1 + param * (p2 - p1);
        }

        public static List<Point3d> PromptPointString(bool forceClose = false)
        {
            var pts = new List<Point3d>();
            var temps = new List<ObjectId>();
            var pt0 = Interaction.GetPoint("\n指定起点");
            if (pt0.IsNull())
            {
                return pts;
            }
            pts.Add(pt0);
            while (true)
            {
                var pt1 = Interaction.GetLineEndPoint("\n指定下一点", pt0);
                if (pt1.IsNull())
                {
                    break;
                }
                pts.Add(pt1);
                temps.Add(Draw.Line(pt0, pt1));
                pt0 = pt1;
            }
            temps.QForEach(t => t.Erase());
            if (forceClose && pts.Last().DistanceTo(pts.First()) > Consts.Epsilon) // newly 20140721
            {
                pts.Add(pts.First());
            }
            return pts;
        }

        private static ObjectId AddLine(Func<IEnumerable<Point3d>, ObjectId> method, bool forceClose)
        {
            var pts = PromptPointString(forceClose);
            if (pts.Count == 0)
            {
                return ObjectId.Null;
            }
            return method(pts);
        }

        public static ObjectId AddLine()
        {
            return AddLine(false);
        }

        public static ObjectId AddLine(bool forceClose)
        {
            return AddLine(Draw.Pline, forceClose);
        }

        public static Func<ObjectId> AddLine(List<Point3d> pts)
        {
            return () => Draw.Pline(pts);
        }

        [Obsolete]
        public static Func<ObjectId> AddHatch(string hatchName)
        {
            return () => AddLine(pts => Draw.Hatch(pts, hatchName), false);
        }

        [Obsolete]
        public static Func<ObjectId> AddHatch(string hatchName, List<Point3d> pts)
        {
            return () => Draw.Hatch(pts, hatchName);
        }

        public static List<ObjectId> AddArea(params string[] hatchNames) // newly 20140724
        {
            var scale = EntityManager.GetGlobalScaleFactor();
            var ids = new List<ObjectId>();
            string[] options = { "绘制边界", "选择边界" };
            var opt = Gui.GetOption("生成方式", options);
            List<Point3d> pts = null;
            if (opt == 0)
            {
                pts = PromptPointString(true);
            }
            else if (opt == 1)
            {
                var id = Interaction.GetEntity("\n选择线", typeof(Curve), false);
                pts = id.QOpenForRead<Curve>().GetPoints().ToList();
            }
            else
            {
                return null;
            }
            if (pts.Count < 3)
            {
                return ids;
            }
            ids.Add(Draw.Pline(pts));
            try
            {
                foreach (var hatchName in hatchNames)
                {
                    ids.Add(Draw.Hatch(pts, hatchName, scale));
                }
            }
            catch (Exception e)
            {
                var ae = e as Autodesk.AutoCAD.Runtime.Exception;
                if (ae != null && ae.ErrorStatus == Autodesk.AutoCAD.Runtime.ErrorStatus.HatchTooDense)
                {
                    System.Windows.MessageBox.Show("AutoCAD 无法完成填充，因为区域面积过大。", "AutoCAD", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                }
                else
                {
                    System.Windows.MessageBox.Show("AutoCAD 无法完成填充。", "AutoCAD", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                }
            }
            return ids;
        }

        public static List<ObjectId> AddFill(Polyline boundary, string block, double interval)
        {
            var scale = EntityManager.GetGlobalScaleFactor();
            interval *= scale;
            var extents = boundary.GeometricExtents; // IsExtentsIn(), GetBlockExtents()
            var tjExtents = new Dreambuild.Geometry.Extent2D(extents.MinPoint.X, extents.MinPoint.Y, extents.MaxPoint.X, extents.MaxPoint.Y);
            var pts = tjExtents.GetHexGrid(interval, 0.5).Select(p => new Point3d(p.X, p.Y, 0)).Where(p => boundary.IsPointIn(p) && boundary.GetDistToPoint(p) > interval / 2).ToList();
            if (pts.Count == 0)
            {
                pts.Add(boundary.GetCenter());
            }
            var bId = DbHelper.GetBlockId(block);
            var brs = pts.Take(1000).Select(p =>
            {
                return NoDraw.Insert(bId, p, 0, scale);
            }).ToArray();
            return brs.AddToCurrentSpace().ToList();
        }

        public static List<ObjectId> AddTextFill(Polyline boundary, string text, double interval, double rotation = 0)
        {
            var scale = EntityManager.GetGlobalScaleFactor();
            interval *= scale;
            var extents = boundary.GeometricExtents;
            var tjExtents = new Dreambuild.Geometry.Extent2D(extents.MinPoint.X, extents.MinPoint.Y, extents.MaxPoint.X, extents.MaxPoint.Y);
            var pts = tjExtents.GetHexGrid(interval, 0.5).Select(p => new Point3d(p.X, p.Y, 0)).Where(p => boundary.IsPointIn(p) && boundary.GetDistToPoint(p) > interval / 2).ToList();
            if (pts.Count == 0)
            {
                pts.Add(boundary.GetCenter());
            }
            var dts = pts.Take(1000).Select(p =>
            {
                return NoDraw.Text(text, 2.5 * scale, p, rotation);
            }).ToArray();
            return dts.AddToCurrentSpace().ToList();
        }

        public static ObjectId AddDimension()
        {
            var pt0 = Interaction.GetPoint("\n第一点");
            if (pt0.IsNull())
            {
                return ObjectId.Null;
            }
            var pt1 = Interaction.GetPoint("\n第二点");
            if (pt1.IsNull())
            {
                return ObjectId.Null;
            }
            var pt2 = Interaction.GetPoint("\n标注点");
            if (pt2.IsNull())
            {
                return ObjectId.Null;
            }
            return Draw.Dimlin(pt0, pt1, pt2);
        }

        private static void SetEntityStyles(Entity ent, string entityID, bool keepWidth = false)
        {
            var def = EntityManager.Entities[entityID];
            var scale = EntityManager.GetGlobalScaleFactor();
            ent.SetCode(def.EntityCode);
            ent.SetFirstXData(Consts.AppNameForName, def.EntityName);
            ent.Layer = def.Layer;
            ent.ColorIndex = def.ColorIndex;
            if (ent.IsLine())
            {
                if (!string.IsNullOrEmpty(def.Linetype))
                {
                    ent.Linetype = def.Linetype;
                    ent.LinetypeScale = scale;
                }
                if (ent is Polyline && !keepWidth)
                {
                    (ent as Polyline).ConstantWidth = def.ConstantWidth * scale;
                }
            }
            else if (ent.IsPoint())
            {
            }
            else if (ent.IsRegion())
            {
                ent.Linetype = "continuous";
                ent.LinetypeScale = scale;
            }
            else if (ent.IsText())
            {
                if (ent is DBText)
                {
                    var dt = ent as DBText;
                    dt.WidthFactor = def.TextXScale == 0 ? 1 : def.TextXScale;
                    dt.Oblique = def.TextItalicAngle;
                }
                else if (ent is MText)
                {
                }
                else if (ent is Dimension)
                {
                }
            }
        }

        private static void SetEntityTextStyle(Entity ent, ObjectId textStyleId)
        {
            if (ent is DBText)
            {
                var dt = ent as DBText;
                dt.TextStyleId = textStyleId;
            }
            else if (ent is MText)
            {
                var mt = ent as MText;
                mt.TextStyleId = textStyleId;
            }
            else if (ent is Dimension)
            {
                var dim = ent as Dimension;
                dim.TextStyleId = textStyleId;
            }
        }

        private static bool IsLine(this Entity ent)
        {
            return ent is Polyline || ent is Line || ent is Arc || ent is Polyline2d;
        }

        private static bool IsRegion(this Entity ent)
        {
            return ent is Region || ent is Hatch;
        }

        private static bool IsPoint(this Entity ent)
        {
            return ent is DBPoint || ent is BlockReference;
        }

        private static bool IsText(this Entity ent)
        {
            return ent is DBText || ent is MText || ent is Dimension;
        }

        private static string GetTextStyleName(string entityID)
        {
            var def = EntityManager.Entities[entityID];
            return def.TextStyle;
        }

        public static Map ExportCiml()
        {
            var map = new Map();
            var layer1 = new VectorLayer("elevation_points", VectorLayer.GEOTYPE_POINT);
            var layer2 = new VectorLayer("contours", VectorLayer.GEOTYPE_LINEAR);
            var layer3 = new VectorLayer("points", VectorLayer.GEOTYPE_POINT);
            var layer4 = new VectorLayer("lines", VectorLayer.GEOTYPE_LINEAR);
            var layer5 = new VectorLayer("regions", VectorLayer.GEOTYPE_REGION);

            var ids = QuickSelection.SelectAll().QWhere(x => x.GetCode() != null).ToList();
            ids.QForEach<Entity>(ent =>
            {
                var code = ent.GetCode();
                var def = EntityManager.Entities.FirstOrDefault(x => x.Value.EntityCode == code).Value;
                if (def != null)
                {
                    var points = new List<Dreambuild.Geometry.Point2D>();
                    if (ent is BlockReference)
                    {
                        var pos = (ent as BlockReference).Position;
                        points.Add(new Dreambuild.Geometry.Point2D(pos.X, pos.Y));
                    }
                    else if (ent is Polyline)
                    {
                        var pts = (ent as Polyline).GetPoints().ToList();
                        pts.ForEach(pt => points.Add(new Dreambuild.Geometry.Point2D(pt.X, pt.Y)));
                    }
                    else if (ent is Polyline2d)
                    {
                        var poly = new Polyline();
                        poly.ConvertFrom(ent, true);
                        var pts = poly.GetPoints().ToList();
                        pts.ForEach(pt => points.Add(new Dreambuild.Geometry.Point2D(pt.X, pt.Y)));
                    }
                    var feature = new Feature(points);
                    feature["code"] = code;
                    feature["name"] = ent.GetFirstXData(Consts.AppNameForName);
                    feature["layer"] = def.Layer;
                    feature["color"] = def.ColorIndex.ToString();

                    if (def.EntityType == "Block")
                    {
                        feature["block"] = def.BlockName;
                        if (code == "110102" || code == "110202" || code == "110302")
                        {
                            feature["height"] = (ent as BlockReference).GetBlockAttributes()["height"];
                            layer1.Features.Add(feature);
                        }
                        else
                        {
                            layer3.Features.Add(feature);
                        }
                    }
                    else if (def.EntityType == "Line")
                    {
                        feature["linetype"] = def.Linetype;
                        feature["cwidth"] = def.ConstantWidth.ToString();
                        if (code == "730101" || code == "730102")
                        {
                            feature["height"] = ent.Id.GetData(DictName.CmDrawing, KeyName.Elevation);
                            layer2.Features.Add(feature);
                        }
                        else
                        {
                            layer4.Features.Add(feature);
                        }
                    }
                    else if (def.EntityType == "Region")
                    {
                        feature["linetype"] = def.Linetype;
                        feature["cwidth"] = def.ConstantWidth.ToString();
                        layer5.Features.Add(feature);
                    }
                }
            });

            map.Layers.AddRange(new List<ILayer> { layer1, layer2, layer3, layer4, layer5 });
            return map;
        }

        public static void ExportShapefile(string folder)
        {
            var exporter = new MultipleShapefileExporter(ExportCiml());
            exporter.Export(folder);
        }
    }

    public class EntityDefinition
    {
        // basic info
        public string EntityID { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        public string EntityType { get; set; }

        // styles
        public string Layer { get; set; }
        public int ColorIndex { get; set; }
        public string Linetype { get; set; }
        public double ConstantWidth { get; set; }
        public string BlockName { get; set; }
        public string TextStyle { get; set; }
        public double TextHeight { get; set; }
        public double TextXScale { get; set; }
        public double TextItalicAngle { get; set; }

        // interactions
        public bool AskForHeight { get; set; }
        public string HeightRange { get; set; }
    }
}
