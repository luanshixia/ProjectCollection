using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BinaryFormatter = System.Runtime.Serialization.Formatters.Binary.BinaryFormatter;

namespace Algorithm
{
    [Serializable]
    public struct Node
    {
        public int Row;
        public int Column;

        public Node(int row, int col)
        {
            Row = row;
            Column = col;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", Row, Column);
        }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    [Serializable]
    public enum CellType
    {
        Wall,
        Space,
        HomeSpace
    }

    [Serializable]
    public class Cell
    {
        private Node _pos;
        /// <summary>
        /// 表示方格位置。
        /// </summary>
        public Node Position { get { return _pos; } }
        private bool _isWall;
        /// <summary>
        /// 表示方格是否为墙壁。
        /// </summary>
        public bool IsWall { get { return _isWall; } }
        private bool _isHome;
        /// <summary>
        /// 在IsWall为False时有效，表示是否为目标方格。
        /// </summary>
        public bool IsHome { get { return _isHome; } }
        private bool _hasBoxOrPorter;
        /// <summary>
        /// 在IsWall为False时有效，表示空位是否被占据。
        /// </summary>
        public bool HasBoxOrPorter { get { return _hasBoxOrPorter; } }
        private bool _hasBox;
        /// <summary>
        /// 在IsWall为False且HasBoxOrPorter为True时有效，表示占位的是否为箱子。
        /// </summary>
        public bool HasBox { get { return _hasBox; } }
        /// <summary>
        /// 表示是否是有箱子的目标方格。
        /// </summary>
        public bool IsBoxAtHome { get { return _isHome && _hasBox; } }
        /// <summary>
        /// 表示是否是可进入的区域。
        /// </summary>
        public bool IsEmpty { get { return !(_isWall || _hasBoxOrPorter); } }
        private MovableBase _boxOrPorter;
        /// <summary>
        /// 获取方格上的可动物件。
        /// </summary>
        public MovableBase BoxOrPorter { get { return _boxOrPorter; } }

        public Cell(int i, int j)
        {
            _pos = new Node(i, j);
        }

        public void SetCellTo(CellType type)
        {
            switch (type)
            {
                case CellType.Wall: _isWall = true; _isHome = false; break;
                case CellType.Space: _isWall = false; _isHome = false; break;
                case CellType.HomeSpace: _isWall = false; _isHome = true; break;
            }
        }

        public void SetMovable(MovableBase boxOrPorter)
        {
            _boxOrPorter = boxOrPorter;
            _hasBoxOrPorter = true;
            if (boxOrPorter is Box)
            {
                _hasBox = true;
            }
            else
            {
                _hasBox = false;
            }
        }

        public void ClearMovable()
        {
            _boxOrPorter = null;
            _hasBoxOrPorter = false;
            _hasBox = false;
        }

        public override string ToString()
        {
            return _pos.ToString();
        }
    }

    [Serializable]
    public class Grid
    {
        private int _rows;
        private int _cols;
        public int Rows { get { return _rows; } }
        public int Columns { get { return _cols; } }
        public int Count { get { return _rows * _cols; } }
        private List<Cell> _cells;
        public List<Cell> Cells { get { return _cells; } }
        public Cell this[int i, int j]
        {
            get
            {
                return _cells[i * _cols + j];
            }
        }
        public Cell this[Node pos]
        {
            get
            {
                return _cells[pos.Row * _cols + pos.Column];
            }
        }
        private Porter _thePorter;
        public Porter ThePorter { get { return _thePorter; } }
        private List<Box> _boxes = new List<Box>();
        public List<Box> Boxes { get { return _boxes; } }

        public Grid(int rows, int cols)
        {
            _rows = rows;
            _cols = cols;
            _cells = new List<Cell>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    _cells.Add(new Cell(i, j));
                }
            }
        }

        public void SetCell(int i, int j, CellType type)
        {
            this[i, j].SetCellTo(type);
        }

        public void SetBorderToWall()
        {
            for (int i = 0; i < _rows; i++)
            {
                if (i == 0 || i == _rows - 1)
                {
                    for (int j = 0; j < _cols; j++)
                    {
                        this[i, j].SetCellTo(CellType.Wall);
                    }
                }
                else
                {
                    this[i, 0].SetCellTo(CellType.Wall);
                    this[i, _cols - 1].SetCellTo(CellType.Wall);
                }
            }
        }

        public void CreatePorter(int i, int j)
        {
            if (_thePorter != null)
            {
                this[_thePorter.Position].ClearMovable();
            }
            _thePorter = new Porter(this, i, j);
            this[i, j].SetMovable(_thePorter);
        }

        public void CreateBox(int i, int j)
        {
            Box aBox = new Box(this, i, j);
            _boxes.Add(aBox);
            this[i, j].SetMovable(aBox);
        }

        public void ClearMovable(int i, int j)
        {
            if (!this[i, j].HasBoxOrPorter) return;
            MovableBase boxOrPorter = this[i, j].BoxOrPorter;
            this[i, j].ClearMovable();
            if (boxOrPorter is Box)
            {
                Boxes.Remove(boxOrPorter as Box);
            }
            else
            {
                _thePorter = null;
            }
        }

        public bool MovePorter(Direction dir)
        {
            if (_thePorter == null) return false;
            if (_thePorter.Move(dir))
            {
                return true;
            }
            else
            {
                Node next = _thePorter.NextPosition(dir);
                Cell nextCell = this[next];
                if (nextCell.HasBox)
                {
                    if (nextCell.BoxOrPorter.Move(dir))
                    {
                        _thePorter.Move(dir);
                        return true;
                    }
                }
            }
            return false;
        }

        public void Serialize(string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create, 
                System.IO.FileAccess.Write, System.IO.FileShare.None))
            {
                bf.Serialize(fs, this);
            }
        }

        public static Grid Deserialize(string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (System.IO.Stream fs = System.IO.File.OpenRead(fileName))
            {
                return bf.Deserialize(fs) as Grid;
            }
        }
    }

    [Serializable]
    public class MovableBase
    {
        private Node _pos;
        public Node Position { get { return _pos; } }
        protected Grid _stage;

        public MovableBase(Grid stage, int i, int j)
        {
            _pos = new Node(i, j);
            _stage = stage;
        }

        public virtual bool Move(Direction dir)
        {
            Node next = NextPosition(dir);
            if (_stage[next].IsEmpty)
            {
                _stage[_pos].ClearMovable();
                _pos = next;
                _stage[next].SetMovable(this);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Node NextPosition(Direction dir)
        {
            Node result = new Node(0, 0);
            switch (dir)
            {
                case Direction.Up: result = new Node(_pos.Row - 1, _pos.Column); break;
                case Direction.Down: result = new Node(_pos.Row + 1, _pos.Column); break;
                case Direction.Left: result = new Node(_pos.Row, _pos.Column - 1); break;
                case Direction.Right: result = new Node(_pos.Row, _pos.Column + 1); break;
            }
            return result;
        }
    }

    [Serializable]
    public class Box : MovableBase
    {
        public Box(Grid stage, int i, int j)
            : base(stage, i, j)
        {
        }

        public bool IsAtHome
        {
            get
            {
                return _stage[Position].IsHome;
            }
        }
    }

    [Serializable]
    public class Porter : MovableBase
    {
        public Porter(Grid stage, int i, int j)
            : base(stage, i, j)
        {
        }
    }
}
