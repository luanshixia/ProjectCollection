using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dreambuild.IO
{
    public class CsvExporter<T>
    {
        public List<T> Data { get; private set; }
        public List<string> PropertyPaths { get; private set; }
        public List<string> Headers { get; private set; }
        public bool ShowHeaders { get; set; }

        public CsvExporter(List<T> data, List<string> propertyPaths)
        {
            Data = data;
            PropertyPaths = propertyPaths;
            Headers = new List<string>();
            ShowHeaders = false;
        }

        public void Export(string fileName)
        {
            var lines = GetLines();
            System.IO.File.WriteAllLines(fileName, lines.ToArray(), Encoding.Default);
        }

        private List<string> GetLines()
        {
            var lines = Data.Select(x => GetLine(x)).ToList();
            if (ShowHeaders)
            {
                lines.Insert(0, GetHeaderLine());
            }
            return lines;
        }

        private string GetLine(T item)
        {
            return string.Join(",", PropertyPaths.Select(x => GetPropertyValue(item, x).ToString()).ToArray());
        }

        private string GetHeaderLine()
        {
            return string.Join(",", Headers.ToArray());
        }

        private static object GetPropertyValue(object obj, string path)
        {
            var paths = path.Split('.');
            var host = obj;
            foreach (var prop in paths)
            {
                host = System.ComponentModel.TypeDescriptor.GetProperties(host)[prop].GetValue(host);
            }
            return host;
        }
    }
}
