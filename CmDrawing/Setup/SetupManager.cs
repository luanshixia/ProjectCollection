using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using System.IO;
using System.Runtime.InteropServices;
using IWshRuntimeLibrary;

namespace TongJi.Setup
{
    public static class SetupManager
    {
        private static XElement _xmlConfig = XDocument.Load("Config.xml").Root;

        public static void Start(SetupConfig config)
        {
            config.SourceAndTargetFolders.ToList().ForEach(x => CopyDirectory(x.Key, x.Value));
            config.DesktopShortcuts.ForEach(x => CreateShortcut(x));
            config.StartMenuShortcuts.ForEach(x => CreateShortcut(x));
        }

        public static void CopyDirectory(string srcDir, string tgtDir)
        {
            DirectoryInfo source = new DirectoryInfo(srcDir);
            DirectoryInfo target = new DirectoryInfo(tgtDir);

            if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new Exception("尝试将父目录拷贝到子目录。");
            }

            if (!source.Exists)
            {
                return;
            }

            if (!target.Exists)
            {
                target.Create();
            }

            FileInfo[] files = source.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                System.IO.File.Copy(files[i].FullName, target.FullName + @"\" + files[i].Name, true);
            }

            DirectoryInfo[] dirs = source.GetDirectories();
            for (int j = 0; j < dirs.Length; j++)
            {
                CopyDirectory(dirs[j].FullName, target.FullName + @"\" + dirs[j].Name);
            }
        }

        public static void CreateShortcut(Shortcut sc)
        {
            //实例化WshShell对象 
            WshShell shell = new WshShell();

            //通过该对象的 CreateShortcut 方法来创建 IWshShortcut 接口的实例对象 
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(string.Format("{0}\\{1}.lnk", sc.Folder, sc.Name));

            //设置快捷方式的目标所在的位置(目标程序完整路径) 
            shortcut.TargetPath = sc.Target;

            //应用程序的工作目录 
            //当用户没有指定一个具体的目录时，快捷方式的目标应用程序将使用该属性所指定的目录来装载或保存文件。 
            shortcut.WorkingDirectory = sc.Target.Remove(sc.Target.LastIndexOf('\\') + 1);

            //目标应用程序窗口类型(1.Normal window普通窗口,3.Maximized最大化窗口,7.Minimized最小化) 
            shortcut.WindowStyle = 1;

            //快捷方式的描述 
            shortcut.Description = sc.Description;

            //可以自定义快捷方式图标.(如果不设置,则将默认源文件图标.) 
            if (!string.IsNullOrEmpty(sc.Icon))
            {
                shortcut.IconLocation = sc.Icon;
            }

            //设置应用程序的启动参数(如果应用程序支持的话) 
            if (!string.IsNullOrEmpty(sc.Args))
            {
                shortcut.Arguments = sc.Args;
            }

            //设置快捷键(如果有必要的话.) 
            //shortcut.Hotkey = "CTRL+ALT+D"; 

            //保存快捷方式 
            shortcut.Save();
        }

        public static string GetLicenseText()
        {
            return System.IO.File.ReadAllText("License.txt");
        }

        public static string GetDefaultInstallationPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\" + _xmlConfig.AttValue("InstallPath");
        }

        public static string GetProductName()
        {
            return _xmlConfig.AttValue("ProductName");
        }

        public static string GetDesktop()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        }

        public static string GetStartMenu()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Programs);
        }

        public static SetupConfig BuildSetupConfig(string path)
        {
            SetupConfig config = new SetupConfig();
            AcadProduct product = AcadProductManager.GetProduct();
            _xmlConfig.Element("Folders").Elements().ToList().ForEach(x =>
            {
                config.SourceAndTargetFolders.Add(x.AttValue("Source"), string.Format(x.AttValue("Target"), path, product.Path));
            });
            _xmlConfig.Element("Desktop").Elements().ToList().ForEach(x =>
            {
                Shortcut sc = new Shortcut(x.AttValue("Name"), GetDesktop(), string.Format(x.AttValue("Target"), path)) { Description = x.AttValue("Description"), Args = x.AttValue("Args"), Icon = x.AttValue("Icon") };
                config.DesktopShortcuts.Add(sc);
            });
            _xmlConfig.Element("StartMenu").Elements().ToList().ForEach(x =>
            {
                string programStartMenu = GetStartMenu() + "\\" + x.AttValue("Folder");
                if (!System.IO.Directory.Exists(programStartMenu))
                {
                    System.IO.Directory.CreateDirectory(programStartMenu);
                }
                Shortcut sc = new Shortcut(x.AttValue("Name"), programStartMenu, string.Format(x.AttValue("Target"), path)) { Description = x.AttValue("Description"), Args = x.AttValue("Args"), Icon = x.AttValue("Icon") };
                config.StartMenuShortcuts.Add(sc);
            });
            return config;
        }

        public static string AttValue(this XElement xe, string attName)
        {
            return xe.Attribute(attName) == null ? string.Empty : xe.Attribute(attName).Value;
        }
    }

    public class SetupConfig
    {
        public Dictionary<string, string> SourceAndTargetFolders { get; private set; }
        public List<Shortcut> DesktopShortcuts { get; private set; }
        public List<Shortcut> StartMenuShortcuts { get; private set; }

        public SetupConfig()
        {
            SourceAndTargetFolders = new Dictionary<string, string>();
            DesktopShortcuts = new List<Shortcut>();
            StartMenuShortcuts = new List<Shortcut>();
        }
    }

    public class Shortcut
    {
        public string Name;
        public string Folder;
        public string Description = string.Empty;
        public string Target;
        public string Args;
        public string Icon;

        public Shortcut(string name, string folder, string target)
        {
            Name = name;
            Folder = folder;
            Target = target;
        }
    }
}
