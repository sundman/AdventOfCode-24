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
        private static int size;
        private string[] map;

        private List<long> nums;

        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            map = File.ReadAllLines($"{dir}/{GetType().Name}.txt");
            size = map.Length;

            regionMap = new Region[size + 2, size + 2];
            taken = new bool[size + 2, size + 2];
            // lets dig a pit of lava around the map to make edge-checks easier (yes, the input is a square)

            for (int x = 0; x < size + 2; x++)
            {
                taken[x, 0] = true;
                taken[x, size + 1] = true;
                taken[0, x] = true;
                taken[size + 1, x] = true;
            }
            findregions();
        }

        static List<int[]> Directions = [[1, 0], [-1, 0], [0, 1], [0, -1]];
        private const int X = 0;
        private const int Y = 1;

        private class Region(char name)
        {
            public char Name = name;
            private readonly HashSet<int> Coords = new();

            public void AddCoord(int[] p)
            {
                Coords.Add((p[X] << 8) + p[Y]);
            }


            public int Area => Coords.Count;

            private Dictionary<int[], HashSet<int>> perimeterByDirection = new();

            public int Perimeter
            {
                get
                {
                    foreach (var direction in Directions)
                    {
                        perimeterByDirection[direction] = new HashSet<int>();                    }

                    var toReturn = 0;
                    foreach (var coord in Coords)
                    {
                        var x = coord >> 8;
                        var y = coord & 0xFF;

                        foreach (var direction in Directions)
                        {
                            if (regionMap[x + direction[X] + 1, y + direction[Y] + 1] != this)
                            {
                                perimeterByDirection[direction].Add(coord);
                                toReturn++;
                            }
                        }
                    }

                    return toReturn;
                }
            }

            public int Sides
            {
                get
                {
                    return perimeterByDirection.Sum(kvp => kvp.Value.Count(coord =>
                          !kvp.Value.Contains(coord + 1) && !kvp.Value.Contains(coord + (1 << 8))));
                }

            }
        }

        static Region[,] regionMap;
        private IList<Region> regions = new List<Region>();
        private bool[,] taken;
        void findregions()
        {
            for (int y = 0; y < map.Length; y++)
            {
                for (int x = 0; x < map[0].Length; x++)
                {
                    if (taken[x + 1, y + 1])
                        continue;

                    var ch = map[y][x];

                    var region = new Region(ch);
                    regions.Add(region);
                    expandRegion(region, x, y);

                }
            }

        }

        private void expandRegion(Region region, int x, int y)
        {

            region.AddCoord([x, y]);
            regionMap[x + 1, y + 1] = region;
            taken[x + 1, y + 1] = true;
            foreach (var dir in Directions)
            {
                var newX = x + dir[X];
                var newY = y + dir[Y];
                if (!taken[newX + 1, newY + 1])
                {
                    if (map[newY][newX] == region.Name)
                    {
                        expandRegion(region, newX, newY);
                    }
                }
            }
        }

        public decimal Part1()
        {
            return regions.Sum(x => x.Area * x.Perimeter);
        }

        public decimal Part2()
        {
            return regions.Sum(x => x.Area * x.Sides);
        }

    }
}
