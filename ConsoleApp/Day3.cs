using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using String = System.String;

namespace ConsoleApp
{
    internal class Day3
    {
        private string line;
        public void ReadInput()
        {
            var data = File.ReadAllLines("Input/Day3.txt");
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
