using System.Diagnostics;

namespace ConsoleApp
{
    internal class Day07 : IDay
    {
        private Dictionary<long, long[]> sums;

        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var data = File.ReadAllLines($"{dir}/{GetType().Name}.txt");

            sums = new Dictionary<long, long[]>();
            foreach (var line in data)
            {
                var parts = line.Split(':');

                sums.Add(long.Parse(parts[0]), 
                    parts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).
                    Select(long.Parse).ToArray());
            }
        }

        private bool Reverse(long[] numbers, long currSum, int pos, bool allowConcat)
        {
            if (pos == 0)
                return currSum == numbers[0];

             // subtract if possible
            if (currSum > numbers[pos] &&
                Reverse(numbers, currSum - numbers[pos], pos - 1, allowConcat))
                return true;

            // division if no remainder
            if (currSum % numbers[pos] == 0 &&
                Reverse(numbers, currSum / numbers[pos], pos - 1, allowConcat))
                return true;

            if (allowConcat)
            {
                // split if remainder matches
                var divideBy = (long)Math.Pow(10, Math.Floor(Math.Log10(numbers[pos])) + 1);

                if (currSum % divideBy == numbers[pos] && 
                    Reverse(numbers, currSum / divideBy, pos - 1, allowConcat))
                    return true;
            }
            return false;

        }
        
        public decimal Part1()
        {
            decimal result = 0;
            foreach (var kvp in sums)
            {
                if (Reverse(kvp.Value, kvp.Key, kvp.Value.Length - 1, false))
                    result += kvp.Key;
            }
            return result;
        }

        public decimal Part2()
        {
            decimal result = 0;
            foreach (var kvp in sums)
            {
                if (Reverse(kvp.Value, kvp.Key, kvp.Value.Length - 1, true))
                      result += kvp.Key;
            }
            return result;

        }
    }
}
