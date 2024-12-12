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

            taken = new bool[size + 2, size + 2];
            // lets dig a pit of lava around the map to make edge-checks easier (yes, the input is a square)

            for (int x = 0; x < size + 2; x++)
            {
                taken[x, 0] = true;
                taken[x, size + 1] = true;
                taken[0, x] = true;
                taken[size + 1, x] = true;
            }
        }

        public static List<(int, int)> Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

        private class Region(char name)
        {
            public char Name = name;
            public List<(int, int)> Coords = new List<(int, int)>();

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

            public int Sides
            {
                get
                {

                    var toReturn = 0;
                    var outSidePerimeter = new bool[size + 2, size + 2, 4];
                    foreach (var coord in Coords.OrderBy(x => x.Item1).ThenBy(x => x.Item2))
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            var direction = Directions[i];
                            var newX = coord.Item1 + direction.Item1;
                            var newY = coord.Item2 + direction.Item2;
                            if (!Coords.Contains((newX, newY)))
                            {
                                outSidePerimeter[newX + 1, newY + 1, i] = true;

                                bool neigbhourOnPerimeter = false;
                                foreach (var dir in Directions.Where(newDir => newDir.Item1 != direction.Item1 && newDir.Item2 != direction.Item2))
                                {

                                    var x = newX + 1 + dir.Item1;
                                    var y = newY + 1 + dir.Item2;


                                    if (outSidePerimeter[x, y, i])
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

                        }
                    }

                    foreach (var coord in Coords.OrderBy(x => x.Item2).ThenBy(x => x.Item1))
                    {
                        for (int i = 2; i < 4; i++)
                        {
                            var direction = Directions[i];
                            var newX = coord.Item1 + direction.Item1;
                            var newY = coord.Item2 + direction.Item2;
                            if (!Coords.Contains((newX, newY)))
                            {
                                outSidePerimeter[newX + 1, newY + 1, i] = true;

                                bool neigbhourOnPerimeter = false;
                                foreach (var dir in Directions.Where(newDir => newDir.Item1 != direction.Item1 && newDir.Item2 != direction.Item2))
                                {

                                    var x = newX + 1 + dir.Item1;
                                    var y = newY + 1 + dir.Item2;


                                    if (outSidePerimeter[x, y, i])
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

                        }
                    }

                    return toReturn;
                }
            }
        }


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

                    taken[x + 1, y + 1] = true;
                    var region = new Region(ch);
                    regions.Add(region);
                    region.Coords.Add((x, y));

                    expandRegion(region, x, y);

                }
            }

        }

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
            //foreach (var region in regions)
            //    Console.WriteLine($"Region {region.Name} has {region.Sides} sides");
            return regions.Sum(x => x.Area * x.Sides);
        }

        public decimal Part1()
        {
            findregions();


            return regions.Sum(x => x.Area * x.Perimeter);

        }
    }
}
