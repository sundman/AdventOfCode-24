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

        public static List<int[]> Directions = [[1, 0], [-1, 0], [0, 1], [0, -1]];
        private const int X = 0;
        private const int Y = 1;

        private class Region(char name)
        {
            public char Name = name;
            public HashSet<int> Coords = new HashSet<int>();

            public void AddCoord(int[] p)
            {
                Coords.Add((p[0] << 8) + p[1]);
            }


            public int Area => Coords.Count;

            public int Perimeter
            {
                get
                {
                    var toReturn = 0;
                    foreach (var coord in Coords)
                    {
                        var x = coord >> 8;
                        var y = coord & 0xFF;

                        foreach (var direction in Directions)
                        {
                            if (regionMap[x + direction[X] + 1, y + direction[Y] + 1] != this)
                                toReturn++;
                        }
                    }

                    return toReturn;
                }
            }

            public int Sides
            {
                get
                {
                    var toReturn = 0;
                    var edgeLookedAt = new bool[size + 2, size + 2, 4];

                    var coords = Coords.Select(x => (int[])[x >> 8, x & 0xFF]).ToList();

                    foreach (var coord in coords.OrderBy(x => x[X]).ThenBy(x => x[Y]))
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            toReturn = FindEdgesWithoutNeighboursFoundInThisDirection(i, coord);
                        }
                    }

                    foreach (var coord in coords.OrderBy(x => x[Y]).ThenBy(x => x[X]))
                    {
                        for (int i = 2; i < 4; i++)
                        {
                            toReturn = FindEdgesWithoutNeighboursFoundInThisDirection(i, coord);
                        }
                    }

                    return toReturn;

                    int FindEdgesWithoutNeighboursFoundInThisDirection(int i, int[] coord)
                    {
                        var direction = Directions[i];
                        var newX = coord[X] + direction[X];
                        var newY = coord[Y] + direction[Y];
                        if (regionMap[newX + 1, newY + 1] != this)
                        {
                            edgeLookedAt[newX + 1, newY + 1, i] = true;

                            bool neigbhourOnPerimeter = false;
                            foreach (var dir in Directions.Where(newDir => newDir[X] != direction[X] && newDir[Y] != direction[Y]))
                            {
                                var x = newX + 1 + dir[X];
                                var y = newY + 1 + dir[Y];

                                if (edgeLookedAt[x, y, i])
                                {
                                    neigbhourOnPerimeter = true;
                                    break;
                                }
                            }
                            if (!neigbhourOnPerimeter)
                            {
                                toReturn++;
                            }
                        }

                        return toReturn;
                    }
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

        private void expandRegion(Region region, int startx, int starty)
        {

            region.AddCoord([startx, starty]);
            regionMap[startx + 1, starty + 1] = region;
            taken[startx + 1, starty + 1] = true;
            foreach (var dir in Directions)
            {
                var x = startx + dir[X];
                var y = starty + dir[Y];
                if (!taken[x + 1, y + 1])
                {
                    if (map[y][x] == region.Name)
                    {
                        expandRegion(region, x, y);
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
