using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ryan.Extensions;
using System.Linq.Expressions;

namespace Ryan.Delegates
{
    class Program
    {
        static void Main(string[] args)
        {

            IEnumerable<string> cities = new[] { "Salt Lake City", "London", "Barcelona", "Layton", "Bangkok", "Sao Paulo", "Las Vegas" };

            IEnumerable<string> query;

            // Example 1 of delegate
            query = cities.Filter(StringThatStartsWithL);

            // Example 2 of delegate (does the same thing as above)
            query = cities.Filter(delegate(string item) 
                                    {
                                        return item.StartsWith("L");
                                    });

            // Example 3 of delegate
            //  item => item.StartsWith("L")
            //  left hand side (item) represents the signature - uses type inference to determine the type (string in this case)
            //  right hand side (item.StartsWith("L")) represents the body of the method
            query = cities.Filter(item => item.StartsWith("L"));

            // Example 4 of delegate = with FUNC
            query = cities.FilterFunc(item => item.StartsWith("L"));

            // Example 5 - uses LINQ, which uses delegates and FUNCs in its construction
            query = cities.Where(city => city.StartsWith("L"))
                          .OrderByDescending(city => city.Length);

            foreach (var city in query)
            {
                Console.WriteLine(city);
            }

            WorkWithFuncs();            


            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
        }

        private static void WorkWithFuncs()
        {
            // Quick examples of FUNC
            //  Func is a generic type and encapsulates delegates (callable code)
            //  Overloaded methods where last param is always the return type and others are possible input params
            Func<int, int> square = x => x * x;
            Func<int, int, int> add = (x, y) => x + y;  // Parenthesis are requried with 0 or 2 params, not required with a single param

            Console.WriteLine(square(3));   // Will return 9
            Console.WriteLine(add(3, 3));   // Will return 4
            Console.WriteLine(square(add(1, 3)));

            // Action
            //  Action is just like FUNC, but without a return type - returns void
            Action<int> write = x => Console.WriteLine(x);
            write(add(2, 4));

            // Expressions
            Expression<Func<int, int>> squareExpression = x => x * x;

            //write(squareExpression(3));
        }

        // Example 1 of delegate
        private static bool StringThatStartsWithL(string s)
        {
            return s.StartsWith("L");
        }

        
    }
}

namespace Ryan.Extensions
{
    public static class FilterExtensions
    {
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> input, FilterDelegate<T> predicate)
        {
            foreach (var item in input)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

        public delegate bool FilterDelegate<T>(T item);
    }


    public static class FilterExtensionsWithFunc
    {
        /// <summary>
        ///     In this example, rather than defining the delegate separately, we can use the FUNC method to define the delegate
        ///     right in the signature of the method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<T> FilterFunc<T>(this IEnumerable<T> input, Func<T, bool> predicate) 
        {
            foreach (var item in input)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }
    }
}
