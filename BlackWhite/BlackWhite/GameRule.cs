using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackWhite
{
    // 表示棋子颜色和棋盘格状态
    public enum Piece
    {
        Empty,
        Black,
        White
    }

    // 表示棋盘状态
    public class Board
    {
        // 棋盘的边长
        public int Size { get; private set; }

        // 不用二维数组，而是自己实现二维索引器。好处：1.数据对外只读；2.可用Linq
        private Piece[] data;

        public Board(int size)
        {
            Size = size;
            data = Enumerable.Repeat(Piece.Empty, size * size).ToArray();

            int center = (size - 1) / 2;
            this[center, center] = Piece.Black;
            this[center + 1, center + 1] = Piece.Black;
            this[center + 1, center] = Piece.White;
            this[center, center + 1] = Piece.White;
        }

        public Piece this[int row, int col]
        {
            get
            {
                return data[row * Size + col];
            }
            private set
            {
                data[row * Size + col] = value;
            }
        }

        public Piece this[Tuple<int, int> pos]
        {
            get
            {
                return data[pos.Item1 * Size + pos.Item2];
            }
            private set
            {
                data[pos.Item1 * Size + pos.Item2] = value;
            }
        }

        // 获取走一步棋后所吃掉的对方棋子数，并走这步棋。
        public int StepReal(Piece color, Tuple<int, int> pos)
        {
            Piece opponent = color == Piece.Black ? Piece.White : Piece.Black;

            if (this[pos] != Piece.Empty || color == Piece.Empty)
            {
                return 0;
            }
            else
            {
                // 落子
                this[pos] = color;

                // 转换颜色
                var cells = Get8DirCells(pos);
                int sum = 1;
                foreach (var dir in cells)
                {
                    var pieces = dir.Select(x => this[x]).ToList();
                    int sameColorIndex = pieces.IndexOf(color);
                    if (sameColorIndex > 0)
                    {
                        if (pieces.Part(Enumerable.Range(0, sameColorIndex)).All(x => x == opponent))
                        {
                            dir.Part(Enumerable.Range(0, sameColorIndex)).ToList().ForEach(x => this[x] = color); // 吃子了！
                            sum += sameColorIndex;
                        }
                    }
                }
                return sum;
            }
        }

        // 获取走一步棋后所吃掉的对方棋子数，而不真正走棋。
        public int StepVirtual(Piece color, Tuple<int, int> pos)
        {
            Piece opponent = color == Piece.Black ? Piece.White : Piece.Black;

            if (this[pos] != Piece.Empty || color == Piece.Empty)
            {
                return 0;
            }
            else
            {
                var cells = Get8DirCells(pos);
                int sum = 1;
                foreach (var dir in cells)
                {
                    var pieces = dir.Select(x => this[x]).ToList();
                    int sameColorIndex = pieces.IndexOf(color);
                    if (sameColorIndex > 0)
                    {
                        if (pieces.Part(Enumerable.Range(0, sameColorIndex)).All(x => x == opponent))
                        {
                            sum += sameColorIndex;
                        }
                    }
                }
                return sum;
            }
        }

        // 获取八个方向的所有棋盘格编号，按方向组织
        private List<List<Tuple<int, int>>> Get8DirCells(Tuple<int, int> pos)
        {
            List<List<Tuple<int, int>>> result = new List<List<Tuple<int, int>>>();

            foreach (var dir in Directions.GetAll())
            {
                List<Tuple<int, int>> dirResult = new List<Tuple<int, int>>();
                var temp = pos;
                while (temp.Item1 < Size && temp.Item2 < Size && temp.Item1 >= 0 && temp.Item2 >= 0)
                {
                    temp = GetNext(temp, dir);
                    if (temp.Item1 < Size && temp.Item2 < Size && temp.Item1 >= 0 && temp.Item2 >= 0)
                    {
                        dirResult.Add(temp);
                    }
                }
                result.Add(dirResult);
            }

            return result;
        }

        private Tuple<int, int> GetNext(Tuple<int, int> pos, Tuple<int, int> offset)
        {
            return Tuple.Create(pos.Item1 + offset.Item1, pos.Item2 + offset.Item2);
        }

        // 获取棋盘上一种格子的数量
        public int GetCount(Piece colorKind)
        {
            return data.Count(x => x == colorKind);
        }

        // 获取赢家
        public Piece GetWinner()
        {
            if (GetCount(Piece.Empty) > 0)
            {
                return Piece.Empty;
            }
            else
            {
                if (GetCount(Piece.Black) > GetCount(Piece.White))
                {
                    return Piece.Black;
                }
                else
                {
                    return Piece.White;
                }
            }
        }

        // 获取一方当前能落子的位置
        public List<Tuple<int, int>> GetPossibleSteps(Piece color)
        {
            List<Tuple<int, int>> result = new List<Tuple<int, int>>();
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (this[i, j] == Piece.Empty)
                    {
                        var pos = Tuple.Create(i, j);
                        if (StepVirtual(color, pos) > 1)
                        {
                            result.Add(pos);
                        }
                    }
                }
            }
            return result;
        }
    }

    // 定义8个方向
    public static class Directions
    {
        public static Tuple<int, int> Up = Tuple.Create(-1, 0);
        public static Tuple<int, int> Down = Tuple.Create(1, 0);
        public static Tuple<int, int> Left = Tuple.Create(0, -1);
        public static Tuple<int, int> Right = Tuple.Create(0, 1);
        public static Tuple<int, int> UpLeft = Tuple.Create(-1, -1);
        public static Tuple<int, int> UpRight = Tuple.Create(-1, 1);
        public static Tuple<int, int> DownLeft = Tuple.Create(1, -1);
        public static Tuple<int, int> DownRight = Tuple.Create(1, 1);

        private static List<Tuple<int, int>> _all = new List<Tuple<int, int>> { Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight };
        public static List<Tuple<int, int>> GetAll()
        {
            return _all;
        }
    }

    public static class Utils
    {
        // 自定义一个Linq运算符，用来根据索引值取子集。
        public static IEnumerable<T> Part<T>(this IEnumerable<T> source, IEnumerable<int> indices)
        {
            foreach (int index in indices)
            {
                yield return source.ElementAt(index);
            }
        }
    }
}
