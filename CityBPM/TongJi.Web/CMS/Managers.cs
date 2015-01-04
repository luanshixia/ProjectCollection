using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TongJi.Web.Models;

namespace TongJi.Web.CMS
{
    public class FileManager
    {
        public static string GetSizeString(int size)
        {
            if (size < 1024)
            {
                return size + " Byte";
            }
            else if (size < 1024 * 1024)
            {
                return string.Format("{0:0.##} KB", size / 1024.0);
            }
            else
            {
                return string.Format("{0:0.##} MB", size / 1024.0 / 1024.0);
            }
        }
    }
}
