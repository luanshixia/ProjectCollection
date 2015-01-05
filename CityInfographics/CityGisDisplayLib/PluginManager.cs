using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;

using System.ComponentModel;

namespace TongJi.Gis.Display
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
            var dlls = System.IO.Directory.GetFiles(AppPath + PluginDirectory).Where(x => x.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)).ToArray();
            foreach (var dll in dlls)
            {
                var plugin = System.Reflection.Assembly.LoadFrom(dll);
                var entryClass = plugin.GetTypes().First(x => x.GetInterfaces().Contains(typeof(IPlugin)));
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
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            return assembly.GetTypes()
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
                foreach (Attribute attr in Attribute.GetCustomAttributes(method))
                {
                    // Check for the DemoTool attribute.
                    if (attr.GetType() == typeof(DemoToolAttribute))
                    {
                        var method1 = method;
                        yield return Tuple.Create<string, RoutedEventHandler>((attr as DemoToolAttribute).DisplayName, (s, args) =>
                        {
                            method1.Invoke(this, null);
                        });
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
            Buttons = new List<ToolbarButton>();
        }

        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="textLebel">文本标签</param>
        /// <param name="clickHandler">单击事件</param>
        /// <param name="imageUri">图片地址</param>
        /// <param name="description">工具提示</param>
        /// <param name="isRadioButton">是否单选按钮</param>
        public void AddButton(string textLebel, Action clickHandler, string imageUri = null, string description = null, ToolbarButtonType type = ToolbarButtonType.Default)
        {
            Buttons.Add(new ToolbarButton { TextLabel = textLebel, ClickHandler = clickHandler, Description = description, ImageUri = imageUri, ButtonType = type });
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
            Window window = new Window { Width = 300, SizeToContent = SizeToContent.Height, WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen, ShowInTaskbar = false, WindowStyle = WindowStyle.ToolWindow, Title = "请选择" };
            StackPanel sp = new StackPanel { Margin = new Thickness(5) };
            TextBlock tb = new TextBlock { Text = (tip == string.Empty ? "请选择一个选项。" : tip), TextWrapping = TextWrapping.Wrap };
            int result = -1;
            var btns = options.Select((x, i) => new Button { Content = x, Tag = i }).ToList();
            btns.ForEach(x => x.Click += (s, args) => { result = (int)x.Tag; window.DialogResult = true; });
            sp.Children.Add(tb);
            btns.ForEach(x => sp.Children.Add(x));
            window.Content = sp;
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
            Window window = new Window { Width = 300, SizeToContent = SizeToContent.Height, WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen, ShowInTaskbar = false, WindowStyle = WindowStyle.ToolWindow, Title = "请选择" };
            StackPanel sp = new StackPanel { Margin = new Thickness(10) };
            TextBlock tb = new TextBlock { Text = (tip == string.Empty ? "请选择一个项目。" : tip), TextWrapping = TextWrapping.Wrap };
            ListBox list = new ListBox { Height = 200 };
            choices.ToList().ForEach(x => list.Items.Add(new ListBoxItem { Content = x }));
            Button btnOk = new Button { Content = "确定", Margin = new Thickness(5) };
            btnOk.Click += (s, args) => window.DialogResult = true;
            sp.Children.Add(tb);
            sp.Children.Add(list);
            sp.Children.Add(btnOk);
            window.Content = sp;
            window.ShowDialog();
            return list.SelectedItem == null ? string.Empty : (list.SelectedItem as ListBoxItem).Content.ToString();
        }

        /// <summary>
        /// 显示一个可多选的列表选择面板
        /// </summary>
        /// <param name="tip"></param>
        /// <param name="choices"></param>
        /// <returns></returns>
        public static string[] GetChoices(string tip, params string[] choices)
        {
            Window window = new Window { Width = 300, SizeToContent = SizeToContent.Height, WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen, ShowInTaskbar = false, WindowStyle = WindowStyle.ToolWindow, Title = "请选择" };
            StackPanel sp = new StackPanel { Margin = new Thickness(10) };
            TextBlock tb = new TextBlock { Text = (tip == string.Empty ? "请选择一个选项。" : tip), TextWrapping = TextWrapping.Wrap };
            ListBox list = new ListBox { Height = 200 };
            choices.ToList().ForEach(x => list.Items.Add(new CheckBox { Content = x }));
            Button btnOk = new Button { Content = "确定", Margin = new Thickness(5) };
            btnOk.Click += (s, args) => window.DialogResult = true;
            sp.Children.Add(tb);
            sp.Children.Add(list);
            sp.Children.Add(btnOk);
            window.Content = sp;
            window.ShowDialog();
            return list.Items.Cast<CheckBox>().Where(x => x.IsChecked == true).Select(x => x.Content.ToString()).ToArray();
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
            TextReport tr = new TextReport();
            tr.Width = width;
            tr.Height = height;
            tr.Title = title;
            tr.txtContent.Text = content;
            if (modal)
            {
                tr.ShowDialog();
            }
            else
            {
                tr.Show();
            }
        }

        /// <summary>
        /// 显示一个多值输入窗口
        /// </summary>
        /// <param name="title"></param>
        /// <param name="entries"></param>
        public static void MultiInputs(string title, Dictionary<string, string> entries)
        {
            MultiInputs mi = new MultiInputs();
            mi.Ready(entries, title);
            mi.ShowDialog();
        }

        /// <summary>
        /// 显示一个输入框
        /// </summary>
        /// <param name="tip"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string InputBox(string tip, string defaultValue = "")
        {
            InputBox input = new InputBox(tip, defaultValue);
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
            TaskProgressWindow tpw = new TaskProgressWindow { Title = title + " (0%)" };
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

        // todo: 实现Web浏览器
        public static void WebBrowser(string title, string html)
        {
            WebBrowser wb = new WebBrowser();
        }

        /// <summary>
        /// 显示一个属性输入窗口
        /// </summary>
        /// <param name="title"></param>
        /// <param name="obj"></param>
        public static void PropertyInputs(string title, object obj, int width, int height)
        {
            PropertyInputs pi = new PropertyInputs { Text = title, PropertyObject = obj, Width = width, Height = height };
            pi.ShowDialog();
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
            StringBuilder sb = new StringBuilder();
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
    public class DemoToolAttribute : Attribute
    {
        public string DisplayName;
        public int Order;

        public DemoToolAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
