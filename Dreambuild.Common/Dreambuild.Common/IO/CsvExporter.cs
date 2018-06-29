using Dreambuild.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dreambuild.IO
{
    /// <summary>
    /// CSV file creation.
    /// </summary>
    /// <typeparam name="T">The type of data record.</typeparam>
    public class CsvExporter<T>
    {
        public List<T> Data { get; }
        public List<string> PropertyPaths { get; }
        public List<string> Headers { get; }
        public bool ShowHeaders { get; set; }

        public CsvExporter(List<T> data, List<string> propertyPaths)
        {
            this.Data = data;
            this.PropertyPaths = propertyPaths;
            this.Headers = new List<string>();
            this.ShowHeaders = false;
        }

        public void Export(string fileName)
        {
            var lines = this.GetLines();
            File.WriteAllLines(fileName, lines.ToArray());
        }

        private List<string> GetLines()
        {
            var lines = this.Data
                .Select(x => this.GetLine(x))
                .ToList();

            if (this.ShowHeaders)
            {
                lines.Insert(0, this.GetHeaderLine());
            }

            return lines;
        }

        private string GetLine(T item)
        {
            return string.Join(
                separator: ",",
                value: this.PropertyPaths
                    .Select(path => item.GetPropertyValue(path).ToString())
                    .ToArray());
        }

        private string GetHeaderLine()
        {
            return string.Join(",", this.Headers.ToArray());
        }
    }
}
