using Dreambuild.Gis.Display;
using IronPython.Hosting;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dreambuild.Gis.Desktop
{
    /// <summary>
    /// PythonConsole.xaml 的交互逻辑
    /// </summary>
    public partial class PythonConsole : UserControl
    {
        private static Microsoft.Scripting.Hosting.ScriptEngine _engine = Python.CreateEngine();
        private static Microsoft.Scripting.Hosting.ScriptScope _scope = _engine.CreateScope();
        private object _result = "Undefined";
        public static PythonConsole Current { get; private set; }

        public PythonConsole()
        {
            InitializeComponent();

            Current = this;
        }

        private void txtCmd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                string cmd = txtCmd.Text.Trim(' ', '\n', '\r');
                if (cmd != string.Empty)
                {
                    try
                    {
                        StringWriter print = new StringWriter();
                        _engine.Runtime.IO.SetOutput(_engine.Runtime.IO.OutputStream, print);
                        _result = _engine.Execute(cmd, _scope);
                        if (_result == null)
                        {
                            _result = print.ToString();
                        }
                        _scope.SetVariable("_", _result);
                    }
                    catch (Exception ex)
                    {
                        _result = ex.Message;
                    }

                    EchoLine("In  >> " + cmd);
                    if (_result == null)
                    {
                        EchoLine("Out >> Undefined");
                    }
                    else if (_result.ToString() == string.Empty)
                    {
                        EchoLine("Out >> Empty");
                    }
                    else
                    {
                        EchoLine("Out >> " + _result.ToString());
                    }
                    EchoLine();
                    txtCmd.Focus();
                }
                txtCmd.Text = string.Empty;
            }
        }

        private void txtCmd_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtCmd.Text.Replace("\r\n", string.Empty) == string.Empty)
                {
                    txtCmd.Text = string.Empty;
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            StringWriter strw = new StringWriter();
            strw.WriteLine("CityGIS IronPython 命令行");
            strw.WriteLine("版本 0.2");
            strw.WriteLine("by WY");
            strw.WriteLine("--------------------------------------------------");
            strw.WriteLine("按 Ctrl+Enter 来执行代码.");
            strw.WriteLine("执行 \'quickref()\' 来获取指南.");
            strw.WriteLine("--------------------------------------------------");
            strw.WriteLine();
            txtCmdHistory.Text = strw.ToString();

            try
            {
                Init();
            }
            catch (Exception ex)
            {
                EchoLine(ex.Message);
            }
        }

        private void Init()
        {
            _scope.SetVariable("_", _result);
            _scope.SetVariable("CommandWindow", this);
            _scope.SetVariable("pycmd", IronPython.Runtime.Types.DynamicHelpers.GetPythonTypeFromType(typeof(PyCmd)));
            _scope.SetVariable("input", IronPython.Runtime.Types.DynamicHelpers.GetPythonTypeFromType(typeof(UserInput)));
            _scope.SetVariable("dm", IronPython.Runtime.Types.DynamicHelpers.GetPythonTypeFromType(typeof(MapDataManager)));
            _scope.SetVariable("ss", IronPython.Runtime.Types.DynamicHelpers.GetPythonTypeFromType(typeof(SelectionSet)));
            _scope.SetVariable("mc", MapControl.Current);
            var path = Environment.CurrentDirectory;
            _engine.ExecuteFile(path + "\\Python\\PreProcessing.py", _scope);
            _engine.ExecuteFile(path + "\\Python\\linq.py", _scope);
        }

        public void SetStaticVariable(string name, Type type) // newly 20130309
        {
            _scope.SetVariable(name, IronPython.Runtime.Types.DynamicHelpers.GetPythonTypeFromType(type));
        }

        public void SetInstanceVariable(string name, object obj) // newly 20130309
        {
            _scope.SetVariable(name, obj);
        }

        public void Echo(string text)
        {
            txtCmdHistory.AppendText(text);
            txtCmdHistory.SelectionStart = txtCmdHistory.Text.Length - 1;
            txtCmdHistory.ScrollToEnd();
        }

        public void EchoLine(string text)
        {
            txtCmdHistory.AppendText("\r\n" + text);
            txtCmdHistory.SelectionStart = txtCmdHistory.Text.Length - 1;
            txtCmdHistory.ScrollToEnd();
        }

        public void EchoLine()
        {
            EchoLine(string.Empty);
        }

        public void ClearHistory()
        {
            txtCmdHistory.Text = string.Empty;
        }

        public void RunScript(string fileName)
        {
            try
            {
                _engine.ExecuteFile(fileName, _scope);
                EchoLine(string.Format("脚本文件 \'{0}\' 已成功执行.", fileName));
            }
            catch (Exception ex)
            {
                EchoLine(ex.Message);
            }
        }

        public void RunString(string cmdString)
        {
            _engine.Execute(cmdString, _scope);
        }

        public void ClearScope()
        {
            _scope = _engine.CreateScope();
            Init();
            EchoLine("Scope cleared.");
        }

        public string ListSymbols()
        {
            var items = _scope.GetItems();
            StringWriter strw = new StringWriter();
            strw.WriteLine("Symbols in scope:");
            strw.WriteLine();
            foreach (var item in items)
            {
                string s;
                try
                {
                    s = item.Value.GetType().ToString();
                }
                catch
                {
                    s = "Unknown";
                }
                strw.WriteLine(string.Format("{0} : {1}", item.Key, s));
            }
            return strw.ToString();
        }

        public string ListVars()
        {
            var items = _scope.GetItems();
            StringWriter strw = new StringWriter();
            strw.WriteLine("Variables in scope:");
            strw.WriteLine();
            foreach (var item in items)
            {
                string s;
                try
                {
                    s = item.Value.GetType().ToString();
                }
                catch
                {
                    s = "Unknown";
                }
                if (s.StartsWith("IronPython.Runtime.Types.PythonType") || s.StartsWith("Microsoft.Scripting.Actions"))
                {
                    continue;
                }
                strw.WriteLine(string.Format("{0} : {1}", item.Key, s));
            }
            return strw.ToString();
        }
    }

    public class PyExecutor
    {
        private static Microsoft.Scripting.Hosting.ScriptEngine _engine = Python.CreateEngine();
        private static Microsoft.Scripting.Hosting.ScriptScope _scope = _engine.CreateScope();

        public PyExecutor()
        {
        }

        public void SetVar(string name, object value)
        {
            _scope.SetVariable(name, value);
        }

        public object GetVar(string name)
        {
            return _scope.GetVariable(name);
        }

        public object RunString(string script)
        {
            return _engine.Execute(script, _scope);
        }

        public void RunFile(string scriptFile)
        {
            _engine.ExecuteFile(scriptFile, _scope);
        }
    }
}
