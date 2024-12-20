using System.Diagnostics;
using System.Drawing;
using System.Xml.Linq;

namespace ConsoleApp
{
    internal class Day20 : IDay
    {

        public static List<int[]> Directions = [[1, 0], [0, 1], [-1, 0], [0, -1]];

        private const int X = 0;
        private const int Y = 1;

        private Node Start;
        private Node End;

        private int size;

        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var rows = File.ReadAllLines($"{dir}/{GetType().Name}.txt");
            size = rows.Length;

            NodeMap = new Node[size, size];
            for (int y = 1; y < rows.Length - 1; y++)
            {
                for (int x = 1; x < rows[y].Length - 1; x++)
                {
                    var ch = rows[y][x];

                    switch (ch)
                    {
                        case '.':
                            NodeMap[x, y] = new Node(x, y, Type.Road);
                            Nodes.Add(NodeMap[x, y]);
                            break;
                        case 'S':
                            NodeMap[x, y] = new Node(x, y, Type.Road);
                            Start = NodeMap[x, y];
                            Nodes.Add(NodeMap[x, y]);
                            break;
                        case 'E':
                            NodeMap[x, y] = new Node(x, y, Type.Road);
                            End = NodeMap[x, y];
                            Nodes.Add(NodeMap[x, y]);
                            break;
                        case '#':
                            NodeMap[x, y] = new Node(x, y, Type.Wall);

                            Nodes.Add(NodeMap[x, y]);
                            break;
                    }
                }
            }
        }

        private List<Node> Nodes = [];

        private enum Type
        {
            Road,
            Wall
        }

        private class Node(int x, int y, Type type)
        {
            public Point Point = new(x, y);
            public Type Type = type;
            public int? distanceFromGoal;
            public List<Node> Roads = [];
            public List<(int steps, Node node)> AccessibleByCheat = [];


        }

        private Node[,] NodeMap;

        private void BuildNodeTree()
        {
            foreach (var node in Nodes.Where(x => x.Type == Type.Road))
            {
                for (int dir = 0; dir < 4; dir++)
                {
                    AddRoads(node, dir);
                }
            }

            //  Start.distanceFromGoal = 0;

        }

        private void AddRoads(Node node, int dir)
        {

            var newX = node.Point.X + Directions[dir][X];
            var newY = node.Point.Y + Directions[dir][Y];

            var newNode = NodeMap[newX, newY];

            if (newNode == null)
                return;

            if (newNode.Type == Type.Road)
            {
                node.Roads.Add(newNode);
            }

            else
            {
                WallStep(node, newNode, dir, 0);
            }

        }

        private void WallStep(Node original, Node node, int direction, int steps = 0)
        {
            // for (int dir = 0; dir < 4; dir++)
            {
                var newX = node.Point.X + Directions[direction][X];
                var newY = node.Point.Y + Directions[direction][Y];

                var newNode = NodeMap[newX, newY];
                if (newNode == null || newNode == original)
                    return;

                if (newNode.Type == Type.Road)
                {
                    original.AccessibleByCheat.Add((steps + 2, newNode));
                }

                //else if (dir == direction && steps == 0)
                //    WallStep(original, newNode, 1, 1);
            }


        }

        private void FindStepsFromGoal()
        {
            var edgeNodes = new Queue<Node>();
            End.distanceFromGoal = 0;
            edgeNodes.Enqueue(End);

            while (edgeNodes.Any())
            {
                var edgeNode = edgeNodes.Dequeue();
                foreach (var newNode in edgeNode.Roads.Where(x => x.distanceFromGoal == null))
                {
                    var cost = edgeNode.distanceFromGoal + 1;
                    newNode.distanceFromGoal = cost;
                    edgeNodes.Enqueue(newNode);
                }
            }
        }

        private int FindCheatsGainingAtLeast(int goalGain)
        {
            Dictionary<int, int> count = [];

            var toReturn = 0;
            foreach (var node in Nodes)
            {
                var distanceFromGoal = node.distanceFromGoal;

                foreach (var cheatNode in node.AccessibleByCheat)
                {
                    var minByCheat = cheatNode.node.distanceFromGoal + cheatNode.steps;


                    var win = distanceFromGoal.Value - minByCheat.Value;

                    if (win < 0)
                        continue;

                    if (!count.ContainsKey(win))
                        count[win] = 0;

                    count[win]++;

                    if (win >= goalGain)
                    {
                        toReturn++;
                    }

                    //var minByCheat = node.AccessibleByCheat
                    //     .Min((cheat) => cheat.node.distanceFromGoal + cheat.steps);

                    //if (minByCheat != null)
                    //{
                    //    if (distanceFromGoal - minByCheat >= gain)
                    //    {
                    //        toReturn++;

                    //        if (!count.ContainsKey(win))
                    //            count[win] = 0;

                    //        count[win]++;
                    //var bestNode = node.AccessibleByCheat
                    //    .First((cheat) => cheat.node.distanceFromGoal + cheat.steps == minByCheat);



                    //if (minByCheat < distanceFromGoal)
                    //{
                    //    Console.WriteLine($"Cheat gaining {distanceFromGoal - minByCheat} from {node.Point} to {bestNode.node.Point} ");
                    //}
                }
            }


            //foreach (var keyValuePair in count.OrderBy(x => x.Key))
            //{
            //    Console.WriteLine($"Cheats gaining {keyValuePair.Key}: {keyValuePair.Value}");
            //}

            return toReturn;
        }


        public decimal Part1()
        {
            BuildNodeTree();
            FindStepsFromGoal();

            print(NodeMap);
            return FindCheatsGainingAtLeast(100);

            return (decimal)Start.distanceFromGoal;
        }

        public decimal Part2()
        {

            //  print(NodeMap);
            return 0;

        }



        HashSet<Node> uniqueNodesVisited = new();
        //private void RecursiveCountUniqueTilesOfCheapestPath(Node from, Node last)
        //{
        //    uniqueNodesVisited.Add(from);

        //    if (from == Start)
        //        return;

        //    for (var i = 0; i < 4; i++)
        //    {
        //        if (from.Roads[i] == null)
        //            continue;

        //        if (from.Roads[i].distanceFromGoal == from.distanceFromGoal - 1 ||
        //            from.Roads[i].distanceFromGoal == last.distanceFromGoal - 2 || // stupid special rule, but it could be a T-junction
        //           from.Roads[i].distanceFromGoal == from.distanceFromGoal - 1001)
        //            RecursiveCountUniqueTilesOfCheapestPath(from.Roads[i], from);
        //    }
        //}



        // debugging friend
        private void print(Node[,] map)
        {
            Console.WriteLine();
            decimal score = 0;
            for (int y = 0; y < map.GetLength(1); y++)
            {

                for (int x = 0; x < map.GetLength(0); x++)
                {
                    if (map[x, y] == null || map[x, y].Type == Type.Wall)
                        Console.Write("#");
                    //else if (uniqueNodesVisited.Contains(map[x, y]))
                    //    Console.Write("O");
                    else
                        Console.Write(".");

                }
                Console.WriteLine();
            }
        }

    }


}
