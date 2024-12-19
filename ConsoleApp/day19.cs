using System.Diagnostics;
using System.Drawing;

namespace ConsoleApp
{
    internal class Day19 : IDay
    {
        // white (w), blue (u), black (b), red (r), or green (g)
        enum color
        {
            white,
            blue,
            black,
            red,
            green
        }

        color mapChar(char c)
        {
            switch (c)
            {
                case 'w': return color.white;
                case 'u': return color.blue;
                case 'b': return color.black;
                case 'r': return color.red;
                case 'g': return color.green;
            }

            throw new Exception("dont do this to me");
        }

        private List<string> parts;
        private List<string> inputs = [];
        private SortedSet<string> sortedParts = [];
        private SortedSet<decimal> sortedInts = [];

        private int maxPartLength;
        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var rows = File.ReadAllLines($"{dir}/{GetType().Name}.txt");

            parts = rows[0].Split(',', StringSplitOptions.TrimEntries).ToList();
            inputs = rows.Skip(2).ToList();
            sortedParts = new SortedSet<string>(parts);

            maxPartLength = sortedParts.Max(x => x.Length);



            foreach (var part in sortedParts)
            {
                sortedInts.Add(stringToNum(part));
            }
        }

        private decimal stringToNum(string input)
        {
            long num = 0;
            foreach (var ch in input)
            {
                var color = mapChar(ch);
                num = (num << 3) + (long)color;
            }

            return num;
        }



        private Dictionary<string, bool> part1Cache = [];


        private bool part1Test(string toTest)
        {
            if (string.IsNullOrEmpty(toTest))
                return true;

            if (part1Cache.TryGetValue(toTest, out var test))
            {
                return test;
            }

            for (int i = 1; i <= maxPartLength && i <= toTest.Length; i++)
            {
                var subStr = toTest.Substring(0, i);

                if (sortedParts.Contains(subStr))
                {

                    var tail = toTest.Substring(subStr.Length, toTest.Length - subStr.Length);

                    //if (part1Test(tail))
                    //    return true;

                    if (!part1Cache.ContainsKey(tail))
                    {
                        part1Cache[tail] = part1Test(tail);
                    }

                    if (part1Cache[tail])
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        private int part2Count = 0;

        private decimal part2Dictionary(string toTest)
        {
            Dictionary<string, decimal> countDictionary = [];
            decimal result = 0;
            countDictionary.Add(toTest, 1);
            while (countDictionary.Count > 0)
            {

                Dictionary<string, decimal> nextDictionary = [];
                foreach (var item in countDictionary)
                {
                    for (int i = 1; i <= maxPartLength && i <= item.Key.Length; i++)
                    {
                        var subStr = item.Key.Substring(0, i);
                        if (sortedParts.Contains(subStr))
                        {
                            if (subStr.Length == item.Key.Length)
                                result += item.Value;
                            else
                            {
                                var tail = item.Key.Substring(subStr.Length, item.Key.Length - subStr.Length);
                                nextDictionary.TryAdd(tail, 0);
                                nextDictionary[tail] += item.Value;
                            }
                        }
                    }
                }

                countDictionary = nextDictionary;

            }

            return result;
        }

        private Dictionary<string, decimal> part2Cache = [];
        private decimal part2Test(string toTest)
        {
            if (string.IsNullOrEmpty(toTest))
            {
                return 1;
            }

            decimal count = 0;

            for (int i = maxPartLength > toTest.Length ? toTest.Length : maxPartLength; i > 0; i--)
            {
                var subStr = toTest.Substring(0, i);

                var num = stringToNum(subStr);
                if (sortedParts.Contains(subStr))
                 //   if (sortedInts.Contains(num))
                {
                    var tail = toTest.Substring(subStr.Length, toTest.Length - subStr.Length);


                    if (part2Cache.TryGetValue(tail, out var value))
                    {
                        count += value;
                    }
                    else
                    {
                        var res = part2Test(tail);
                        part2Cache[tail] = res;
                        count += res;
                    }

                }
            }

            return count;

        }

        public decimal Part1()
        {
            var count = 0;

            foreach (var input in inputs)
            {
                if (part1Test(input))
                {
                    // Console.WriteLine($"{input} passed the test");
                    count++;
                }
            }

            return count;
        }

        public decimal Part2()
        {
            decimal count = 0;
            foreach (var input in inputs)
            {
                if (!part1Test(input))
                {
                    continue;
                }
                //  part2Cache.Clear();
                count += part2Test(input);

            }
            return count;
        }




    }


}
