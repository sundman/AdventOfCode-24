using System.Diagnostics;

namespace ConsoleApp
{
    internal class Day01 : IDay
    {
        private int[] list1;
        private int[] list2;

        public void ReadInput()
        {

            var dir = Debugger.IsAttached ? "Example" : "Input";
            var data = File.ReadAllLines($"{dir}/{GetType().Name}.txt");
            
            list1 = new int[data.Length];
            list2 = new int[data.Length];

            for (var row = 0; row < data.Length; row++)
            {
                var line = data[row];

                var pointer = 0;
                var number = 0;

                while (line[pointer] != ' ')
                {
                    number = number * 10 + (line[pointer] - '0');
                    ++pointer;
                }
                list1[row] = number;

                number = 0;
                while (line[pointer] == ' ')
                {
                    ++pointer;
                }

                while (pointer < line.Length)
                {
                    number = number * 10 + (line[pointer] - '0');
                    ++pointer;
                }
                list2[row] = number;
            }
        }

        public decimal Part1()
        {
            decimal totalDiff = 0;
            Array.Sort(list1);
            Array.Sort(list2);
            
            for (var i = 0; i < list1.Length; i++)
            {
                totalDiff += Math.Abs(list1[i] - list2[i]);
            }

            return totalDiff;
        }

        public decimal Part2()
        {
            decimal score = 0;

            var countEm = new Dictionary<int, int>();

            foreach (var num in list2)
            {
                countEm.TryAdd(num, 0);
                ++countEm[num];
            }

            foreach (var num in list1)
            {
                if (countEm.ContainsKey(num))
                    score += num * countEm[num];
            }

            return score;
        }
    }
}
