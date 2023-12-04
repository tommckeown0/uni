using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] vowelFrequencyArray = { 0, 0, 0, 0, 0 };
            string input = System.IO.File.ReadAllText(@".\obj\Debug\Treasureisland.txt");
            System.Console.WriteLine(input);

            Console.WriteLine("Characters total = " + input.Length);
            Console.WriteLine("Printable ASCII characters = " + numOfPrintableCharsASCII(input));
            Console.WriteLine("Non-printable ASCII characters = " + numOfNonPrintableCharsASCII(input));
            Console.WriteLine("ASCII Vowels = " + numOfVowelsASCII(input));
            Console.WriteLine("ASCII Consonants = " + numOfConsonantsASCII(input));

            vowelFrequency(input, vowelFrequencyArray);
            string vowels = "aeiou";
            for (int i = 0; i < vowelFrequencyArray.Length; i++)
            {
                Console.WriteLine("Frequency of " + vowels[i] + " = " + vowelFrequencyArray[i]);
            }
        }

        public static int numOfPrintableCharsASCII(string input)
        {
            int num = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] > 32)
                {
                    num++;
                }
            }
            return num;
        }

        public static int numOfNonPrintableCharsASCII(string input)
        {
            int num = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] < 33)
                {
                    num++;
                }
            }
            return num;
        }

        public static int numOfVowelsASCII(string input)
        {
            int num = 0;
            int[] vowels = { 97, 101, 105, 111, 117 };
            for (int i = 0; i < input.Length; i++)
            {
                if (vowels.Contains(char.ToLower(input[i])))
                {
                    num++;
                }
            }
            return num;
        }

        public static int numOfConsonantsASCII(string input)
        {
            int num = 0;
            int[] vowels = { 97, 101, 105, 111, 117 };
            for (int i = 0; i < input.Length; i++)
            {
                if ((char.ToLower(input[i]) > 96 && char.ToLower(input[i]) < 123) && !vowels.Contains(char.ToLower(input[i])))
                {
                    num++;
                }
            }
            return num;
        }

        public static void vowelFrequency(string input, int[] vowelFrequencyArray)
        {
            for (int i = 0; i < input.Length; i++)
            {
                switch (char.ToLower(input[i]))
                {
                    case 'a':
                        vowelFrequencyArray[0]++;
                        break;
                    case 'e':
                        vowelFrequencyArray[1]++;
                        break;
                    case 'i':
                        vowelFrequencyArray[2]++;
                        break;
                    case 'o':
                        vowelFrequencyArray[3]++;
                        break;
                    case 'u':
                        vowelFrequencyArray[4]++;
                        break;
                }
            }
        }
    }
}
