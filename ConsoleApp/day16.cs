using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
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
            public Node[] Connections = new Node[4];
            public int?[] cheapestByDirection = new int?[4];
            public Node[] sourceForCheapest = new Node[4];

            public int CostToMoveTo(int direction)
            {
                var min = -1;


                int? costSameDirection = cheapestByDirection[direction] >= 0 ? cheapestByDirection[direction] : null;

                int? cheapestAnyOtherDirection = null;
                for (int i = 0; i < 4; i++)
                {
                    if (i == direction || cheapestByDirection[i] == null)
                        continue;


                    if (!cheapestAnyOtherDirection.HasValue)
                        cheapestAnyOtherDirection = cheapestByDirection[i];
                    else
                        cheapestAnyOtherDirection = cheapestAnyOtherDirection < cheapestByDirection[i] ? cheapestAnyOtherDirection : cheapestByDirection[i];

                }

                if (!costSameDirection.HasValue)
                    return cheapestAnyOtherDirection.Value + 1001;

                if (!cheapestAnyOtherDirection.HasValue)
                    return costSameDirection.Value + 1;

                return (costSameDirection.Value + 1) < (cheapestAnyOtherDirection.Value + 1001) ? (costSameDirection.Value + 1) : (cheapestAnyOtherDirection.Value + 1001);
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

            NodeMap[Start.Point.X, Start.Point.Y].cheapestByDirection[0] = 0;

        }


        private void FindCheapestPath()
        {
            var edgeNodes = new HashSet<Node> { Start };
            while (edgeNodes.Any())
            {
                var newEdgeNodes = new HashSet<Node>();

                foreach (var edgeNode in edgeNodes)
                {

                    for (int i = 0; i < 4; i++)
                    {
                        if (edgeNode.Connections[i] == null)
                            continue;

                        var cost = edgeNode.CostToMoveTo(i);
                        if ((edgeNode.Connections[i].cheapestByDirection[i] == null
                             || edgeNode.Connections[i].cheapestByDirection[i] > cost))
                        {
                            edgeNode.Connections[i].cheapestByDirection[i] = edgeNode.CostToMoveTo(i);
                            edgeNode.Connections[i].sourceForCheapest[i] = edgeNode;
                            newEdgeNodes.Add(edgeNode.Connections[i]);
                        }
                    }
                }

                edgeNodes = newEdgeNodes;
            }

        }


        public decimal Part1()
        {
            BuildNodeTree();
            FindCheapestPath();

            return (decimal)End.cheapestByDirection.Min();
        }

        public decimal Part2()
        {
            RecursiveCountUniqueTilesOfCheapestPath(End, -1);

            // print(NodeMap);
            return uniqueNodesVisited.Count;

        }



        HashSet<Node> uniqueNodesVisited = new HashSet<Node>();
        private void RecursiveCountUniqueTilesOfCheapestPath(Node from, int latestDir)
        {
            uniqueNodesVisited.Add(from);

            if (from == Start)
                return;

            var minCheapest = from.cheapestByDirection.Min();

            for (int i = 0; i < 4; i++)
            {
                if (from.cheapestByDirection[i] == minCheapest
                    || (i == latestDir && from.cheapestByDirection[i] == minCheapest + 1000))
                    RecursiveCountUniqueTilesOfCheapestPath(from.sourceForCheapest[i], i);
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
