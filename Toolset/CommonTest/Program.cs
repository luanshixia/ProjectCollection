using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Dreambuild.Functional;

namespace CommonTest
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Name of the method to test (Press ESC to exit): ");
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape)
                {
                    break;
                }
                var name = key.KeyChar + Console.ReadLine();
                var method = typeof(TestMethods).GetMethod(name);
                if (method != null)
                {
                    method.Invoke(null, null);
                }
                else
                {
                    Console.WriteLine("No such method found.");
                }
            }
        }
    }

    public static class TestMethods
    {
        public static void HelloWorld()
        {
            Console.WriteLine("Hello world!");
        }

        public static void Options()
        {
            var option1 = Option<int>.None;
            var option2 = Option.Some(1);

        }

        public static void Closure()
        {
            Func<Action> func = () =>
            {
                int i = 0;
                Action inner = () =>
                {
                    i++;
                    Console.WriteLine(i);
                };
                return inner;
            };
            Action action = func();
            for (int i = 0; i < 5; i++)
            {
                action();
            }
        }
    }
}
