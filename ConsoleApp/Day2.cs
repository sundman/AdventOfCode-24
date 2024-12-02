using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class Day2
    {
        List<List<int>> handleInput()
        {
            var data = File.ReadAllLines("Input/Day2.txt");



            var list1 = new List<List<int>>();

            foreach (var line in data)
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                list1.Add(parts.Select(int.Parse).ToList());

            }

            return list1;
        }


        bool isSafePart1(List<int> list)
        {
            var direction = list[0] - list[1];

            for (int i = 1; i < list.Count; i++)
            {
                var a = list[i - 1];
                var b = list[i];
                if (Math.Abs(a - b) > 3 || a == b)
                {

                    return false;
                }

                if (direction * (a - b) < 0)
                {
                    return false;
                }
            }

            return true;
        }




        public decimal Part1()
        {
            var result = 0;
            var data = handleInput();
            foreach (var row in data)
            {
                if (isSafePart1(row))
                {
                    result++;
                }
            }

            return result;
        }

        public decimal Part2()
        {
            var result = 0;
            var data = handleInput();


            foreach (var row in data)
            {
                if (isSafePart1(row))
                {
                    result++;

                }
                else
                {

                    for (int i = 0; i < row.Count; i++)
                    {
                        var copy = new List<int>(row);
                        copy.RemoveAt(i);
                        if (isSafePart1(copy))
                        {
                            result++;
                            break;
                        }

                    }
                }
            }
            return result;
        }
    }
}
