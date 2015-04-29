using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLeetCode
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    public class Solution
    {
        //
        // 204 - Count Primes
        //
        public int CountPrimes(int n)
        {
            bool[] sieve = Enumerable.Repeat(true, n).ToArray();
            if (n > 0) sieve[0] = false;
            if (n > 1) sieve[1] = false;
            for (int i = 2; i * i < n; i++)
            {
                if (sieve[i])
                {
                    for (int k = i * i; k < n; k += i)
                    {
                        sieve[k] = false;
                    }
                }
            }
            return sieve.Count(x => x);
        }

        //
        // 201 - Bitwise AND of Numbers Range
        //
        public int RangeBitwiseAnd(int m, int n)
        {
            if (m == 0 || n == 0)
            {
                return 0;
            }
            int a = (int)Math.Log(m, 2);
            int b = (int)Math.Log(n, 2);
            if (b != a)
            {
                return 0;
            }
            return Enumerable.Range(m, n - m + 1).Aggregate((x, y) => x & y);
        }

        //
        // 199 - Binary Tree Right Side View
        //
        //public IList<int> RightSideView(TreeNode root)
        //{
        //    Dictionary<int, List<int>> rows = new Dictionary<int, List<int>>();
        //    Traverse(rows, root, 0);
        //    return rows.Select(r => r.Value.Last()).ToList();
        //}

        //private static void Traverse(Dictionary<int, List<int>> rows, TreeNode node, int level)
        //{
        //    if (node == null)
        //    {
        //        return;
        //    }
        //    if (!rows.ContainsKey(level))
        //    {
        //        rows[level] = new List<int>();
        //    }
        //    rows[level].Add(node.val);
        //    if (node.left != null)
        //    {
        //        Traverse(rows, node.left, level + 1);
        //    }
        //    if (node.right != null)
        //    {
        //        Traverse(rows, node.right, level + 1);
        //    }
        //}

        public IList<int> RightSideView(TreeNode root)
        {
            Dictionary<int, int> rows = new Dictionary<int, int>();
            Traverse(rows, root, 0);
            return rows.Select(r => r.Value).ToList();
        }

        private static void Traverse(Dictionary<int, int> rows, TreeNode node, int level)
        {
            if (node == null)
            {
                return;
            }
            rows[level] = node.val;
            if (node.left != null)
            {
                Traverse(rows, node.left, level + 1);
            }
            if (node.right != null)
            {
                Traverse(rows, node.right, level + 1);
            }
        }
    }

    public class TreeNode
    {
        public int val;
        public TreeNode left;
        public TreeNode right;
        public TreeNode(int x) { val = x; }
    }
}
