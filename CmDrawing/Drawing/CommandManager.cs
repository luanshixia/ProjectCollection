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
using Autodesk.AutoCAD.Windows;

using AutoCADCommands;

[assembly: ExtensionApplication(typeof(TongJi.Drawing.CommandManager))]

namespace TongJi.Drawing
{
    public class CommandManager : IExtensionApplication
    {
        private static System.Windows.Window menuWindow = new Menu();
        private static PaletteSet menuPalette = new PaletteSet(ProductManager.GetProductTitle()); // 20110623
        private static PaletteSet galleryPalette = new PaletteSet("快捷工具面板"); // 20140710

        #region 程序功能

        public void Initialize()
        {
            Services.CreateLoaderFeedback();
            Services.CheckLicenseOnStart();
            Services.InitializeDocument();

            Application.DocumentManager.DocumentCreated += DocumentHandler;
            Application.DocumentManager.DocumentActivated += DocumentHandler;

            //ShowMenu();
            ShowExpressGallery();
            //ShowPropertyWindow();
            Interaction.SetActiveDocFocus();
        }

        private void DocumentHandler(object sender, DocumentCollectionEventArgs e)
        {
            Services.InitializeDocument();
        }

        public void Terminate()
        {
        }

        [CommandMethod("ShowMenu")]
        public static void ShowMenu()
        {
            string productID = ProductManager.ProductID;
            ShowMenuByID(productID);
        }

        [CommandMethod("ShowMenuDev")]
        public static void ShowMenuDev()
        {
            string productID = Interaction.GetString("Product ID");
            ProductManager.ProductID = productID;
            ShowMenuByID(productID);
        }

        private static void ShowMenuByID(string productID)
        {
            if (menuPalette.Visible)
            {
                menuPalette.Close();
                menuPalette.Dispose();
            }

            Product product = ProductManager.GetProductRecordByID(productID);
            string title = ProductManager.GetProductTitle();
            menuWindow.Close();
            menuWindow = product.GetProductMenuWindow();
            Application.MainWindow.Text = title;

            menuPalette = new PaletteSet(title);
            System.Windows.Controls.ScrollViewer root = menuWindow.Content as System.Windows.Controls.ScrollViewer;
            root.Width = 240;
            root.Height = 2000;
            root.Margin = new System.Windows.Thickness(0);
            menuPalette.AddVisual("菜单", root);
            menuPalette.Size = new System.Drawing.Size(200, 640);
            menuPalette.MinimumSize = new System.Drawing.Size(200, 640);
            menuPalette.SizeChanged += (sender, e) => root.Width = menuPalette.Size.Width;
            menuPalette.Visible = true;
            menuPalette.Dock = DockSides.Right;
            menuPalette.DockEnabled = DockSides.Right;
        }

        [CommandMethod("ShowExpressGallery")]
        public static void ShowExpressGallery()
        {
            if (galleryPalette.Visible)
            {
                return;
            }

            var gallery = new Gallery();
            galleryPalette = new PaletteSet("崇明滩涂");
            galleryPalette.AddVisual("Gallery", gallery);
            galleryPalette.Size = new System.Drawing.Size(200, 640);
            galleryPalette.MinimumSize = new System.Drawing.Size(200, 640);
            galleryPalette.SizeChanged += (sender, e) =>
            {
                gallery.Width = galleryPalette.Size.Width;
                gallery.Height = galleryPalette.Size.Height - 20;
            };
            galleryPalette.Visible = true;
            galleryPalette.Dock = DockSides.Left;
            galleryPalette.DockEnabled = DockSides.Left;
        }

        [CommandMethod("ShowPropertyWindow")]
        public static void ShowPropertyWindow()
        {

        }

        [CommandMethod("ShowGallery")]
        public static void ShowGallery()
        {
            GalleryWindow gw = new GalleryWindow();
            Application.ShowModalWindow(gw);
        }

        [CommandMethod("ProductHelp")]
        public static void ProductHelp()
        {
            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();
            if (Utils.ParseIniFile(App.CurrentFolder + "Drawing.cfg", result) == true)
            {
                string helpFile = App.CurrentFolder + result["Information"]["HelpFile"];
                if (System.IO.File.Exists(helpFile))
                {
                    System.Diagnostics.Process.Start(helpFile);
                }
                else
                {
                    Interaction.WriteLine("找不到帮助文件，请确认程序目录完整。");
                }
            }
        }

