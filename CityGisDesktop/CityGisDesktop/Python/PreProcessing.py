import clr
clr.AddReference("PresentationCore")
clr.AddReference("PresentationFramework")
clr.AddReference("WindowsBase")
clr.AddReference("Dreambuild.Common")
clr.AddReference("Dreambuild.Gis")
clr.AddReference("Dreambuild.Gis.Display")
import System
import Microsoft
import Dreambuild
from System import *
from System.Windows import *
from System.IO import *

def clear():
    CommandWindow.ClearHistory()
    
def quickref():
    strw = StringWriter()
    strw.WriteLine('clear()      - Clear command history.')
    strw.WriteLine('script()     - Browse external .py file to execute.')
    strw.WriteLine('runscript()  - Execute external .py file.')
    strw.WriteLine('clearscope() - Clear all variables.')
    strw.WriteLine('vars()       - List all variables.')
    strw.WriteLine('symbols()    - List all symbols.')
    MessageBox.Show(strw.ToString(), 'Reference')
    
def script():
    ofd = Microsoft.Win32.OpenFileDialog()
    ofd.Title = 'Choose .py file'
    ofd.Filter = 'Python files (*.py)|*.py'
    if ofd.ShowDialog() == True:
        CommandWindow.RunScript(ofd.FileName)
 
def runscript(fileName):
    CommandWindow.RunScript(fileName)
    
def clearscope():
    CommandWindow.ClearScope()
    
def vars():
    return CommandWindow.ListVars()

def symbols():
    return CommandWindow.ListSymbols()
