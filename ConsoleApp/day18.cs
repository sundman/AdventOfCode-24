using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Xml.XPath;
using MathNet.Numerics.RootFinding;

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
                Corrupted.Add(new Point(parts[X], parts[Y]));
            }
        }


        private Dictionary<int, int> cheapestNumberOfStepsDict = new Dictionary<int, int>();

        private HashSet<int> corruptedHash = [];

        private void BuildMap(int mapsize, int numberOfCorruptedNodes)
        {

            // lets dig a pit of lava around the map to make edge-checks easier (yes, the input is a square)
            for (int x = 0; x < mapsize + 1; x++)
            {
                corruptedHash.Add((x << 16) + 0);
                corruptedHash.Add((x << 16) + mapsize + 1);
                corruptedHash.Add(x);
                corruptedHash.Add(((mapsize + 1) << 16) + x);

            }

            for (int i = 0; i < numberOfCorruptedNodes; i++)
            {
                var point = Corrupted[i];
                corruptedHash.Add(((point.X + 1) << 16) + point.Y + 1);
            }

        }



        private void FindCheapestPath(int start, int end, int maxSteps = -1)
        {
            var edgeNodes = new Queue<int>();

            var cheapestToReachGoal = -1;

            edgeNodes.Enqueue(start);

            cheapestNumberOfStepsDict[start] = 0;

            while (edgeNodes.Count != 0)
            {
                var edgeNode = edgeNodes.Dequeue();

                if (maxSteps > 0 && cheapestNumberOfStepsDict[edgeNode] >= maxSteps)
                    continue;

                var cheapestToReachEdge = cheapestNumberOfStepsDict[edgeNode];

                if (cheapestToReachGoal > 0 && cheapestToReachGoal <= cheapestToReachEdge)
                    continue;

                for (int i = 0; i < 4; i++)
                {

                    var newNode = edgeNode + (Directions[i][X] << 16) + Directions[i][Y];

                    if (corruptedHash.Contains(newNode))
                        continue;

                    var costToReachNewNode = cheapestToReachEdge + 1;

                    if (cheapestNumberOfStepsDict.TryGetValue(newNode, out var oldCheapest))
                    {
                        if (oldCheapest <= costToReachNewNode)
                            continue;
                    }

                    cheapestNumberOfStepsDict[newNode] = costToReachNewNode;

                    if (newNode == end)
                    {
                        cheapestToReachGoal = cheapestNumberOfStepsDict[newNode];
                    }
                    else
                    {
                        edgeNodes.Enqueue(newNode);
                    }
                }
            }
        }



        private void RecursiveFindCheapestPathBack(int current, int end, HashSet<int> nodesVisited, List<int> orderVisited)
        {
            nodesVisited.Add(current);
            orderVisited.Add(current);

            if (current == end)
                return;

            var currentCost = cheapestNumberOfStepsDict[current];
            for (var i = 0; i < 4; i++)
            {
                var newNode = current + (Directions[i][X] << 16) + Directions[i][Y];
                if (cheapestNumberOfStepsDict.ContainsKey(newNode) && cheapestNumberOfStepsDict[newNode] == currentCost - 1)
                {
                    RecursiveFindCheapestPathBack(newNode, end, nodesVisited, orderVisited);
                    return;
                }

            }
        }



        public decimal Part1()
        {
            var size = 71;

            BuildMap(size, 1024);

            FindCheapestPath((1 << 16) + 1, (size << 16) + size);

            //   print(size);



            return cheapestNumberOfStepsDict[(size << 16) + size];
        }

        public decimal Part2()
        {
            var size = 71;
            int currentIndex = 1024;
            while (true)
            {
                // find shortest path
                var shortest = new HashSet<int>();
                var orderVisited = new List<int>();
                RecursiveFindCheapestPathBack((size << 16) + size, (1 << 16) + 1, shortest, orderVisited);

                // drop corrupt bytes until we break shortest path
                while (true)
                {
                    var newPoint = Corrupted[currentIndex++];
                    var newHash = ((newPoint.X + 1) << 16) + newPoint.Y + 1;

                    corruptedHash.Add(newHash);

                    if (shortest.Contains(newHash))
                    {
                        if (!orderVisited.Contains(newHash))
                            break;

                        // lets try to mend it by trying to find a path from the neighbours of this break in the path
                        var index = orderVisited.IndexOf(newHash);
                        var start = orderVisited[index - 1];
                        var end = orderVisited[index + 1];

                        cheapestNumberOfStepsDict.Clear();
                        FindCheapestPath(start, end, 35);
                        if (!cheapestNumberOfStepsDict.ContainsKey(end))
                            break;

                        // we must add these new nodes to the step
                        var newShortest = new HashSet<int>();
                        RecursiveFindCheapestPathBack(start, end, newShortest, new List<int>());

                        foreach (var item in newShortest)
                            shortest.Add(item);
                    }
                }

                // recheck if we broke the map
                cheapestNumberOfStepsDict.Clear();
                FindCheapestPath((1 << 16) + 1, (size << 16) + size);

                // we cant reach goal any longer
                if (!cheapestNumberOfStepsDict.ContainsKey((size << 16) + size))
                    break;
            }

            var breakingPoint = Corrupted[currentIndex - 1];

            return breakingPoint.X * 1000000 + breakingPoint.Y;
        }



        private void print(int size)
        {
            Console.WriteLine();
            for (int y = 0; y < size + 2; y++)
            {
                for (int x = 0; x < size + 2; x++)
                {
                    var coord = (x << 16) + y;

                    if (corruptedHash.Contains(coord))
                    {
                        Console.Write('#');
                    }

                    else if (cheapestNumberOfStepsDict.ContainsKey(coord))
                    {
                        Console.Write('O');
                    }
                    else
                        Console.Write('.');
                }

                Console.WriteLine();
            }

        }

    }


}
