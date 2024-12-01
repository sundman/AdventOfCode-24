using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class Day1
    {
        (List<int>,List<int>) handleInput()
        {
            var data = File.ReadAllLines("Input/Day1.txt");

            decimal totalDiff = 0;
            decimal part2Score = 0;

            var list1 = new List<int>();
            var list2 = new List<int>();
            foreach (var line in data)
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                list1.Add(int.Parse(parts[0]));
                list2.Add(int.Parse(parts[1]));
            }
            return (list1, list2);
        }

         public decimal Part1()
        {
            var data = handleInput();
            decimal totalDiff = 0;
            data.Item1.Sort();
            data.Item2.Sort();

            for (int i = 0; i < data.Item1.Count; i++)
            {
                totalDiff += Math.Abs(data.Item1[i] - data.Item2[i]);

            }

            return totalDiff;
        }

       public decimal Part2()
        {
            decimal score = 0;
            var data = handleInput();


            for (int i = 0; i < data.Item1.Count; i++)
            {

                score += data.Item1[i] * data.Item2.Count(x => x == data.Item1[i]);
            }

            return score;
        }
    }
}
