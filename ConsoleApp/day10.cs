using System.Diagnostics;
using System.Drawing;
using System.Xml.XPath;

namespace ConsoleApp
{


    internal class Day10 : IDay
    {

        private string[] data;

        private int[,] map;

        private List<Point> startingLocations;

        private int size;

        public void ReadInput()
        {

            var dir = Debugger.IsAttached ? "Example" : "Input";
            data = File.ReadAllLines($"{dir}/{GetType().Name}.txt");
            size = data.Length;

            map = new int[size + 2, size + 2];
            startingLocations = [];

            for (int y = 0; y < data.Length; y++)
            {
                for (int x = 0; x < data[y].Length; x++)
                {
                    var num = data[y][x] - '0';
                    map[x + 1, y + 1] = num;

                    if (num == 0)
                        startingLocations.Add(new Point(x + 1, y + 1));
                }
            }

            // lets dig a pit of lava around the map to make edge-checks easier (yes, the input is a square)
            for (int x = 0; x < size; x++)
            {
                map[x, 0] = -1;
                map[x, size + 1] = -1;
                map[0, x] = -1;
                map[size + 1, x] = -1;
            }
        }
     

        readonly Point[] directions =
        [
            new(1, 0), new(0, 1), new(-1, 0), new(0, -1)
        ];

        private int uniqueTopsReached;
        private int topsReached;
        private bool[,] topsFoundMap;

        private void WalkToAllTops(int currX, int currY)
        {
            if (map[currX, currY] == 9)
            {
                if (!topsFoundMap[currX, currY])
                {
                    topsFoundMap[currX, currY] = true;
                    uniqueTopsReached++;
                }

                topsReached++;
                return;
            }

            foreach (var direction in directions)
            {
                if (map[currX + direction.X, currY + direction.Y] == map[currX, currY] + 1)
                {
                    WalkToAllTops(currX + direction.X, currY + direction.Y);
                }
            }
        }

        public decimal Part1()
        {
            foreach (var location in startingLocations)
            {
                // reset tops found map, we want to count unique tops by starting location
                topsFoundMap = new bool[size+2, size+2];
                WalkToAllTops(location.X, location.Y);
            }

            return uniqueTopsReached;
        } 

        public decimal Part2()
        {
            // Perhaps cheating... but doing part1 already produced the answer...
            return topsReached;
        }
    }
}
