using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace DesktopClient
{
    public static class ProjIconManager
    {
        public static List<string> Icons { get; private set; }
        private const string PictureDirectory = "\\proj_icons\\";
        private static string[] _pictureExtensions = { "jpg", "gif", "png" };
        public static string dllPath = typeof(ProjIconManager).Assembly.Location;
        public static string dllDir = dllPath.Remove(dllPath.LastIndexOf('\\') + 1);
        private static string _normalImage;
        private static string _hoverImage;

        static ProjIconManager()
        {
            Icons = new List<string>();
            var files = Directory.GetFiles(dllDir + PictureDirectory);
            foreach (var file in files)
            {
                var extension = file.Substring(file.LastIndexOf('.') + 1).ToLower();
                if (_pictureExtensions.Contains(extension))
                {
                    //string path = "pack://application:,,,/DesktopClient;Component/proj_icons/" + file;
                    Icons.Add(file);
                }
            }
        }

        public static string GetIcon(int iconId, int frameId)
        {
            if (frameId == 1)
            {
                _hoverImage = Icons[iconId + 1];
                return _hoverImage;
            }
            else
            {
                _normalImage = Icons[iconId];
                return _normalImage;
            }
        }

        public static string GetName(int iconId)
        {
            string name = "";
            string files = Icons[iconId];
            name = files.Substring(files.LastIndexOf('\\') + 1);
            name = name.Remove(name.LastIndexOf('.'));
            return name;
        }
    }
}
