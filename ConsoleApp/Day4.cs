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
    internal class Day4
    {
        private string[] data;
        public void ReadInput()
        {

            data = File.ReadAllLines("Input/Day4.txt");
        }

        public decimal Part1()
        {
            decimal result = 0;

            for (int y = 0; y < data.Length; y++)
            {
                for (int x = 0; x < data[y].Length; x++)
                {
                    result += xmasSearch(data, x, y);
                }
            }
            return result;
        }

        private decimal xmasSearch(string[] data, int x, int y)
        {
            var toFind = "XMAS";
            int toReturn = 0;

            if (data[y][x] != 'X')
                return 0;

            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    int foundChars = 1;
                    while (foundChars < toFind.Length)
                    {
                        if (x + dx * foundChars < 0
                            || x + dx * foundChars >= data[0].Length
                            || y + dy * foundChars < 0
                            || y + dy * foundChars >= data.Length)
                        {
                            break;
                        }

                        if (data[y + dy * foundChars][x + dx * foundChars] == toFind[foundChars])
                        {
                            foundChars++;

                        }
                        else
                        {
                            break;
                        }
                    }

                    if (foundChars == toFind.Length)
                    {
                        toReturn++;
                    }
                }
            }
            return toReturn;
        }

        public decimal Part2()
        {
            decimal result = 0;

            for (int y = 0; y < data.Length; y++)
            {
                for (int x = 0; x < data[y].Length; x++)
                {
                    result += part2Search(data, x, y);
                }
            }
            return result;



        }

        private decimal part2Search(string[] data, int x, int y)
        {
            if (data[y][x] != 'A')
                return 0;

            if (x < 1 || y < 1 || x >= data[0].Length - 1 || y >= data.Length - 1)
                return 0;

            var c1 = data[y + 1][x + 1];
            var c2 = data[y + 1][x - 1];
            var c3 = data[y - 1][x + 1];
            var c4 = data[y - 1][x - 1];

            var valid = new List<char>() { 'M', 'S' };

            if (valid.Contains(c1) && valid.Contains(c2) && valid.Contains(c3) && valid.Contains(c4))
            {
                return c1 != c4 && c2 != c3 ? 1 : 0;
            }


            return 0;
        }
    }
}
