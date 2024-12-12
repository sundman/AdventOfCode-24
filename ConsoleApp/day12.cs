using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Xml.XPath;

namespace ConsoleApp
{


    internal class Day12 : IDay
    {
        private string[] map;

        private List<long> nums;

        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            map = File.ReadAllLines($"{dir}/{GetType().Name}.txt");
        }


        private class Region(char name)
        {
            public char Name = name;
            public List<(int, int)> Coords = new List<(int, int)>();

            public int Price => Area * Perimeter;
            public int Area => Coords.Count;

            public int Perimeter
            {
                get
                {
                    var toReturn = 0;
                    foreach (var coord in Coords)
                    {
                        foreach (var direction in Directions)
                        {
                            if (!Coords.Contains((coord.Item1 + direction.Item1, coord.Item2 + direction.Item2)))
                                toReturn++;

                        }
                    }

                    return toReturn;
                }
            }
        }


        private Region[,] regionMap;
        private IList<Region> regions = new List<Region>();
        private bool[,] taken;
        void findregions()
        {
            var size = map.Length;
            taken = new bool[size + 2, size + 2];
            // lets dig a pit of lava around the map to make edge-checks easier (yes, the input is a square)

            for (int x = 0; x < size + 2; x++)
            {
                taken[x, 0] = true;
                taken[x, size + 1] = true;
                taken[0, x] = true;
                taken[size + 1, x] = true;
            }


            regionMap = new Region[map[0].Length, map.Length];
            for (int y = 0; y < map.Length; y++)
            {
                for (int x = 0; x < map[0].Length; x++)
                {
                    if (taken[x + 1, y + 1])
                        continue;

                    var ch = map[y][x];

                    taken[x + 1, y + 1] = true;
                    var region = new Region(ch);
                    regions.Add(region);
                    region.Coords.Add((x, y));

                    expandRegion(region, x, y);

                }
            }

        }

        public static List<(int, int)> Directions = [(1, 0), (0, 1), (-1, 0), (0, -1)];

        private void expandRegion(Region region, int startx, int starty)
        {

            foreach (var dir in Directions)
            {
                var x = startx + dir.Item1;
                var y = starty + dir.Item2;
                if (!taken[x + 1, y + 1])
                {
                    if (map[y][x] == region.Name)
                    {
                        taken[x + 1, y + 1] = true;
                        region.Coords.Add((x, y));
                        expandRegion(region, x, y);
                    }
                }
            }
        }

        public decimal Part2()
        {
            return 0;
        }

        public decimal Part1()
        {
            findregions();


            return regions.Sum(x => x.Price);

        }
    }
}
