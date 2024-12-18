using System.Diagnostics;

namespace ConsoleApp
{
    internal class Day2022_10 
    {
        private List<Tuple<Action<int>, int>> Operations = [];
        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var rows = File.ReadAllLines($"{dir}/{GetType().Name}.txt");

            foreach (var row in rows)
            {
                var parts = row.Split(' ');

                if (parts[0] == "noop")
                    Operations.Add(new(noop, 0));
                else
                    Operations.Add(new(addx, int.Parse(parts[1])));
            }
        }

        private List<int> xDuringTime = [];
        private int currentX = 1;

        public void noop(int input)
        {

            xDuringTime.Add(currentX);
        }

        public void addx(int input)
        {
            xDuringTime.Add(currentX);
            xDuringTime.Add(currentX);
            currentX += input;
        }

        public decimal Part1()
        {
            xDuringTime.Add(currentX);

            Operations.ForEach(x => x.Item1(x.Item2));

            xDuringTime.Add(currentX);

            decimal sum = 0;
            for (int i = 20; i < xDuringTime.Count; i += 40)
            {
                sum += xDuringTime[i] * i;
            }

            return sum;
        }

        public decimal Part2()
        {
            for (int i = 0; i < xDuringTime.Count - 1; i++)
            {
                if (i % 40 == 0)
                    Console.WriteLine();

                if (Math.Abs((i % 40) - xDuringTime[i + 1]) <= 1)
                    Console.Write('#');
                else
                    Console.Write('.');
            }

            return 0;
        }
    }
}
