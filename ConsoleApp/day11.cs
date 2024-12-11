using System.Diagnostics;
using System.Drawing;
using System.Xml.XPath;

namespace ConsoleApp
{


    internal class Day11 : IDay
    {

        private string[] data;


        private List<long> nums;


        public void ReadInput()
        {

            var dir = Debugger.IsAttached ? "Example" : "Input";
            data = File.ReadAllLines($"{dir}/{GetType().Name}.txt");

            nums = data[0].Split(' ').Select(long.Parse).ToList();

        }


        private Dictionary<long, long> Rules(Dictionary<long, long> input)
        {
            var toReturn = new Dictionary<long, long>();


            foreach (var kvp in input)
            {
                var i = kvp.Key;

                if (i == 0)
                {
                    toReturn.TryAdd(1, 0);
                    toReturn[1] += kvp.Value;
                    continue;
                }


                var len = Math.Floor(Math.Log10(i)) + 1;
                if (len % 2 == 0)
                {
                    var divideBy = (long)Math.Pow(10, len / 2);

                    toReturn.TryAdd(i / divideBy, 0);
                    toReturn.TryAdd(i % divideBy, 0);
                    toReturn[i / divideBy] += kvp.Value;
                    toReturn[(i % divideBy)] += kvp.Value;
                    continue;
                }

                var ii = i * 2024;
                toReturn.TryAdd(ii, 0);
                toReturn[ii] += kvp.Value;

            }

            return toReturn;
        }



        public decimal Part1()
        {
            // var list = nums.ToDictionary(); 
            var numberCounts = nums.ToDictionary(g => g, g => (long)1);
            for (int i = 0; i < 25; i++)
            {
                numberCounts = Rules(numberCounts);
            }

            return numberCounts.Sum(x => x.Value);
        }

        public decimal Part2()
        {
            var numberCounts = nums.ToDictionary(g => g, g => (long)1);
            for (int i = 0; i < 75; i++)
            {
                numberCounts = Rules(numberCounts);
            }

            return numberCounts.Sum(x => x.Value);
        }
    }
}
