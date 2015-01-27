def commonPrimeDivisors(A, B):
    N = len(A)
    count = 0
    for i in xrange(N):
        a = A[i]
        b = B[i]
        c = min(a, b)
        d = max(a, b)
        if d % c != 0:
            continue;
            pass
        while d > c:
            d /= c
            pass
        if c % d == 0:
            count += 1
            pass
        pass
    return count
    pass