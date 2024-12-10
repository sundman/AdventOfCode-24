using System.Diagnostics;
using System.Text.RegularExpressions;
using String = System.String;

namespace ConsoleApp
{
    internal class Day03 : IDay
    {
        private string line;
        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var data = File.ReadAllLines($"{dir}/{GetType().Name}.txt");
            line = String.Join("", data);
        }

        private static int ParseToInt(string str)
        {
            int number = 0;
            foreach (var ch in str)
            {
                number = number * 10 + ch - '0';
            }

            return number;
        }

        public decimal Part1()
        {
            decimal result = 0;

            var matches = Regex.Matches(line, "mul\\(\\d{1,3},\\d{1,3}\\)");

            foreach (var match in matches.ToList())
            {
                var nums = Regex.Matches(match.Value, "\\d+");
                result += ParseToInt(nums[0].Value) * ParseToInt(nums[1].Value);
            }

            return result;
        }

        public decimal Part2()
        {
            decimal result = 0;

            var doSection = Regex.Matches(line, "(do\\(\\)|^).*?(don't\\(\\)|$)");

            foreach (var doit in doSection.ToList())
            {
                var matches = Regex.Matches(doit.Value, "mul\\(\\d{1,3},\\d{1,3}\\)");

                foreach (var match in matches.ToList())
                {
                    var nums = Regex.Matches(match.Value, "\\d+");
                    result += ParseToInt(nums[0].Value) * ParseToInt(nums[1].Value);
                }
            }

            return result;
        }
    }
}
