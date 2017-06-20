using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ryan.Encryption.EncryptionUtil;

namespace Ryan.Encryption
{
    class Program
    {
        static void Main(string[] args)
        {

            //Console.WriteLine("Please enter a password/salt to use:");
            //string password = Console.ReadLine();
            Console.WriteLine("Please enter a string to encrypt:");
            string plaintext = Console.ReadLine();
            Console.WriteLine("");

            string encryptedstring = EncryptionUtil.Encrypt(plaintext);
            Console.WriteLine("Your encrypted string is: " + encryptedstring);
            Console.WriteLine("");


            Console.WriteLine("Your decrypted string is:");
            string decryptedstring = EncryptionUtil.Decrypt(encryptedstring);
            Console.WriteLine(decryptedstring);
            Console.WriteLine("");

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();


        }
    }
}
