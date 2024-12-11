using System.Diagnostics;
using System.Drawing;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Xml.XPath;

namespace ConsoleApp
{


    internal class Day11 : IDay
    {
        private string[] data;

        private List<long> nums;

        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            data = File.ReadAllLines($"{dir}/{GetType().Name}.txt");

            nums = data[0].Split(' ').Select(long.Parse).ToList();

        }


        Dictionary<long, long[]> blinkCache = new Dictionary<long, long[]>();

        long[] CachedBlinkOnce(long input)
        {
            if (blinkCache.TryGetValue(input, out var value))
                return value;

            long a;
            if (input == 0)
            {
                blinkCache.Add(input, [1]);
                return [1];
            }

            var len = (long)(Math.Log10(input)) + 1;
            if (len % 2 == 0)
            {
                var divideBy = (long)Math.Pow(10, len / 2);
                a = input / divideBy;
                var b = input % divideBy;

                blinkCache.Add(input, [a, b]);
                return [a, b];
            }

            a = input * 2024;

            blinkCache.Add(input, [a]);
            return [a];
        }

        private long BlinkDictionary(Dictionary<long, long> data, int blinks)
        {
            var stones = new Dictionary<long, long>(data);
            long result = 0;
            for (var blink = 0; blink < blinks; blink++)
            {
                result = 0;
                var afterBlink = new Dictionary<long, long>();
                foreach (var kvp in stones)
                {
                    var i = kvp.Key;

                    var newStones = CachedBlinkOnce(i);

                    foreach (var stone in newStones)
                    {
                        if (!afterBlink.TryAdd(stone, kvp.Value))
                            afterBlink[stone] += kvp.Value;
                        result += kvp.Value;
                    }
                }
                stones = new Dictionary<long, long>(afterBlink);
            }

            return result;
        }

        private Dictionary<long, long> Cache = new();

        private long BlinkRecursive(long input, int blinks)
        {
            if (blinks == 0)
            {
                return 1;
            }
            blinks--;

            var storedKey = (input << 8) + blinks;

            if (Cache.TryGetValue(storedKey, out var value))
                return value;

            var result = CachedBlinkOnce(input);

            var toReturn = BlinkRecursive(result[0], blinks);

            if (result.Length > 1)
            {
                toReturn += BlinkRecursive(result[1], blinks);

            }

            Cache[storedKey] = toReturn;

            return toReturn;
        }

        public decimal Part2()
        {
            var numberCounts = nums.ToDictionary(g => g, g => (long)1);
            return BlinkDictionary(numberCounts, 75);

        }

        public decimal Part1()
        {
            long result = 0;
            foreach (var num in nums)
            {
                result += BlinkRecursive(num, 25);

            }
            return result;
        }






    }
}
