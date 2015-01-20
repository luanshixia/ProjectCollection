package com.company;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Stack;

public class Main {

    public static void main(String[] args) throws IOException {
        System.out.println("Hello");

        int[] data = {4, 3, 4, 4, 4, 2};
        Main main = new Main();
        int result = main.equiLeader(data);

        System.in.read();
    }

    public int equilibrium(int[] A) {
        int n = A.length;
        long sum = 0;
        long left = 0;
        for (int value : A) {
            sum += value;
        }
        for (int i = 0; i < n; i++) {
            int value = A[i];
            long right = sum - value - left;
            if (right == left) {
                return i;
            }
            left += value;
        }
        return -1;
    }

    public int mindiff(int[] A) {
        int n = A.length;
        double sum = 0;
        double min = Double.MAX_VALUE;
        double left = 0;
        double right = 0;
        for (int value : A) {
            sum += value;
        }
        for (int i = 0; i < n; i++)
        {
            int value = A[i];
            right = sum - left;
            double diff = Math.abs(left - right);
            if (diff < min)
            {
                min = diff;
            }
            left += value;
        }
        return (int)min;
    }

    public int permcheck(int[] A) {
        HashMap<Integer, Integer> counting = new HashMap<>();
        int n = A.length;
        for (int i = 0; i < n; i++) {
            counting.put(i + 1, 0);
        }
        for (int value : A) {
            if (counting.containsKey(value)) {
                counting.put(value, counting.get(value) + 1);
            }
        }
        for (int i = 0; i < n; i++) {
            if (counting.get(i + 1) != 1) {
                return 0;
            }
        }
        return 1;
    }

    public int frogRiverOne(int X, int[] A) {
        HashMap<Integer, Integer> counting = new HashMap<>();
        int n = A.length;
        for (int i = 0; i < n; i++) {
            if (!counting.containsKey(A[i])) {
                counting.put(A[i], i);
            }
        }
        int max = -1;
        for (int i = 1; i <= X; i++) {
            if (!counting.containsKey(i)) {
                return -1;
            }
            if (counting.get(i) > max) {
                max = counting.get(i);
            }
        }
        return max;
    }

    public int missingInteger(int[] A) {
        HashMap<Integer, Integer> counting = new HashMap<>();
        int n = A.length;
        int max = 0;
        for (int value : A) {
            if (counting.containsKey(value)) {
                counting.put(value, counting.get(value) + 1);
            } else {
                counting.put(value, 1);
            }
            if (value > max) {
                max = value;
            }
        }
        for (int i = 1; i <= max; i++) {
            if (!counting.containsKey(i)) {
                return i;
            }
        }
        return max + 1;
    }

    public int[] maxCounters(int N, int[] A) {
        int[] result = new int[N];
        int M = A.length;
        int max = 0;
        for (int i = 0; i < M; i++) {
            int k = A[i];
            if (k >= 1 && k <= N) {
                result[k - 1]++;
                if (result[k - 1] > max) {
                    max = result[k - 1];
                }
            } else if (k == N + 1) {
                for (int j = 0; j < N; j++) {
                    result[j] = max;
                }
            }
        }
        return result;
    }

    public int passingCars(int[] A) {
        int n = A.length;
        int[] p = new int[n];
        int sum = 0;
        int count = 0;
        for (int i = 0; i < n; i++) {
            sum += A[i];
            p[i] = sum;
        }
        for (int i = 0; i < n; i++) {
            if (A[i] == 0) {
                count += p[n - 1] - p[i];
            }
        }
        return count;
    }

    public int minAvgTwoSlick(int[] A) {
        int n = A.length;
        int[] p = new int[n + 1];
        p[0] = 0;
        for (int i = 1; i <= n; i++) {
            p[i] = p[i - 1] + A[i - 1];
        }
        double min = Double.MAX_VALUE;
        int result = 0;
        for (int i = 0; i < n; i++) {
            for (int j = i + 1; j < n; j++) {
                double avg = (double)(p[j + 1] - p[i]) / (j - i + 1);
                if (avg < min) {
                    min = avg;
                    result = i;
                }
            }
        }
        return result;
    }

    public int countDiv(int A, int B, int K) { // 100%
//        int count = 0;
//        for (int i = A; i <= B; i++) {
//            if (i % K == 0) {
//                count++;
//            }
//        }
//        return count;

        int count = Math.floorDiv(B, K) - Math.floorDiv(A, K);
        if (A % K == 0) {
            count += 1;
        }
        return count;
    }

    public int[] genomicRangeQuery(String S, int[] P, int[] Q) { // 100%
        int N = S.length();
        int M = P.length;
        int[] result = new int[M];
        HashMap<Character, int[]> countings = new HashMap<>();
        countings.put('A', new int[N + 1]);
        countings.put('C', new int[N + 1]);
        countings.put('G', new int[N + 1]);
        countings.put('T', new int[N + 1]);
        for (int i = 1; i <= N; i++) {
            countings.get('A')[i] = countings.get('A')[i - 1];
            countings.get('C')[i] = countings.get('C')[i - 1];
            countings.get('G')[i] = countings.get('G')[i - 1];
            countings.get('T')[i] = countings.get('T')[i - 1];
            countings.get(S.charAt(i - 1))[i]++;
        }
        for (int i = 0; i < M; i++) {
            int p = P[i];
            int q = Q[i];
            if (countings.get('A')[q + 1] - countings.get('A')[p] > 0) {
                result[i] = 1;
            } else if (countings.get('C')[q + 1] - countings.get('C')[p] > 0) {
                result[i] = 2;
            } else if (countings.get('G')[q + 1] - countings.get('G')[p] > 0) {
                result[i] = 3;
            } else if (countings.get('T')[q + 1] - countings.get('T')[p] > 0) {
                result[i] = 4;
            }
        }
        return result;
    }

