﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TP = Test Point

namespace P1B
{
    internal static class Messages
    {
        public const string FileNotFound = "File not found. Please enter again. ";
        public const string FileAlreadyExists = "File already exists. Please enter again. ";
    }

    class Program
    {
        static void Main(string[] args)
        {
            string fileName = PromptForValidInput("Enter file name: ", x => IsFileNameProper(x)); // [TP1] empty file name
            if (fileName == null)
            {
                return;
            }
            string option = PromptForValidInput("Read, write, or modify? (r/w/m): ", x => x == "r" || x == "w" || x == "m"); // [TP2] prompt for valid input
            if (option == null)
            {
                return;
            }
            if (option == "r")
            {
                fileName = PromptForValidInput(Messages.FileNotFound, x => System.IO.File.Exists(x), fileName); // [TP3] file not found
                if (fileName == null)
                {
                    return;
                }
                int index = 0;
                var arrays = LoadArray(fileName);
                while (true)
                {
                    var indexStr = PromptForValidInput("Enter key index: ", x => int.TryParse(x, out index));
                    if (indexStr == null)
                    {
                        return;
                    }
                    var array = arrays.Select(x => x[index]).ToArray();
                    var stat = new Statistics(array);
                    Console.WriteLine("Col={0}|Min={1}|Max={2}|Avg={3}|SD={4}", index, stat.Min, stat.Max, stat.Avg, stat.SD);
                    Console.WriteLine("Read file succeeded.");
                    if (PromptForValidInput("Save to file? ", x => true) == "y")
                    {
                        // Save
                    }
                }
            }
            else if (option == "w")
            {
                fileName = PromptForValidInput(Messages.FileAlreadyExists, x => !System.IO.File.Exists(x), fileName);
                if (fileName == null)
                {
                    return;
                }
                int n = 0;
                string nStr = PromptForValidInput("Enter number of records: ", x => int.TryParse(x, out n));
                if (nStr == null)
                {
                    return;
                }
                var list = Enumerable.Range(1, n).Select(i =>
                {
                    string line = PromptForValidInput(string.Format("Enter record {0}: ", i), x => ValidArray(x));
                    return ParseArray(line);
                }).ToList();
                SaveArray(list, fileName);
                Console.WriteLine("Successfully saved to {0}.", fileName);
            }
            else if (option == "m") // m is not modified for arryas.
            {
                fileName = PromptForValidInput(Messages.FileNotFound, x => System.IO.File.Exists(x), fileName);
                if (fileName == null)
                {
                    return;
                }
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

        static string PromptForValidInput(string msg, Predicate<string> pred, string input = null)
        {
            if (input != null && pred(input))
            {
                return input;
            }
            Console.Write(msg);
            while (true)
            {
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    return null;
                }
                if (pred(input))
                {
                    break;
                }
                Console.Write("Improper input. Input again, or press ENTER to terminate: ");
            }
            return input;
        }

        static bool IsFileNameProper(string fileName)
        {
            return System.IO.Path.GetInvalidFileNameChars().All(c => !fileName.Contains(c));
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

        static void SaveArray(List<List<double>> list, string fileName)
        {
            System.IO.File.WriteAllLines(fileName,
                list.Select(x => FormatArray(x))
                .ToArray());
        }

        static List<List<double>> LoadArray(string fileName)
        {
            return System.IO.File.ReadAllLines(fileName)
                .Select(x => ParseArray(x))
                .ToList();
        }

        static string FormatArray(List<double> values)
        {
            return string.Join(",", values);
        }

        static List<double> ParseArray(string line)
        {
            return line.Split(',').Select(y => Convert.ToDouble(y)).ToList();
        }

        static bool ValidArray(string line)
        {
            return line.Replace(",", "").Replace(".", "").All(c => char.IsDigit(c));
        }

        static void SortArrays(List<List<double>> arrays, int key)
        {
            arrays.Sort((x, y) =>
            {
                double a = x[key];
                double b = y[key];
                if (a > b)
                {
                    return 1;
                }
                else if (a < b)
                {
                    return -1;
                }
                else // a == b
                {
                    return 0;
                }
            });
        }
    }

    public class Statistics
    {
        public double Min { get; private set; }
        public double Max { get; private set; }
        public double Avg { get; private set; }
        public double Var { get; private set; }
        public double SD { get; private set; }

        public Statistics(double[] values)
        {
            Min = values.Min();
            Max = values.Max();
            Avg = values.Average();
            Var = values.Average(x => (x - Avg) * (x - Avg));
            SD = Math.Sqrt(Var);
        }
    }
}
