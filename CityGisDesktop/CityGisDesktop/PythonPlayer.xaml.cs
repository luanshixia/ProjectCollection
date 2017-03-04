using System;
using System.Linq;
using System.Windows;

namespace Dreambuild.Gis.Desktop
{
    /// <summary>
    /// PythonPlayer.xaml 的交互逻辑
    /// </summary>
    public partial class PythonPlayer : Window
    {
        public string ScriptFolder = "script_player\\";
        private string[] _scripts;
        private int _index = 0;

        public PythonPlayer()
        {
            InitializeComponent();
        }

        public PythonPlayer(string folder)
        {
            InitializeComponent();

            ScriptFolder = folder;
            _scripts = System.IO.Directory.GetFiles(ScriptFolder).Where(x => x.EndsWith("py", StringComparison.OrdinalIgnoreCase)).ToArray();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (_index < _scripts.Length)
            {
                PythonConsole.Current.RunScript(_scripts[_index]);
                _index++;
            }
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            _index = 0;
        }
    }
}
