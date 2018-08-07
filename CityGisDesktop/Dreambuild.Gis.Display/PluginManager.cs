using Dreambuild.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Dreambuild.Gis.Display
{
    /// <summary>
    /// 插件管理
    /// </summary>
    public static class PluginManager
    {
        /// <summary>
        /// 外部插件目录
        /// </summary>
        public const string PluginDirectory = "\\Plugins\\";

        /// <summary>
        /// 获取应用程序路径
        /// </summary>
        public static string AppPath
        {
            get
            {
                string s = typeof(PluginManager).Assembly.Location;
                return s.Remove(s.LastIndexOf('\\') + 1);
            }
        }

        /// <summary>
        /// 扫描并加载外部插件
        /// </summary>
        public static void ScanAndLoadPlugins()
        {
            var dlls = Directory
                .GetFiles(AppPath + PluginDirectory)
                .Where(x => x.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            foreach (var dll in dlls)
            {
                var plugin = Assembly.LoadFrom(dll);
                var entryClass = plugin
                    .GetTypes()
                    .First(x => x.GetInterfaces().Contains(typeof(IPlugin)));

                var entryMethod = entryClass.GetMethod("OnLoad");
                entryMethod.Invoke(plugin.CreateInstance(entryClass.FullName), null);
            }
        }

        /// <summary>
        /// 获取内部插件列表
        /// </summary>
        /// <returns>内部插件列表</returns>
        public static List<IPlugin> GetBuiltInPlugins()
        {
            var assembly = Assembly.GetEntryAssembly();
            return assembly
                .GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IPlugin)))
                .Select(t => assembly.CreateInstance(t.FullName))
                .Cast<IPlugin>()
                .ToList();
        }
    }

    /// <summary>
    /// 插件接口
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// 插件名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 插件描述
        /// </summary>
        string Description { get; }
        /// <summary>
        /// 插件的初始化代码
        /// </summary>
        void OnLoad();
    }

    /// <summary>
    /// 演示
    /// </summary>
    public abstract class DemoBase : IPlugin
    {
        /// <summary>
        /// 演示名称
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// 演示描述
        /// </summary>
        public string Description { get; protected set; }
        /// <summary>
        /// 演示的初始化代码
        /// </summary>
        public virtual void OnLoad()
        {
        }

        /// <summary>
        /// 获取当前Demo的所有DemoTools，供添加到菜单等。
        /// </summary>
        /// <returns>DemoTools</returns>
        public IEnumerable<Tuple<string, RoutedEventHandler>> GetDemoTools()
        {
            // Iterate through all the methods of the class.
            foreach (var method in this.GetType().GetMethods())
            {
                // Iterate through all the Attributes for each method.
                foreach (var attr in Attribute.GetCustomAttributes(method))
                {
                    // Check for the DemoTool attribute.
                    if (attr.GetType() == typeof(DemoToolAttribute))
                    {
                        var method1 = method;
                        yield return Tuple.Create<string, RoutedEventHandler>(
                            (attr as DemoToolAttribute).Name,
                            (s, args) => method1.Invoke(this, null));
                    }
                }
            }
        }
    }

    /// <summary>
    /// 工具栏按钮
    /// </summary>
    public class ToolbarButton
    {
        /// <summary>
        /// 文本标签
        /// </summary>
        public string TextLabel { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImageUri { get; set; }
        /// <summary>
        /// 描述或工具提示
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 按钮类型
        /// </summary>
        public ToolbarButtonType ButtonType { get; set; }
        /// <summary>
        /// 单击事件处理
        /// </summary>
        public Action ClickHandler { get; set; }
    }

    /// <summary>
    /// 工具栏按钮类型
    /// </summary>
    public enum ToolbarButtonType
    {
        /// <summary>
        /// 默认按钮
        /// </summary>
        Default,
        /// <summary>
        /// 单选按钮
        /// </summary>
        Radio,
        /// <summary>
        /// 状态按钮
        /// </summary>
        Toggle,
        /// <summary>
        /// 重复按钮
        /// </summary>
        Repeat
    }

    /// <summary>
    /// 工具栏
    /// </summary>
    public class Toolbar
    {
        /// <summary>
        /// 按钮列表
        /// </summary>
        public List<ToolbarButton> Buttons { get; private set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Toolbar()
        {
            this.Buttons = new List<ToolbarButton>();
        }

        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="textLebel">文本标签</param>
        /// <param name="clickHandler">单击事件</param>
        /// <param name="imageUri">图片地址</param>
        /// <param name="description">工具提示</param>
        /// <param name="isRadioButton">是否单选按钮</param>
        public void AddButton(
            string textLebel,
            Action clickHandler,
            string imageUri = null,
            string description = null,
            ToolbarButtonType type = ToolbarButtonType.Default)
        {
            this.Buttons.Add(new ToolbarButton
            {
                TextLabel = textLebel,
                ClickHandler = clickHandler,
                Description = description,
                ImageUri = imageUri,
                ButtonType = type
            });
        }

        /// <summary>
        /// 返回使用ViewerTool的单击事件处理函数
        /// </summary>
        /// <param name="tool">指定ViewerTool</param>
        /// <returns>单击事件处理函数</returns>
        public static Action ViewerTool(ViewerTool tool)
        {
            return () => ViewerToolManager.ExclusiveTool = tool;
        }
    }

    /// <summary>
    /// 图形用户交互
    /// </summary>
    public static class Gui
    {
        /// <summary>
        /// 显示一个命令选项面板
        /// </summary>
        /// <param name="tip"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static int GetOption(string tip, params string[] options)
        {
            string title = LocalizationHelper.CurrentLocale == Locales.ZH_CN ? "请选择" : "Please choose";

            var window = new Window
            {
                Width = 300,
                SizeToContent = SizeToContent.Height,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.ToolWindow,
                Title = title,
                Owner = Application.Current.MainWindow
            };

            var stackPanel = new StackPanel
            {
                Margin = new Thickness(5)
            };

            var textBlock = new TextBlock
            {
                Text = (string.IsNullOrEmpty(tip) ? "Choose one." : tip),
                TextWrapping = TextWrapping.Wrap
            };

            int result = -1;
            var buttons = options
                .Select((option, i) => new Button
                {
                    Content = option,
                    Tag = i
                })
                .ToList();

            buttons.ForEach(button => button.Click += (s, args) =>
            {
                result = (int)button.Tag; window.DialogResult = true;
            });

            stackPanel.Children.Add(textBlock);
            buttons.ForEach(x => stackPanel.Children.Add(x));
            window.Content = stackPanel;
            window.ShowDialog();
            return result;
        }

        /// <summary>
        /// 显示一个列表选择面板
        /// </summary>
        /// <param name="tip"></param>
        /// <param name="choices"></param>
        /// <returns></returns>
        public static string GetChoice(string tip, params string[] choices)
        {
            var window = new Window
            {
                Width = 300,
                SizeToContent = SizeToContent.Height,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.ToolWindow,
                Title = "Please choose"
            };

            var stackPanel = new StackPanel
            {
                Margin = new Thickness(10)
            };

            var textBlock = new TextBlock
            {
                Text = (string.IsNullOrEmpty(tip) ? "Choose one." : tip),
                TextWrapping = TextWrapping.Wrap
            };

            var listBox = new ListBox
            {
                Height = 200
            };

            choices.ToList().ForEach(choice => listBox.Items.Add(new ListBoxItem
            {
                Content = choice
            }));

            var okButton = new Button
            {
                Content = "OK",
                Margin = new Thickness(5)
            };

            okButton.Click += (s, args) => window.DialogResult = true;
            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(listBox);
            stackPanel.Children.Add(okButton);
            window.Content = stackPanel;
            window.ShowDialog();

            return listBox.SelectedItem == null
                ? string.Empty
                : (listBox.SelectedItem as ListBoxItem).Content.ToString();
        }

        /// <summary>
        /// 显示一个可多选的列表选择面板
        /// </summary>
        /// <param name="tip"></param>
        /// <param name="choices"></param>
        /// <returns></returns>
        public static string[] GetChoices(string tip, params string[] choices)
        {
            var window = new Window
            {
                Width = 300,
                SizeToContent = SizeToContent.Height,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.ToolWindow,
                Title = "Please choose"
            };

            var stackPanel = new StackPanel
            {
                Margin = new Thickness(10)
            };

            var textBlock = new TextBlock
            {
                Text = (string.IsNullOrEmpty(tip) ? "Choose one." : tip),
                TextWrapping = TextWrapping.Wrap
            };

            var listBox = new ListBox
            {
                Height = 200
            };

            choices.ToList().ForEach(choice => listBox.Items.Add(new CheckBox
            {
                Content = choice
            }));

            var okButton = new Button
            {
                Content = "OK",
                Margin = new Thickness(5)
            };

            okButton.Click += (s, args) => window.DialogResult = true;
            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(listBox);
            stackPanel.Children.Add(okButton);
            window.Content = stackPanel;
            window.ShowDialog();

            return listBox.Items
                .Cast<CheckBox>()
                .Where(x => x.IsChecked == true)
                .Select(x => x.Content.ToString())
                .ToArray();
        }

        /// <summary>
        /// 显示一个文本报告窗口
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="modal"></param>
        public static void TextReport(string title, string content, double width, double height, bool modal = false)
        {
            var textReport = new TextReport
            {
                Width = width,
                Height = height,
                Title = title
            };

            textReport.ContentTextBox.Text = content;
            if (modal)
            {
                textReport.ShowDialog();
            }
            else
            {
                textReport.Show();
            }
        }

        /// <summary>
        /// 显示一个多值输入窗口
        /// </summary>
        /// <param name="title"></param>
        /// <param name="entries"></param>
        public static bool? MultiInputs(string title, Dictionary<string, string> entries)
        {
            var multiInputs = new MultiInputs();
            multiInputs.Ready(entries, title);
            return multiInputs.ShowDialog();
        }

        /// <summary>
        /// 显示一个输入框
        /// </summary>
        /// <param name="tip"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string InputBox(string tip, string defaultValue = "")
        {
            var input = new InputBox(tip, defaultValue);
            if (input.ShowDialog() == true)
            {
                return input.Value;
            }

            return string.Empty;
        }

        /// <summary>
        /// 繁忙提示框
        /// </summary>
        /// <param name="title"></param>
        /// <param name="worker"></param>
        public static void BusyIndicator(string title, BackgroundWorker worker)
        {
            // todo: BusyIndicator
        }

        /// <summary>
        /// 进度提示框
        /// </summary>
        /// <param name="title"></param>
        /// <param name="worker"></param>
        public static void ProgressIndicator(string title, BackgroundWorker worker)
        {
            var tpw = new TaskProgressWindow { Title = title + " (0%)" };
            tpw.CancelButton.Click += (s, args) =>
            {
                tpw.CancelButton.IsEnabled = false;
                worker.CancelAsync();
            };
            worker.ProgressChanged += (s, args) =>
            {
                tpw.ProgressBar.Value = args.ProgressPercentage;
                tpw.Title = string.Format("{0} ({1}%)", title, args.ProgressPercentage);
            };
            worker.RunWorkerCompleted += (s, args) => tpw.Close();
            tpw.ShowDialog();
        }

        /// <summary>
        /// 显示一个Web浏览器
        /// </summary>
        /// <param name="title"></param>
        /// <param name="url"></param>
        public static void WebBrowser(string title, string url, int width, int height)
        {
            var form = new System.Windows.Forms.Form
            {
                Text = title,
                Width = width,
                Height = height,
                StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen,
                MaximizeBox = false,
                MinimizeBox = false,
                TopMost = true,
                ShowIcon = false,
                ShowInTaskbar = false
            };

            var browser = new WebKit.WebKitBrowser
            {
                Dock = System.Windows.Forms.DockStyle.Fill
            };

            form.Controls.Add(browser);
            browser.Navigate(url);
            form.Show();
        }

        /// <summary>
        /// 显示一个属性输入窗口
        /// </summary>
        /// <param name="title"></param>
        /// <param name="obj"></param>
        public static void PropertyWindow(string title, object obj, int width, int height, bool detail = false)
        {
            var propertyInputs = new PropertyInputs
            {
                Text = title,
                Width = width,
                Height = height
            };

            if (detail)
            {
                propertyInputs.SetDetailData(obj as object[]);
            }
            else
            {
                propertyInputs.SetData(obj);
            }

            propertyInputs.ShowDialog();
        }
    }

    /// <summary>
    /// 纯文本表格
    /// </summary>
    public class LogTable
    {
        private int[] _colWidths;
        /// <summary>
        /// Tab字符的宽度，为8
        /// </summary>
        public const int TabWidth = 8;

        /// <summary>
        /// 初始化新实例
        /// </summary>
        /// <param name="colWidths">列宽，应为TabWidth的整数倍</param>
        public LogTable(params int[] colWidths)
        {
            _colWidths = colWidths;
        }

        /// <summary>
        /// 获取一行的字符串表示
        /// </summary>
        /// <param name="contents">行中元素</param>
        /// <returns>字符串表示</returns>
        public string GetRow(params object[] contents)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < contents.Length; i++)
            {
                string content = contents[i].ToString();
                sb.Append(content);
                int nTab = (int)Math.Ceiling((double)(_colWidths[i] - GetStringWidth(content)) / TabWidth);
                for (int j = 0; j < nTab; j++)
                {
                    sb.Append('\t');
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 统计字符串宽度，ASCII字符宽度为1，其他字符宽度为2
        /// </summary>
        /// <param name="content">字符串</param>
        /// <returns>宽度</returns>
        public static int GetStringWidth(string content)
        {
            return content.Sum(c => c > 255 ? 2 : 1);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class DemoToolAttribute : Attribute
    {
        public string Name;
        public string Category;
        public int Order;

        public DemoToolAttribute(string displayName)
        {
            Name = displayName;
        }
    }
}
