using System.Diagnostics;

namespace ConsoleApp
{
    internal class Day02 : IDay
    {
        private List<int>[] list;
        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var data = File.ReadAllLines($"{dir}/{GetType().Name}.txt");

            list = new List<int>[data.Length];

            for (int row = 0; row < data.Length; row++)
            {
                var numbers = new List<int>();
                int currentNumber = 0;
                foreach (var ch in data[row])
                {
                    if (ch == ' ') {
                        numbers.Add(currentNumber);
                        currentNumber = 0;
                    }
                    else
                    {
                        currentNumber = currentNumber * 10 + (ch - '0');
                    }
                }

                numbers.Add(currentNumber);
                list[row] = numbers;

            }

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

        public bool isSafePart2(List<int> list)
        {
            if (isSafePart1(list))
            {
                return true;
            }

            for (var i = 0; i < list.Count; i++)
            {
                var copy = new List<int>(list);
                copy.RemoveAt(i);
                if (isSafePart1(copy))
                {
                    return true;
                }
            }

            return false;
        }



        public decimal Part1()
        {
            var result = 0;
            foreach (var row in list)
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


            foreach (var row in list)
            {
                if (isSafePart2(row))
                {
                    result++;
                }
            }
            return result;
        }
    }
}
