using System.Diagnostics;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using String = System.String;

namespace ConsoleApp
{
    internal class Day22 : IDay
    {
        private List<long> StartNumbers;
        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var data = File.ReadAllLines($"{dir}/{GetType().Name}.txt");
            StartNumbers = data.Select(long.Parse).ToList();
        }



        public decimal Part1()
        {
            long result = 0;


            //for (int i = 0; i < 10; i++)
            //    Console.WriteLine(calculateSecret(123, i + 1));
            foreach (var number in StartNumbers)
            {
                var newSecret = calculateSecret(number, 2000);
                //       Console.WriteLine($"{number}: {newSecret}");
                result += newSecret;
            }



            return result;
        }

        private long calculateSecret(long input, int iterations)
        {
            long current = input;
            for (int i = 0; i < iterations; i++)
            {
                var next = current;
                next ^= next << 6;
                next %= 16777216;
                next ^= next >> 5;
                next %= 16777216;
                next ^= next << 11;
                next %= 16777216;
                current = next;
            }

            return current;
        }


        private decimal[] MonkeyPrices = new decimal[20 * 20 * 20 * 20];
        private void generateDictionary(long number)
        {
            var array = new int[2000];
            bool[] usedKeys = new bool[20 * 20 * 20 * 20];

            int lastNum = (int)(number % 10);
            long current = number;
            for (int i = 0; i < 2000; i++)
            {
                var next = current;
                next ^= next << 6;
                next %= 16777216;
                next ^= next >> 5;
                next %= 16777216;
                next ^= next << 11;
                next %= 16777216;
                current = next;

                var tail = (int)(next % 10);
                array[i] = tail - lastNum + 10;
                lastNum = tail;

                if (i >= 5)
                {
                    var key = array[i] +
                              array[i - 1] * 20 +
                              array[i - 2] * 20 * 20 +
                              array[i - 3] * 20 * 20 * 20;

                    if (!usedKeys[key])
                    {
                        usedKeys[key] = true;
                        MonkeyPrices[key] += tail;
                    }
                }
            }

        }

        public decimal Part2()
        {
            foreach (var number in StartNumbers)
            {
                generateDictionary(number);
            }

            var max = MonkeyPrices.Max(x => x);
            return max;
        }


    }
}
