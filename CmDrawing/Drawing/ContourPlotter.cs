using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

using OSGeo.GDAL;
using Dreambuild.Geometry;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace TongJi.Drawing
{
    public class ContourPlotter
    {
        #region memebers
        // 网格数据
        private int m_xLength;
        private int m_yLength;               // 网格范围
        private double[,] m_elevation;       // 高程数据
        private double m_nodatavalue;        // 无高程数据的点的值
        private Grid[,] m_grids;             // 网格集合
        private Extents3d m_extents;         // 网格范围
        // 等高线样式
        private double m_hInterval;    // 等高线间距
        private int m_majorSpan;       // 主等高线跨越几条小等高线
        private byte m_minColor;       // 小等高线的颜色
        private byte m_majorColor;     // 主等高线颜色
        // 运行时数据
        private List<Grid> m_truetable; // 真值表，存放有等值点的网格的索引
        // 生成的等高线结果数组
        private List<Polyline[]> m_contours;       // 生成的等高线数组

        #endregion

        #region properties
        // 读取或设置等高距
        public double HInterval
        {
            get
            {
                return m_hInterval;
            }
            set
            {
                m_hInterval = value;
            }
        }
        #endregion
        /// <summary>
        /// 传入曲面构造
        /// </summary>
        /// <param name="plottingSurface">要绘制等高线的曲面</param>
        public ContourPlotter(Surface plottingSurface)
        {
            // 获取曲面数据            
            m_xLength = plottingSurface.ColCount;
            m_yLength = plottingSurface.RowCount;
            m_elevation = new double[m_xLength, m_yLength];
            double[] data = plottingSurface.getElevations();
            for (int i = 0; i < data.GetLength(0); i++)
            {
                int y = i / m_xLength;
                int x = i - y * m_xLength;
                m_elevation[x, y] = data[i];
            }
            m_extents = plottingSurface.GetXYRange();
            m_nodatavalue = plottingSurface.NoDataValue;
            // 设置默认样式
            m_hInterval = 5;
            m_majorColor = 1;
            m_minColor = 0;
            m_majorSpan = 3;
            // 初始化网格
            m_grids = new Grid[m_xLength - 1, m_yLength - 1];
            for (int i = 0; i < m_xLength - 1; i++)
            {
                for (int j = 0; j < m_yLength - 1; j++)
                {
                    double[] heights = new double[4];
                    heights[0] = m_elevation[i, j];
                    heights[1] = m_elevation[i, j + 1];
                    heights[2] = m_elevation[i + 1, j + 1];
                    heights[3] = m_elevation[i + 1, j];
                    m_grids[i, j] = new Grid(i, j, heights);
                }
            }
            //初始化等高线结果list
            m_contours = new List<Polyline[]>();
        }

        /// <summary>
        /// 设置等高线绘制样式。
        /// </summary>
        /// <param name="minor">小等高线间距，量纲米</param>
        /// <param name="major">主等高线是每几个小等高线，量纲一</param>
        /// <param name="minorColor">小等高线颜色索引</param>
        /// <param name="majorColor">主等高线颜色索引</param>
        public void SetContourStyle(double minor, int major, byte minorColor, byte majorColor)
        {
            m_hInterval = minor;
            m_majorSpan = major;
            m_minColor = minorColor;
            m_majorColor = majorColor;
        }

        /// <summary>
        /// 根据指定样式（未指定为默认样式）绘制等高线到AutoCAD视图
        /// </summary>
        public void PlotContour()
        {
            Interval inter = GetElevationRange();
            int n = (int)((inter.UpperBound - inter.LowerBound) / m_hInterval);
            // 依次生成n条等高线
            for (int i = 0; i < n + 1; i++)
            {
                double elevation = inter.LowerBound + (i + 0.01) * m_hInterval;
                if (elevation > inter.UpperBound)
                {
                    continue;
                }
                Polyline[] contour = GetContourAt(elevation).ToArray();

                if (contour != null)
                {
                    m_contours.Add(contour);

                }
                else
                {
                    //  throw new System.Exception();
                }
            }
            // 绘制等高线到AutoCAD视图
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)(trans.GetObject(db.BlockTableId, OpenMode.ForRead));
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                for (int i = 0; i < m_contours.Count; i++)
                {
                    Polyline[] plines = m_contours[i];
                    int color = 0;
                    if ((int)(i / m_majorSpan) - (double)i / m_majorSpan == 0)
                    {
                        color = m_majorColor;
                    }
                    else
                    {
                        color = m_minColor;
                    }
                    for (int j = 0; j < plines.GetLength(0); j++)
                    {
                        plines[j].ColorIndex = color;
                        AutoCADCommands.DbHelper.AddTag(plines[j], TagName.Contour);  // 标记，便于查找
                        plines[j].Layer = LayerName.LockedLayer;
                        btr.AppendEntity(plines[j]);
                        trans.AddNewlyCreatedDBObject(plines[j], true);
                    }
                }

                trans.Commit();
            }
        }

        /// <summary>
        /// 根据指定样式（未指定为默认样式）绘制指定范围内的等高线到AutoCAD视图
        /// </summary>
        /// <param name="extents">平面范围</param>
        public void PlotContour(Extents3d extents)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取指定高程处的等高线。与绘制样式无关（即这个高程可能位于相邻小等高线之间，不绘制，但用此函数无论如何总可以得到这条等高线）。高程超范围抛出ArgumentOutOfRangeException。
        /// </summary>
        /// <param name="elevation">高程</param>
        /// <returns>等高线</returns>
        public Polyline[] GetContourAt(double elevation)
        {
            Interval inter = GetElevationRange();
            if (elevation > inter.UpperBound || elevation < inter.LowerBound)
            {
                throw new ArgumentException("指定高程超出范围");
            }
            // 内插等值点，将有等值点的网格加入到真值表中
            List<Polyline> contours = new List<Polyline>();
            m_truetable = new List<Grid>();
            for (int i = 0; i < m_xLength - 1; i++)
            {
                for (int j = 0; j < m_yLength - 1; j++)
                {
                    int nCount = m_grids[i, j].setContourValue(elevation);
                    if (nCount > 0)
                    {
                        m_truetable.Add(m_grids[i, j]);
                    }
                }
            }

            while (m_truetable.Count > 0)
            {
                // 任取一个网格开始追踪
                Grid grid = m_truetable[0];
                Point2d point = grid.getEPoint(0);
                // 记录第一个网格和第一个等值点的位置，用于反向跟踪
                int firstgrid_x = grid.X;
                int firstgrid_y = grid.Y;
                int firstEPPos = grid.getEPointPos(0);
                bool flag = false;      // 反向标记,如果未进行反向追踪为false，否则为true
                Polyline pline = new Polyline();  // 待追踪的等高线

                pline.AddVertexAt(pline.NumberOfVertices, tranCoordinate(point), 0, 0, 0);
                while (grid != null && !pline.Closed)
                {
                    int pos;
                    point = grid.findNextEPoint(point, out pos);
                    if (flag)
                    {
                        pline.AddVertexAt(0, tranCoordinate(point), 0, 0, 0);
                    }
                    else
                    {
                        pline.AddVertexAt(pline.NumberOfVertices, tranCoordinate(point), 0, 0, 0);
                    }

                    // 如果该网格无等值点，则将其从真值表中移出
                    if (grid.EPCount == 0)
                    {
                        m_truetable.Remove(grid);
                    }
                    bool boundflag;   // 边界标志，是否碰到边界
                    grid = findnext(grid.X, grid.Y, pos, out boundflag);
                    if (grid == null && boundflag && !flag)
                    {
                        flag = true;
                        // 反向追踪
                        bool firstboundflag;  // 第一个网格上的第一个等值点是否在边界上
                        grid = findnext(firstgrid_x, firstgrid_y, firstEPPos, out firstboundflag);
                        if (firstboundflag)
                        {
                            break;
                        }
                    }
                }
                contours.Add(pline);
            }
            return contours.ToArray();
        }

        /// <summary>
        /// 追踪下一个网格
        /// </summary>
        /// <param name="x">当前网格的x坐标</param>
        /// <param name="y">当前网格的y坐标</param>
        /// <param name="pos">当前等值点的位置</param>
        /// <param name="boundflag">输出参数，是否碰到边界</param>
        /// <returns></returns>
        private Grid findnext(int x, int y, int pos, out bool boundflag)
        {
            switch (pos)
            {
                case 0:
                    x--;
                    break;
                case 1:
                    y++;
                    break;
                case 2:
                    x++;
                    break;
                case 3:
                    y--;
                    break;
                default:
                    throw new ArgumentException();
            }
            if (x > m_xLength - 1 || y > m_yLength - 1)
            {
                boundflag = true;
                return null;
            }
            else
            {
                boundflag = false;
            }
            for (int i = 0; i < m_truetable.Count; i++)
            {
                if (m_truetable[i].X == x
                    && m_truetable[i].Y == y)
                {
                    return m_truetable[i];
                }

            }
            return null;
        }

        /// <summary>
        /// 获取高程范围
        /// </summary>
        /// <returns>高程范围区间</returns>
        public Interval GetElevationRange()
        {
            double min = 10000;
            double max = -10000;
            for (int i = 0; i < m_elevation.GetLength(0); i++)
            {
                for (int j = 0; j < m_elevation.GetLength(1); j++)
                {
                    if (m_elevation[i, j] != m_nodatavalue)
                    {
                        if (m_elevation[i, j] > max)
                        {
                            max = m_elevation[i, j];
                        }
                        if (m_elevation[i, j] < min)
                        {
                            min = m_elevation[i, j];
                        }
                    }
                }
            }
            return new Interval(min, max, 0);
        }

        /// <summary>
        /// 网格坐标转换为实际坐标
        /// </summary>
        /// <param name="point">待转换的网格坐标</param>
        /// <returns>实际坐标</returns>
        private Point2d tranCoordinate(Point2d point)
        {
            double x = m_extents.MinPoint.X + point.X * (m_extents.MaxPoint.X - m_extents.MinPoint.X) / (m_xLength - 1);
            double y = m_extents.MinPoint.Y + point.Y * (m_extents.MaxPoint.Y - m_extents.MinPoint.Y) / (m_yLength - 1);
            return new Point2d(x, y);
        }
    }
    /// <summary>
    /// 网格对象，记录该网格的位置、顶点高程、4条边上的等值点以及相关的操作
    /// </summary>
    public class Grid
    {
        #region memebers

        private int m_x;
        private int m_y;        // 网格的位置索引
        private double[] m_heights = new double[4]; // 网格4个顶点的高程,左下角点开始顺时针

        private List<Point2d> m_EPoints;         // 等值点，前两个一组，后两个一组
        private List<int> m_EPointPositions;    // 等值点位置，0,1,2,3分别代表左上右下

        #endregion

        #region properties

        public int X
        {
            get
            {
                return m_x;
            }
            set
            {
                m_x = value;
            }
        }
        public int Y
        {
            get
            {
                return m_y;
            }
            set
            {
                m_y = value;
            }
        }
        public int EPCount
        {
            get
            {
                return m_EPoints.Count;
            }
        }
        #endregion
        public Grid()
        {

        }
        public Grid(Grid preGrid)
        {
            m_x = preGrid.X;
            m_y = preGrid.Y;
        }
        //public 
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="heights"></param>
        public Grid(int x, int y, double[] heights)
        {
            m_x = x;
            m_y = y;
            m_heights = (double[])heights.Clone();
        }
        /// <summary>
        /// 设定等高线的高程值，并内插等值点
        /// </summary>
        /// <param name="C">等高线高程</param>
        /// <returns>该网格上等值点的数量</returns>
        public int setContourValue(double C)
        {
            m_EPoints = new List<Point2d>();
            m_EPointPositions = new List<int>();
            // 计算各边等值点
            // 如果经过顶点，则对顶点高程修改一个可以忽略的数字
            double[] heights = (double[])m_heights.Clone();
            for (int i = 0; i < 4; i++)
            {
                if (heights[i] == C)
                {
                    heights[i] += 0.0000001;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                int n1, n2;
                n1 = i; n2 = i + 1;
                if (n2 > 3)
                {
                    n2 = 0;
                }
                if ((heights[n1] - C) * (heights[n2] - C) < 0) // 异号
                {
                    m_EPoints.Add(insertPoint(i, C));
                    m_EPointPositions.Add(i);
                }
            }
            // 根据中心点的符号调整点的序号
            int nCount = m_EPoints.Count;
            if (nCount == 3 || nCount == 1)
            {
                throw new System.Exception("");
            }
            if (nCount == 4)
            {
                double midHeight = getCenterElevation();
                if ((m_heights[0] - C) * (midHeight - C) < 0)
                {
                    // 调整顺序
                    m_EPoints.Insert(1, m_EPoints[3]);
                    m_EPointPositions.Insert(1, m_EPointPositions[3]);
                    m_EPoints.RemoveAt(4);
                    m_EPointPositions.RemoveAt(4);
                }
            }
            return nCount;
        }

        /// <summary>
        /// 查找该网格指定位置上的等值点
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Point2d findEPoint(int pos)
        {
            for (int i = 0; i < m_EPoints.Count; i++)
            {
                if (m_EPointPositions[i] == pos)
                {
                    return m_EPoints[i];
                }
            }
            throw new System.Exception("未找到该点");
        }
        /// <summary>
        /// 指定索引的等高点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Point2d getEPoint(int index)
        {
            return m_EPoints[index];
        }
        /// <summary>
        /// 指定索引的等值点的位置
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int getEPointPos(int index)
        {
            return m_EPointPositions[index];
        }
        /// <summary>
        /// 查询给定等值点的索引
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private int findEPIndex(Point2d point)
        {
            for (int i = 0; i < m_EPoints.Count; i++)
            {
                if (m_EPoints[i].IsEqualTo(point))
                {
                    return i;
                }
            }
            throw new System.Exception("未找到该点");
        }
        /// <summary>
        /// 给出一个等值点，追踪该网格的下一个等值点，并返回该点的位置
        /// </summary>
        /// <param name="point"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Point2d findNextEPoint(Point2d point, out int pos)
        {
            int index = findEPIndex(point);
            Point2d ptRet = new Point2d();
            if (index == 0)
            {
                ptRet = m_EPoints[1];
                pos = m_EPointPositions[1];
            }
            else if (index == 1)
            {
                ptRet = m_EPoints[0];
                pos = m_EPointPositions[0];
            }
            else if (index == 2)
            {
                ptRet = m_EPoints[3];
                pos = m_EPointPositions[3];
            }
            else
            {
                ptRet = m_EPoints[2];
                pos = m_EPointPositions[2];
            }
            // 从等值点列表中移出这两个追踪过的等值点
            m_EPoints.Remove(point);
            m_EPointPositions.RemoveAt(index);
            m_EPoints.Remove(ptRet);
            m_EPointPositions.Remove(pos);
            return ptRet;

        }
        /// <summary>
        /// 网格等值点内插
        /// </summary>
        /// <param name="index">等值点所在的边，0,1,2,3分别代表左上右下</param>
        /// <param name="elevation">高程</param>
        /// <returns>内插点的平面坐标</returns>
        private Point2d insertPoint(int index, double elevation)
        {
            double z1, z2;
            int x1, y1, x2, y2;
            if (index == 0)
            {
                x1 = m_x;
                y1 = m_y;
                x2 = m_x;
                y2 = m_y + 1;
                z1 = m_heights[0];
                z2 = m_heights[1];
            }
            else if (index == 1)
            {
                x1 = m_x;
                y1 = m_y + 1;
                x2 = m_x + 1;
                y2 = m_y + 1;
                z1 = m_heights[1];
                z2 = m_heights[2];
            }
            else if (index == 2)
            {
                x1 = m_x + 1;
                y1 = m_y + 1;
                x2 = m_x + 1;
                y2 = m_y;
                z1 = m_heights[2];
                z2 = m_heights[3];
            }
            else
            {
                x1 = m_x + 1;
                y1 = m_y;
                x2 = m_x;
                y2 = m_y;
                z1 = m_heights[3];
                z2 = m_heights[0];
            }

            double x = (float)(x1 + (elevation - z1) * (x2 - x1) / (z2 - z1));
            double y = (float)(y1 + (elevation - z1) * (y2 - y1) / (z2 - z1));
            Point2d pt = new Point2d(x, y);
            return pt;

        }
        /// <summary>
        /// 获取网格中心点的高程
        /// </summary>
        /// <returns>中心点高程</returns>
        private double getCenterElevation()
        {
            double sum = m_heights[0] + m_heights[1] + m_heights[2] + m_heights[3];
            return sum / 4.0;
        }
    }
    public class Surface
    {
        #region memebers
        private Point3d[] m_points;     // 离散点高程数据

        private double[] m_elevations;  // 网格点高程数据，按从左往右，从下往上依次存储
        private int m_YSize;            // 网格点纵向点数
        private int m_XSize;            // 网格点横向点数
        private double m_nodatavalue = -1000000;   // 没有数据的网格点的默认值

        private Extents3d m_extents;    // 范围        

        private int m_lbound = -10000;           // 高程下限值，用于去除异常点
        private int m_ubound = 10000;            // 高程上限值，用于去除异常点

        private bool _pointsAreGrid;    // 是否为网格数据
        #endregion

        #region properties
        /// <summary>
        /// 网格行数
        /// </summary>
        public int RowCount
        {
            get
            {
                return m_YSize;
            }
        }
        /// <summary>
        /// 网格列数
        /// </summary>
        public int ColCount
        {
            get
            {
                return m_XSize;
            }
        }

        public double NoDataValue
        {
            get
            {
                return m_nodatavalue;
            }

        }
        #endregion

        private Surface()
        {
        }

        private void readSurferGridData(string path)
        {
            // 读数据
            double yfirst = 0;  //记录第一个读出来的y坐标，如果发生变化，说明第一行数据读取完毕

            List<double> databuf = new List<double>();
            StreamReader sr = new StreamReader(path);
            int index = 0;
            while (!sr.EndOfStream)
            {
                string strRead = sr.ReadLine();
                if (strRead != "")
                {
                    string[] strs = strRead.Split(' ');
                    databuf.Add(double.Parse(strs[2]));
                    // 记录图幅范围,并统计行数
                    if (index == 0)
                    {
                        yfirst = double.Parse(strs[1]);
                        Point3d pt = new Point3d(double.Parse(strs[0]), double.Parse(strs[1]), 0);
                        m_extents.Set(pt, pt);
                    }
                    if (sr.EndOfStream)
                    {
                        m_extents.AddPoint(new Point3d(double.Parse(strs[0]), double.Parse(strs[1]), 0));
                    }
                    // y值发生变化，记录下列数
                    if (double.Parse(strs[1]) != yfirst && m_XSize == 0)
                    {
                        m_XSize = index;
                    }
                }
                index++;
            }
            m_YSize = databuf.Count / m_XSize;
            m_elevations = databuf.ToArray();
            sr.Close();
        }

        public static Surface FromDatFile(string path)
        {
            Surface surf = new Surface();
            surf.readSurferGridData(path);
            surf._pointsAreGrid = true;
            return surf;
        }

        public static void ToDatFile(Surface surf, string path)
        {
            Surface surf1 = new Surface();
            if (surf._pointsAreGrid == false)
            {
                Extents3d extentss = surf.m_extents;
                double gridSize = (extentss.MaxPoint.X - extentss.MinPoint.X) / 100;
                surf1 = surf.GetGridedSurface(gridSize);
            }
            else if (surf._pointsAreGrid == true)
            {
                surf1 = surf;
            }
            Extents3d extents = surf1.m_extents;
            int XSize = surf1.m_XSize;
            int YSize = surf1.m_YSize;
            System.IO.StreamWriter sw = new System.IO.StreamWriter(path);
            double gridXSize = (extents.MaxPoint.X - extents.MinPoint.X) / (XSize - 1);
            double gridYSize = (extents.MaxPoint.Y - extents.MinPoint.Y) / (YSize - 1);
            int col = (int)((extents.MaxPoint.X - extents.MinPoint.X) / gridXSize);
            int row = (int)((extents.MaxPoint.Y - extents.MinPoint.Y) / gridYSize);
            var xs = Enumerable.Range(0, col).Select(x => extents.MinPoint.X + x * gridXSize).ToArray();
            var ys = Enumerable.Range(0, row).Select(x => extents.MinPoint.Y + x * gridYSize).ToArray();
            for (int j = 0; j < row; j++)
            {
                for (int i = 0; i < col; i++)
                {
                    sw.WriteLine("{0} {1} {2}", xs[i], ys[j], surf.GetElevationAt(xs[i], ys[j]));
                }
            }
            sw.Close();
        }

        public static Surface FromTxtFile(string path)
        {
            List<Point3d> points = new List<Point3d>();
            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    string[] coords = line.Trim().Split(' ');
                    points.Add(new Point3d(Convert.ToDouble(coords[0]), Convert.ToDouble(coords[1]), Convert.ToDouble(coords[2])));
                }
            }
            Extents3d extents = new Extents3d(points[0], points[0]);
            points.ToList().ForEach(x => extents.AddPoint(x));
            Surface surf = new Surface();
            surf.m_points = points.ToArray();
            surf.m_extents = extents;
            surf._pointsAreGrid = false;
            surf.BuildTIN();
            return surf;
        }

        public static Surface FromPoint3ds(Point3d[] points)
        {
            Extents3d extents = new Extents3d(points[0], points[0]);
            points.ToList().ForEach(x => extents.AddPoint(x));

            Surface surf = new Surface();
            surf.m_points = points;
            surf.m_extents = extents;
            surf._pointsAreGrid = false;
            surf.BuildTIN();
            return surf;
        }

        /// <summary>
        /// 从DEM中获取数据
        /// </summary>
        /// <param name="path">DEM数据的路径</param>
        /// <param name="space">取点间隔，每隔几个点取一个数据，0表示无间隔</param>
        /// <returns>Surface对象</returns>
        public static Surface FromDem(string path, int space)
        {
            // 初始化gdal
            Gdal.AllRegister();
            Surface surf = new Surface();
            Dataset ds = Gdal.Open(path, Access.GA_ReadOnly);
            surf.m_XSize = ds.RasterXSize / (space + 1);
            surf.m_YSize = ds.RasterYSize / (space + 1);
            int count = ds.RasterCount;
            Band demband = ds.GetRasterBand(1);
            double[] databuf = new double[ds.RasterXSize * ds.RasterYSize];
            double[] gt = new double[6];
            ds.GetGeoTransform(gt);
            double nodatavalue;
            int hasval;
            demband.GetNoDataValue(out nodatavalue, out hasval);
            surf.m_nodatavalue = nodatavalue;
            Point3d pt1 = new Point3d(gt[0], gt[3], 0);
            Point3d pt2 = new Point3d(gt[0] + ds.RasterXSize * gt[1] + ds.RasterYSize * gt[2],
                                   gt[3] + ds.RasterXSize * gt[4] + ds.RasterYSize * gt[5], 0);
            surf.m_extents.Set(pt1, pt1);
            surf.m_extents.AddPoint(pt2);

            demband.ReadRaster(0, 0, ds.RasterXSize, ds.RasterYSize, databuf, ds.RasterXSize, ds.RasterYSize, 0, 0);

            surf.m_elevations = new double[surf.m_XSize * surf.m_YSize];

            for (int i = 0; i < surf.m_YSize; i++)
            {
                for (int j = 0; j < surf.m_XSize; j++)
                {
                    // 改为从左往右，从下往上记录的形式
                    // (i, j)databuf中的(x, y)坐标
                    int y = ds.RasterYSize - 1 - (space + 1) * i;
                    int x = (space + 1) * j;
                    // 去除异常高程
                    if (databuf[y * ds.RasterXSize + x] < surf.m_lbound || databuf[y * ds.RasterXSize + x] > surf.m_ubound)
                    {
                        surf.m_elevations[i * surf.m_XSize + j] = nodatavalue;
                    }
                    else
                    {
                        surf.m_elevations[i * surf.m_XSize + j] = databuf[y * ds.RasterXSize + x];
                    }
                }
            }
            surf._pointsAreGrid = true;
            return surf;
        }

        /// <summary>
        /// 获取一个指定标高和范围的矩形平面
        /// </summary>
        /// <param name="extents">范围</param>
        /// <param name="elevation">标高</param>
        /// <returns>平面</returns>
        public static Surface GetHorizontalPlane(Extents3d extents, double elevation)
        {
            double a = extents.MinPoint.X;
            double b = extents.MaxPoint.X;
            double c = extents.MinPoint.Y;
            double d = extents.MaxPoint.Y;
            Surface surf = Surface.FromPoint3ds(new Point3d[] {
                new Point3d(a,c,elevation),
                new Point3d(b,c,elevation),
                new Point3d(a,d,elevation),
                new Point3d(b,d,elevation)
            });
            return surf;
        }

        /// <summary>
        /// 获取指定范围的surface
        /// </summary>
        /// <param name="extents">指定的范围</param>
        /// <returns>Surface对象</returns>
        public Surface GetExtentedSurface(Extents3d extents)
        {
            if (extents.MinPoint.X < m_extents.MinPoint.X
                || extents.MinPoint.Y < m_extents.MinPoint.Y
                || extents.MaxPoint.X > m_extents.MaxPoint.X
                || extents.MaxPoint.Y > m_extents.MaxPoint.Y)
            {
                throw new ArgumentException("指定的范围超出当前surface类的范围！");
            }
            // 最小点网格坐标和最大点网格坐标
            double gridXSize = (m_extents.MaxPoint.X - m_extents.MinPoint.X) / (m_XSize - 1);
            double gridYSize = (m_extents.MaxPoint.Y - m_extents.MinPoint.Y) / (m_YSize - 1);
            int minGridX = (int)((extents.MinPoint.X - m_extents.MinPoint.X) / gridXSize);
            int minGridY = (int)((extents.MinPoint.Y - m_extents.MinPoint.Y) / gridYSize);
            int maxGridX = (int)((extents.MaxPoint.X - m_extents.MinPoint.X) / gridXSize);
            int maxGridY = (int)((extents.MaxPoint.Y - m_extents.MinPoint.Y) / gridYSize);
            int col = maxGridX - minGridX + 1;
            int row = maxGridY - minGridY + 1;
            double[] elevations = new double[col * row];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    elevations[i * col + j] = m_elevations[(minGridY + i) * m_XSize + (minGridX + j)];
                }
            }
            Surface surf = new Surface();
            surf._pointsAreGrid = true;
            surf.m_extents = extents;
            surf.m_elevations = elevations.ToArray();
            surf.m_nodatavalue = this.m_nodatavalue;
            surf.m_XSize = col;
            surf.m_YSize = row;
            return surf;
        }

        /// <summary>
        /// 获取网格化后的surface类
        /// </summary>
        /// <param name="gridSize">网格大小</param>
        /// <returns></returns>
        public Surface GetGridedSurface(double gridSize)
        {
            Extents3d extents = this.m_extents;

            int col = (int)((extents.MaxPoint.X - extents.MinPoint.X) / gridSize);
            int row = (int)((extents.MaxPoint.Y - extents.MinPoint.Y) / gridSize);
            List<double> elevations = new List<double>();
            var xs = Enumerable.Range(0, col).Select(x => extents.MinPoint.X + x * gridSize).ToArray();
            var ys = Enumerable.Range(0, row).Select(x => extents.MinPoint.Y + x * gridSize).ToArray();
            for (int j = 0; j < row; j++)
            {
                for (int i = 0; i < col; i++)
                {
                    elevations.Add(this.GetElevationAt(xs[i], ys[j]));
                }
            }
            Surface surf = new Surface();
            surf._pointsAreGrid = true;
            surf.m_extents = extents;
            surf.m_elevations = elevations.ToArray();
            surf.m_nodatavalue = this.m_nodatavalue;
            surf.m_XSize = col;
            surf.m_YSize = row;
            return surf;
        }

        public ObjectId ToPolygonMesh(double gridSize) // newly 20130109
        {
            //Extents3d extents = this.m_extents;
            //int col = (int)((extents.MaxPoint.X - extents.MinPoint.X) / gridSize);
            //int row = (int)((extents.MaxPoint.Y - extents.MinPoint.Y) / gridSize);
            //List<Point3d> points = new List<Point3d>();
            //var xs = Enumerable.Range(0, col).Select(x => extents.MinPoint.X + x * gridSize).ToArray();
            //var ys = Enumerable.Range(0, row).Select(x => extents.MinPoint.Y + x * gridSize).ToArray();
            //for (int i = 0; i < col; i++)
            //{
            //    for (int j = 0; j < row; j++)
            //    {
            //        points.Add(new Point3d(xs[i], ys[j], this.GetElevationAt(xs[i], ys[j])));
            //    }
            //}
            var demPoints = _tin.GetDEMPoints(gridSize);
            var points = demPoints.Select(p => new Point3d(p.X, p.Y, p.Z)).ToList();
            int col = demPoints.Select(p => p.X).Distinct().Count();
            int row = demPoints.Select(p => p.Y).Distinct().Count();
            return AutoCADCommands.Draw.PolygonMesh(points, col, row);
        }

        /// <summary>
        /// 获取范围
        /// </summary>
        /// <returns></returns>
        public Extents3d GetXYRange()
        {
            return m_extents;
        }

        /// <summary>
        /// 获取网格高程数据
        /// </summary>
        /// <returns></returns>
        public double[] getElevations()
        {
            return (double[])m_elevations.Clone();
        }

        private Gis.Topography.TIN _tin;
        public const double PreviewGridDivCount = 100;

        private void BuildTIN()
        {
            _tin = new Gis.Topography.TIN(m_points.Select(p => new Point3D(p.X, p.Y, p.Z)).ToList());
            double gridSize = (m_points.Max(p => p.X) - m_points.Min(p => p.X)) / Surface.PreviewGridDivCount;
            var demPoints = _tin.GetDEMPoints(gridSize);
            var extents = new Extent2D(demPoints.Select(p => p.ToPoint2D()).ToList());
            int xs = demPoints.Select(p => p.X).Distinct().Count();
            int ys = demPoints.Select(p => p.Y).Distinct().Count();
            List<double> elevations = new List<double>();
            for (int j = 0; j < ys; j++)
            {
                for (int i = 0; i < xs; i++)
                {
                    elevations.Add(demPoints[i * ys + j].Z);
                }
            }
            this.m_elevations = elevations.ToArray();
            this.m_extents = new Extents3d(new Point3d(extents.min.X, extents.min.Y, 0), new Point3d(extents.max.X, extents.max.Y, 0));
            this.m_XSize = xs;
            this.m_YSize = ys;
        }

        /// <summary>
        /// 获取指定位置高程。
        /// </summary>
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        /// <returns>高程</returns>
        public double GetElevationAt(double x, double y, ElevationInterpolationMethod method = ElevationInterpolationMethod.Bilinear)
        {
            if (method == ElevationInterpolationMethod.Bilinear)
            {
                // 网格插值,使用双线性插值方法
                // 判断所在网格
                double colWidth = (m_extents.MaxPoint.X - m_extents.MinPoint.X) / (m_XSize - 1);
                int col = (int)((x - m_extents.MinPoint.X) / colWidth);
                double rowWidth = (m_extents.MaxPoint.Y - m_extents.MinPoint.Y) / (m_YSize - 1);
                int row = (int)((y - m_extents.MinPoint.Y) / rowWidth);
                double lbElevation = m_elevations[m_XSize * row + col]; // 左下网格点高程
                double ltElevation = m_elevations[m_XSize * (row + 1) + col]; // 左上网格点高程
                double rtElevation = m_elevations[m_XSize * (row + 1) + col + 1]; // 右上网格点高程
                double rbElevation = m_elevations[m_XSize * row + col + 1];  // 右下网格点高程
                double u = (x - m_extents.MinPoint.X) / colWidth - col;
                double v = (y - m_extents.MinPoint.Y) / rowWidth - row;
                return (1 - u) * (1 - v) * lbElevation + (1 - u) * v * ltElevation + u * (1 - v) * rbElevation + u * v * rtElevation;
            }
            else if (method == ElevationInterpolationMethod.InverseDistanceWeighted)
            {
                // 距离反比加权插值
                double epsilon = 1e-6;
                if (m_points.Any(p => (p.X - x) * (p.X - x) + (p.Y - y) * (p.Y - y) < epsilon))
                {
                    return m_points.First(p => (p.X - x) * (p.X - x) + (p.Y - y) * (p.Y - y) < epsilon).Z;
                }
                var weights = m_points.Select(p => 1.0 / ((p.X - x) * (p.X - x) + (p.Y - y) * (p.Y - y))).ToArray();
                double weightsSum = weights.Sum();
                return Enumerable.Range(0, m_points.Length).Sum(i => weights[i] * m_points[i].Z) / weightsSum;
            }
            else if (method == ElevationInterpolationMethod.TIN)
            {
                // TIN插值
                return _tin.InterpolateZ(new Point2D(x, y));
            }
            return 0;
        }

        /// <summary>
        /// 获取指定位置高程梯度。
        /// </summary>
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        /// <returns>高程梯度</returns>
        public Vector3d GetFirstDerivativeAt(double x, double y)
        {
            double epsilon = 1e-6;
            double partialX = (GetElevationAt(x + epsilon, y) - GetElevationAt(x - epsilon, y)) / (2 * epsilon);
            double partialY = (GetElevationAt(x, y + epsilon) - GetElevationAt(x, y - epsilon)) / (2 * epsilon);
            return new Vector3d(partialX, partialY, 0);
        }
    }

    public enum ElevationInterpolationMethod
    {
        Bilinear,
        InverseDistanceWeighted,
        TIN,
        Voronoi,
        TrendSurface,
        ThinPlateSpline,
        Kriging
    }
}

namespace TongJi.Gis.Topography
{
    public class TIN
    {
        public TIN(List<Point3D> pts) { }
        public double InterpolateZ(Point2D xy) { return 0; }
        public List<Point3D> GetDEMPoints(double size) { return new List<Point3D>(); }
    }
}
