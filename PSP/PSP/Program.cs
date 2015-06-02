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
            Console.Write("Read, write, or modify? (r/w/m): ");
            string option = Console.ReadLine();
            if (option != "r" && option != "w" && option != "m")
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
                Console.WriteLine("Successfully saved to {0}.", fileName);
            }
            else if (option == "m")
            {
                var list = Load(fileName);
                Console.WriteLine("Operations for items:");
                Console.WriteLine("[a] Accept");
                Console.WriteLine("[r] Replace");
                Console.WriteLine("[d] Delete");
                Console.WriteLine("[i] Insert After");
                Console.WriteLine("[e] Accept Remainder");
                int i = 0;
                while (i < list.Count)
                {
                    Console.Write("Item {0} of {1}: {2}. ", i + 1, list.Count, list[i]);
                    var key = Console.ReadLine();
                    if (key == "a")
                    {
                        i++;
                    }
                    else if (key == "r")
                    {
                        Console.Write("Enter new value: ");
                        string rStr = Console.ReadLine();
                        double r;
                        double.TryParse(rStr, out r);
                        list[i] = r;
                        i++;
                    }
                    else if (key == "d")
                    {
                        list.RemoveAt(i);
                    }
                    else if (key == "i")
                    {
                        Console.Write("Enter new value: ");
                        string rStr = Console.ReadLine();
                        double r;
                        double.TryParse(rStr, out r);
                        list.Insert(i + 1, r);
                        i += 2;
                    }
                    else if (key == "e")
                    {
                        break;
                    }
                }
                Console.Write("Enter new file name (empty to save directly): ");
                string newName = Console.ReadLine();
                if (string.IsNullOrEmpty(newName))
                {
                    newName = fileName;
                }
                Save(list, newName);
                Console.WriteLine("Successfully saved to {0}.", newName);
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
