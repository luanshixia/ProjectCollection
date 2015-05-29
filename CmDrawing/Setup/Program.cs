using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TongJi.Setup
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static int Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            return 0;
        }
    }
}
