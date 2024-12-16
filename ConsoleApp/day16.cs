using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using MathNet.Numerics.RootFinding;

namespace ConsoleApp
{
    internal class Day16 : IDay
    {


        public static List<int[]> Directions = [[1, 0], [0, 1], [-1, 0], [0, -1]];

        private const int X = 0;
        private const int Y = 1;

        enum Direction
        {
            East,
            West,
            South,
            North
        }

        private List<Direction> Moves = [];


        private Node Start;
        private Node End;

        private int size;

        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var rows = File.ReadAllLines($"{dir}/{GetType().Name}.txt");
            size = rows.Length;

            NodeMap = new Node[size, size];
            for (int y = 0; y < rows.Length; y++)
            {
                for (int x = 0; x < rows[y].Length; x++)
                {
                    var ch = rows[y][x];

                    switch (ch)
                    {
                        case '.':
                            NodeMap[x, y] = new Node(x, y);
                            Nodes.Add(NodeMap[x, y]);
                            break;
                        case 'S':
                            NodeMap[x, y] = new Node(x, y);
                            Start = NodeMap[x, y];
                            Nodes.Add(NodeMap[x, y]);
                            break;
                        case 'E':
                            NodeMap[x, y] = new Node(x, y);
                            End = NodeMap[x, y];
                            Nodes.Add(NodeMap[x, y]);
                            break;
                    }
                }
            }
        }

        private List<Node> Nodes = [];

        private class Node(int x, int y)
        {
            public Point Point = new(x, y);

            public int? cheapest;
            public int? cheapestDirection;
            public Node[] Connections = new Node[4];

            public int CostToMoveTo(int direction)
            {
                if (direction == cheapestDirection)
                    return cheapest.Value + 1;
                return cheapest.Value + 1001;
            }
        }

        private class Connection
        {
            public Node To;
            public int Cost;
        }

        private Node[,] NodeMap;

        private void BuildNodeTree()
        {

            foreach (var node in Nodes)
            {
                for (int dir = 0; dir < 4; dir++)
                {
                    {
                        var newX = node.Point.X + Directions[dir][X];
                        var newY = node.Point.Y + Directions[dir][Y];
                        if (NodeMap[newX, newY] != null)
                        {
                            NodeMap[node.Point.X, node.Point.Y].Connections[dir] = NodeMap[newX, newY];
                        }
                    }
                }
            }

            Start.cheapest = 0;
            Start.cheapestDirection = 0;

        }



        private void FindCheapestPath()
        {
            var edgeNodes = new List<Node> { Start };
            while (edgeNodes.Any())
            {

                var edgeNode = edgeNodes.OrderBy(x => x.cheapest).First();

                edgeNodes.Remove(edgeNode);



                for (int i = 0; i < 4; i++)
                {
                    if (edgeNode.Connections[i] == null)
                        continue;

                    var cost = edgeNode.CostToMoveTo(i);
                    if (edgeNode.Connections[i].cheapest == null || edgeNode.Connections[i].cheapest > cost)
                    {
                        edgeNode.Connections[i].cheapest = cost;
                        edgeNode.Connections[i].cheapestDirection = i;
                        edgeNodes.Add(edgeNode.Connections[i]);
                    }
                }

            }

        }




        public decimal Part1()
        {
            BuildNodeTree();
            FindCheapestPath();

            return (decimal)End.cheapest;
        }

        public decimal Part2()
        {
            RecursiveCountUniqueTilesOfCheapestPath(End, End);

            //  print(NodeMap);
            return uniqueNodesVisited.Count;

        }



        HashSet<Node> uniqueNodesVisited = new();
        private void RecursiveCountUniqueTilesOfCheapestPath(Node from, Node last)
        {
            uniqueNodesVisited.Add(from);

            if (from == Start)
                return;


            for (var i = 0; i < 4; i++)
            {
                if (from.Connections[i] == null)
                    continue;

                if (from.Connections[i].cheapest == from.cheapest - 1 ||
                    from.Connections[i].cheapest == last.cheapest - 2 || // stupid special rule, but it could be a T-junction
                   from.Connections[i].cheapest == from.cheapest - 1001)
                    RecursiveCountUniqueTilesOfCheapestPath(from.Connections[i], from);

            }
        }



        // debugging friend
        private void print(Node[,] map)
        {
            Console.WriteLine();
            decimal score = 0;
            for (int y = 0; y < map.GetLength(1); y++)
            {

                for (int x = 0; x < map.GetLength(0); x++)
                {
                    if (map[x, y] == null)
                        Console.Write("#");
                    else if (uniqueNodesVisited.Contains(map[x, y]))
                        Console.Write("O");
                    else
                        Console.Write(".");

                }
                Console.WriteLine();
            }
        }

    }


}
