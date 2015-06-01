using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocCounter
{
    /// <summary>
    /// The program class.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The entry point.
        /// </summary>
        /// <param name="args">The command line args.</param>
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }
            string fileName = args[0];
            if (!CodeCounter.IsFileCSharpSource(fileName))
            {
                Console.WriteLine("File is not CSharp source code.");
            }
            int loc = CodeCounter.LocCount(fileName);
            Console.WriteLine("File processed and saved as {0}.", CodeCounter.GetProcessedFileName(fileName));
            Console.WriteLine("Total LOC is {0}.", loc);
            Console.ReadLine();
        }
    }

    /// <summary>
    /// The code counter module.
    /// </summary>
    public static class CodeCounter
    {
        /// <summary>
        /// Count the code lines.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static int LocCount(string fileName)
        {
            var lines = System.IO.File.ReadAllLines(fileName).ToList();
            if (IsFileCSharpSource(fileName))
            {
                ProcessLines(lines);
            }
            System.IO.File.WriteAllLines(GetProcessedFileName(fileName), lines);
            return lines.Count;
        }

        private static void ProcessLines(List<string> lines)
        {
            var list = lines.ToList();
            foreach (var line in list)
            {
                // Blank lines
                if (string.IsNullOrWhiteSpace(line))
                {
                    lines.Remove(line);
                }

                var l = line.Trim();

                // Curly braces
                if (l == "{" || l == "}")
                {
                    lines.Remove(line);
                }

                // Comments
                if (l.StartsWith("//"))
                {
                    lines.Remove(line);
                }
            }
        }

        /// <summary>
        /// Gets the save as file name.
        /// </summary>
        /// <param name="originalFileName"></param>
        /// <returns></returns>
        public static string GetProcessedFileName(string originalFileName)
        {
            return System.IO.Path.ChangeExtension(originalFileName, "txt");
        }

        /// <summary>
        /// Determines if file is C# source.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsFileCSharpSource(string fileName)
        {
            return fileName.EndsWith("cs");
        }
    }
}
