using System.Diagnostics;
using System.Net;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleApp
{
    internal class Day7
    {

        private Dictionary<decimal, List<decimal>> sums;

        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var data = File.ReadAllLines($"{dir}/{GetType().Name}.txt");

            sums = new Dictionary<decimal, List<decimal>>();
            foreach (var line in data)
            {
                var parts = line.Split(':');

                sums.Add(decimal.Parse(parts[0]), parts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList());
            }
        }

        private bool Part1Recursive(decimal[] numbers, decimal currSum, int pos, decimal goal)
        {
            if (numbers.Length == pos + 1)
                return currSum == goal;
            
            if (currSum > goal)
                return false;

            return
                Part1Recursive(numbers, currSum + numbers[pos + 1], pos + 1, goal) ||
                Part1Recursive(numbers, currSum * numbers[pos + 1], pos + 1, goal);

        }

        private bool Part2Recursive(double[] numbers, double currSum, int pos, double goal)
        {
            if (numbers.Length == pos + 1)
                return currSum == goal;

            if (currSum > goal)
                return false;

            return
                Part2Recursive(numbers, currSum + numbers[pos + 1], pos + 1, goal) ||
                Part2Recursive(numbers, currSum * numbers[pos + 1], pos + 1, goal) || 
                Part2Recursive(numbers, currSum * Math.Pow(10, Math.Floor(Math.Log10(numbers[pos + 1]))+1) + numbers[pos + 1], pos + 1, goal);

        }


        public decimal Part1()
        {
            decimal result = 0;
            foreach (var kvp in sums)
            {
                if (Part1Recursive(kvp.Value.ToArray(), kvp.Value[0], 0 ,kvp.Key))
                    result += kvp.Key;
            }

            return result;
        }

        public decimal Part2()
        {
            decimal result = 0;
            foreach (var kvp in sums)
            {
                if (Part2Recursive(kvp.Value.Select(x => (double)x).ToArray(), (double)kvp.Value[0], 0, (double)kvp.Key))
                    result += kvp.Key;

            }
            return result;

        }


    }
}
