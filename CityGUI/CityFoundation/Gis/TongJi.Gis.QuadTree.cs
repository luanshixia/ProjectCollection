using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TongJi.Geometry;

namespace TongJi.Gis
{
    public class QuadTree<T>
    {
        public QuadTreeNode<T> Root { get; protected set; }

        public QuadTree(Extent2D extents)
        {
            Root = new QuadTreeNode<T>(extents);
        }

        public List<T> PointQuery(Point2D point)
        {
            return Root.PointQuery(point);
        }

        //public List<T> WindowQuery(Extent2D extents)
        //{

        //}

        //public List<T> CrossQuery(Extent2D extents)
        //{

        //}
    }

    public class QuadTreeNode<T>
    {
        public Extent2D Extents { get; protected set; }
        public bool IsLeaf { get; protected set; }
        public Dictionary<Quad, QuadTreeNode<T>> Children { get; protected set; }
        public Dictionary<Quad, Extent2D> ChildrenExtents { get; protected set; }
        public Dictionary<T, Extent2D> Data { get; protected set; }
        public const int SplitThreshold = 1000;
        private static Quad[] _quads = { Quad.Child1, Quad.Child2, Quad.Child3, Quad.Child4 };

        public QuadTreeNode(Extent2D extents)
        {
            Extents = extents;
            IsLeaf = true;
            Children = new Dictionary<Quad, QuadTreeNode<T>>();
            ChildrenExtents = new Dictionary<Quad, Extent2D>();
            Data = new Dictionary<T, Extent2D>();

            _quads.ForEach(q => ChildrenExtents[q] = GetChildExtents(extents, q));
        }

        protected void Split()
        {
            _quads.ForEach(q => Children[q] = new QuadTreeNode<T>(ChildrenExtents[q]));
            IsLeaf = false;
        }

        public void Insert(T item, Extent2D extents)
        {
            if (IsLeaf)
            {
                Data[item] = extents;
                if (Data.Count > SplitThreshold)
                {
                    Split();
                }
            }
            else
            {
                bool inserted = false;
                foreach (var quad in ChildrenExtents)
                {
                    if (quad.Value.IsExtentsIn(extents))
                    {
                        Children[quad.Key].Insert(item, extents);
                        inserted = true;
                        break;
                    }
                }
                if (!inserted)
                {
                    Data[item] = extents;
                }
            }
        }

        public Dictionary<T, Extent2D> GetFlattenedData()
        {
            return Children.Values.SelectMany(x => x.Data).Union(Data).ToDictionary(x => x.Key, x => x.Value);
        }

        public List<T> PointQuery(Point2D point)
        {
            List<T> result = new List<T>();
            result.AddRange(Data.Keys);
            if (!IsLeaf)
            {
                if (ChildrenExtents.Any(x => x.Value.IsPointIn(point)))
                {
                    var child = Children[ChildrenExtents.First(x => x.Value.IsPointIn(point)).Key];
                    result.AddRange(child.PointQuery(point));
                }
            }
            return result;
        }

        public static Extent2D GetChildExtents(Extent2D extents, Quad pos)
        {
            Extent2D result = extents;
            if (pos == Quad.Child1)
            {
                result = new Extent2D(extents.min.x, extents.min.y + extents.YRange / 2, extents.max.x - extents.XRange / 2, extents.max.y);
            }
            else if (pos == Quad.Child2)
            {
                result = new Extent2D(extents.min.x + extents.XRange / 2, extents.min.y + extents.YRange / 2, extents.max.x, extents.max.y);
            }
            else if (pos == Quad.Child3)
            {
                result = new Extent2D(extents.min.x, extents.min.y, extents.max.x - extents.XRange / 2, extents.max.y - extents.XRange / 2);
            }
            else if (pos == Quad.Child4)
            {
                result = new Extent2D(extents.min.x + extents.XRange / 2, extents.min.y, extents.max.x, extents.max.y - extents.XRange / 2);
            }
            return result;
        }
    }

    public enum Quad
    {
        All,
        Child1,  // NW
        Child2,  // NE
        Child3,  // SW
        Child4   // SE
    }
}
