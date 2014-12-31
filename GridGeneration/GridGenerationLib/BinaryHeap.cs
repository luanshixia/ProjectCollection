using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roger.Testing
{
    public class BinaryHeap<T> where T : IComparable
    {
        protected List<T> MyList;

        public BinaryHeap()
        {
            MyList = new List<T>();
        }
        public BinaryHeap(int capbility)
        {
            MyList = new List<T>(capbility);
        }

        public bool IsEmpty
        {
            get {
                return MyList.Count < 1;
            }
        }

        public T GetTop()
        {
            if (MyList.Count < 1) throw new IndexOutOfRangeException();
            T t = MyList[0];
            MyList[0] = MyList[MyList.Count - 1];
            MyList.RemoveAt(MyList.Count - 1);

            CheckChildren(0);

            return t;
        }

        public void Add(T t)
        {
            MyList.Add(t);

            CheckParent(MyList.Count - 1);
        }


        public void Clear()
        {
            MyList.Clear();
        }

        public bool Contains(T item)
        {
            return MyList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            MyList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 比较和父亲关系
        ///     3
        ///    / \
        ///   2   4
        /// </summary>
        /// <param name="pos"></param>
        private void CheckParent(int pos)
        {
            if (pos < 0 || pos > MyList.Count - 1) throw new IndexOutOfRangeException();

            int parent = pos < 2 ? 0 : (int)(pos / 2) - 1;

            while (pos != 0 && parent >= 0)
            {
                if (MyList[parent].CompareTo(MyList[pos]) > 0)
                {
                    Swap(pos,parent);
                    pos = parent;
                    parent = (int)(pos / 2) - 1;
                }
                else break;
            }
        }

        private void CheckChildren(int pos)
        {
            if (pos < 0) throw new IndexOutOfRangeException();

            int left = -1, right = -1;
            left = (pos + 1) * 2 - 1;
            right = left + 1;

            //int childIndex = -1;
            while (left <= MyList.Count-1)
            {
                bool hasFound = false;
                right = left + 1;
                if (right >= MyList.Count) { right = left; left--; }
                if (MyList[pos].CompareTo(MyList[left]) > 0 && MyList[left].CompareTo(MyList[right]) < 0)
                {
                    Swap(pos, left);
                    pos = left;
                    hasFound = true;
                } 
                if (MyList[pos].CompareTo(MyList[right]) > 0 && MyList[right].CompareTo(MyList[left]) < 0)
                {
                    Swap(pos, right);
                    pos = right;
                    hasFound = true;
                }

                if (!hasFound) return;

                left = (pos + 1) * 2 - 1;
                

            }
        }

        private void Swap(int x, int y)
        {
            T z = MyList[x];
            MyList[x] = MyList[y];
            MyList[y] = z;
        
            
        }

        public int Count
        {
            get { return MyList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return MyList.Remove(item);
        }


        public IEnumerator<T> GetEnumerator()
        {
            return MyList.GetEnumerator();
        }

    }

    public class BinaryTree<T> : ICollection<T>
    {

        #region ICollection<T> Members

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(T item)
        {
            return false;
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
