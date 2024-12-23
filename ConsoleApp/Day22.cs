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


        private Dictionary<int, decimal> MonkeyPrices = new();
        private void generateDictionary(long number)
        {
            var array = new int[2000];
            HashSet<int> usedKeys = [];

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
                array[i] = tail - lastNum;
                lastNum = tail;

                if (i >= 5)
                {
                    var key = (array[i] + 10) +
                              ((array[i - 1] + 10) << 8) +
                              ((array[i - 2] + 10) << 16) +
                              ((array[i - 3] + 10) << 24);

                    if (usedKeys.Add(key))
                    {
                        MonkeyPrices.TryAdd(key, 0);
                        MonkeyPrices[key] += tail;
                    }
                }
            }

        }

        public decimal Part2()
        {
            decimal result = 0;
            List<Tuple<int, int?>[]> monkeyList = [];
            foreach (var number in StartNumbers)
            {
                generateDictionary(number);
            }


            var max = MonkeyPrices.Max(x => x.Value);

            var kvp = MonkeyPrices.First(x => x.Value == max);

            var key = kvp.Key;

            var part1 = (key & 0xff000000) >> 24;
            var part2 = (key & 0x00ff0000) >> 16;
            var part3 = (key & 0x0000ff00) >> 8;
            var part4 = key & 0x000000ff;

            Console.WriteLine($"{part1 - 10},{part2 - 10},{part3 - 10},{part4 - 10} ");

            // lets check their fucking key
            // -2,1,-1,3

            key = (-2 + 10) << 24;
            key += (1 + 10) << 16;
            key += (-1 + 10) <<8;
            key += (3 + 10);
            Console.WriteLine($"Their example key: {MonkeyPrices[key]}");

            return max;
        }


    }
}
