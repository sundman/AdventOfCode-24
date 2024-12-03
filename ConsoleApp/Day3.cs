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
        public decimal Part1()
        {
            decimal result = 0;
            var data = File.ReadAllLines("Input/Day3.txt");
            
            var line = String.Join("", data);

            var matches = Regex.Matches(line, "mul\\(\\d{1,3},\\d{1,3}\\)");

            foreach (var match in matches.ToList())
            {
                var nums = Regex.Matches(match.Value, "\\d+");
                result += int.Parse(nums[0].Value) * int.Parse(nums[1].Value);

            }


            return result;
        }

        public decimal Part2()
        {
            decimal result = 0;
            var data = File.ReadAllLines("Input/Day3.txt");

            var line = String.Join("", data);

            var doSection = Regex.Matches(line, "(do\\(\\)|^).*?(don't\\(\\)|$)");

            foreach (var doit in doSection.ToList())
            {
                var matches = Regex.Matches(doit.Value, "mul\\(\\d{1,3},\\d{1,3}\\)");
                
                foreach (var match in matches.ToList())
                {
                    var nums = Regex.Matches(match.Value, "\\d+");
                    result += int.Parse(nums[0].Value) * int.Parse(nums[1].Value);
                }
            }

            return result;
        }
    }
}
