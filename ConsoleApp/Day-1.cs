using System.Diagnostics;

namespace ConsoleApp
{
    internal class Day_1 //: IDay
    {
        private List<long> list;

        public void ReadInput()
        {

            var dir = Debugger.IsAttached ? "Example" : "Input";
            var data = File.ReadAllLines($"{dir}/{GetType().Name}.txt");

            list = data[0].Split(',').Select(long.Parse).ToList();
        }

        private long[] numCount = new long[9];


        void proceed()
        {
            var newNums = new long[9];

            newNums[6] = numCount[0];
            newNums[8] = numCount[0];

            for (int i = 1; i < 9; i++)
            {
                newNums[i - 1] += numCount[i];
            }

            numCount = newNums;
        }

        public decimal Part1()
        {
            for (int i = 1; i < 9; i++)
            {
                numCount[i] = list.Count(x => x == i);
            }
            
            for(int i = 0; i < 80; i++)
                proceed();

            return numCount.Sum();
        }

        public decimal Part2()
        {
            Array.Clear(numCount);
            for (int i = 1; i < 9; i++)
            {
                numCount[i] = list.Count(x => x == i);
            }

            for (int i = 0; i < 256; i++)
                proceed();

            return numCount.Sum();

        }
    }
}
