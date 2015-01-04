import clr
clr.AddReference("System.Windows.Forms")
clr.AddReference("System.Drawing")
clr.AddReference("CityGuiLib")
import System
from System import *
from System.Windows.Forms import *
from System.Drawing import *
from System.IO import *
from TongJi.City import *
import linq

def clear():
    CommandWindow.ClearHistory()
    
def quickref():
    strw = StringWriter()
    strw.WriteLine('clear()      - Clear command history.')
    strw.WriteLine('runscript()  - Execute external .py file.')
    strw.WriteLine('clearscope() - Clear all variables.')
    MessageBox.Show(strw.ToString(), 'Reference')
    
def script():
    ofd = OpenFileDialog()
    ofd.Title = 'Choose .py file'
    ofd.Filter = 'Python files (*.py)|*.py'
    if ofd.ShowDialog() == DialogResult.OK:
        CommandWindow.RunScript(ofd.FileName)
 
def runscript(fileName):
    CommandWindow.RunScript(fileName)
    
def clearscope():
    CommandWindow.ClearScope()
    
def vars():
    return CommandWindow.ListVars()

def symbols():
    return CommandWindow.ListSymbols()
