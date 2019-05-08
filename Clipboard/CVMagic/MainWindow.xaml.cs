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
                        foreach (var spell in MagicBook.Spells)
                        {
                            if (Regex.Match(text, spell.Pattern, RegexOptions.IgnoreCase) is Match match1 && match1.Success)
                            {
                                Clipboard.SetText(Regex.Replace(
                                    input: text,
                                    pattern: spell.Pattern,
                                    replacement: spell.Cast(input: text, match: match1).Result,
                                    options: RegexOptions.IgnoreCase));

                                break;
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
