import clr
clr.AddReference('System.Core')

import System
from System.Linq import Enumerable
from System import Func
from System import Action

# Pythonic wrappers around some common linq functions
def count(col, fun = lambda x: True):
    return Enumerable.Count[object](col, Func[object, bool](fun))

def sum(col, fun = lambda x: x):
    return Enumerable.Sum[object](col, Func[object, System.Double](fun))

def min(col, fun = lambda x: x):
    return Enumerable.Min[object](col, Func[object, System.Double](fun))

def max(col, fun = lambda x: x):
    return Enumerable.Max[object](col, Func[object, System.Double](fun))

def avg(col, fun = lambda x: x):
    return Enumerable.Average[object](col, Func[object, System.Double](fun))

def orderby(col, fun):
    return Enumerable.OrderBy[object, object](col, Func[object, object](fun))

def orderbydesc(col, fun):
    return Enumerable.OrderByDescending[object, object](col, Func[object, object](fun))

def map(col, fun):
    return Enumerable.Select[object, object](col, Func[object, object](fun))

def single(col, fun):
    return Enumerable.Single[object](col, Func[object, bool](fun))

def first(col, fun):
    return Enumerable.First[object](col, Func[object, bool](fun))

def last(col, fun):
    return Enumerable.Last[object](col, Func[object, bool](fun))

def filter(col, fun):
    return Enumerable.Where[object](col, Func[object, bool](fun))

def foreach(col, fun):
    Enumerable.ToList[object](col).ForEach(Action[object](fun))

def tolist(col):
    return Enumerable.ToList(col)

def toarray(col):
    return Enumerable.ToArray(col)


# Save all Pythonic wrappers into dict
this_module = __import__(__name__)
linqs = {}
for name in dir(this_module):
    if name.startswith('__') or name in ('this_module', 'linqs'):
        continue
    linqs[name] = getattr(this_module, name)


def is_enumerable(obj):
    ienum_object = System.Collections.Generic.IEnumerable[object]
    return isinstance(obj, ienum_object)

class LinqAdapter(object):
    def __init__(self, col):
        self.col = col

    def __iter__(self):
        return iter(self.col)

    def __str__(self):
        return '[%s]' % ', '.join( (str(v) for v in self) )

    def __repr__(self):
        return str(self)

    def __getattr__(self, attr):
        def decorator(*arg, **kws):
            result = linqs[attr](self.col, *arg, **kws)
            if attr == "tolist" or attr == "toarray":
                return result
            else:
                if is_enumerable(result):
                    return LinqAdapter(result)
                else:
                    return result
        return decorator

def ready(col):
    if (is_enumerable(col)):
        return LinqAdapter(col)
    else:
        return LinqAdapter( (c for c in col) )
