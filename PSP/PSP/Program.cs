using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P1B
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter file name: ");
            string fileName = Console.ReadLine();
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            Console.Write("Read or write? (r/w): ");
            string option = Console.ReadLine();
            if (option != "r" && option != "w")
            {
                return;
            }
            if (option == "r")
            {
                if (!System.IO.File.Exists(fileName))
                {
                    Console.WriteLine("File not found.");
                }
                else
                {
                    Load(fileName)
                        .ForEach(x => Console.WriteLine(x));
                    Console.WriteLine("Read file succeeded.");
                }
            }
            else if (option == "w")
            {
                Console.Write("Enter number of reals: ");
                string nStr = Console.ReadLine();
                int n;
                if (!int.TryParse(nStr, out n))
                {
                    return;
                }
                var list = Enumerable.Range(1, n).Select(i =>
                {
                    Console.Write("Enter real number {0}: ", i);
                    string rStr = Console.ReadLine();
                    double r;
                    double.TryParse(rStr, out r);
                    return r;
                }).ToList();
                Save(list, fileName);
                Console.WriteLine("Write file succeeded.");
            }
            Console.ReadLine();
        }

        static void Save(List<double> list, string fileName)
        {
            System.IO.File.WriteAllLines(fileName, 
                list.Select(x => x.ToString())
                .ToArray());
        }

        static List<double> Load(string fileName)
        {
            return System.IO.File.ReadAllLines(fileName)
                .Select(x => Convert.ToDouble(x))
                .ToList();
        }
    }
}
