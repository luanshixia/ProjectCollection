using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace Dreambuild.Common.Utils
{
    /// <summary>
    /// Short ID generator. 
    /// Inspired by http://stackoverflow.com/questions/9543715/generating-human-readable-usable-short-but-unique-ids
    /// </summary>
    public class ShortID
    {
        private static char[] _base62chars =
            "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
            .ToCharArray();

        private static Random _random = new Random();

        public static string GetBase62(int length)
        {
            var sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                sb.Append(_base62chars[_random.Next(62)]);
            }

            return sb.ToString();
        }

        public static string GetBase36(int length)
        {
            var sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                sb.Append(_base62chars[_random.Next(36)]);
            }

            return sb.ToString();
        }
    }
}
