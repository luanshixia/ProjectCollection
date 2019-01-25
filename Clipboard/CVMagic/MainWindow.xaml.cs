using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CVMagic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.SourceInitialized += (sender, e) =>
            {
                var clipboardManager = new ClipboardManager(this);
                clipboardManager.ClipboardChanged += (sender1, e1) =>
                {
                    if (Clipboard.ContainsText())
                    {
                        var text = Clipboard.GetText();
                        if (text.StartsWith("ago"))
                        {
                            Clipboard.SetText("bingo!");
                        }
                        else if (Regex.Match(text, @"TIMESTAMP\s*>\s*ago\((?<point>[0-9]+[dhms])\)", RegexOptions.IgnoreCase) is Match match && match.Success)
                        {
                            var point = match.Groups["point"].Value;
                            if (Regex.Match(point, "(?<days>[0-9]+)d") is Match dayMatch && dayMatch.Success)
                            {
                                var days = dayMatch.Groups["days"].Value;
                                var datetime = DateTime.UtcNow.AddDays(-int.Parse(days));
                                var replacement = $"TIMESTAMP > datetime({datetime})";
                                Clipboard.SetText(Regex.Replace(text, @"TIMESTAMP\s*>\s*ago\([0-9]+[dhms]\)", replacement, RegexOptions.IgnoreCase));
                            }
                        }
                    }
                };
            };
        }
    }
}
