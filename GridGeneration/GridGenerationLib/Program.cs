using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Roger.Testing
{
    public static class ExpandMethods
    {
        public static int CompareTo(this int vt, int other)
        {
            return vt - other;
        }
    }


    public class Testing
    {
        public static void Main()
        {            
            List<bool> map = new List<bool>
			{
				true, true, true, false, false,
				false, true, false, true,false,
				false, false, false, true,false,
				false,true,false, false, false,
				false, false, false, true, false
			};
            int width = 5;
            int start = 10;
            int end = 24;


            if (map[start] || map[end]) {
                Console.Write("起始点或者结束点为不合法障碍物，！");
                Console.Read();
                return;
            }

            Console.WriteLine("队列形式的A*算法开始");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            AStarSearch finder = new AStarSearch(map, width);
            Dictionary<int, int> path = finder.FindPath(start, end);
            watch.Stop();
            Console.WriteLine("耗费时间： {0}", watch.ElapsedMilliseconds);

            Console.WriteLine("------------------------------------------------- ");

            Console.WriteLine("红黑树形式的A*算法开始");
            Stopwatch watch2 = new Stopwatch();
            watch2.Start();
            AStarRedBlackSearch finder2 = new AStarRedBlackSearch(map, width);
            Dictionary<int, int> path2 = finder2.FindPath(start, end);
            watch2.Stop();
            Console.WriteLine("耗费时间： {0}", watch2.ElapsedMilliseconds);


            Console.WriteLine("------------------------------------------------- ");

            Console.WriteLine("二项堆排序形式的A*算法开始");
            Stopwatch watch3 = new Stopwatch();
            watch3.Start();
            AStarBinaryHeapSearch finder3 = new AStarBinaryHeapSearch(map, width);
            Dictionary<int, int> path3 = finder3.FindPath(start, end);
            watch3.Stop();
            Console.WriteLine("耗费时间： {0}", watch3.ElapsedMilliseconds);

            // ===========================================================================
            if (path == null)
            {
                Console.WriteLine("没有找到对应的路径"); Console.Read(); return;
            }

            // 得到最后一个Key的值
            int key = end;
            List<int> link = new List<int>() { end };
            while (path[key] != start)
            {
                key = path[key];
                link.Add(key);                
            }
            link.Add(start);
            
            for (int i = 0; i < map.Count; i++ )
            {
                if (i % width == 0) Console.WriteLine();
                if (map[i]) Console.Write("#");
                else if (link.Contains(i)) Console.Write("1");
                else Console.Write("*");

                Console.Write(" ");
            }

            Console.Read();
        }
    }

    #region A* 最短路径搜索(队列实现)
    /// <summary>
    /// A* 最短路径搜索
    /// </summary>
    public class AStarSearch
    {
        private List<bool> Map;
        private int Width;
        private int Height;
        private bool IsValidPosition(int start)
        {
            return start >= 0 && start <= Map.Count;
        }
        public AStarSearch(List<bool> map, int Width)
        {
            if (map == null) throw new ArgumentNullException();

            Map = map;
            this.Width = Width;
            this.Height = Map.Count / Width;
        }

        private Queue<int> Open;
        private Queue<int> Close;

        public Dictionary<int, int> FindPath(int start, int end)
        {
            if (!IsValidPosition(start) || !IsValidPosition(end)) throw new ArgumentOutOfRangeException();
            this.Start = start; this.End = end;
            Open = new Queue<int>();
            Close = new Queue<int>();
            GScore = new Dictionary<int, int>();
            FScore = new Dictionary<int, int>();
            ComeFrom = new Dictionary<int, int>();

            // 将开始节点入队列
            Open.Enqueue(start);

            int x = start;
            while (Open.Count > 0)
            {
                x = GetLowestF();
                if (x == End)
                {
                    // Trace From
                    return ComeFrom;
                }

                Open.Dequeue();
                Close.Enqueue(x);

                foreach (int y in GetNodesAround(x))
                {
                    if (Close.Contains(y))
                    {
                        continue;
                    }

                    int newGValue = GetCost(x) + GetDistance(x, y);
                    bool newIsBetter = false;

                    if (!Open.Contains(y))
                    {
                        Open.Enqueue(y);
                        newIsBetter = true;
                    }
                    else if (newGValue < GScore[y])
                        newIsBetter = true;


                    if(newIsBetter)
                    {
                        if (ComeFrom.ContainsKey(y))
                            ComeFrom[y] = x;
                        else
                            ComeFrom.Add(y, x);

                        GScore[y] = newGValue;
                        FScore[y] = GScore[y] + GetHeuristic(y);
                    }

                }
            }

            return null;
        }

        private int Start;
        private int End;


        private IList<int> GetNodesAround(int pos)
        {
            List<int> list = new List<int>(4);
            int x = pos % Width; int y = pos / Width;
            // Left
            if (x > 0 && !Map[x - 1 + y * Width]) list.Add(x - 1 + y * Width);
            // Up
            if (y > 0 && !Map[x + (y - 1) * Width]) list.Add(x + (y - 1) * Width);
            // Right
            if (x < Width-1 && !Map[x + 1 + y * Width]) list.Add(x + 1 + y * Width);
            // Down
            if (y < Height-1 && !Map[x + (y + 1) * Width]) list.Add(x + (y + 1) * Width);

            return list;
        }

        private int GetCost(int current)
        {
            // UNDONE
            int xDistance = (int)Math.Abs(Start % Width - current % Width);
            int yDistance = (int)Math.Abs(Start / Width - current / Width);

            if (!GScore.ContainsKey(current))
                GScore.Add(current ,(xDistance + yDistance) * 10);

            return GScore[current];
        }

        private int GetLowestF()
        {
            int temp = GetFScore(Open.Peek());
            int lowest = Open.Peek();

            foreach (int i in Open)
            {
                if (temp > GetFScore(i))
                {
                    temp = GetFScore(i);
                    lowest = i;
                }
            }

            return lowest;
        }

        private int GetFScore(int pos)
        {
            if (!FScore.ContainsKey(pos))
                FScore.Add(pos, GetCost(pos) + GetHeuristic(pos));

            return FScore[pos];
        }

        private Dictionary<int, int> GScore;
        private Dictionary<int, int> FScore;
        private Dictionary<int, int> ComeFrom;

        // 得到预估的距离
        private int GetHeuristic(int current)
        {
            return GetDistance(current, End);
        }

        private int GetDistance(int x, int y)
        {
            int xDistance = (int)Math.Abs(y % Width - x % Width);
            int yDistance = (int)Math.Abs(y / Width - x / Width);

            return 10 * (xDistance + yDistance);

        }
    }
    #endregion

    #region 采用SortDictionary的排序(红黑树)

    /// <summary>
    /// A* 最短路径搜索
    /// </summary>
    public class AStarRedBlackSearch
    {
        private List<bool> Map;
        private int Width;
        private int Height;
        private bool IsValidPosition(int start)
        {
            return start >= 0 && start <= Map.Count;
        }
        public AStarRedBlackSearch(List<bool> map, int Width)
        {
            if (map == null) throw new ArgumentNullException();

            Map = map;
            this.Width = Width;
            this.Height = Map.Count / Width;
        }

        private SortedDictionary<int, int> Open;
        private SortedDictionary<int, int> Close;

        public Dictionary<int, int> FindPath(int start, int end)
        {
            if (!IsValidPosition(start) || !IsValidPosition(end)) throw new ArgumentOutOfRangeException();
            this.Start = start; this.End = end;
            Open = new SortedDictionary<int, int>();
           
            Close = new SortedDictionary<int,int>();
            GScore = new Dictionary<int, int>();
            FScore = new Dictionary<int, int>();
            ComeFrom = new Dictionary<int, int>();

            // 将开始节点入队列
            Open.Add(start, GetFScore(start));

            int x = start;
            while (Open.Count > 0)
            {
                x = GetLowestF();
                if (x == End)
                {
                    // Trace From
                    return ComeFrom;
                }

                Open.Remove(x);
                Close.Add(x, GetFScore(x));

                foreach (int y in GetNodesAround(x))
                {
                    if (Close.ContainsKey(y))
                    {
                        continue;
                    }

                    int newGValue = GetCost(x) + GetDistance(x, y);
                    bool newIsBetter = false;

                    if (!Open.ContainsKey(y))
                    {
                        Open.Add(y, GetFScore(y));
                        newIsBetter = true;
                    }
                    else if (newGValue < GScore[y])
                        newIsBetter = true;


                    if (newIsBetter)
                    {
                        if (ComeFrom.ContainsKey(y))
                            ComeFrom[y] = x;
                        else
                            ComeFrom.Add(y, x);

                        GScore[y] = newGValue;
                        FScore[y] = GScore[y] + GetHeuristic(y);
                    }

                }
            }

            return null;
        }

        private int Start;
        private int End;


        private IList<int> GetNodesAround(int pos)
        {
            List<int> list = new List<int>(4);
            int x = pos % Width; int y = pos / Width;
            // Left
            if (x > 0 && !Map[x - 1 + y * Width]) list.Add(x - 1 + y * Width);
            // Up
            if (y > 0 && !Map[x + (y - 1) * Width]) list.Add(x + (y - 1) * Width);
            // Right
            if (x < Width - 1 && !Map[x + 1 + y * Width]) list.Add(x + 1 + y * Width);
            // Down
            if (y < Height - 1 && !Map[x + (y + 1) * Width]) list.Add(x + (y + 1) * Width);

            return list;
        }

        private int GetCost(int current)
        {
            // UNDONE
            int xDistance = (int)Math.Abs(Start % Width - current % Width);
            int yDistance = (int)Math.Abs(Start / Width - current / Width);

            if (!GScore.ContainsKey(current))
                GScore.Add(current, (xDistance + yDistance) * 10);

            return GScore[current];
        }

        private int GetLowestF()
        {
            int lowest = -1;
            int temp = 0;
            foreach (var item in Open)
            {
                if (lowest == -1 || temp > item.Value)
                {
                    lowest = item.Key;
                    temp = item.Value;
                }
            }

            return lowest;
        }

        private int GetFScore(int pos)
        {
            if (!FScore.ContainsKey(pos))
                FScore.Add(pos, GetCost(pos) + GetHeuristic(pos));

            return FScore[pos];
        }

        private Dictionary<int, int> GScore;
        private Dictionary<int, int> FScore;
        private Dictionary<int, int> ComeFrom;

        // 得到预估的距离
        private int GetHeuristic(int current)
        {
            return GetDistance(current, End);
        }

        private int GetDistance(int x, int y)
        {
            int xDistance = (int)Math.Abs(y % Width - x % Width);
            int yDistance = (int)Math.Abs(y / Width - x / Width);

            return 10 * (xDistance + yDistance);

        }
    }
    #endregion

    #region 采用BinaryHeap的排序(堆排序)

    /// <summary>
    /// A* 最短路径搜索
    /// </summary>
    public class AStarBinaryHeapSearch
    {
        private List<bool> Map;
        private int Width;
        private int Height;
        private bool IsValidPosition(int start)
        {
            return start >= 0 && start <= Map.Count;
        }
        public AStarBinaryHeapSearch(List<bool> map, int Width)
        {
            if (map == null) throw new ArgumentNullException();

            Map = map;
            this.Width = Width;
            this.Height = Map.Count / Width;
        }

        private BinaryHeap<int> Open;
        private BinaryHeap<int> Close;

        public Dictionary<int, int> FindPath(int start, int end)
        {
            if (!IsValidPosition(start) || !IsValidPosition(end)) throw new ArgumentOutOfRangeException();
            this.Start = start; this.End = end;
            Open = new BinaryHeap<int>();
            Close = new BinaryHeap<int>();
            GScore = new Dictionary<int, int>();
            FScore = new Dictionary<int, int>();
            ComeFrom = new Dictionary<int, int>();

            // 将开始节点入队列
            Open.Add(start);

            int x = start;
            while (!Open.IsEmpty)
            {
                x = Open.GetTop();
                if (x == End)
                {
                    // Trace From
                    return ComeFrom;
                }

                Close.Add(x);

                foreach (int y in GetNodesAround(x))
                {
                    if (Close.Contains(y))
                    {
                        continue;
                    }

                    int newGValue = GetCost(x) + GetDistance(x, y);
                    bool newIsBetter = false;

                    if (!Open.Contains(y))
                    {
                        Open.Add(y);
                        newIsBetter = true;
                    }
                    else if (newGValue < GScore[y])
                        newIsBetter = true;


                    if (newIsBetter)
                    {
                        if (ComeFrom.ContainsKey(y))
                            ComeFrom[y] = x;
                        else
                            ComeFrom.Add(y, x);

                        GScore[y] = newGValue;
                        FScore[y] = GScore[y] + GetHeuristic(y);
                    }

                }
            }

            return null;
        }

        private int Start;
        private int End;


        private IList<int> GetNodesAround(int pos)
        {
            List<int> list = new List<int>(4);
            int x = pos % Width; int y = pos / Width;
            // Left
            if (x > 0 && !Map[x - 1 + y * Width]) list.Add(x - 1 + y * Width);
            // Up
            if (y > 0 && !Map[x + (y - 1) * Width]) list.Add(x + (y - 1) * Width);
            // Right
            if (x < Width - 1 && !Map[x + 1 + y * Width]) list.Add(x + 1 + y * Width);
            // Down
            if (y < Height - 1 && !Map[x + (y + 1) * Width]) list.Add(x + (y + 1) * Width);

            return list;
        }

        private int GetCost(int current)
        {
            // UNDONE
            int xDistance = (int)Math.Abs(Start % Width - current % Width);
            int yDistance = (int)Math.Abs(Start / Width - current / Width);

            if (!GScore.ContainsKey(current))
                GScore.Add(current, (xDistance + yDistance) * 10);

            return GScore[current];
        }

        private int GetLowestF()
        {
            return Open.GetTop();
        }

        private int GetFScore(int pos)
        {
            if (!FScore.ContainsKey(pos))
                FScore.Add(pos, GetCost(pos) + GetHeuristic(pos));

            return FScore[pos];
        }

        private Dictionary<int, int> GScore;
        private Dictionary<int, int> FScore;
        private Dictionary<int, int> ComeFrom;

        // 得到预估的距离
        private int GetHeuristic(int current)
        {
            return GetDistance(current, End);
        }

        private int GetDistance(int x, int y)
        {
            int xDistance = (int)Math.Abs(y % Width - x % Width);
            int yDistance = (int)Math.Abs(y / Width - x / Width);

            return 10 * (xDistance + yDistance);

        }
    }
    #endregion
}