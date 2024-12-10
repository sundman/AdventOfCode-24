using System.Diagnostics;
using System.Drawing;

namespace ConsoleApp
{
    internal class Day08 : IDay
    {
        private Dictionary<char, List<Point>> antennas;
        private int mapSize;
        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var data = File.ReadAllLines($"{dir}/{GetType().Name}.txt");
            mapSize = data.Length;
            antennas = new Dictionary<char, List<Point>>();
            for (int y = 0; y < data.Length; y++)
            {
                for (int x = 0; x < data[y].Length; x++)
                {
                    var ch = data[y][x];

                    if (ch != '.')
                    {
                        if (!antennas.ContainsKey(ch))
                            antennas.Add(ch, new List<Point>());

                        antennas[ch].Add(new Point(x, y));
                    }
                }


            }
        }


        public decimal Part1()
        {
            decimal result = 0;

            var listOfCoords = new HashSet<int>();
            foreach (var kvp in antennas)
            {
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    for (int j = i + 1; j < kvp.Value.Count; j++)
                    {
                        var pointA = kvp.Value[i];
                        var pointB = kvp.Value[j];

                        var diffX = pointA.X - pointB.X;
                        var diffY = pointA.Y - pointB.Y;

                        var newXa = pointA.X + diffX;
                        var newYa = pointA.Y + diffY;

                        var newXb = pointB.X - diffX;
                        var newYb = pointB.Y - diffY;

                        if (newXa < mapSize && newXa >= 0 && newYa < mapSize && newYa >= 0)
                        {
                            var coords = (newXa << 16) + newYa;
                            listOfCoords.Add(coords);
                        }
                        if (newXb < mapSize && newXb >= 0 && newYb < mapSize && newYb >= 0)
                        {
                            var coords = (newXb << 16) + newYb;
                            listOfCoords.Add(coords);
                        }
                    }
                }


            }

            return listOfCoords.Count;
        }
        public decimal Part2Linear()
        {
            var listOfCoords = new HashSet<int>();


            foreach (var kvp in antennas)
            {
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    for (int j = i + 1; j < kvp.Value.Count; j++)
                    {

                        var pointA = kvp.Value[i];
                        var pointB = kvp.Value[j];

                        var a = (double)(pointB.Y - pointA.Y) / (pointB.X - pointA.X);
                        var k = pointA.Y - (a * pointA.X);

                        for (int x = 0; x < mapSize; x++)
                        {
                            double y = a * x + k;
                            
                            var roundedY = (int)Math.Round(y);
                            // Check if y is an integer by comparing it with its rounded value
                            if (Math.Abs(y - roundedY) < (double)1e-4m && roundedY >= 0 && roundedY < mapSize)
                            {
                                var coords = (x << 16) + roundedY;
                                

                                listOfCoords.Add(coords);
                            }
                        }
                    }
                }
            }
            
            return listOfCoords.Count;

        }

        public decimal Part2()
        {

          
            var listOfCoords = new HashSet<int>();
            foreach (var kvp in antennas)
            {
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    for (int j = i + 1; j < kvp.Value.Count; j++)
                    {
                        var pointA = kvp.Value[i];
                        var pointB = kvp.Value[j];

                        var diffX = pointA.X - pointB.X;
                        var diffY = pointA.Y - pointB.Y;

                        int mult = 0;
                        while (true)
                        {

                            var newX = pointA.X + diffX * mult;
                            var newY = pointA.Y + diffY * mult;

                            if (newX < mapSize && newX >= 0 && newY < mapSize && newY >= 0)
                            {
                                var coords = (newX << 16) + newY;
                                

                                listOfCoords.Add(coords);
                            }
                            else
                            {
                                break;
                            }

                            ++mult;
                        }

                        mult = 0;
                        while (true)
                        {

                            var newX = pointA.X - diffX * mult;
                            var newY = pointA.Y - diffY * mult;

                            if (newX < mapSize && newX >= 0 && newY < mapSize && newY >= 0)
                            {
                                var coords = (newX << 16) + newY;
                                listOfCoords.Add(coords);
                            }
                            else
                            {
                                break;
                            }

                            ++mult;
                        }
                    }
                }


            }


            return listOfCoords.Count;

        }
    }
}
