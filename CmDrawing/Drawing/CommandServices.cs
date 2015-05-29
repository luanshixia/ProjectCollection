using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

using AutoCADCommands;

namespace TongJi.Drawing
{
    using Polygon = List<clipper.IntPoint>;
    using Polygons = List<List<clipper.IntPoint>>;

    public class Product
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string MenuWindowName { get; set; }

        public System.Windows.Window GetProductMenuWindow()
        {
            return (System.Windows.Window)(typeof(ProductManager).Assembly.CreateInstance("TongJi.Drawing." + MenuWindowName));
        }
    }

    public static class ProductManager
    {
        private static List<Product> _products = new List<Product>
        {
            new Product { ID = "1", Name = "崇明滩涂", MenuWindowName = "Menu" },
        };

        public static string ProductID { get; set; }

        static ProductManager()
        {
            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();
            string cfg = App.CurrentFolder + "Drawing.cfg";
            if (System.IO.File.Exists(cfg))
            {
                if (Utils.ParseIniFile(cfg, result) == true)
                {
                    ProductID = result["Product"]["ID"];
                    return;
                }
            }
            ProductID = "1";
        }

        public static Product GetProductRecord()
        {
            return GetProductRecordByID(ProductID);
        }

        public static Product GetProductRecordByID(string productID)
        {
            return _products.Any(x => x.ID == productID) ? _products.Single(x => x.ID == productID) : _products.First();
        }

        public static string GetProductTitle() // newly 20130816
        {
            if (Services.LicenseManager.License.State == LicenseState.Licensed)
            {
                return GetProductRecord().Name;
            }
            else
            {
                return GetProductRecord().Name + " (未激活)";
            }
        }
    }

    public class LayerName
    {
        public const string Annotation = "tjAnnotation";
        public const string LockedLayer = "tjLocked";
    }

    public class BlockName
    {
        public const string Hidden = "tjHidden";
    }

    public class AppName
    {
        public const string CassCode = "SOUTH";
    }

    public class CodeName
    {
        
    }

    public class TagName
    {
        public const string Generated = "Generated";
        public const string Contour = "Contour";
    }

    public class DictName
    {
        public const string General = "General";
        public const string GlobalStyles = "GlobalStyles";
        public const string Surface = "Surface";
        public const string CmDrawing = "CmDrawing";
    }

    public class KeyName
    {
        // Keys for dict [CmDrawing]
        public const string Elevation = "Elevation";
        public const string GlobalScale = "GlobalScale";
    }

    /// <summary>
    /// 封装一些静态方法
    /// </summary>
    public static class Services
    {
        public static LicenseManager LicenseManager { get; private set; }

        static Services()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Drawing\\";
            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }
            LicenseManager = new LicenseManager(folder + "license.dat");
        }

        public static void CreateLoaderFeedback()
        {
            System.IO.File.Create(App.CurrentFolder + "loaded.tmp");
        }

        public static void InitializeDocument()
        {
            DbHelper.InitializeDatabase();
            DbHelper.AffirmRegApp(AppName.CassCode);
            Services.CreateHidden();
            //Application.DocumentManager.MdiActiveDocument.ImpliedSelectionChanged -= PickSetHandler;
            //Application.DocumentManager.MdiActiveDocument.ImpliedSelectionChanged += PickSetHandler;

            if (App.IsDocumentNew())
            {
                if (CustomDictionary.GetValue(DictName.CmDrawing, KeyName.GlobalScale) == string.Empty)
                {
                    CommandManager.SetGlobalScale();
                }
            }
        }

        private static void PickSetHandler(object sender, EventArgs e)
        {
            // 禁止选择Locked图层的内容
            var pickSet = Interaction.GetPickSet();
            if (pickSet.QCount(x => x.Layer == LayerName.LockedLayer) > 0)
            {
                pickSet = pickSet.QWhere(x => x.Layer != LayerName.LockedLayer).ToArray();
                Interaction.SetPickSet(pickSet);
            }
        }

        public static void CheckLicenseOnStart()
        {
            if (LicenseManager.License.State == LicenseState.Illegal)
            {
                System.Windows.MessageBox.Show("软件许可出现非法状态。程序将马上退出。", "产品许可", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Stop);
                throw new System.Exception("产品许可异常。");
            }
            else if (LicenseManager.License.State == LicenseState.Trial)
            {
                ActivationWindow aw = new ActivationWindow { HasTimeLimit = true };
                Application.ShowModalWindow(aw);
                if (LicenseManager.License.State == LicenseState.Trial && LicenseManager.IsTrialExpired())
                {
                    System.Windows.MessageBox.Show("产品试用已结束，无法继续运行。请激活产品。", "产品许可");
                    throw new System.Exception("产品试用已结束。");
                }
                //HostApplicationServices.WorkingDatabase.BeginSave += (sender, e) =>
                //{
                //    System.Windows.MessageBox.Show("试用版产品不可保存。", "警告");
                //    throw new System.Exception("试用版产品不可保存。");
                //};
            }
        }

        public static bool CheckLicenseOfSpecialFeature()
        {
            if (LicenseManager.License.State != LicenseState.Licensed)
            {
                System.Windows.MessageBox.Show("此功能必须激活后才能使用，请您购买并激活产品。", "产品许可", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Asterisk);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 清理具有Generated标记的图元
        /// </summary>
        public static void EraseGenerated()
        {
            QuickSelection.SelectAll("*LINE,ARC").QWhere(x => x.HasTag(TagName.Generated)).QForEach(x => x.Erase());
        }

        public static void CreateHidden()
        {
            if (!DbHelper.GetAllBlockNames().Contains(BlockName.Hidden))
            {
                Draw.BlockOfDwg(BlockName.Hidden, App.CurrentFolder + "\\Resources\\Res.dwg");
            }
            CheckSurfaceLink();
        }

        public static string GetProfile()
        {
            return "0,0|1,0";
        }

        public static Surface GetSurface()
        {
            Database db = HostApplicationServices.WorkingDatabase;

            string relativePath = CustomDictionary.GetValue(DictName.Surface, "FileName");
            if (relativePath == string.Empty) // 未定义外部链接
            {
                return Surface.GetHorizontalPlane(new Extents3d(db.Extmin, db.Extmax), 0);
            }
            else // 定义了外部链接
            {
                if (CheckSurfaceLink() == true) // 检查链接，直接或经修复后找到参照
                {
                    Surface surf;
                    string path = (relativePath.Contains(":") ? string.Empty : App.DocumentFolder) + relativePath;
                    string extension = System.IO.Path.GetExtension(path).Substring(1);
                    if (extension.ToUpper() == "DAT")
                    {
                        surf = Surface.FromDatFile(path);
                    }
                    else if (extension.ToUpper() == "TXT")
                    {
                        surf = Surface.FromTxtFile(path);
                    }
                    else if (extension.ToUpper() == "DEM" || extension.ToUpper() == "TIF")
                    {
                        surf = Surface.FromDem(path, 10);
                    }
                    else
                    {
                        surf = Surface.GetHorizontalPlane(new Extents3d(db.Extmin, db.Extmax), 0);
                    }
                    return surf;
                }
                else // 检查链接，修复后仍未找到参照
                {
                    return Surface.GetHorizontalPlane(new Extents3d(db.Extmin, db.Extmax), 0);
                }
            }
        }

        public static bool CheckSurfaceLink()
        {
            string relativePath = CustomDictionary.GetValue(DictName.Surface, "FileName");
            if (relativePath == string.Empty)
            {
                return false;
            }
            string path = (relativePath.Contains(":") ? string.Empty : App.DocumentFolder) + relativePath;
            if (System.IO.File.Exists(path)) // 直接找到
            {
                return true;
            }
            else // 未找到
            {
                if (Interaction.TaskDialog("无法定位地形参照文件", "选择地形参照", "移除地形参照", ProductManager.GetProductTitle(), "您希望执行什么操作？") == true) // 修复链接
                {
                    //CommandManager.AddSurface();
                    if (CustomDictionary.GetValue(DictName.Surface, "FileName") != relativePath) // 修复成功
                    {
                        return true;
                    }
                    else // 修复失败
                    {
                        return false;
                    }
                }
                else // 移除链接
                {
                    CustomDictionary.SetValue(DictName.Surface, "FileName", string.Empty);
                    return false;
                }
            }
        }

        public static ObjectId[] PolyUnion(ObjectId[] ids, int expand = 0, clipper.PolyFillType fillType = clipper.PolyFillType.pftNonZero)
        {
            int preExpand = 2;
            var polys = ids.QSelect(x => (x as Polyline).GetPolylineFitPoints(5).Select(y => new clipper.IntPoint((long)y.X, (long)y.Y)).ToList()).ToList();
            polys = clipper.Clipper.OffsetPolygons(polys, preExpand);

            Polygons solution = new Polygons();
            clipper.Clipper c = new clipper.Clipper();
            c.AddPolygons(polys, clipper.PolyType.ptSubject);
            bool succeeded = c.Execute(clipper.ClipType.ctUnion, solution, fillType, fillType);

            List<ObjectId> result = new List<ObjectId>();
            if (succeeded)
            {
                solution = clipper.Clipper.OffsetPolygons(solution, expand - preExpand);
                foreach (var poly in solution)
                {
                    var polyPoints = poly.Select(x => new Point3d(x.X, x.Y, 0)).ToList();
                    if (polyPoints.Last().DistanceTo(polyPoints.First()) > Consts.Epsilon)
                    {
                        polyPoints.Add(polyPoints.First());
                    }
                    var id = Draw.Pline(polyPoints);
                    result.Add(id);
                }
            }
            return result.ToArray();
        }

        public static Polyline[] PolyUnion(Polyline[] cvs, int expand = 0, clipper.PolyFillType fillType = clipper.PolyFillType.pftNonZero)
        {
            int preExpand = 2;
            var polys = cvs.Select(x => x.GetPolylineFitPoints().Select(y => new clipper.IntPoint((long)y.X, (long)y.Y)).ToList()).ToList();
            polys = clipper.Clipper.OffsetPolygons(polys, preExpand);

            Polygons solution = new Polygons();
            clipper.Clipper c = new clipper.Clipper();
            c.AddPolygons(polys, clipper.PolyType.ptSubject);
            bool succeeded = c.Execute(clipper.ClipType.ctUnion, solution, fillType, fillType);

            List<Polyline> result = new List<Polyline>();
            if (succeeded)
            {
                solution = clipper.Clipper.OffsetPolygons(solution, expand - preExpand);
                foreach (var poly in solution)
                {
                    var polyPoints = poly.Select(x => new Point3d(x.X, x.Y, 0)).ToList();
                    if (polyPoints.Last().DistanceTo(polyPoints.First()) > Consts.Epsilon)
                    {
                        polyPoints.Add(polyPoints.First());
                    }
                    var pl = NoDraw.Pline(polyPoints);
                    result.Add(pl);
                }
            }
            return result.ToArray();
        }

        public static void BuildLockedLayer()
        {
            ObjectId lockedLayerId = DbHelper.GetLayerId(LayerName.LockedLayer);
            lockedLayerId.QOpenForWrite<LayerTableRecord>(lockedLayer =>
            {
                lockedLayer.IsLocked = true;
            });
        }

        public static void CheckDocSave(Action success)
        {
            if (App.DocumentFolder == string.Empty)
            {
                if (Interaction.TaskDialog("图形尚未保存", "保存图形", "暂不保存", ProductManager.GetProductTitle(), "这是一个新创建的图形，请保存到磁盘目录，以便我们和参照文件一起处理。") == true)
                {
                    Interaction.Command("save ");
                }
            }
            else
            {
                success();
            }
        }

        public static ObjectId[] HighlightEntity(IEnumerable<ObjectId> ids, int color)
        {
            var result = ids.QSelect(x => (x.Clone() as Entity).AddToCurrentSpace()).ToArray();
            result.QForEach<Entity>(ent =>
            {
                if (ent is Polyline)
                {
                    (ent as Polyline).ConstantWidth = 10;
                }
                ent.ColorIndex = color;
            });
            result.Draworder(DraworderOperation.MoveToTop);
            return result;
        }
    }
}
