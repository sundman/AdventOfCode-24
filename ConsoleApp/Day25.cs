using System.Diagnostics;

namespace ConsoleApp
{
    internal class Day25 : IDay
    {

        private List<int[]> Locks = [];
        private List<int[]> Keys = [];


        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var data = File.ReadAllLines($"{dir}/{GetType().Name}.txt");

            bool handleInputs = true;

            int rowNumber = 0;

            bool isKey = false;

            while (rowNumber < data.Length)
            {
                isKey = data[rowNumber][0] == '.';

                rowNumber++;
                var newArray = new int[5];
                while (rowNumber < data.Length &&!string.IsNullOrEmpty(data[rowNumber]) )
                {
                    for (var i = 0; i < data[rowNumber].Length; i++)
                    {
                        if (data[rowNumber][i] == '#')
                            newArray[i]++;
                    }

                    rowNumber++;
                }

                rowNumber++;
                if (isKey)
                    Keys.Add(newArray);
                else
                    Locks.Add(newArray);
            }
        }


        public decimal Part1()
        {
            long result = 0;

            foreach (var key in Keys)
            {
                foreach (var l in Locks)
                {
                    bool fit = true;
                    for (int i = 0; i < 5; i++)
                    {
                        if (key[i] + l[i] >= 7)
                        {
                            fit = false;
                            break;
                        }
                    }

                    if (fit)
                        result++;
                }
            }


            return result;
        }

        public decimal Part2()
        {
            return 0;

            return 0;
        }


    }
}