    public int triangle(int[] A) { // 93%
        Arrays.sort(A);
        int n = A.length;
        for (int i = 0; i < n - 2; i++) {
            if (A[i] + A[i + 1] > A[i + 2]) {
                return 1;
            }
        }
        return 0;
    }

    public int maxProductOfThree(int[] A) { // 100%
        Arrays.sort(A);
        int n = A.length;
        return Math.max(A[n-1]*A[n-2]*A[n-3], A[0]*A[1]*A[n-1]);
    }

    public int numberOfDiscIntersections(int[] A) { // 81%: overflow return -1
        int n = A.length;
        ArrayList<DiscEdge> edges = new ArrayList<>();
        for (int i = 0; i < n; i++) {
            edges.add(new DiscEdge(i - A[i], 1));
            edges.add(new DiscEdge(i + A[i], -1));
        }
        edges.sort((a, b) -> 10 * (a.pos - b.pos) + 1 * (b.type - a.type));
        int pairs = 0;
        int count = 0;
        for (DiscEdge edge : edges) {
            if (edge.type == 1) {
                pairs += count;
            }
            count += edge.type;
        }
        return pairs;
    }

    public class DiscEdge {
        public int pos;
        public int type;

        public DiscEdge(int pos, int type) {
            this.pos = pos;
            this.type = type;
        }
    }

    public int brackets(String S) { // 100%
        Stack<Character> stack = new Stack<>();
        char[] chars = S.toCharArray();
        for (char c : chars) {
            if (c == '(' || c == '[' || c == '{') {
                stack.push(c);
            } else {
                if (!stack.empty()) {
                    char top = stack.peek();
                    if (top == '(' && c == ')' || top == '[' && c == ']' || top == '{' && c == '}') {
                        stack.pop();
                    } else {
                        return 0;
                    }
                } else {
                    return 0;
                }
            }
        }
        if (stack.empty()) {
            return 1;
        } else {
            return 0;
        }
    }

    public int fish(int[] A, int[] B) { // 100%
        Stack<Integer> stack = new Stack<>();
        int n = A.length;
        for (int i = 0; i < n; i++) {
            while (true) {
                if (stack.empty()) {
                    stack.push(i);
                    break;
                }
                int j = stack.peek();
                if (B[j] == 0) {
                    stack.push(i);
                    break;
                } else { // B[j] == 1
                    if (B[i] == 1) {
                        stack.push(i);
                        break;
                    } else { // B[i] == 0
                        if (A[j] < A[i]) {
                            stack.pop();
                        } else {
                            break;
                        }
                    }
                }
            }
        }
        return stack.size();
    }

    public int equiLeader(int[] A) { // 100%
        Stack<Integer> stack = new Stack<>();
        int n = A.length;
        for (int i = 0; i < n; i++) {
            if (stack.empty()) {
                stack.push(A[i]);
            } else {
                if (stack.peek() == A[i]) {
                    stack.push(A[i]);
                } else {
                    stack.pop();
                }
            }
        }
        if (stack.empty()) {
            return 0;
        } else {
            int leader = stack.peek();
            int[] p = new int[n + 1];
            p[0] = 0;
            for (int i = 1; i <= n; i++) {
                p[i] = p[i - 1] + (A[i - 1] == leader ? 1 : 0);
            }
            if (p[n] <= n / 2) {
                return 0;
            } else {
                int count = 0;
                for (int i = 1; i <= n; i++) {
                    if (p[i] > i / 2 && p[n] - p[i] > (n - i) / 2) {
                        count++;
                    }
                }
                return count;
            }
        }
    }

    public int maxProfit(int[] A) { // 100%
        int minPrice = Integer.MAX_VALUE;
        int maxProfit = 0;
        for (int i = 0; i < A.length; i++) {
            maxProfit = Math.max(A[i] - minPrice, maxProfit);
            minPrice = Math.min(A[i], minPrice);
        }
        return maxProfit;
    }

    public int minPerimeterRectangle(int N) { // 100%
        int m = (int)Math.floor(Math.sqrt(N));
        int min = Integer.MAX_VALUE;
        for (int i = 1; i <= m; i++) {
            if (N % i == 0) {
                int j = N / i;
                min = Math.min(min, 2 * (i + j));
            }
        }
        return min;
    }

    public int countFactors(int N) { // 100%
        int m = (int)Math.floor(Math.sqrt(N));
        int count = 0;
        for (int i = 1; i <= m; i++) {
            if (N % i == 0) {
                int j = N / i;
                if (i == j) {
                    count++;
                } else {
                    count += 2;
                }
            }
        }
        return count;
    }
}
