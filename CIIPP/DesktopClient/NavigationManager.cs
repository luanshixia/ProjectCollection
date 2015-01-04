using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopClient
{
    public static class NavigationManager
    {
        private static List<Uri> _history = new List<Uri> { new Uri("RichMainPage.xaml", UriKind.Relative) };
        public static List<Uri> History { get { return _history; } }

        private static List<string> _params = new List<string> { string.Empty };
        //public static List<string> Params { get { return _params; } }

        private static int _navPos = 0;
        public static string Param { get { return _params[_navPos]; } }

        private static void NavInternal(int navPos)
        {
            MainWindow.Current.NavFrame.Source = _history[navPos];
            _navPos = navPos;
        }

        public static void Navigate(string pageName, string param = "")
        {
            Uri uri = new Uri(pageName, UriKind.Relative);
            Navigate(uri, param);
        }

        public static void Navigate(Uri uri, string param = "")
        {
            if (MainWindow.Current.NavFrame.Source.ToString().Contains(uri.ToString()))
            {
                MainWindow.Current.NavFrame.Refresh();
            }
            else
            {
                MainWindow.Current.NavFrame.Source = uri;
            }
            if (uri.ToString() != _history[_navPos].ToString())
            {
                if (CanForward)
                {
                    _history.RemoveRange(_navPos + 1, _history.Count - _navPos - 1);
                    _params.RemoveRange(_navPos + 1, _params.Count - _navPos - 1);
                }
                _history.Add(uri);
                _params.Add(param);
                _navPos++;
            }
        }

        public static void Back()
        {
            if (CanBack)
            {
                NavInternal(_navPos - 1);
            }
        }

        public static void Forward()
        {
            if (CanForward)
            {
                NavInternal(_navPos + 1);
            }
        }

        public static bool CanBack { get { return _history.Count > 1 && _navPos > 0; } }
        public static bool CanForward { get { return _history.Count - 1 > _navPos; } }

        public static string GetQueryString(string key)
        {
            return QueryString(Param, key);
        }

        private static string QueryString(string source, string key)
        {
            if (source == string.Empty)
            {
                return string.Empty;
            }
            Dictionary<string, string> dict = new Dictionary<string,string> ();
            source.Split('&').Select(x => x.Split('=')).ToList().ForEach(x => dict.Add(x[0], x[1]));
            if (dict.Keys.Contains(key))
            {
                return dict[key];
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
