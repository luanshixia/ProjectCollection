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

            //var list = new LinkedList();
            //list.add(1);
            //list.add(2);
            //list.add(2);
            //list.add(1);
            //Console.WriteLine(list);
            //list.head = sol.RemoveElements(list.head, 2);
            //Console.WriteLine(list);

            //var node = new TreeNode(1);
            //var bsti = new Solution.BSTIterator(node);
            //if (bsti.HasNext())
            //{
            //    bsti.Next();
            //}

            Console.WriteLine(sol.TrailingZeroes(2147483647));

            Console.ReadLine();
        }
    }

    public class Solution
    {
        //
        // 231 - Power of Two
        //
        public bool IsPowerOfTwo(int n)
        {
            if (n <= 0)
            {
                return false;
            }
            return Math.Pow(2, (int)Math.Log(n, 2)) == n;
        }

        //
        // 220 - Contains Duplicate III
        //
        public bool ContainsNearbyAlmostDuplicate(int[] nums, int k, int t)
        {
            if (nums == null || nums.Length < 2 || k < 1 || t < 0)
            {
                return false;
            }
            var set = new SortedSet<long>();
            for (int i = 0; i < nums.Length; i++)
            {
                long num = nums[i];
                var sub = set.GetViewBetween(num - t, num + t); // use long to avoid overflow
                if (sub.Count > 0)
                {
                    return true;
                }
                if (i - k >= 0)
                {
                    set.Remove(nums[i - k]);
                }
                set.Add(num); // after remove!
            }
            return false;
        }

        //
        // 219 - Contains Duplicate II
        //
        public bool ContainsNearbyDuplicate(int[] nums, int k)
        {
            var dict = new Dictionary<int, int>();
            for (int i = 0; i < nums.Length; i++)
            {
                int num = nums[i];
                if (dict.ContainsKey(num) && i - dict[num] <= k)
                {
                    return true;
                }
                dict[num] = i;
            }
            return false;
        }

        //
        // 217 - Contains Duplicate
        //
        public bool ContainsDuplicate(int[] nums)
        {
            return nums.Distinct().Count() != nums.Length;
        }

        //
        // 206 - Reverse Linked List
        //
        public ListNode ReverseList(ListNode head)
        {
            var cur = head;
            ListNode prev = null;
            ListNode next = null;
            while (cur != null)
            {
                next = cur.next;
                cur.next = prev;
                prev = cur;
                cur = next;
            }
            return prev;
        }

        //
        // 205 - Isomorphic Strings
        //
        public bool IsIsomorphic(string s, string t)
        {
            int n = s.Length;
            if (t.Length != n)
            {
                return false;
            }
            var dict = new Dictionary<char, char>();
            var set = new HashSet<char>();
            for (int i = 0; i < n; i++)
            {
                if (dict.ContainsKey(s[i]))
                {
                    if (dict[s[i]] != t[i])
                    {
                        return false;
                    }
                }
                else
                {
                    if (set.Contains(t[i]))
                    {
                        return false;
                    }
                    else
                    {
                        dict[s[i]] = t[i];
                        set.Add(t[i]);
                    }
                }
            }
            return true;
        }

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

        //
        // 179 - Largest Number
        //
        public string LargestNumber1(int[] nums) // Wrong - suppliment '9's
        {
            Array.Sort(nums, (x, y) =>
            {
                int digitCountX = (int)Math.Floor(Math.Log10(x) + 1);
                int digitCountY = (int)Math.Floor(Math.Log10(y) + 1);
                if (digitCountX > digitCountY)
                {
                    y = (y + 1) * (int)Math.Pow(10, digitCountX - digitCountY) - 1;
                }
                else if (digitCountY > digitCountX)
                {
                    x = (x + 1) * (int)Math.Pow(10, digitCountY - digitCountX) - 1;
                }
                return y - x;
            });
            return string.Join(string.Empty, nums);
        }

        public string LargestNumber(int[] nums)
        {
            Array.Sort(nums, (x, y) =>
            {
                if (x == 0 || y == 0)
                {
                    return y - x;
                }
                int digitCountX = (int)Math.Floor(Math.Log10(x) + 1);
                int digitCountY = (int)Math.Floor(Math.Log10(y) + 1);
                int yx = y * (int)Math.Pow(10, digitCountX) + x;
                int xy = x * (int)Math.Pow(10, digitCountY) + y;
                return yx - xy;
            });
            string result = string.Join(string.Empty, nums);
            if (result.All(c => c == '0'))
            {
                return "0";
            }
            return result;
        }

        //
        // 174 - Dungeon Game
        //
        public int CalculateMinimumHP(int[,] dungeon)
        {
            int M = dungeon.GetLength(0);
            int N = dungeon.GetLength(1);
            int[,] solution = new int[M, N];
            for (int i = M - 1; i >= 0; i--)
            {
                for (int j = N - 1; j >= 0; j--)
                {
                    if (i == M - 1 && j == N - 1)
                    {
                        solution[i, j] = Math.Max(1, 1 - dungeon[i, j]);
                    }
                    else if (i == M - 1)
                    {
                        solution[i, j] = Math.Max(1, solution[i, j + 1] - dungeon[i, j]);
                    }
                    else if (j == N - 1)
                    {
                        solution[i, j] = Math.Max(1, solution[i + 1, j] - dungeon[i, j]);
                    }
                    else
                    {
                        solution[i, j] = Math.Max(1, Math.Min(solution[i, j + 1], solution[i + 1, j]) - dungeon[i, j]);
                    }
                }
            }
            return solution[0, 0];
        }

        //
        // 173 - Binary Search Tree Iterator
        //
        public class BSTIterator
        {
            private Stack<TreeNode> _nodes = new Stack<TreeNode>(); // left path

            public BSTIterator(TreeNode root)
            {
                while (root != null)
                {
                    _nodes.Push(root);
                    root = root.left;
                }
            }

            /** @return whether we have a next smallest number */
            public bool HasNext()
            {
                return _nodes.Count > 0;
            }

            /** @return the next smallest number */
            public int Next()
            {
                var node = _nodes.Pop();
                var result = node.val;
                if (node.right != null)
                {
                    node = node.right;
                    while (node != null)
                    {
                        _nodes.Push(node);
                        node = node.left;
                    }
                }
                return result;
            }
        }

        //
        // 172 - Factorial Trailing Zeroes
        //
        public int TrailingZeroes1(int n)
        {
            double log = Math.Log(n, 5);
            int result = 0;
            int temp = 1;
            for (int i = 1; i <= log; i++)
            {
                temp *= 5;
                result += n / temp;
            }
            return result;
        }

        public int TrailingZeroes(int n)
        {
            int result = 0;
            for (long temp = 5; temp <= n; temp *= 5) // mind for overflow
            {
                result += n / (int)temp;
            }
            return result;
        }

        //
        // 171 - Excel Sheet Column Number
        //
        public int TitleToNumber(string s)
        {
            int result = 0;
            int weight = 1;
            for (int i = 0; i < s.Length; i++)
            {
                result += (s[s.Length - i - 1] - 'A' + 1) * weight;
                weight *= 26;
            }
            return result;
        }

        //
        // 169 - Majority Element
        //
        public int MajorityElement(int[] nums)
        {
            Array.Sort(nums);
            return nums[nums.Length / 2];
        }

        //
        // 168 - Excel Sheet Column Title
        //
        public string ConvertToTitle(int n)
        {
            string result = string.Empty;
            while (n > 0)
            {
                int remainder = (n - 1) % 26 + 1;
                result = (char)('A' + remainder - 1) + result;
                n = (n - remainder) / 26;
            }
            return result;
        }

        //
        // 165 - Compare Version Numbers
        //
        public int CompareVersion(string version1, string version2)
        {
            if (version1 == version2)
            {
                return 0;
            }
            var v1 = version1.Split('.').Select(x => Convert.ToInt32(x)).ToList();
            var v2 = version2.Split('.').Select(x => Convert.ToInt32(x)).ToList();
            if (v1.Count > v2.Count)
            {
                for (int i = 0; i < v1.Count - v2.Count; i++)
                {
                    v2.Add(0);
                }
            }
            else if (v2.Count > v1.Count)
            {
                for (int i = 0; i < v2.Count - v1.Count; i++)
                {
                    v1.Add(0);
                }
            }
            for (int i = 0; i < v1.Count; i++)
            {
                if (v1[i] > v2[i])
                {
                    return 1;
                }
                if (v1[i] < v2[i])
                {
                    return -1;
                }
            }
            return 0;
        }

        //
        // 164 - Maximum Gap
        //
        //public int MaximumGap(int[] nums) // wrong
        //{
        //    if (nums.Length < 2)
        //    {
        //        return 0;
        //    }
        //    int left = Math.Min(nums[0], nums[1]);
        //    int right = Math.Max(nums[0], nums[1]);
        //    int maxGap = right - left;
        //    for (int i = 2; i < nums.Length; i++)
        //    {
        //        int value = nums[i];
        //        if (left - value > maxGap)
        //        {
        //            right = left;
        //            left = value;
        //            maxGap = right - left;
        //        }
        //        else if (value - right > maxGap)
        //        {
        //            left = right;
        //            right = value;
        //            maxGap = right - left;
        //        }
        //        else if (value - left > right - value)
        //        {
        //            right = value;
        //            // ...
        //        }
        //    }
        //    return maxGap;
        //}

        //
        // 1 - Two Sum
        //
        public int[] TwoSum(int[] nums, int target)
        {
            var dict = new Dictionary<int, int>();
            for (int i = 0; i < nums.Length; i++)
            {
                int sub = target - nums[i];
                if (dict.ContainsKey(nums[i]))
                {
                    return new[] { dict[nums[i]] + 1, i + 1 };
                }
                dict[sub] = i;
            }
            return null;
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
