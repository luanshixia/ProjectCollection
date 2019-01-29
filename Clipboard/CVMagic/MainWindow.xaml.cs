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
                            var dateTime = DateTime.UtcNow;
                            if (Regex.Match(point, "(?<days>[0-9]+)d") is Match dayMatch && dayMatch.Success)
                            {
                                dateTime = dateTime.AddDays(-int.Parse(dayMatch.Groups["days"].Value));
                            }
                            else if (Regex.Match(point, "(?<hours>[0-9]+)h") is Match hourMatch && hourMatch.Success)
                            {
                                dateTime = dateTime.AddHours(-int.Parse(hourMatch.Groups["hours"].Value));
                            }
                            else if (Regex.Match(point, "(?<minutes>[0-9]+)h") is Match minuteMatch && minuteMatch.Success)
                            {
                                dateTime = dateTime.AddHours(-int.Parse(minuteMatch.Groups["minutes"].Value));
                            }
                            else if (Regex.Match(point, "(?<seconds>[0-9]+)h") is Match secondMatch && secondMatch.Success)
                            {
                                dateTime = dateTime.AddHours(-int.Parse(secondMatch.Groups["seconds"].Value));
                            }

                            Clipboard.SetText(Regex.Replace(
                                input: text, 
                                pattern: @"TIMESTAMP\s*>\s*ago\([0-9]+[dhms]\)",
                                replacement: $"TIMESTAMP > datetime({dateTime}) and TIMESTAMP < datetime({DateTime.UtcNow})", 
                                options: RegexOptions.IgnoreCase));
                        }
                    }
                };
            };
        }
    }
}
