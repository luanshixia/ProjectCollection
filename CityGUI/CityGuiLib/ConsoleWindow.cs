using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using IronPython.Hosting;

using WeifenLuo.WinFormsUI.Docking;

namespace TongJi.City
{
    public partial class PyConsole : DockContent
    {
        private static Microsoft.Scripting.Hosting.ScriptEngine _engine = Python.CreateEngine();
        private static Microsoft.Scripting.Hosting.ScriptScope _scope = _engine.CreateScope();
        private object _result = "Undefined";
        //private object _addition = string.Empty;

        public PyConsole()
        {
            InitializeComponent();
        }

        private void txtCmd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Control == true)
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
                ValueBuffer.UpdateValues();
                Viewer.Current.Canvas.Invalidate(); // newly 20110805
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StringWriter strw = new StringWriter();
            strw.WriteLine("CityGIS IronPython Console");
            strw.WriteLine("Version 0.1");
            strw.WriteLine("by WY");
            strw.WriteLine("--------------------------------------------------");
            strw.WriteLine("Press Ctrl+Enter to execute.");
            strw.WriteLine("Execute \'quickref()\' for instructions.");
            strw.WriteLine("--------------------------------------------------");
            strw.WriteLine();
            txtCmdHistory.Text = strw.ToString();

            Init();
        }

        private void Init()
        {
            _scope.SetVariable("_", _result);
            _scope.SetVariable("CommandWindow", this);
            _scope.SetVariable("options", OptionsManager.Singleton);
            _scope.SetVariable("values", ValueBuffer.Units);
            _scope.SetVariable("pycmd", IronPython.Runtime.Types.DynamicHelpers.GetPythonTypeFromType(typeof(PyCmd)));
            _scope.SetVariable("pydraw", IronPython.Runtime.Types.DynamicHelpers.GetPythonTypeFromType(typeof(PyDraw)));
            _engine.ExecuteFile("PreProcessing.py", _scope);
        }

        private void txtCmd_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtCmd.Text.Replace("\r\n", string.Empty) == string.Empty)
                {
                    txtCmd.Text = string.Empty;
                }
            }
        }

        public void Echo(string text)
        {
            //StringWriter strw = new StringWriter();
            //strw.Write(txtCmdHistory.Text);
            //strw.Write(text);
            //txtCmdHistory.Text = strw.ToString();
            txtCmdHistory.AppendText(text);

            txtCmdHistory.SelectionStart = txtCmdHistory.Text.Length - 1;
            txtCmdHistory.ScrollToCaret();
        }

        public void EchoLine(string text)
        {
            //StringWriter strw = new StringWriter();
            //strw.Write(txtCmdHistory.Text);
            //strw.WriteLine(text);
            //txtCmdHistory.Text = strw.ToString();
            txtCmdHistory.AppendText("\r\n" + text);

            txtCmdHistory.SelectionStart = txtCmdHistory.Text.Length - 1;
            txtCmdHistory.ScrollToCaret();
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
                EchoLine(string.Format("Script \'{0}\' executed successfully.", fileName));
            }
            catch(Exception ex)
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

        public void SetCmdSingleLine(int total = 100, int cmd = 20)
        {
            this.Height = total;
            txtCmd.Height = cmd;
        }

        private void PyConsole_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
