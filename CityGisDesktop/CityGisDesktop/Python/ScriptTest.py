MessageBox.Show('Running from script.')

def fun1(n):
    def fun2(m):
        return m+n
    return fun2

fun1(1)(2)