using System;
using System.Text.RegularExpressions;
using System.Windows;

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

            var notifyIcon = new System.Windows.Forms.NotifyIcon()
            {
                Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri("CVMagic.ico", UriKind.Relative)).Stream),
                Text = "CV Magic",
                Visible = true,
                BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info,
                BalloonTipText = "CV Magic is running.",
                BalloonTipTitle = "CV Magic",
                ContextMenu = new System.Windows.Forms.ContextMenu(new[]
                {
                    new System.Windows.Forms.MenuItem("E&xit", (sender, e) => Application.Current.Shutdown())
                })
            };

            notifyIcon.Click += (sender, e) =>
            {
                this.Visibility = Visibility.Visible;
                this.WindowState = WindowState.Normal;
            };

            this.Closing += (sender, e) =>
            {
                e.Cancel = true;
                this.Visibility = Visibility.Hidden;
                notifyIcon.ShowBalloonTip(3000);
            };

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
                        else if (Regex.Match(text, @"TIMESTAMP\s*>\s*ago\((?<number>[0-9]+)(?<unit>[dhms])\)", RegexOptions.IgnoreCase) is Match match && match.Success)
                        {
                            var number = match.Groups["number"].Value;
                            var unit = match.Groups["unit"].Value;
                            var dateTime = DateTime.UtcNow;

                            if (unit == "d")
                            {
                                dateTime = dateTime.AddDays(-int.Parse(number));
                            }
                            else if (unit == "h")
                            {
                                dateTime = dateTime.AddHours(-int.Parse(number));
                            }
                            else if (unit == "m")
                            {
                                dateTime = dateTime.AddMinutes(-int.Parse(number));
                            }
                            else if (unit == "s")
                            {
                                dateTime = dateTime.AddSeconds(-int.Parse(number));
                            }

                            Clipboard.SetText(Regex.Replace(
                                input: text,
                                pattern: @"TIMESTAMP\s*>\s*ago\([0-9]+[dhms]\)",
                                replacement: $"TIMESTAMP > datetime({dateTime}) and TIMESTAMP < datetime({DateTime.UtcNow})",
                                options: RegexOptions.IgnoreCase));
                        }
                        else
                        {
                            foreach (var spell in MagicBook.Spells)
                            {
                                if (Regex.Match(text, spell.Pattern, RegexOptions.IgnoreCase) is Match match1 && match1.Success)
                                {
                                    Clipboard.SetText(Regex.Replace(
                                        input: text,
                                        pattern: spell.Pattern,
                                        replacement: spell.Cast().Result,
                                        options: RegexOptions.IgnoreCase));

                                    break;
                                }
                            }
                        }
                    }
                };

                this.Visibility = Visibility.Hidden;
                notifyIcon.ShowBalloonTip(3000);
            };
        }
    }
}
