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


        // cache support really provieds miniscule gains. If that....
        Dictionary<long, long[]> blinkCache = new Dictionary<long, long[]>();
        
        long[] BlinkOnceWithCacheSupport(long input)
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

        private Dictionary<long, long> stones;
        private Dictionary<long, long> afterBlink;
        private long BlinkDictionary(HashSet<long> input, int blinks)
        {
            afterBlink = new Dictionary<long, long>();
            stones = input.ToDictionary(g => g, g => (long)1);

            for (var blink = 0; blink < blinks; blink++)
            {
                foreach (var stone in stones)
                {

                    var newStones = BlinkOnceWithCacheSupport(stone.Key);

                    foreach (var newStone in newStones)
                    {
                        if (!afterBlink.TryAdd(newStone, stone.Value))
                            afterBlink[newStone] += stone.Value;

                        stones[stone.Key] = 0;
                    }
                }

                (stones, afterBlink) = (afterBlink, stones);
            }

            return stones.Sum(x => x.Value);
        }

        private Dictionary<long, int> UniqueNumberMap = new Dictionary<long, int>();
        int UniqueNumberCounter = 0;
        private const int MaxUniqueNumbers = 5000;

        private int GetIndex(long number)
        {
            if (UniqueNumberMap.TryGetValue(number, out var value))
                return value;

            else UniqueNumberMap[number] = UniqueNumberCounter++;

            return UniqueNumberMap[number];
        }


        private long[] array = new long[MaxUniqueNumbers * 2];
  
        private long BlinkArray(HashSet<long> data, int blinks)
        {
            var currentStones = data;
            long result = 0;

            for (var blink = 0; blink < blinks; blink++)
            {
                var readOffset = MaxUniqueNumbers * ((blink + 1) % 2);
                var writeOffset = MaxUniqueNumbers * (blink % 2);
                result = 0;
                var afterBlink = new HashSet<long>();
                foreach (var stone in currentStones)
                {
                    var newStones = BlinkOnceWithCacheSupport(stone);

                    foreach (var newStone in newStones)
                    {
                        var fromIndex = GetIndex(stone) + readOffset;
                        var toIndex = GetIndex(newStone) + writeOffset;

                        array[toIndex] += array[fromIndex];
                        result += array[fromIndex];
                        afterBlink.Add(newStone);
                    }
                }

                currentStones = afterBlink;
                // read part will be write part next loop so need to clear it
                Array.Clear(array, readOffset, MaxUniqueNumbers);
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

            var result = BlinkOnceWithCacheSupport(input);

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
            Array.Clear(array);
            foreach (var number in nums)
            {
                array[GetIndex(number) + MaxUniqueNumbers] = 1;
            }

            return BlinkArray(nums.ToHashSet(), 75);
        }

        public decimal Part1()
        {
            foreach (var number in nums)
            {
                array[GetIndex(number) + MaxUniqueNumbers] = 1;
            }

            return BlinkArray(nums.ToHashSet(), 25);
        }
    }
}
