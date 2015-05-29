using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

using TongJi.TongJiGlobal;

using AutoCADCommands;

namespace TongJi.Drawing
{
    public class CommandManager : IExtensionApplication
    {
        private static RoadInspector riWindow = new RoadInspector();
        private static Menu menuWindow = new Menu();
        private static bool smartVisible = true;

        public void Initialize()
        {
            Services.CreateHidden();
            Services.LoadDll("TZ.dll");
            Application.MainWindow.Text = "同济筑城";
            Application.DocumentManager.MdiActiveDocument.ImpliedSelectionChanged += pickSetHandler;
            Application.DocumentManager.DocumentActivationChanged += (sender, e) =>
            {
                Application.DocumentManager.MdiActiveDocument.ImpliedSelectionChanged -= pickSetHandler;
                Application.DocumentManager.MdiActiveDocument.ImpliedSelectionChanged += pickSetHandler;
            };
            Application.DocumentManager.DocumentCreated += (sender, e) =>
            {
                Services.CreateHidden();
                Application.MainWindow.Text = "同济筑城";
            };
            Application.ShowModelessWindow(menuWindow);
            Interaction.SetActiveDocFocus();
        }

        public void Terminate()
        {
        }

        private static void pickSetHandler(object sender, EventArgs e)
        {
            ObjectId[] ids = Interaction.GetPickSet();
            if (ids.Length > 0 && ids.All(x => TongJiCode.GetCode(x) == TjCode.Road))
            {
                riWindow.Close();
                riWindow = new RoadInspector();
                Application.ShowModelessWindow(riWindow);
                Interaction.SetActiveDocFocus();
            }
            else
            {
                riWindow.Hide();
            }
        }

        [CommandMethod("MakeRoad", CommandFlags.UsePickSet)]
        public static void MakeRoad()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptSelectionResult proSelRes = ed.SelectImplied();
            if (proSelRes.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\n选择道路中心线");
                proSelRes = ed.GetSelection();
                if (proSelRes.Status != PromptStatus.OK)
                {
                    return;
                }
            }
            ObjectId[] ids = proSelRes.Value.GetObjectIds();
            ids = ids.Where(x => CADBase.GetFirstXData(x, AppName.Managed) != "true").ToArray();
            string section = Services.GetSection();
            using (Transaction trans = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id in ids)
                {
                    Entity ent = trans.GetObject(id, OpenMode.ForWrite) as Entity;
                    if (ent is Curve)
                    {
                        TongJiCode.AddCode(id, TjCode.Road);
                        CADBase.SetFirstXData(id, AppName.Section, section);
                        CADBase.SetEntityLayer(ent, LayerName.Alignment);
                    }
                }
                trans.Commit();
            }

