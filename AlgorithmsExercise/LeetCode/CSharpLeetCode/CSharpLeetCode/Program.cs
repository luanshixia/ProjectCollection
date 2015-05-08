﻿using System;
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
            var sol = new Solution();

            //sol.Rotate(new[] { 1, 2 }, 1);

            //for (int i = 2; i < 100; i++)
            //    Console.WriteLine(sol.GetHappyListString(i));

            var list = new LinkedList();
            list.add(1);
            list.add(2);
            list.add(2);
            list.add(1);
            Console.WriteLine(list);
            list.head = sol.RemoveElements(list.head, 2);
            Console.WriteLine(list);

            Console.ReadLine();
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
        // 203 - Remove Linked List Elements
        //
        public ListNode RemoveElements(ListNode head, int val)
        {
            while (head != null && head.val == val)
            {
                head = head.next;
            }
            ListNode cur = head;
            while (cur != null && cur.next != null)
            {
                if (cur.next.val == val)
                {
                    cur.next = cur.next.next;
                }
                else // !***! "cur = cur.next" should be wrapped in "else"
                {
                    cur = cur.next;
                }
            }
            return head;
        }

        //
        // 202 - Happy Number
        //
        public bool IsHappy(int n)
        {
            return GetHappyList(n).Last() == 1;
        }

        public string GetHappyListString(int n)
        {
            return string.Join(",", GetHappyList(n));
        }

        private static int SumSqDigits(int n)
        {
            return n.ToString().Sum(c =>
            {
                int d = Convert.ToInt32(c.ToString());
                return d * d;
            });
        }

        private static List<int> GetHappyList(int n)
        {
            var set = new HashSet<int>();
            var list = new List<int>();
            while (!set.Contains(n) && n != 1)
            {
                set.Add(n);
                list.Add(n);
                n = SumSqDigits(n);
            }
            list.Add(n);
            return list;
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

        //
        // 198 - House Robber
        //
        public int Rob(int[] nums)
        {
            int N = nums.Length;
            if (N == 0) return 0;
            else if (N == 1) return nums[0];
            int[] dp = new int[N];
            dp[0] = nums[0];
            dp[1] = Math.Max(nums[0], nums[1]);
            for (int i = 2; i < N; i++)
            {
                dp[i] = Math.Max(dp[i - 1], dp[i - 2] + nums[i]);
            }
            return dp[N - 1];
        }

        //
        // 191 - Number of 1 Bits
        //
        public int HammingWeight(uint n)
        {
            int ones = 0;
            while (n > 0)
            {
                uint m = n >> 1 << 1;
                if (m != n)
                {
                    ones++;
                }
                n = n >> 1;
            }
            return ones;
        }

        //
        // 190 - Reverse Bits
        //
        public uint reverseBits(uint n)
        {
            uint m = 0;
            for (int i = 0; i < 32; i++)
            {
                m = (m << 1) | (n ^ (n >> 1 << 1));
                n >>= 1;
            }
            return m;
        }

        //
        // 189 - Rotate Array
        //
        public void Rotate1(int[] nums, int k)
        {
            k = k % nums.Length;
            for (int i = 0; i < k; i++)
            {
                ShiftRightOneStep(nums);
            }
        }

        private static void ShiftRightOneStep(int[] nums)
        {
            int N = nums.Length;
            int last = nums[N - 1];
            for (int i = N - 1; i > 0; i--)
            {
                nums[i] = nums[i - 1];
            }
            nums[0] = last;
        }

        // In-place with O(1) extra space; O(N) time.
        public void Rotate(int[] nums, int k)
        {
            int N = nums.Length;
            k = k % N;
            int count = 0;
            int i0 = N - 1;
            int i;
            int j;
            int temp;
            while (count < N)
            {
                i = i0;
                j = (i - k + N) % N;
                temp = nums[i];
                while (j != i0)
                {
                    nums[i] = nums[j];
                    count++;
                    j = (j - k + N) % N;
                    i = (i - k + N) % N;
                }
                nums[i] = temp;
                count++;
                i0 = (i0 - 1 + N) % N;
            }
        }

        //
        // 187 - Repeated DNA Sequences
        //
        public IList<string> FindRepeatedDnaSequences1(string s)
        {
            var set = new HashSet<string>();
            if (s.Length >= 10)
            {
                var store = new[] { 'A', 'C', 'G', 'T' }.ToDictionary(c => c, c => new HashSet<string>());
                for (int i = 0; i <= s.Length - 10; i++)
                {
                    var sub = s.Substring(i, 10);
                    if (store[sub[0]].Contains(sub))
                    {
                        set.Add(sub);
                    }
                    store[sub[0]].Add(sub); // too much memory!
                }
            }
            return set.ToList();
        }

        private static char[] _dnaChars = new[] { 'A', 'C', 'G', 'T' };

        // Divide-and-Conquer
        public IList<string> FindRepeatedDnaSequences(string s)
        {
            var result = new List<string>();
            if (s.Length >= 10)
            {
                var store = _dnaChars.ToDictionary(c => c, c => new List<int>());
                for (int i = 0; i <= s.Length - 10; i++)
                {
                    store[s[i]].Add(i);
                }
                foreach (char c in _dnaChars)
                {
                    if (store[c].Count > 1)
                    {
                        FindRepeatedDnaSequencesWithPrefix(s, c.ToString(), store[c].ToArray(), result);
                    }
                }
            }
            return result;
        }

        private static void FindRepeatedDnaSequencesWithPrefix(string s, string prefix, int[] indices, IList<string> result)
        {
            if (prefix.Length == 10)
            {
                if (indices.Length > 1)
                {
                    result.Add(prefix);
                }
            }
            else // prefix.Length < 10
            {
                var store = _dnaChars.ToDictionary(c => c, c => new List<int>());
                foreach (int i in indices)
                {
                    store[s[i + prefix.Length]].Add(i);
                }
                foreach (char c in _dnaChars)
                {
                    if (store[c].Count > 1)
                    {
                        FindRepeatedDnaSequencesWithPrefix(s, prefix + c, store[c].ToArray(), result);
                    }
                }
            }
        }
    }

    public class LinkedList
    {
        public ListNode head;
        public ListNode tail;

        public void add(int val)
        {
            ListNode node = new ListNode(val);
            if (head == null)
            {
                head = node;
            }
            else
            {
                tail.next = node;
            }
            tail = node;
        }

        public List<int> ToList()
        {
            ListNode cur = head;
            List<int> list = new List<int>();
            while (cur != null)
            {
                list.Add(cur.val);
                cur = cur.next;
            }
            return list;
        }

        public override string ToString()
        {
            return "{" + string.Join(",", this.ToList()) + "}";
        }
    }

    public class ListNode
    {
        public int val;
        public ListNode next;
        public ListNode(int x) { val = x; }
    }

    public class TreeNode
    {
        public int val;
        public TreeNode left;
        public TreeNode right;
        public TreeNode(int x) { val = x; }
    }
}
