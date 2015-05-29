using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

using AutoCADCommands;

namespace TongJi.Drawing
{
    public class ActionManager
    {
        public ILookup<string, ActionConfigEntry> Entries { get; private set; }
        public IList<string> Groups { get; private set; }

        public ActionManager()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            string path = App.CurrentFolder + "\\Resources\\ActionConfig.csv";
            var actions = typeof(ActionManager).GetMethods().Where(m => m.IsPublic).Select(m => m.Name).ToList();
            var actionSet = new HashSet<string>(actions);
            Entries = System.IO.File.ReadAllLines(path, Encoding.Default)
                .Where(line => !string.IsNullOrEmpty(line))
                .Select(line =>
                {
                    var values = line.Split(',');
                    return new ActionConfigEntry
                    {
                        ID = values[0],
                        Name = values[1],
                        Description = values[2],
                        Group = values[3],
                        Command = values[4],
                        IconPath = App.CurrentFolder + "/Resources/Icons/" + values[5],
                        IsEnabled = actionSet.Contains(values[0])
                    };
                }).ToLookup(entry => entry.Group, entry => entry);
            Groups = Entries.Select(entry => entry.Key).Distinct().ToList();
        }
    }

    public class ActionConfigEntry
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
        public string Command { get; set; }
        public string IconPath { get; set; } // format: "Resources/Icons/{ID}.png"
        public bool IsEnabled { get; set; } // newly 20140530
    }

    public static class Actions
    {
        #region 测量 [1-6]

        [CommandMethod("CmSjd")]
        public static void A01() // 三角点
        {
            A01_02_03("1", "4");
        }

        [CommandMethod("CmSzd")]
        public static void A02() // 水准点
        {
            A01_02_03("2", "5");
        }

        [CommandMethod("CmGpsKzd")]
        public static void A03() // GPS控制点
        {
            A01_02_03("3", "6");
        }

        private static void A01_02_03(string entityID, string textID)
        {
            var name = Interaction.GetString("\n点名");
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            var elevation = Interaction.GetValue("\n高程");
            if (double.IsNaN(elevation))
            {
                return;
            }
            var id = EntityManager.AddBlock(entityID)().SetStyles(entityID);
            if (id.IsNull)
            {
                return;
            }
            id.SetFirstXData(Consts.AppNameForID, name);
            var pos = id.QOpenForRead<BlockReference>().Position;
            var scale = EntityManager.GetGlobalScaleFactor();
            EntityManager.AddLine(new List<Point3d> { pos + new Vector3d(2.5 * scale, 0, 0), pos + new Vector3d(10 * scale, 0, 0) })().SetStyles(entityID);
            EntityManager.AddText(textID, name, pos + new Vector3d(3 * scale, 0.5 * scale, 0))().SetStyles(textID);
            EntityManager.AddText(textID, elevation.ToString("0.00"), pos + new Vector3d(3 * scale, -2.1 * scale, 0))().SetStyles(textID);
        }

        #endregion

        #region 河流 [7-11]

        [CommandMethod("CmDmhl")]
        public static void B01() // 地面河流
        {
            var entityID = "7";
            EntityManager.AddLine().SetStyles(entityID);
        }

        [CommandMethod("CmHlzj")]
        public static void B02() // 河流注记
        {
            string[] options = { "长江口", "长江", "黄浦江", "其他河流"};
            string[] entityIDs = { "8", "9", "10", "11", "11" };
            var opt = Gui.GetOption("\n选择类型", options);
            if (opt < 0)
            {
                return;
            }
            var entityID = entityIDs[opt];
            var text = opt == 3 ? null : options[opt];
            EntityManager.AddText(entityID, text, null, true)().SetStyles(entityID); // done: text path 20140709
        }

        #endregion

        #region 沟渠 [12-20, 31, 284]
        [CommandMethod("CmGanquyibili")]
        public static void C00() // 干渠依比例
        {
            var entityID = "284";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmGanqu")]
        public static void C01() // 干渠
        {
            var entityID = "12";
            EntityManager.AddLine().SetStyles(entityID);
        }

        [CommandMethod("CmZhiqu")]
        public static void C02() // 支渠
        {
            var entityID = "13";
            EntityManager.AddLine().SetStyles(entityID);
        }

        [CommandMethod("CmHandong")]
        public static void C03() // 涵洞
        {
            var entityID = "14";
            var cvId = Interaction.GetEntity("\n选择线路"); // mod 20140704
            if (cvId.IsNull)
            {
                return;
            }
            var pos = Interaction.GetPoint("\n指定基点");
            if (pos.IsNull())
            {
                return;
            }
            var cv = cvId.QOpenForRead<Curve>();
            if (cv == null)
            {
                return;
            }
            var param = cv.GetParamAtPointX(pos);
            var dir = cv.GetFirstDerivative(param);
            var angle = dir.ToVector2d().DirAngleZeroTo2Pi();
            var id = EntityManager.AddBlock(entityID, pos, angle)().SetStyles(entityID);
            if (id.IsNull)
            {
                return;
            }
            var opt = Gui.GetOption("确认角度", "现状", "反向");
            if (opt == 1)
            {
                id.QOpenForWrite<BlockReference>(br =>
                {
                    angle = (angle + Math.PI) % (Math.PI * 2);
                    br.Rotation = angle;
                });
            }
        }

        [CommandMethod("CmSxGangou")]
        public static void C04() // 双线干沟
        {
            var entityID = "15";
            var entityID1 = "16";
            Interaction.WriteLine("请绘制双线干沟的左侧线。"); // mod 20140709
            var id = EntityManager.AddLine().SetStyles(entityID);
            if (id.IsNull)
            {
                return;
            }
            var width = Interaction.GetValue("\n宽度");
            if (double.IsNaN(width))
            {
                return;
            }
            //var pt = Interaction.GetPoint("\n方向");
            //if (pt.IsNull())
            //{
            //    return;
            //}
            var cv = id.QOpenForRead<Curve>();
            var cv1 = cv.GetOffsetCurves(width);
            //var cv2 = cv.GetOffsetCurves(-width);
            //var dist1 = cv1.Cast<Curve>().Min(x => x.GetDistToPoint(pt));
            //var dist2 = cv2.Cast<Curve>().Min(x => x.GetDistToPoint(pt));
            var cvs = cv1; //dist1 < dist2 ? cv1 : cv2;
            var ids = cvs.Cast<Entity>().AddToCurrentSpace();
            ids.SetStyles(entityID1);
        }

        [CommandMethod("CmDxGangou")]
        public static void C05() // 单线干沟
        {
            var entityID = "17";
            EntityManager.AddLine().SetStyles(entityID);
        }

        [CommandMethod("CmQushuikou")]
        public static void C06() // 取水口
        {
            var entityID = "18";
            EntityManager.AddLine().SetStyles(entityID);
        }

        [CommandMethod("CmGqMcZj")]
        public static void C07() // 干渠名称注记
        {
            var entityID = "19";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }

        [CommandMethod("CmGqXzZj")]
        public static void C08() // 干渠性质注记
        {
            var entityID = "20";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmCsg")]
        public static void CmCsg() // 潮水沟
        {
            var entityID = "31";
            EntityManager.AddLine().SetStyles(entityID);
        }
        #endregion

        #region 水域 [21-26]
        [CommandMethod("CmHupo")]
        public static void CmHupo() // 湖泊
        {
            var entityID = "21";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmChitang")]
        public static void CmChitang() // 池塘
        {
            var entityID = "22";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmHpCtMczj")]
        public static void CmHpCtMczj() // 湖泊、池塘名称注记
        {
            var entityID = "23";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmHpCtXzzj")]
        public static void CmHpCtXzzj() // 湖泊、池塘性质注记
        {
            var entityID = "24";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmShuiku")]
        public static void CmShuiku() // 水库
        {
            var entityID = "25";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmSkMczj")]
        public static void CmSkMczj() // 水库名称注记
        {
            var entityID = "26";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        #endregion

        #region 滩地 [27-30]
        [CommandMethod("CmShatan")]
        public static void E01() // 沙滩
        {
            var entityID = "27";
            EntityManager.AddArea("CmST").SetStyles(entityID);
        }

        [CommandMethod("CmShalitan")]
        public static void E02() // 砂砾滩
        {
            var entityID = "28";
            var ids = EntityManager.AddArea("CmST").SetStyles(entityID);
            if (ids.Count() > 0)
            {
                EntityManager.AddFill(ids.First().QOpenForRead<Polyline>(), "250402", 9).SetStyles(entityID);
            }
        }

        [CommandMethod("CmYunitan")]
        public static void E03() // 淤泥滩
        {
            var entityID = "29";
            var ids = EntityManager.AddArea("CmYNT").SetStyles(entityID);
        }

        [CommandMethod("CmShanitan")]
        public static void E04() // 沙泥滩
        {
            var entityID = "30";
            var ids = EntityManager.AddArea("CmST", "CmYNT").SetStyles(entityID);
        }

        #endregion

        #region 岛礁 [32-43]
        private static void CongJiao(string entityID)
        {
            var pt = Interaction.GetPoint("指定插入点：");
            Point3d pt1 = new Point3d(pt.X - 1.855, pt.Y + 0.5327, pt.Z);
            Point3d pt2 = new Point3d(pt.X + 1.855, pt.Y, pt.Z);
            Point3d pt3 = new Point3d(pt.X, pt.Y - 1.8619, pt.Z);
            EntityManager.AddBlock(entityID, pt1)().SetStyles(entityID);
            EntityManager.AddBlock(entityID, pt2)().SetStyles(entityID);
            EntityManager.AddBlock(entityID, pt3)().SetStyles(entityID);
        }
        [CommandMethod("CmMingjiao")]
        public static void CmMingjiao() // 明礁
        {
            var entityID = "32";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmMingjiaoCong")]
        public static void CmMingjiaoCong() // 明礁(丛)
        {
            CongJiao("32");
        }
        [CommandMethod("CmAnjiao")]
        public static void CmAnjiao() // 暗礁
        {
            var entityID = "33";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmAnjiaoCong")]
        public static void CmAnjiaoCong() // 暗礁(丛)
        {
            CongJiao("33");
        }
        [CommandMethod("CmGanChuJiao")]
        public static void CmGanChuJiao() // 干出礁
        {
            var entityID = "34";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmGanchujiaoCong")]
        public static void CmGanchujiaoCong() // 干出礁(丛)
        {
            CongJiao("34");
        }
        [CommandMethod("CmJsMczj")]
        public static void CmJsMczj() // 礁石名称注记
        {
            var entityID = "35";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmJsXzzj")]
        public static void CmJsXzzj() // 礁石性质注记
        {
            var entityID = "36";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmHaidao")]
        public static void CmHaidao() // 海岛
        {
            var entityID = "37";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmHdMczj")]
        public static void CmHdMczj() // 海岛名称注记
        {
            var entityID = "38";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmHyHwMczj")]
        public static void CmHyHwMczj() // 海洋海湾名称注记
        {
            var entityID = "39";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmGqlxDx")]
        public static void CmGqlxDx() // 沟渠流向(单向)
        {
            var entityID = "40";
            EntityManager.AddBlock(entityID, null, 0, true)().SetStyles(entityID);
        }
        [CommandMethod("CmGqlxWf")]
        public static void CmGqlxWf() // 沟渠流向(往复)
        {
            var entityID = "41";
            EntityManager.AddBlock(entityID, null, 0, true)().SetStyles(entityID);
        }
        [CommandMethod("CmZcl")]
        public static void CmZcl() // 涨潮流
        {
            var entityID = "42";
            EntityManager.AddBlock(entityID, null, 0, true)().SetStyles(entityID);
        }
        [CommandMethod("CmLcl")]
        public static void CmLcl() // 落潮流
        {
            var entityID = "43";
            EntityManager.AddBlock(entityID, null, 0, true)().SetStyles(entityID);
        }
        #endregion

        #region 大堤 [44-71]

        private static void G01To07(string d, string dg, string s, string w, bool bG08 = false)  // bG08 = true表示用于G08命令
        {
            ObjectId id1, id2;

            // 生成
            string[] options = { "绘制中线", "选择中线", "绘制边线", "选择边线" };
            var opt = Gui.GetOption("生成方式", options);
            if (opt == 0)
            {
                var pts = EntityManager.PromptPointString();
                if (pts.Count < 2)
                {
                    return;
                }
                var align1 = NoDraw.Pline(pts);
                var align2 = align1.Clone() as Polyline;
                align2.ReverseCurve();
                id1 = (align1.GetOffsetCurves(0.25)[0] as Entity).AddToCurrentSpace();
                id2 = (align2.GetOffsetCurves(0.25)[0] as Entity).AddToCurrentSpace();
            }
            else if (opt == 1)
            {
                var id = Interaction.GetEntity("\n选择线", typeof(Curve), false);
                if (id.IsNull)
                {
                    return;
                }
                var align1 = id.QOpenForRead<Curve>();
                var align2 = align1.Clone() as Curve;
                align2.ReverseCurve();
                var offset = align1 is Line ? -0.25 : 0.25;
                id1 = (align1.GetOffsetCurves(offset)[0] as Entity).AddToCurrentSpace();
                id2 = (align2.GetOffsetCurves(offset)[0] as Entity).AddToCurrentSpace();
            }
            else if (opt == 2)
            {
                var pts = EntityManager.PromptPointString();
                var id = Draw.Pline(pts);
                var pt = Interaction.GetPoint("\n另一侧方向");
                id.QOpenForWrite(x => x.Erase());
                if (pt.IsNull())
                {
                    return;
                }
                var side1 = NoDraw.Pline(pts);
                var side2 = side1.Clone() as Polyline;
                side2.ReverseCurve();
                var cv1 = side1.GetOffsetCurves(0.5)[0] as Curve;
                var cv2 = side2.GetOffsetCurves(0.5)[0] as Curve;
                var dist1 = cv1.GetDistToPoint(pt);
                var dist2 = cv2.GetDistToPoint(pt);
                if (dist1 < dist2)
                {
                    id1 = side2.AddToCurrentSpace();
                    id2 = cv1.AddToCurrentSpace();
                }
                else
                {
                    id1 = side1.AddToCurrentSpace();
                    id2 = cv2.AddToCurrentSpace();
                }
            }
            else if (opt == 3)
            {
                var id = Interaction.GetEntity("\n选择线", typeof(Curve), false);
                if (id.IsNull)
                {
                    return;
                }
                var pt = Interaction.GetPoint("\n另一侧方向");
                if (pt.IsNull())
                {
                    return;
                }
                var side1 = id.QOpenForRead<Curve>().Clone() as Curve;
                var side2 = side1.Clone() as Curve;
                side2.ReverseCurve();
                var offset = side1 is Line ? -0.5 : 0.5;
                var cv1 = side1.GetOffsetCurves(offset)[0] as Curve;
                var cv2 = side2.GetOffsetCurves(offset)[0] as Curve;
                var dist1 = cv1.GetDistToPoint(pt);
                var dist2 = cv2.GetDistToPoint(pt);
                if (dist1 < dist2)
                {
                    id1 = side2.AddToCurrentSpace();
                    id2 = cv1.AddToCurrentSpace();
                }
                else
                {
                    id1 = side1.AddToCurrentSpace();
                    id2 = cv2.AddToCurrentSpace();
                }
            }
            else
            {
                return;
            }

            // 加固
            string[] options1 = { "双侧加固", "单侧加固", "不加固" };
            string[] options2 = { "双侧加固", "不加固" };
            int opt1 = 0;
            // var opt1 = Gui.GetOption("加固方式", options1);
            if (!bG08)
            {
                opt1 = Gui.GetOption("加固方式", options1);
            }
            else
            {
                opt1 = Gui.GetOption("加固方式", options2);
                if (opt1 == 1) opt1 = 2;    // 此处1对应不加固，为和上面统一，所以要切换成2
            }
            if (opt1 == 0)
            {
                id1.SetStyles(s);
                id2.SetStyles(s);
            }
            else if (opt1 == 1)
            {
                var id = Interaction.GetEntity("\n选择加固侧", typeof(Curve), false);
                if (id.IsNull)
                {
                    return;
                }
                if (id == id1)
                {
                    id1.SetStyles(dg);
                    id2.QOpenForWrite<Curve>(cv => cv.ReverseCurve());
                    id2.SetStyles(d);
                }
                else if (id == id2)
                {
                    id1.QOpenForWrite<Curve>(cv => cv.ReverseCurve());
                    id1.SetStyles(d);
                    id2.SetStyles(dg);
                }
            }
            else if (opt1 == 2)
            {
                id1.QOpenForWrite<Curve>(cv => cv.ReverseCurve());
                id2.QOpenForWrite<Curve>(cv => cv.ReverseCurve());
                id1.SetStyles(w);
                id2.SetStyles(w);
            }
            else
            {
                return;
            }
        }

        [CommandMethod("CmDadi")]
        public static void G01() // 大堤
        {
            G01To07("44", "45", "46", "47");
        }

        [CommandMethod("CmDadi98")]
        public static void G02() // 98大堤
        {
            G01To07("48", "49", "50", "51");
        }

        [CommandMethod("CmDadi03")]
        public static void G03() // 03大堤
        {
            G01To07("52", "53", "54", "55");
        }

        [CommandMethod("CmDadi08")]
        public static void G04() // 08大堤
        {
            G01To07("56", "57", "58", "59");
        }

        [CommandMethod("CmDadi13")]
        public static void G05() // 13大堤
        {
            G01To07("60", "61", "62", "63");
        }

        [CommandMethod("CmDadi18")]
        public static void G06() // 18大堤
        {
            G01To07("64", "65", "66", "67");
        }

        [CommandMethod("CmDadi23")]
        public static void G07() // 23大堤
        {
            G01To07("68", "69", "70", "71");
        }
        [CommandMethod("CmZhubeidi")]
        public static void G08() // 主备堤
        {
            G01To07("72", "73", "72", "73", true);
        }
        [CommandMethod("CmCibeidi")]
        public static void G10() // 次备堤
        {
            var entityID = "74";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmWeidijiagu")]
        public static void G11() // 圩堤加固
        {
            var entityID = "75";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmWeidiweijiagu")]
        public static void G12() // 圩堤未加固
        {
            var entityID = "76";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmXiaoweidi")]
        public static void G13() // 一般小圩堤
        {
            var entityID = "77";
            EntityManager.AddLine().SetStyles(entityID);
        }
        #endregion

        #region 水闸、坝、管桩、防浪墙 [78-88]
        [CommandMethod("CmTongcheshuizha")]
        public static void CmTongcheshuizha() // 通车水闸
        {
            var entityID = "78";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmButongcheshuizha")]
        public static void CmButongcheshuizha() // 不通车水闸
        {
            var entityID = "79";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmSzMczj")]
        public static void CmSzMczj() // 水闸名称注记
        {
            var entityID = "80";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmDinggouba")]
        public static void CmDinggouba() // 丁勾坝
        {
            var entityID = "81";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmDgbMczj")]
        public static void CmDgbMczj() // 丁勾坝名称注记
        {
            var entityID = "82";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmShunba")]
        public static void CmShunba() // 顺坝
        {
            var entityID = "83";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmSbMczj")]
        public static void CmSbMczj() // 顺坝名称注记
        {
            var entityID = "84";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmGuanzhuang")]
        public static void CmGuanzhuang() // 管桩
        {
            var entityID = "85";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmGzMczj")]
        public static void CmGzMczj() // 管桩名称注记
        {
            var entityID = "86";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmJiaguanF")]
        public static void CmJiaguanF() // 加固岸，有防浪墙
        {
            var entityID = "87";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmJiaguan")]
        public static void CmJiaguan() // 加固岸，无防浪墙
        {
            var entityID = "88";
            EntityManager.AddLine().SetStyles(entityID);
        }
        #endregion

        #region 房屋 [89-95]

        private static void CmFangwu(string entityID, string hatchPattern)
        {
            EntityManager.AddArea(hatchPattern).SetStyles(entityID);
        }

        [CommandMethod("CmPutongFangwu")]
        public static void CmPutongFangwu() // 普通房屋
        {
            CmFangwu("89", "SOLID");
        }

        [CommandMethod("CmTuchuFangwu")]
        public static void CmTuchuFangwu() // 高层或突出房屋
        {
            CmFangwu("90", "SOLID");
        }

        [CommandMethod("CmGaocengFangwu")]
        public static void CmGaocengFangwu() // 高层房屋
        {
            CmFangwu("91", "CmGCFW");
        }

        [CommandMethod("CmPengfang")]
        public static void CmPengfang() // 棚房
        {
            var entityID = "92";
            var pts = EntityManager.PromptPointString(true);
            if (pts.Count < 3)
            {
                return;
            }
            var id = EntityManager.AddLine(pts)().SetStyles(entityID);
            EntityManager.AddTextFill(id.QOpenForRead<Polyline>(), "棚", 10);//.SetStyles(entityID);
        }

        [CommandMethod("CmPohuaiFangwu")]
        public static void CmPohuaiFangwu() // 破坏房屋
        {
            CmFangwu("93", "CmPHFW");
        }

        [CommandMethod("CmJmdMingchengZhuji")]
        public static void CmJmdMingchengZhuji() // 居民地名称注记
        {
            var entityID = "94";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }

        [CommandMethod("CmJmdXingzhiZhuji")]
        public static void CmJmdXingzhiZhuji() // 居民地性质注记
        {
            var entityID = "95";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }

        #endregion

        #region 工矿设施 [96-107]
        [CommandMethod("CmShuichang")]
        public static void CmShuichang() // 水厂
        {
            var entityID = "96";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmWscZj")]
        public static void CmWscZj() // 污水厂注记
        {
            var entityID = "97";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmYqZcsb")]
        public static void CmYqZcsb() // 液气贮存设备
        {
            var entityID = "98";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmYqZcsbFwx")]
        public static void CmYqZcsbFwx() // 液气贮存设备范围线
        {
            var entityID = "99";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmGyt")]
        public static void CmGyt() // 工业塔类建筑
        {
            var entityID = "100";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmShuita")]
        public static void CmShuita() // 水塔
        {
            var entityID = "101";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmShuitaYc")]
        public static void CmShuitaYc() // 水塔烟囱
        {
            var entityID = "102";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmYancong")]
        public static void CmYancong() // 烟囱
        {
            var entityID = "103";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmLtsb")]
        public static void CmLtsb() // 露天设备
        {
            var entityID = "104";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmLtsbFyx")]
        public static void CmLtsbFyx() // 露天设备范围线
        {
            var entityID = "105";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmGkssMczj")]
        public static void CmGkssMczj() // 工况设施名称注记
        {
            var entityID = "106";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmGkssXzzj")]
        public static void CmGkssXzzj() // 工况设施性质注记
        {
            var entityID = "107";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        #endregion

        #region 农业设施 [108-119]
        //不依比例饲养场
        //依比例饲养场边线
        //依比例饲养场范围线
        //不依比例水产养殖场
        //依比例水产养殖场
        //粮库符号
        //粮库边线
        //粮库范围线
        //水磨房、水车
        //风磨房、风车
        //农业及其设施名称注记
        //农业及其设施性质注记
        [CommandMethod("CmByblSyc")]
        public static void CmByblSyc() // 不依比例饲养场
        {
            var entityID = "108";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmYblSycBx")]
        public static void CmYblSycBx() // 依比例饲养场边线
        {
            var entityID = "109";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmYblSycFyx")]
        public static void CmYblSycFyx() // 依比例饲养场范围线
        {
            var entityID = "110";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmByblScyzc")]
        public static void CmByblScyzc() // 不依比例水产养殖场
        {
            var entityID = "111";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmYblScyzc")]
        public static void CmYblScyzc() // 依比例水产养殖场
        {
            var entityID = "112";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmLkfh")]
        public static void CmLkfh() // 粮库符号
        {
            var entityID = "113";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmLkbx")]
        public static void CmLkbx() // 粮库边线
        {
            var entityID = "114";
            var SymbolID = "113";
            var id = EntityManager.AddLine().SetStyles(entityID);
            EntityManager.AddBlock(SymbolID, id.GetCenter())().SetStyles(SymbolID);
        }
        [CommandMethod("CmLkFyx")]
        public static void CmLkFyx() // 粮库范围线
        {
            var entityID = "115";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmSmfSc")]
        public static void CmSmfSc() // 水磨房、水车
        {
            var entityID = "116";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmFmfFc")]
        public static void CmFmfFc() // 风磨房、风车
        {
            var entityID = "117";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmNyssMczj")]
        public static void CmNyssMczj() // 农业设施名称注记
        {
            var entityID = "118";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmNyssXzzj")]
        public static void CmNyssXzzj() // 农业设施性质注记
        {
            var entityID = "119";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmScyzcXzzj")]
        public static void CmScyzcXzzj() // 水产养殖场性质注记
        {
            var entityID = "285";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        #endregion

        #region 公共服务设施、名胜古迹、科学观测站 [120-135]
        //电视发射塔
        //移动通信塔
        //微波传送塔
        //公共服务及其设施名称注记
        //公共服务及其设施性质注记
        //纪念碑、柱、墩
        //牌楼、牌坊
        //亭
        //文物碑石
        //碉堡、地堡
        //名胜古迹注记
        //水文站、验潮站
        //环保监测站
        //卫星地面站
        //科学试验站
        //科学观测站注记
        [CommandMethod("CmDsFst")]
        public static void CmDsFst() // 电视发送塔
        {
            var entityID = "120";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmYdTxt")]
        public static void CmYdTxt() // 移动通信塔
        {
            var entityID = "121";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmWbCst")]
        public static void CmWbCst() // 微波传送塔
        {
            var entityID = "122";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmGgfwssMczj")]
        public static void CmGgfwssMczj() // 公共服务设施名称注记
        {
            var entityID = "123";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmGgfwssXzzj")]
        public static void CmGgfwssXzzj() // 公共服务设施性质注记
        {
            var entityID = "124";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmJnbzd")]
        public static void CmJnbzd() // 纪念碑、柱、墩
        {
            var entityID = "125";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmPlPf")]
        public static void CmPlPf() // 牌楼、牌坊
        {
            var entityID = "126";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmTing")]
        public static void CmTing() // 亭
        {
            var entityID = "127";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmWwbs")]
        public static void CmWwbs() // 文物碑石
        {
            var entityID = "128";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmDbdb")]
        public static void CmDbdb() // 碉堡地堡
        {
            var entityID = "129";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmMsgjZj")]
        public static void CmMsgjZj() // 名胜古迹注记
        {
            var entityID = "130";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmSwzYcz")]
        public static void CmSwzYcz() // 水文站、验潮站
        {
            var entityID = "131";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmHbJcz")]
        public static void CmHbJcz() // 环保监测站
        {
            var entityID = "132";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmWxDmz")]
        public static void CmWxDmz() // 卫星地面站
        {
            var entityID = "133";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmKxSyz")]
        public static void CmKxSyz() // 科学试验站
        {
            var entityID = "134";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmKxgcZj")]
        public static void CmKxgcZj() // 科学观测注记
        {
            var entityID = "135";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }

        #endregion

        #region 其他建筑物 [136-145]

        [CommandMethod("CmWeiqiang")]
        public static void CmWeiqiang() // 围墙
        {
            var entityID = "136";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmZhalan")]
        public static void CmZhalan() // 栅栏
        {
            var entityID = "137";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmLiba")]
        public static void CmLiba() // 篱笆
        {
            var entityID = "138";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmTswDw")]
        public static void CmTswDw() // 铁丝网、电网
        {
            var entityID = "139";
            EntityManager.AddLine().SetStyles(entityID);
        }

        [CommandMethod("CmYblDxjzCrk")]
        public static void CmYblDxjzCrk() // 依比例地下建筑出入口
        {
            var entityID = "140";
            EntityManager.AddLine().SetStyles(entityID);
            CmYblDxjzCrkFh();
        }
        [CommandMethod("CmYblDxjzCrkFh")]
        public static void CmYblDxjzCrkFh() // 依比例地下建筑出入口符号
        {
            var entityID = "141";
            EntityManager.AddBlock(entityID, null, 0, true)().SetStyles(entityID);
        }
        [CommandMethod("CmByblDxjzCrk")]
        public static void CmByblDxjzCrk() // 不依比例地下建筑出入口
        {
            var entityID = "142";
            EntityManager.AddBlock(entityID, null, 0, true)().SetStyles(entityID);
        }

        [CommandMethod("CmYblTaijie")]
        public static void CmYblTaijie() // 依比例台阶
        {
            var entityID = "143";
            var pt1 = Interaction.GetPoint("\n指定第一点");
            if (pt1.IsNull())
            {
                return;
            }
            var pt2 = Interaction.GetLineEndPoint("\n指定第二点", pt1);
            if (pt2.IsNull())
            {
                return;
            }
            var id = Draw.Line(pt1, pt2);
            var pt3 = Interaction.GetPoint("\n指定第三点");
            if (pt3.IsNull())
            {
                return;
            }
            var step = 1.0;
            var ids = SymbolPack.Stairs(pt1, pt2, pt3, step);
            ids.SetStyles(entityID);
            id.QOpenForWrite(x => x.Erase());
        }

        [CommandMethod("CmByblTaijie")]
        public static void CmByblTaijie() // 不依比例台阶
        {
            var entityID = "144";
            EntityManager.AddBlock(entityID, null, 0, true)().SetStyles(entityID);
        }
        [CommandMethod("CmQtjzwZj")]
        public static void CmQtjzwZj() // 其他建筑物及设施注记
        {
            var entityID = "145";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }

        #endregion

        #region 道路交通设施 [146-182]
        private static void Daolu(string d, double width)
        {
            ObjectId id1, id2;

            // 生成
            string[] options = { "绘制中线", "选择中线" };
            var opt = Gui.GetOption("生成方式", options);
            if (opt == 0)
            {
                var pts = EntityManager.PromptPointString();
                if (pts.Count < 2)
                {
                    return;
                }
                var align1 = NoDraw.Pline(pts);
                var align2 = align1.Clone() as Polyline;
                align2.ReverseCurve();
                id1 = (align1.GetOffsetCurves(width / 2.0)[0] as Entity).AddToCurrentSpace();
                id2 = (align2.GetOffsetCurves(width / 2.0)[0] as Entity).AddToCurrentSpace();
            }
            else if (opt == 1)
            {
                var id = Interaction.GetEntity("\n选择线", typeof(Curve), false);
                if (id.IsNull)
                {
                    return;
                }
                var align1 = id.QOpenForRead<Curve>();
                var align2 = align1.Clone() as Curve;
                align2.ReverseCurve();
                id1 = (align1.GetOffsetCurves(width / 2.0)[0] as Entity).AddToCurrentSpace();
                id2 = (align2.GetOffsetCurves(width / 2.0)[0] as Entity).AddToCurrentSpace();
            }
            else
            {
                return;
            }
            id1.QOpenForWrite<Curve>(cv => cv.ReverseCurve());
            id2.QOpenForWrite<Curve>(cv => cv.ReverseCurve());
            id1.SetStyles(d);
            id2.SetStyles(d);
        }
        [CommandMethod("CmGuodao")]
        public static void CmGuodao() // 国道
        {
            Daolu("146", 1.2);
        }
        [CommandMethod("CmShengdao")]
        public static void CmShengdao() // 省道
        {
            Daolu("147", 0.9);
        }
        [CommandMethod("CmXiandao")]
        public static void CmXiandao() // 县道
        {
            Daolu("148", 0.9);
        }
        [CommandMethod("CmXiangdao")]
        public static void CmXiangdao() // 乡道
        {
            Daolu("149", 0.9);
        }
        [CommandMethod("CmZygl")]
        public static void CmZygl() // 专用公路
        {
            Daolu("150", 0.9);
        }
        [CommandMethod("CmCjglMczj")]
        public static void CmCjglMczj() // 城际公路名称注记
        {
            var entityID = "151";
            EntityManager.AddText(entityID, null, null, true)().SetStyles(entityID);
        }
        [CommandMethod("CmCjglXzzj")]
        public static void CmCjglXzzj() // 城际公路性质注记
        {
            var entityID = "152";
            EntityManager.AddText(entityID, null, null, true)().SetStyles(entityID);
        }

        [CommandMethod("CmDitie")]
        public static void CmDitie() // 地铁
        {
            var entityID = "153";
            var pts = EntityManager.PromptPointString();
            var poly = NoDraw.Pline(pts);
            SymbolPack.LineBundle(poly, new[] { new LineBundleDefinition { Width = 0, Offset = -0.3 }, new LineBundleDefinition { Width = 0, Offset = 0.3 }, new LineBundleDefinition { Width = 0.6, Offset = 0, DashArray = new[] { 0.033, 0.95, 0.033, 0.95, 0.034, 6.0 } } }).SetStyles(entityID, true);
        }

        [CommandMethod("CmQinggui")]
        public static void CmQinggui() // 轻轨
        {
            var entityID = "154";
            var pts = EntityManager.PromptPointString();
            var poly = NoDraw.Pline(pts);
            SymbolPack.LineBundle(poly, new[] { new LineBundleDefinition { Width = 0, Offset = -0.3 }, new LineBundleDefinition { Width = 0, Offset = 0.3 }, new LineBundleDefinition { Width = 0.6, Offset = 0, DashArray = new[] { 2.0, 6.0 } } }).SetStyles(entityID, true);
        }

        [CommandMethod("CmCsdlMczj")]
        public static void CmCsdlMczj() // 城市道路名称注记
        {
            var entityID = "155";
            EntityManager.AddText(entityID, null, null, true)().SetStyles(entityID);
        }
        [CommandMethod("CmCsdlXzzj")]
        public static void CmCsdlXzzj() // 城市道路性质注记
        {
            var entityID = "156";
            EntityManager.AddText(entityID, null, null, true)().SetStyles(entityID);
        }
        [CommandMethod("CmXcl")]
        public static void CmXcl() // 乡村路
        {
            var entityID = "157";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmXiaolu")]
        public static void CmXiaolu() // 小路
        {
            var entityID = "158";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmXcdlMczj")]
        public static void CmXcdlMczj() // 乡村道路名称注记
        {
            var entityID = "159";
            EntityManager.AddText(entityID, null, null, true)().SetStyles(entityID);
        }
        [CommandMethod("CmXcdlXzzj")]
        public static void CmXcdlXzzj() // 乡村道路性质注记
        {
            var entityID = "160";
            EntityManager.AddText(entityID, null, null, true)().SetStyles(entityID);
        }
        [CommandMethod("CmDtz")]
        public static void CmDtz() // 地铁站
        {
            var entityID = "161";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmQgz")]
        public static void CmQgz() // 轻轨站
        {
            var entityID = "162";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }

        [CommandMethod("CmYblCxq")]
        public static void CmYblCxq() // 依比例车行桥
        {
            var entityID = "163";
            Interaction.WriteLine("请绘制桥的左侧。");
            var id1 = EntityManager.AddLine();
            if (id1.IsNull)
            {
                return;
            }
            var width = Interaction.GetValue("\n宽度");
            if (double.IsNaN(width))
            {
                return;
            }
            var cv1 = id1.QOpenForRead<Curve>();
            var cv2 = cv1.GetOffsetCurves(width)[0] as Curve;
            var id2 = cv2.AddToCurrentSpace();
            var ears1 = SymbolPack.BridgeEarFor(cv1, false, 0.5);
            var ears2 = SymbolPack.BridgeEarFor(cv2, true, 0.5);
            var ids = new List<ObjectId> { id1, id2, ears1.Item1, ears1.Item2, ears2.Item1, ears2.Item2 };
            ids.SetStyles(entityID);
        }

        [CommandMethod("CmByblCxq")]
        public static void CmByblCxq() // 不依比例车行桥
        {
            var entityID = "164";
            EntityManager.AddBlock(entityID, null, 0, true)().SetStyles(entityID);
        }

        [CommandMethod("CmYblRxq")]
        public static void CmYblRxq() // 依比例人行桥
        {
            var entityID = "165";
            var id = EntityManager.AddLine();
            if (id.IsNull)
            {
                return;
            }
            var cv = id.QOpenForRead<Curve>();
            var ears1 = SymbolPack.BridgeEarFor(cv, false, 0.5);
            var ears2 = SymbolPack.BridgeEarFor(cv, true, 0.5);
            var ids = new List<ObjectId> { id, ears1.Item1, ears1.Item2, ears2.Item1, ears2.Item2 };
            ids.SetStyles(entityID);
        }

        [CommandMethod("CmByblRxq")]
        public static void CmByblRxq() // 不依比例人行桥
        {
            var entityID = "166";
            EntityManager.AddBlock(entityID, null, 0, true)().SetStyles(entityID);
        }

        [CommandMethod("CmZhanqiao")]
        public static void CmZhanqiao() // 栈桥边线
        {
            var entityID = "167";
            Interaction.WriteLine("请绘制桥的左侧。");
            var id1 = EntityManager.AddLine();
            if (id1.IsNull)
            {
                return;
            }
            //var width = Interaction.GetValue("\n宽度");
            //if (double.IsNaN(width))
            //{
            //    return;
            //}
            var cv1 = id1.QOpenForRead<Curve>();
            //var cv2 = cv1.GetOffsetCurves(width)[0] as Curve;
            //var id2 = cv2.AddToCurrentSpace();
            var keyword = Interaction.GetKewords("是否绘制桥耳？", new string[] { "Y", "N" }, 1);
            if (keyword == "Y")
            {
                var ears1 = SymbolPack.BridgeEarFor(cv1, false, 0.5);
                //var ears2 = SymbolPack.BridgeEarFor(cv2, true, 0.5);
                var ids = new List<ObjectId> { id1, ears1.Item1, ears1.Item2 };
                ids.SetStyles(entityID);
            }
            else
            {
                var ids = new List<ObjectId> { id1 };
                ids.SetStyles(entityID);
            }
        }

        [CommandMethod("CmQcSdbx")]
        public static void CmQcSdbx() // 汽车隧道连线
        {
            var entityID = "168";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmQcSdcrk")]
        public static void CmQcSdcrk() // 汽车隧道出入口
        {
            var entityID = "169";
            EntityManager.AddBlock(entityID, null, 0, true)().SetStyles(entityID);
        }
        [CommandMethod("CmQcdtSdbx")]
        public static void CmQcdtSdbx() // 汽车地铁隧道连线
        {
            var entityID = "170";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmQcdtSdcrk")]
        public static void CmQcdtSdcrk() // 汽车地铁隧道出入口
        {
            var entityID = "171";
            EntityManager.AddBlock(entityID, null, 0, true)().SetStyles(entityID);
        }
        [CommandMethod("CmGlLcbz")]
        public static void CmGlLcbz() // 公路零公里里程标志
        {
            var entityID = "172";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmLubiao")]
        public static void CmLubiao() // 路标
        {
            var entityID = "173";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmLichengbei")]
        public static void CmLichengbei() // 里程碑
        {
            var entityID = "174";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("Cm98Lichengbei")]
        public static void Cm98Lichengbei() // 98里程碑
        {
            var entityID = "175";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("Cm03Lichengbei")]
        public static void Cm03Lichengbei() // 03里程碑
        {
            var entityID = "176";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("Cm08Lichengbei")]
        public static void Cm08Lichengbei() // 08里程碑
        {
            var entityID = "177";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("Cm13Lichengbei")]
        public static void Cm13Lichengbei() // 13里程碑
        {
            var entityID = "178";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("Cm18Lichengbei")]
        public static void Cm18Lichengbei() // 18里程碑
        {
            var entityID = "179";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("Cm23Lichengbei")]
        public static void Cm23Lichengbei() // 23里程碑
        {
            var entityID = "180";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmDlgzwMczj")]
        public static void CmDlgzwMczj() // 道路构造物及附属设施名称注记
        {
            var entityID = "181";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmDlgzwXzzj")]
        public static void CmDlgzwXzzj() // 道路构造物及附属设施性质注记
        {
            var entityID = "182";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }

        #endregion

        #region 水运及其他交通设施 [183-206]
        [CommandMethod("CmChuanmatou")]
        public static void CmChuanmatou() // 船码头
        {
            var entityID = "183";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmSygKyz")]
        public static void CmSygKyz() // 水运港客运站符号
        {
            var entityID = "184";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmYblGdsaMt")]
        public static void CmYblGdsaMt() // 依比例固定顺岸码头
        {
            var entityID = "185";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmByblGdsaMt")]
        public static void CmByblGdsaMt() // 不依比例固定顺岸码头
        {
            var entityID = "186";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmYblGddbMt")]
        public static void CmYblGddbMt() // 依比例固定堤坝码头
        {
            var entityID = "187";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmByblGddbMt")]
        public static void CmByblGddbMt() // 不依比例固定堤坝码头
        {
            var entityID = "188";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmZqsMt")]
        public static void CmZqsMt() // 栈桥式码头
        {
            var entityID = "189";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmFumatou")]
        public static void CmFumatou() // 浮码头
        {
            var entityID = "190";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmGcy")]
        public static void CmGcy() // 干船坞
        {
            var entityID = "191";
            EntityManager.AddBlock(entityID, null, 0, true)().SetStyles(entityID);
        }
        [CommandMethod("CmTbc")]
        public static void CmTbc() // 停泊场
        {
            var entityID = "192";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmDengta")]
        public static void CmDengta() // 灯塔
        {
            var entityID = "193";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmDengzhuang")]
        public static void CmDengzhuang() // 灯桩
        {
            var entityID = "194";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmDengchuan")]
        public static void CmDengchuan() // 灯船
        {
            var entityID = "195";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmFubiao")]
        public static void CmFubiao() // 浮标
        {
            var entityID = "196";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmAbLb")]
        public static void CmAbLb() // 岸标、立标
        {
            var entityID = "197";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmXinhaogan")]
        public static void CmXinhaogan() // 信号杆
        {
            var entityID = "198";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmJcft")]
        public static void CmJcft() // 系船浮筒
        {
            var entityID = "199";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmLccc")]
        public static void CmLccc() // 露出沉船
        {
            var entityID = "200";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmYmcc")]
        public static void CmYmcc() // 露出沉船
        {
            var entityID = "201";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmSyssMczj")]
        public static void CmSyssMczj() // 水运设施名称注记
        {
            var entityID = "202";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmQcdk")]
        public static void CmQcdk() // 汽车渡口
        {
            var entityID = "203";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmRdk")]
        public static void CmRdk() // 人渡口
        {
            var entityID = "204";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmQtjtssMczj")]
        public static void CmQtjtssMczj() // 其他交通设施名称注记
        {
            var entityID = "205";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmQtjtssXzzj")]
        public static void CmQtjtssXzzj() // 其他交通设施性质注记
        {
            var entityID = "206";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        #endregion

        #region 管线 [207-229]
        [CommandMethod("CmJkGySdx")]
        public static void CmJkGySdx() // 架空高压输电线
        {
            var entityID = "207";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmSdxRdk")]
        public static void CmSdxRdk() // 输电线入地口
        {
            var entityID = "208";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmByzWq")]
        public static void CmByzWq() // 变压站外墙
        {
            var entityID = "209";
            var SymbolID = "210";
            var id = EntityManager.AddLine().SetStyles(entityID);
            EntityManager.AddBlock(SymbolID, id.GetCenter())().SetStyles(SymbolID);
        }
        [CommandMethod("CmByzFh")]
        public static void CmByzFh() // 变压站符号
        {
            var entityID = "210";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmFlfdz")]
        public static void CmFlfdz() // 风力发电站
        {
            var entityID = "211";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmSdxZj")]
        public static void CmSdxZj() // 输电线注记
        {
            var entityID = "212";
            EntityManager.AddText(entityID, null, null, true, false)().SetStyles(entityID);
        }
        [CommandMethod("CmDsTxx")]
        public static void CmDsTxx() // 地上通信线
        {
            var entityID = "213";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmDxTxx")]
        public static void CmDxTxx() // 地下通信线
        {
            var entityID = "214";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmTxxRdk")]
        public static void CmTxxRdk() // 通信线入地口
        {
            var entityID = "215";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmTxxXzzj")]
        public static void CmTxxXzzj() // 通信线性质注记
        {
            var entityID = "216";
            EntityManager.AddText(entityID, null, null, true, false)().SetStyles(entityID);
        }

        [CommandMethod("CmDsYgd")]
        public static void CmDsYgd() // 地上油管道
        {
            var entityID = "217";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmDxYgd")]
        public static void CmDxYgd() // 地下油管道
        {
            var entityID = "218";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmYgdRdk")]
        public static void CmYgdRdk() // 油管道入地口
        {
            var entityID = "219";
            EntityManager.AddBlock(entityID, null, 0, true)().SetStyles(entityID);
        }
        [CommandMethod("CmJkYgd")]
        public static void CmJkYgd() // 架空油管道
        {
            var entityID = "220";
            EntityManager.AddLine().SetStyles(entityID);
        }

        [CommandMethod("CmDsQgd")]
        public static void CmDsQgd() // 地上气管道
        {
            var entityID = "221";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmDxQgd")]
        public static void CmDxQgd() // 地下气管道
        {
            var entityID = "222";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmQgdRdk")]
        public static void CmQgdRdk() // 气管道入地口
        {
            var entityID = "223";
            EntityManager.AddBlock(entityID, null, 0, true)().SetStyles(entityID);
        }
        [CommandMethod("CmJkQgd")]
        public static void CmJkQgd() // 架空气管道
        {
            var entityID = "224";
            EntityManager.AddLine().SetStyles(entityID);
        }


        [CommandMethod("CmDsSgd")]
        public static void CmDsSgd() // 地上水管道
        {
            var entityID = "225";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmDxSgd")]
        public static void CmDxSgd() // 地下水管道
        {
            var entityID = "226";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmSgdRdk")]
        public static void CmSgdRdk() // 水管道入地口
        {
            var entityID = "227";
            EntityManager.AddBlock(entityID, null, 0, true)().SetStyles(entityID);
        }
        [CommandMethod("CmJkSgd")]
        public static void CmJkSgd() // 架空水管道
        {
            var entityID = "228";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmYqsGdzj")]
        public static void CmYqsGdzj() // 油气水管道注记
        {
            var entityID = "229";
            EntityManager.AddText(entityID, null, null, true, false)().SetStyles(entityID);
        }
        #endregion

        #region 行政界线、其他区域界线 [230-249]
        [CommandMethod("CmSjxzqJx")]
        public static void CmSjxzqJx() // 省级行政区界线
        {
            var entityID = "230";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmSjxzqJz")]
        public static void CmSjxzqJz() // 省级行政区界桩
        {
            var entityID = "231";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmSjxzqZj")]
        public static void CmSjxzqZj() // 省级行政区注记
        {
            var entityID = "232";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmDjxzqJx")]
        public static void CmDjxzqJx() // 地级行政区界线
        {
            var entityID = "233";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmDjxzqJz")]
        public static void CmDjxzqJz() // 地级行政区界桩
        {
            var entityID = "234";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmDjxzqZj")]
        public static void CmDjxzqZj() // 地级行政区注记
        {
            var entityID = "235";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmXjxzqJx")]
        public static void CmXjxzqJx() // 县级行政区界线
        {
            var entityID = "236";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmXjxzqJz")]
        public static void CmXjxzqJz() // 县级行政区界桩
        {
            var entityID = "237";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmXjxzqZj")]
        public static void CmXjxzqZj() // 县级行政区注记
        {
            var entityID = "238";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmXzjxzqJx")]
        public static void CmXzjxzqJx() // 乡镇级行政区界线
        {
            var entityID = "239";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmXzjxzqJz")]
        public static void CmXzjxzqJz() // 乡镇级行政区界桩
        {
            var entityID = "240";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmXzjxzqZj")]
        public static void CmXzjxzqZj() // 乡镇级行政区注记
        {
            var entityID = "241";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmZrwhBhqJx")]
        public static void CmZrwhBhqJx() // 自然文化、保护区界线
        {
            var entityID = "242";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmTsdqJx")]
        public static void CmTsdqJx() // 特殊地区界线
        {
            var entityID = "243";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmKfqBsqJx")]
        public static void CmKfqBsqJx() // 开发区、保税区界线
        {
            var entityID = "244";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmGylclcJx")]
        public static void CmGylclcJx() // 国有农场、林场界线
        {
            var entityID = "245";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmDyFwx")]
        public static void CmDyFwx() // 岛屿范围线
        {
            var entityID = "246";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmQtqyZj")]
        public static void CmQtqyZj() // 其他区域注记
        {
            var entityID = "247";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmSgqyFwx")]
        public static void CmSgqyFwx() // 施工区域范围线
        {
            var entityID = "248";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmSgqyZj")]
        public static void CmSgqyZj() // 施工区域注记
        {
            var entityID = "249";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        #endregion

        #region 高程点和等高线 [250-257]

        [CommandMethod("CmGaochengdian")]
        public static void CmGaochengdian() // 高程点
        {
            var entityID = "250";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }

        [CommandMethod("CmGaochengdianZhuji")]
        public static void CmGaochengdianZhuji() // 高程点注记
        {
            var entityID = "251";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }

        [CommandMethod("CmShouquxian")]
        public static void CmShouquxian() // 首曲线
        {
            var entityID = "252";
            EntityManager.AddLine(true).SetStyles(entityID);
        }

        [CommandMethod("CmJiquxian")]
        public static void CmJiquxian() // 计曲线
        {
            var entityID = "253";
            EntityManager.AddLine(true).SetStyles(entityID);
        }

        [CommandMethod("CmShipoxian")]
        public static void CmShipoxian() // 示坡线
        {
            var entityID = "254";            
            EntityManager.AddBlock(entityID, null, 0, true)().SetStyles(entityID);
        }

        [CommandMethod("CmGaochengZhuji")]
        public static void CmGaochengZhuji() //等值线高程注记
        {
            var entityID = "255";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }

        [CommandMethod("CmShuixiaGaochengdian")]
        public static void CmShuixiaGaochengdian() // 水下高程点
        {
            var entityID = "256";
            EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }

        [CommandMethod("CmShuixiaGaochengdianZhuji")]
        public static void CmShuixiaGaochengdianZhuji() // 水下高程点注记
        {
            var entityID = "257";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }

        #endregion

        #region 自然、人工地貌 [258-264]
        [CommandMethod("CmYttk")]
        public static void CmYttk() // 有滩土坎
        {
            var entityID = "258";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmYtsk")]
        public static void CmYtsk() // 有滩石坎
        {
            var entityID = "259";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmWttk")]
        public static void CmWttk() // 无滩土坎
        {
            var entityID = "260";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmWtsk")]
        public static void CmWtsk() // 无滩石坎
        {
            var entityID = "261";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmZrdmXzzj")]
        public static void CmZrdmXzzj() // 自然地貌性质注记
        {
            var entityID = "262";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        [CommandMethod("CmRgk")]
        public static void CmRgk() // 人工坎
        {
            var entityID = "263";
            EntityManager.AddLine().SetStyles(entityID);
        }
        [CommandMethod("CmRgdmXzzj")]
        public static void CmRgdmXzzj() // 人工地貌性质注记
        {
            var entityID = "264";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }
        #endregion

        #region 农林用地（植被）[265-278]

        [CommandMethod("CmDileijie")]
        public static void CmDileijie() // 地类界
        {
            var entityID = "265";
            EntityManager.AddLine().SetStyles(entityID); // 新式写法，建议采用
        }

        [CommandMethod("CmDaotian")]
        public static void CmDaotian() // 稻田
        {
            CmZhibei("266");
        }

        [CommandMethod("CmHandi")]
        public static void CmHandi() // 旱地
        {
            CmZhibei("267");
        }

        [CommandMethod("CmCaidi")]
        public static void CmCaidi() // 菜地
        {
            CmZhibei("268");
        }

        [CommandMethod("CmShuishengzuowudi")]
        public static void CmShuishengzuowudi() // 水生作物地
        {
            CmZhibei("269");
        }

        [CommandMethod("CmYuandi")]
        public static void CmYuandi() // 园地
        {
            CmZhibei("270");
        }

        [CommandMethod("CmLindi")]
        public static void CmLindi() // 林地
        {
            CmZhibei("271");
        }

        [CommandMethod("CmLuwei")]
        public static void CmLuwei() // 芦苇
        {
            CmZhibei("272");
        }

        [CommandMethod("CmHuangcaodi")]
        public static void CmHuangcaodi() // 荒草地
        {
            CmZhibei("273");
        }

        [CommandMethod("CmSicao")]
        public static void CmSicao() // 丝草
        {
            CmZhibei("274");
        }

        [CommandMethod("CmPucao")]
        public static void CmPucao() // 蒲草
        {
            CmZhibei("275");
        }

        [CommandMethod("CmHuhuamicao")]
        public static void CmHuhuamicao() // 互花米草
        {
            CmZhibei("276");
        }

        [CommandMethod("CmHaisanlenglucao")]
        public static void CmHaisanlenglucao() // 海三棱藨草
        {
            CmZhibei("277");
        }

        private static void CmZhibei(string entityID)
        {
            var id = Interaction.GetEntity("\n选择边界", typeof(Polyline));
            if (id.IsNull)
            {
                return;
            }
            EntityManager.AddFill(id.QOpenForRead<Polyline>(), EntityManager.Entities[entityID].BlockName, 10).SetStyles(entityID);
        }

        [CommandMethod("CmNonglinYongdiZhuji")]
        public static void CmNonglinYongdiZhuji() // 农林用地注记
        {
            var entityID = "278";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }

        #endregion

        #region 土质 [279-283]

        [CommandMethod("CmBaibandiY")]
        public static void CmBaibandiY() // 依比例白板地
        {
            var entityID = "279";
            var textID = "283";
            var id = EntityManager.AddLine().SetStyles(entityID);
            EntityManager.AddText(textID, "白板地", id.GetCenter())().SetStyles(textID);
        }

        [CommandMethod("CmBaibandiB")]
        public static void CmBaibandiB() // 不依比例白板地
        {
            var entityID = "280";
            var id = EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }

        [CommandMethod("CmShikuaidiY")]
        public static void CmShikuaidiY() // 石块地范围
        {
            var entityID = "281";
            var fillID = "282";
            var id = EntityManager.AddLine().SetStyles(entityID);
            EntityManager.AddFill(id.QOpenForRead<Polyline>(), EntityManager.Entities[fillID].BlockName, 10).SetStyles(entityID);
        }

        [CommandMethod("CmShikuaidiB")]
        public static void CmShikuaidiB() // 石块地符号
        {
            var entityID = "282";
            var id = EntityManager.AddBlock(entityID)().SetStyles(entityID);
        }

        [CommandMethod("CmTuzhiZhuji")]
        public static void CmTuzhiZhuji() // 土质注记
        {
            var entityID = "283";
            EntityManager.AddText(entityID)().SetStyles(entityID);
        }

        #endregion
    }
}