            Services.UpdateDrawing();
        }

        [CommandMethod("SectionStyle")]
        public static void SectionStyle()
        {
            Application.ShowModalWindow(new SectionStyle());
        }

        [CommandMethod("Make3D")]
        public static void Make3D()
        {
            // 这是为3D组导出sdf准备图形
            Point3d ambient = Interaction.GetPoint("\n指定Ambient点");
            ObjectId ambientId = Draw.Point(ambient);
            TongJiCode.AddCode(ambientId, "100017");
            string[] otherCodes = { "100013", "100014", "100015", "100020", "100021", "100022" };
            for (int i = 0; i < otherCodes.Length; i++)
            {
                ObjectId id = Draw.Point(ambient.Add(new Vector3d((i + 1) * 100, 0, 0)));
                TongJiCode.AddCode(id, otherCodes[i]);
            }
        }

        [CommandMethod("AppendCode", CommandFlags.UsePickSet)]
        public static void AppendCode()
        {
            string code = Interaction.GetString("\n指定编码");
            ObjectId[] ids = Interaction.GetPickSet();
            if (ids.Length == 0)
            {
                ids = Interaction.GetSelection("\n选择实体");
            }
            ids.ToList().ForEach(x => TongJiCode.AddCode(x, code));
        }

        [CommandMethod("BreakSeperation")]
        public static void BreakSeperation()
        {
            ObjectId idCenter;
            while (true)
            {
                idCenter = Interaction.GetEntity("\n选择道路中心线");
                if (idCenter.QSelect(x => TongJiCode.GetCode(x) == TjCode.Road)) // idCenter.QSelect(x => TongJiCode.GetCode(x)) == TjCode.Road 效果一样
                {
                    break;
                }
            }
            Point3d breakPoint;
            RoadSide side;
            while (true)
            {
                breakPoint = Interaction.GetPoint("\n指定中心线一侧一点以打断");
                side = (idCenter.QOpenForRead() as Curve).GetSideOfPoint(breakPoint);
                if (side != RoadSide.On)
                {
                    break;
                }
            }
            breakPoint = (idCenter.QOpenForRead() as Curve).GetClosestPointTo(breakPoint, false);
            double breakDist = (idCenter.QOpenForRead() as Curve).GetDistAtPointX(breakPoint);
            string breakingDef = string.Format("{0:0.00}|{1}|{2}", breakDist, side.ToString(), Services.GetSeperationBreakStyle());
            RoadBreakingManager.AddSeperationBreak(idCenter, breakingDef);

            Services.UpdateDrawing();
        }

        [CommandMethod("AddBusStop")]
        public static void AddBusStop()
        {
            ObjectId idCenter;
            while (true)
            {
                idCenter = Interaction.GetEntity("\n选择道路中心线");
                if (idCenter.QSelect(x => TongJiCode.GetCode(x) == TjCode.Road)) // idCenter.QSelect(x => TongJiCode.GetCode(x)) == TjCode.Road 效果一样
                {
                    break;
                }
            }
            Point3d breakPoint;
            RoadSide side;
            while (true)
            {
                breakPoint = Interaction.GetPoint("\n指定中心线一侧一点以打断");
                side = (idCenter.QOpenForRead() as Curve).GetSideOfPoint(breakPoint);
                if (side != RoadSide.On)
                {
                    break;
                }
            }
            breakPoint = (idCenter.QOpenForRead() as Curve).GetClosestPointTo(breakPoint, false);
            double breakDist = (idCenter.QOpenForRead() as Curve).GetDistAtPointX(breakPoint);
            string breakingDef = string.Format("{0:0.00}|{1}|{2}", breakDist, side.ToString(), Services.GetBusStopStyle());
            RoadBreakingManager.AddBusStop(idCenter, breakingDef);

            Services.UpdateDrawing();
        }

        [CommandMethod("ShowMenu")]
        public static void ShowMenu()
        {
            menuWindow.Close();
            menuWindow = new Menu();
            Application.ShowModelessWindow(menuWindow);
        }

        [CommandMethod("SmartBreak")]
        public static void SmartBreak()
        {
            string block = TjCode.SeperationBreak;

            Point3d insertPoint = Interaction.GetPoint("\n指定插入点");
            Draw.Block(block, Services.FolderPath + "BlockLib.dwg");
            ObjectId insert = Draw.Insert(block, insertPoint);

            string style = Services.GetSeperationBreakStyle();
            TongJiCode.AddCode(insert, block);
            CADBase.SetFirstXData(insert, "长度", Convert.ToDouble(style));
        }

        [CommandMethod("SmartBusStop")]
        public static void SmartBusStop()
        {
            string block = TjCode.BusStop;

            Point3d insertPoint = Interaction.GetPoint("\n指定插入点");
            Draw.Block(block, Services.FolderPath + "BlockLib.dwg");
            ObjectId insert = Draw.Insert(block, insertPoint);

            string[] style = Services.GetBusStopStyle().Split('|');
            TongJiCode.AddCode(insert, block);
            CADBase.SetFirstXData(insert, "长度", Convert.ToDouble(style[0]));
            CADBase.SetFirstXData(insert, "进站缓长", Convert.ToDouble(style[1]));
            CADBase.SetFirstXData(insert, "出站缓长", Convert.ToDouble(style[2]));
            CADBase.SetFirstXData(insert, "宽度", Convert.ToDouble(style[3]));
        }

        [CommandMethod("ToggleSmart")]
        public static void ToggleSmart()
        {
            smartVisible = !smartVisible;
            var ids = QuickSelection.SelectAll("INSERT").Where(x => TongJiCode.GetCode(x) == TjCode.SeperationBreak || TongJiCode.GetCode(x) == TjCode.BusStop);
            using (Transaction trans = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction())
            {
                foreach (var id in ids)
                {
                    Entity ent = trans.GetObject(id, OpenMode.ForWrite) as Entity;
                    ent.Visible = smartVisible;
                }
                trans.Commit();
            }
        }
    }

    /// <summary>
    /// 封装一些常量
    /// </summary>
    public class Consts
    {
        public const double Epsilon = 0.001;
        public const string DefaultRoadSection = "h(3)y(3){(1.5)}(3.5)|(3.5)@(3.5)|(3.5){(1.5)}(3)y(3)h";
        public const string Hidden = "tjHidden";
    }

    public class TjCode
    {
        public const string Road = "road";
        public const string SeperationBreak = "SeperationBreak";
        public const string BusStop = "BusStop";
    }

    public class AppName
    {
        public const string RoadName = "名称";
        public const string Section = "道路断面";
        public const string SectionAlias = "断面符号";
        public const string Generated = "Generated";
        public const string Managed = "Managed";
    }

    public class LayerName
    {
        public const string Alignment = "AlignmentPoly";

        public const string Yuanshixian = "tjYuanshixian";
        public const string Hongxian = "tjHongxian";
        public const string Center = "tjCenter";
        public const string Lane = "tjLane";
        public const string Seperation = "tjSeperation";
    }

    public class DictName
    {
        public const string GlobalStyles = "GlobalStyles";
        public const string SectionAliasDef = "SectionAliasDef";
    }

    /// <summary>
    /// 封装一些静态方法
    /// </summary>
    public class Services
    {
        /// <summary>
        /// 获取缺省断面
        /// </summary>
        /// <returns>断面字符串</returns>
        public static string GetSection()
        {
            return Consts.DefaultRoadSection;
        }

        public static string GetSeperationBreakStyle()
        {
            string style = CustomDictionary.GetValue(DictName.GlobalStyles, TjCode.SeperationBreak);
            if (style == string.Empty)
            {
                style = string.Format("{0}", 5);
            }
            return style;
        }

        public static string GetBusStopStyle()
        {
            string style = CustomDictionary.GetValue(DictName.GlobalStyles, TjCode.BusStop);
            if (style == string.Empty)
            {
                style = string.Format("{0}|{1}|{2}|{3}", 30, 5, 5, 5);
            }
            return style;
        }

        [CommandMethod("UpdateDrawing")]
        /// <summary>
        /// 重新绘制Generated层
        /// </summary>
        public static void UpdateDrawing()
        {
            Services.EraseGenerated();
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptSelectionResult proSelRes = ed.SelectAll(new SelectionFilter(new TypedValue[] { new TypedValue(8, LayerName.Alignment), new TypedValue(0, "*LINE,ARC") }));
            if (proSelRes.Status != PromptStatus.OK)
            {
                return;
            }
            ObjectId[] ids = proSelRes.Value.GetObjectIds();
            using (Transaction trans = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction())
            {
                List<Curve> curves = new List<Curve>();
                foreach (ObjectId id in ids)
                {
                    Entity ent = trans.GetObject(id, OpenMode.ForRead) as Entity;
                    if (TongJiCode.GetCode(ent) == TjCode.Road)
                    {
                        curves.Add(ent as Curve);
                    }
                }
                DrawingManager dm = new DrawingManager(curves);
                dm.TestDrawing();
                trans.Commit();
            }

            // 以下不知为何出eNotOpenForWrite
            //var centers = QuickSelection.SelectAll("*LINE,ARC").Where(x => TongJiCode.GetCode(x) == TjCode.Road);
            //List<Curve> centerCurves = new List<Curve>();
            //foreach (var center in centers)
            //{
            //    centerCurves.Add(center.QOpenForRead() as Curve);
            //}
            //DrawingManager dm = new DrawingManager(centerCurves);
            //dm.TestDrawing();
        }

        [Obsolete]
        /// <summary>
        /// 清空Generated层上的图元
        /// </summary>
        public static void ClearGenerated()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptSelectionResult proSelRes = ed.SelectAll(new SelectionFilter(new TypedValue[] { new TypedValue(8, AppName.Generated) }));
            if (proSelRes.Status != PromptStatus.OK)
            {
                return;
            }
            ObjectId[] ids = proSelRes.Value.GetObjectIds();
            using (Transaction trans = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction())
            {
                foreach (var id in ids)
                {
                    var ent = trans.GetObject(id, OpenMode.ForWrite);
                    ent.Erase();
                }
                trans.Commit();
            }
        }

        [CommandMethod("cleangenerated")]
        /// <summary>
        /// 清理具有Generated标记的图元
        /// </summary>
        public static void EraseGenerated()
        {
            var readyToErase = QuickSelection.SelectAll("*LINE,ARC").Where(x => CADBase.GetFirstXData(x, AppName.Generated) == "true").ToList();
            readyToErase.ForEach(x => x.Erase());
        }

        /// <summary>
        /// 获取曲线子集
        /// </summary>
        /// <param name="curve">曲线</param>
        /// <param name="interval">曲线子集的长度区间</param>
        /// <returns>曲线子集</returns>
        public static Curve GetSubCurve(Curve curve, RoadInterval interval)
        {
            double start = curve.GetParamAtDist(interval.start);
            double end = curve.GetParamAtDist(interval.end);
            DBObjectCollection splits = curve.GetSplitCurves(new DoubleCollection(new double[] { start, end }));
            if (splits.Count == 1 || interval.start == 0)
            {
                return splits[0] as Curve;
            }
            else
            {
                return splits[1] as Curve;
            }
        }

        /// <summary>
        /// 求父区间与若干子集并集的差集
        /// </summary>
        /// <param name="total">父集</param>
        /// <param name="source">子集集合</param>
        /// <returns>差集集合</returns>
        public static List<RoadInterval> InverseInterval(RoadInterval total, List<RoadInterval> source)
        {
            List<RoadInterval> result = new List<RoadInterval>();
            List<RoadInterval> sortedSource = source.OrderBy(x => x.start).ToList();
            if (source.Count == 0)
            {
                result.Add(total);
                return result;
            }
            if (total.start < sortedSource.First().start)
            {
                result.Add(new RoadInterval { start = total.start, end = sortedSource[0].start });
            }
            for (int i = 0; i < sortedSource.Count - 1; i++)
            {
                if (sortedSource[i].end < sortedSource[i + 1].start)
                {
                    result.Add(new RoadInterval { start = sortedSource[i].end, end = sortedSource[i + 1].start });
                }
            }
            if (total.end > sortedSource.Last().end)
            {
                result.Add(new RoadInterval { start = sortedSource.Last().end, end = total.end });
            }
            return result;
        }

        /// <summary>
        /// 加载其他程序集
        /// </summary>
        /// <param name="dllName">程序集文件名（相对路径）</param>
        public static void LoadDll(string dllName)
        {
            System.Reflection.Assembly.LoadFrom(FolderPath + dllName);
        }

        /// <summary>
        /// 加载其他DWG作为块定义
        /// </summary>
        /// <param name="dwgName">DWG文件名（相对路径）</param>
        public static void LoadDwg(string dwgName)
        {
            Database sourceDb = new Database(false, false);
            sourceDb.ReadDwgFile(FolderPath + dwgName, FileOpenMode.OpenForReadAndAllShare, true, "");
            HostApplicationServices.WorkingDatabase.Insert("tjHidden", sourceDb, false);
        }

        public static void CreateHidden()
        {
            if (QuickSelection.SelectAll("INSERT").QCount(x => (x as BlockReference).Name == Consts.Hidden) == 0)
            {
                Services.LoadDwg("Res.dwg");
                ObjectId id = Draw.Insert(Consts.Hidden, Point3d.Origin);
            }

            // 创建默认打断样式
            CustomDictionary.SetValue(DictName.GlobalStyles, TjCode.SeperationBreak, "5");
            CustomDictionary.SetValue(DictName.GlobalStyles, TjCode.BusStop, "30|5|5|5");
        }

        public static string FolderPath
        {
            get
            {
                string s = System.Reflection.Assembly.GetCallingAssembly().Location;
                return s.Remove(s.LastIndexOf('\\') + 1);
            }
        }
    }

    /// <summary>
    /// 二元组
    /// </summary>
    /// <typeparam name="T1">元素1类型</typeparam>
    /// <typeparam name="T2">元素2类型</typeparam>
    public class Tuple<T1, T2>
    {
        private T1 _item1;
        private T2 _item2;

        public T1 Item1 { get { return _item1; } }
        public T2 Item2 { get { return _item2; } }

        public Tuple(T1 item1, T2 item2)
        {
            _item1 = item1;
            _item2 = item2;
        }
    }

    /// <summary>
    /// 二元组集合
    /// </summary>
    /// <typeparam name="T1">元素1类型</typeparam>
    /// <typeparam name="T2">元素2类型</typeparam>
    public class TupleList<T1, T2> : List<Tuple<T1, T2>>
    {
        public void Add(T1 item1, T2 item2)
        {
            base.Add(new Tuple<T1, T2>(item1, item2));
        }
    }
}
