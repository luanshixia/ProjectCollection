
#include <stdlib.h>
#include <limits.h>

int divs(int A[], int N, int expect) {
    int sum = 0;
    int groups = 0;
    for (int i = 0; i < N; i++) {
        sum += A[i];
        if (sum > expect) {
            sum = A[i];
            groups++;
        }
    }
    if (sum > 0) {
        groups++;
    }
    return groups;
}

int minMaxDivision(int K, int M, int A[], int N) { // 100%
    int sum = 0;
    int max = 0;
    for (int i = 0; i < N; i++) {
        sum += A[i];
        if (A[i] > max) {
            max = A[i];
        }
    }
    int left = max;
    int right = sum;
    while (left <= right) {
        int mid = (left + right) / 2;
        if (divs(A, N, mid) <= K) {
            right = mid - 1;
        }
        else {
            left = mid + 1;
        }
    }
    return right + 1;
}

int nailingPlanks(int A[], int B[], int N, int C[], int M) { // xxx
    int steps = 0;
    int pos[60000];
    int sum[60001];
    for (int i = 0; i < M; i++) {
        pos[C[i]]++;
    }
    sum[0] = 0;
    for (int i = 1; i <= 2 * M; i++) {
        sum[i] = sum[i - 1] + pos[i - 1];
    }

    int left = 0;
    int right = M - 1;
    while (left <= right) {
        int mid = (left + right) / 2;
        int can = 1;
        for (int i = 0; i <= N; i++) {
            if (sum[B[i]] - sum[A[i] - 1] == 0) {
                can = 0;
                break;
            }
        }
        if (can) {
            right = mid - 1;
        }
        else {
            left = mid + 1;
        }
    }
    int step = right + 1;
    if (step == M) {
        return -1;
    }
    return step + 1;
}

int absDistinct(int A[], int N) {
    int count = 1;
    int left = 0;
    int right = N - 1;
    int cur = max(abs(A[left]), abs(A[right]));
    while (left <= right) {
        if (abs(A[left]) == cur) {
            left++;
        }
        else if (abs(A[right]) == cur) {
            right--;
        }
        else {
            if (abs(A[left]) > abs(A[right])) {
                cur = abs(A[left]);
                left++;
            }
            else {
                cur = abs(A[right]);
                right--;
            }
            count++;
        }
    }
    return count;
}

int countDistinctSlices(int M, int A[], int N) {
    int ans = 0, pre = -1;
    int hash[100001];
    for (int i = 0; i < M; i++) hash[i] = -1;
    for (int i = 0; i < N; i++) {
        if (hash[A[i]] > pre) {
            pre = hash[A[i]];
        }
        ans += i - pre;
        hash[A[i]] = i;
        if (ans > 1000000000) return 1000000000;
    }
    return ans;
}

int cmpfunc(const void * a, const void * b)
{
    return (*(int*)a - *(int*)b);
}

int minAbsSumOfTwo(int A[], int N) { // 100%
    qsort(A, N, sizeof(int), cmpfunc);
    int left = 0;
    int right = N - 1;
    if (A[left] >= 0) {
        return A[left] + A[left];
    }
    else if (A[right] <= 0) {
        return -A[right] - A[right];
    }
    int min = INT_MAX;
    while (left <= right) {
        int val = abs(A[left] + A[right]);
        if (val == 0) {
            return 0;
        }
        if (val < min) {
            min = val;
        }
        if (abs(A[left]) > abs(A[right])) {
            left++;
        }
        else {
            right--;
        }
    }
    return min;
}

int countTriangles(int A[], int N) { // 100%
    qsort(A, N, sizeof(int), cmpfunc);
    int count = 0;
    for (int i = 0; i < N - 2; i++) {
        int k = i + 2;
        for (int j = i + 1; j < N - 1; j++) {
            while (k < N && A[i] + A[j] > A[k]) {
                k++;
            }
            count += k - 1 - j;
        }
    }
    return count;
}

int tieRopes(int K, int A[], int N) { // 100%
    int count = 0;
    int sum[100001];
    sum[0] = 0;
    for (int i = 1; i <= N; i++) {
        sum[i] = sum[i - 1] + A[i - 1];
    }
    int j = 0;
    for (int i = 0; i < N; i++) {
        if (sum[i + 1] - sum[j] >= K) {
            count++;
            j = i + 1;
        }
    }
    return count;
}

int maxNonoverlappingSegments(int A[], int B[], int N) { // 90% - empty and single
    int count = 1;
    int i = 0;
    for (int j = 1; j < N; j++) {
        if (A[j] > B[i]) {
            count++;
            i = j;
        }
    }
    return count;
}

int monthCode(char *month) {
    char *months[] = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
    for (int i = 0; i < 12; i++) {
        if (!strcmp(month, months[i])) {
            return i;
        }
    }
}

int dayCode(char *day) {
    char *days[] = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
    for (int i = 0; i < 7; i++) {
        if (!strcmp(day, days[i])) {
            return i;
        }
    }
}

int wholeWeeks(int Y, char *A, char *B, char *W) {
    int daysInMonths[] = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};
    if (Y % 4 == 0 && Y % 100 != 0 || Y % 400 == 0) {
        daysInMonths[1]++;
    }
    int sum[13];
    sum[0] = 0;
    for (int i = 1; i <= 12; i++) {
        sum[i] = sum[i - 1] + daysInMonths[i - 1];
    }
    int month1 = monthCode(A);
    int month2 = monthCode(B);
    int day0 = dayCode(W);
    int day1 = (day0 + sum[month1]) % 7;
    int days = sum[month2 + 1] - sum[month1];
    int pre = (8 - day1) % 7;
    return (days - pre) % 7;
}

int main() {
    /*int arr[] = { 2, 1, 5, 1, 2, 2, 2 };
    int result = minMaxDivision(3, 5, arr, 7);*/

    /*int arr[] = { -8, 4, 5, -10, 3 };
    int result = minAbsSumOfTwo(arr, 5);*/

    int reuslt = wholeWeeks(2014, "April", "May", "Wednesday");
}

