using System.Diagnostics;
using System.Drawing;

namespace ConsoleApp
{
    internal class Day18 : IDay
    {

        public static List<int[]> Directions = [[1, 0], [0, 1], [-1, 0], [0, -1]];

        private const int X = 0;
        private const int Y = 1;

        private List<Point> Corrupted = [];

        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var rows = File.ReadAllLines($"{dir}/{GetType().Name}.txt");

            foreach (var row in rows)
            {
                var parts = row.Split(',').Select(int.Parse).ToList();
                Corrupted.Add(new Point(parts[X] + 1, parts[Y] + 1));
            }
        }



        private bool[,] corruptedMap;

        private void BuildMap(int mapsize, int numberOfCorruptedNodes)
        {
            corruptedMap = new bool[mapsize + 2, mapsize + 2];


            // lets dig a pit of lava around the map to make edge-checks easier (yes, the input is a square)
            for (int x = 0; x < mapsize + 1; x++)
            {

                corruptedMap[x, 0] = true;
                corruptedMap[x, mapsize + 1] = true;
                corruptedMap[0, x] = true;
                corruptedMap[mapsize + 1, x] = true;
            }

            for (int i = 0; i < numberOfCorruptedNodes; i++)
            {
                var point = Corrupted[i];

                corruptedMap[point.X, point.Y] = true;
            }

        }


        private int[,] costToVisit;
        private void FindCheapestPath(int start, int end, int maxSteps = -1)
        {
            costToVisit = new int[corruptedMap.GetLength(0), corruptedMap.GetLength(0)];

            var edgeNodes = new Queue<int>();


            edgeNodes.Enqueue(start);

            while (edgeNodes.Count != 0)
            {
                var edgeNode = edgeNodes.Dequeue();

                var x = edgeNode >> 8;
                var y = edgeNode & 0xFF;

                var numberOfMovesDone = costToVisit[x, y] + 1;

                if (maxSteps > 0 && numberOfMovesDone >= maxSteps)
                    break;

                foreach (var direction in Directions)
                {

                    var newX = x + direction[X];
                    var newY = y + direction[Y];

                    if (corruptedMap[newX, newY] || costToVisit[newX, newY] != 0)
                        continue;

                    costToVisit[newX, newY] = numberOfMovesDone;

                    var newNode = (newX << 8) + newY;

                    if (newNode == end)
                    {
                        return;
                    }
                    else
                    {
                        edgeNodes.Enqueue(newNode);
                    }
                }
            }
        }

        private void RecursiveFindCheapestPathBack(int current, int end, bool[,] nodesVisited)
        {
            var currX = current >> 8;
            var currY = current & 0xFF;

            nodesVisited[currX, currY] = true;
            if (current == end)
                return;

            var currentCost = costToVisit[currX, currY];
            for (var i = 0; i < 4; i++)
            {
                var newX = currX + Directions[i][X];
                var newY = currY + Directions[i][Y];

                if (!corruptedMap[newX, newY] && costToVisit[newX, newY] == currentCost - 1)
                {
                    RecursiveFindCheapestPathBack((newX << 8) + newY, end, nodesVisited);
                    return;
                }
            }
        }

        public decimal Part1()
        {
            var size = 71;

            BuildMap(size, 1024);

            FindCheapestPath((1 << 8) + 1, (size << 8) + size);

            return costToVisit[size, size];
        }

        public decimal Part2()
        {
            var size = 71;
            int currentIndex = 1024;
            while (true)
            {
                // find shortest path
                var shortest = new bool[corruptedMap.GetLength(0), corruptedMap.GetLength(0)];
                RecursiveFindCheapestPathBack((size << 8) + size, (1 << 8) + 1, shortest);

                // drop corrupt bytes until we break shortest path
                while (true)
                {
                    var newPoint = Corrupted[currentIndex++];

                    corruptedMap[newPoint.X, newPoint.Y] = true;

                    if (shortest[newPoint.X, newPoint.Y])
                    {
                        // try to mend the path by finding new route between the 2 points this disconnected
                        var mendPoints = new List<int>();
                        foreach (var direction in Directions)
                        {
                            var newX = newPoint.X + direction[X];
                            var newY = newPoint.Y + direction[Y];

                            if (shortest[newX, newY] && !corruptedMap[newX, newY])
                            {
                                mendPoints.Add((newX << 8) + newY);
                            }
                        }

                        // only try to mend if we have exactly 2 neighbours to the impact.
                        // otherwise we might find some old mendings and try to connect bits that are on same side
                        if (mendPoints.Count == 2)
                        {
                            FindCheapestPath(mendPoints[0], mendPoints[1], 35);

                            if (costToVisit[mendPoints[1] >> 8, mendPoints[1] & 0xff] == 0)
                                break;

                            // we do this to add the new route to the shortest list
                            RecursiveFindCheapestPathBack(mendPoints[1], mendPoints[0], shortest);

                        }
                    }
                }

                // recheck if we broke the map
                FindCheapestPath((1 << 8) + 1, (size << 8) + size);
                if (costToVisit[size, size] == 0)
                    break;
            }

            var breakingPoint = Corrupted[currentIndex - 1];

            return (breakingPoint.X - 1) * 1000000 + (breakingPoint.Y);
        }



        // debugging friend
        private void print(bool[,] corrupted, bool[,] visited)
        {
            Console.WriteLine();
            decimal score = 0;
            for (int y = 0; y < corrupted.GetLength(1); y++)
            {

                for (int x = 0; x < corrupted.GetLength(0); x++)
                {
                    if (corrupted[x, y])
                        Console.Write("#");
                    else if (visited[x, y])
                        Console.Write("O");
                    else
                        Console.Write(".");

                }
                Console.WriteLine();
            }
        }
    }


}
