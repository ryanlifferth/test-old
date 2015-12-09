using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.GoodCodingRules
{
    class Program
    {
        static void Main(string[] args)
        {
            // Good method writing
            var palindromeWord = "dad dad";

            Console.WriteLine("Bad Palindrome Example: " + Test(palindromeWord));
            Console.WriteLine("Better Palindrome Example: " + Check(palindromeWord));
            Console.WriteLine("Best Palindrome Example: " + IsPalindrome(palindromeWord));

            // Good testing helper functions - Account example


            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
        }

        #region Good Method Writing
        /// <summary>
        ///     Bad palindrome example
        ///         - Hard to read and follow
        ///         - Bad naming conventions
        ///         - Too much focus on types (e.g., strInput)
        ///         - Not a great name for method
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        private static bool Test(string strInput)
        {
            string strTrimmed = strInput.Replace(" ", ""); // Not a trim
            string strReversed = new string(strTrimmed.Reverse().ToArray());
            return strReversed.Equals(strReversed);
        }

        /// <summary>
        ///     Good palindrome example
        ///         - use of var keyword
        ///         - better naming (not focused on type)
        ///         - still a little hard to read (e.g., reusing input param can be confusing)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static bool Check(string input)
        {
            input = input.Replace(" ", "");
            var reversed = new string(input.Reverse().ToArray());
            return reversed.Equals(input);
        }

        /// <summary>
        ///     Best palindrome example
        ///         - Good method name
        ///         - Great variable names (forwards, backwards)
        ///         - with good variable names, the bool check is the VERY definition of a palindrome (i.e., method is easy to follow)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static bool IsPalindrome(string input)
        {
            var forwards = input.Replace(" ", "");
            var backwards = new string(forwards.Reverse().ToArray());
            return backwards.Equals(forwards);
        }

        #endregion


    }
}