        [CommandMethod("AboutProduct")]
        public static void AboutProduct()
        {
            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();
            if (Utils.ParseIniFile(App.CurrentFolder + "Drawing.cfg", result) == true)
            {
                string[] items = result["Product"].Select(x => x.Value).ToArray();
                System.Windows.MessageBox.Show(string.Join("\n", items), "关于", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }

        [CommandMethod("CleanGenerated")]
        public static void CleanGenerated()
        {
            Services.EraseGenerated();
        }

        [CommandMethod("ShowPythonConsole")]
        public void ShowPythonConsole()
        {

        }

        #endregion

        #region 工具功能

        [CommandMethod("PolyUnion")]
        public static void PolyUnion()
        {
            var ids = Interaction.GetSelection("\n选择多段线", "LWPOLYLINE");
            if (ids.Length == 0)
            {
                return;
            }
            var union = Services.PolyUnion(ids);
            ids.QForEach(x => x.Erase());
        }

        [CommandMethod("PolyOverkill")]
        public static void PolyOverkill()
        {
            if (Interaction.TaskDialog("请注意...", "运行命令", "返回", ProductManager.GetProductTitle(), "将运行AutoCAD自带命令OVERKILL来执行操作，此命令需要2012以上版本，或者已安装Express Tools，否则无法运行。", "请确保您可以执行此命令。", "可咨询您的技术支持。") == true)
            {
                Interaction.Command("overkill ");
            }
        }

        [CommandMethod("PolyMerge")]
        public static void PolyMerge()
        {
            if (Interaction.TaskDialog("请注意...", "运行命令", "返回", ProductManager.GetProductTitle(), "将运行AutoCAD自带命令PEDIT来执行操作。选择要合并的多段线后右击，再选择“合并”即可。", "请确保您已充分理解此命令的使用方法。", "可咨询您的技术支持。") == true)
            {
                Interaction.Command("pe m ");
            }
        }

        #endregion

        #region 开发辅助

        [CommandMethod("CDatabase")]
        public static void CDatabase()
        {
            Interaction.WriteLine(HostApplicationServices.WorkingDatabase.Filename);
        }

        [CommandMethod("ViewXData")]
        public static void ViewXData()
        {
            while (true)
            {
                ObjectId id = Interaction.GetEntity("\n选择实体");
                if (id == ObjectId.Null)
                {
                    break;
                }
                DBObject obj = id.QOpenForRead();
                using (System.IO.StringWriter sw = new System.IO.StringWriter())
                {
                    sw.WriteLine(RXClass.GetClass(obj.GetType()).DxfName);
                    var xdata = obj.XData;
                    if (xdata != null)
                    {
                        TypedValue[] data = obj.XData.AsArray();
                        foreach (TypedValue value in data)
                        {
                            sw.WriteLine(value.ToString());
                        }
                    }
                    else
                    {
                        sw.WriteLine("无扩展数据。");
                    }
                    Gui.TextReport("XData", sw.ToString(), 300, 300, true);
                    //CppImport.RunCommand(false, "textscr");
                }
            }
        }

        #endregion

        #region CM

        [CommandMethod("PropQuery")]
        public static void PropQuery()
        {
        }

        [CommandMethod("PropMark")]
        public static void PropMark()
        {
        }

        [CommandMethod("PropMatch")]
        public static void PropMatch()
        {
        }

        [CommandMethod("ConvertPoint")]
        public static void ConvertPoint()
        {
            var entityID = "250";
            DbHelper.AffirmRegApp(AppName.CassCode);
            var ids = QuickSelection.SelectAll("INSERT").QWhere(x => x.GetFirstXData(AppName.CassCode) == "202101").ToArray();
            ids.SetStyles(entityID);
        }

        [CommandMethod("ConvertContour")]
        public static void ConvertContour()
        {
            var intermediateContourID = "252";
            var indexContourID = "253";
            DbHelper.AffirmRegApp(AppName.CassCode);
            var ids1 = QuickSelection.SelectAll().QWhere(x => x is Curve && x.GetFirstXData(AppName.CassCode) == "186301").ToArray();
            var ids2 = QuickSelection.SelectAll().QWhere(x => x is Curve && x.GetFirstXData(AppName.CassCode) == "186302").ToArray();
            ids1.SetStyles(intermediateContourID);
            ids2.SetStyles(indexContourID);
        }

        [CommandMethod("ConvertCass")]
        public static void ConvertCass()
        {
            var choices = EntityManager.Entities.Select(x => string.Format("{0}.{1}.{2}", x.Key, x.Value.EntityCode, x.Value.EntityName)).ToArray();
            var selects = Gui.GetChoices("选择类型", choices);
            if (selects.Length == 0)
            {
                return;
            }
            DbHelper.AffirmRegApp(AppName.CassCode);
            foreach (var select in selects)
            {
                var code = select.Split('.')[0];
                var entityID = select.Split('.')[1];
                var ids = QuickSelection.SelectAll().QWhere(x => x.GetFirstXData(AppName.CassCode) == code).ToArray();
                ids.SetStyles(entityID);
            }
        }

        [CommandMethod("CmCo")]
        public static void CmCo()
        {
            ConvertEntity();
        }

        [CommandMethod("ConvertEntity")]
        public static void ConvertEntity()
        {
            var ids = Interaction.GetSelection("\n选择对象");
            if (ids.Length == 0)
            {
                return;
            }
            var choices = EntityManager.Entities.Select(x => string.Format("{0}.{1}", x.Key, x.Value.EntityName)).ToArray();
            var choice = Gui.GetChoice("选择类型", choices);
            if (string.IsNullOrEmpty(choice))
            {
                return;
            }
            var entityID = choice.Split('.')[0];
            ids.SetStyles(entityID);
        }

        [CommandMethod("SplitLayout")]
        public static void SplitLayout()
        {
        }

        [CommandMethod("EditContourElevation")]
        public static void EditContourElevation()
        {
            while (true)
            {
                var id = Interaction.GetEntity("\n选择等高线");
                if (id.IsNull)
                {
                    return;
                }
                var value = Interaction.GetValue("\n输入高程");
                if (double.IsNaN(value))
                {
                    return;
                }
                id.SetData(DictName.CmDrawing, KeyName.Elevation, value.ToString());
            }
        }

        [CommandMethod("ViewContourElevation")]
        public static void ViewContourElevation()
        {
            var pt1 = Interaction.GetPoint("\n截线起点");
            if (pt1.IsNull())
            {
                return;
            }
            var pt2 = Interaction.GetLineEndPoint("\n截线终点", pt1);
            if (pt2.IsNull())
            {
                return;
            }
            var line = NoDraw.Line(pt1, pt2);
            var ids = QuickSelection.SelectAll().QWhere(x => x.GetCode() == "730101" || x.GetCode() == "730102").ToList();
            var temps = new List<ObjectId>();
            ids.QForEach<Curve>(cv =>
            {
                if (cv != null)
                {
                    var points = new Point3dCollection();
                    Algorithms.IntersectWith3264(line, cv, Intersect.OnBothOperands, points);
                    var text = cv.ObjectId.GetData(DictName.CmDrawing, KeyName.Elevation);
                    points.Cast<Point3d>().ForEach(p =>
                    {
                        temps.Add(Draw.Text(text, 2.5, p));
                    });
                }
            });
            Interaction.GetString("\n按ESC退出");
            temps.QForEach(x => x.Erase());
        }

        [CommandMethod("SetGlobalScale")]
        public static void SetGlobalScale()
        {
            var tip = "指定比例尺。一旦指定请勿随意更改，因为更改只影响之后的绘制。";
            string[] scales = { "1:1000", "1:2500", "1:5000", "1:10000" };
            var opt = Gui.GetOption(tip, scales);
            if (opt < 0)
            {
                return;
            }
            CustomDictionary.SetValue(DictName.CmDrawing, KeyName.GlobalScale, scales[opt]);
        }

        [CommandMethod("ViewGlobalScale")]
        public static void ViewGlobalScale()
        {
            var scale = CustomDictionary.GetValue(DictName.CmDrawing, KeyName.GlobalScale);
            Interaction.WriteLine(scale);
        }

        #endregion
    }
}
