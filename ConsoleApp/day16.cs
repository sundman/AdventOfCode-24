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


        public static List<int[]> Directions = [[1, 0], [-1, 0], [0, 1], [0, -1]];

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

        private HashSet<Point> walkable = [];

        private Point Start;
        private Point End;

        private int size;

        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var rows = File.ReadAllLines($"{dir}/{GetType().Name}.txt");
            size = rows.Length;
            for (int y = 0; y < rows.Length; y++)
            {
                for (int x = 0; x < rows[y].Length; x++)
                {
                    var ch = rows[y][x];

                    switch (ch)
                    {
                        case '.':
                            walkable.Add(new Point(x, y));
                            break;
                        case 'S':
                            walkable.Add(new Point(x, y));
                            Start = new Point(x, y);
                            break;
                        case 'E':
                            walkable.Add(new Point(x, y));
                            End = new Point(x, y);
                            break;
                    }
                }
            }
        }

        private class Node
        {
            public Point Point;
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



        private void BuildNodeTreeAndMap()
        {
            NodeMap = new Node[size, size];

            foreach (var point in walkable)
            {
                NodeMap[point.X, point.Y] = new Node() { Point = point };
            }


            foreach (var point in walkable)
            {
                for (int dir = 0; dir < 4; dir++)
                {
                    {
                        var newX = point.X + Directions[dir][X];
                        var newY = point.Y + Directions[dir][Y];
                        if (NodeMap[newX, newY] != null)
                        {
                            NodeMap[point.X, point.Y].Connections[dir] = NodeMap[newX, newY];
                        }
                    }
                }
            }

            NodeMap[Start.X, Start.Y].cheapestByDirection[0] = 0;
        }

        //private List<int> scoresThatLeadsToGoal = new List<int>();
        //private HashSet<Node> alreadyVisitedPoints = new HashSet<Node>();



        private int deadEndCounter = 0;
        //private void WalkTheWalk(Node currentPosition, int currentDirection, int currentScore)
        //{
        //    if (currentPosition == NodeMap[End.X, End.Y])
        //    {
        //        scoresThatLeadsToGoal.Add(currentScore);
        //        Console.WriteLine($"Found {scoresThatLeadsToGoal.Count} unique paths to end. {scoresThatLeadsToGoal.Min()} being the cheapest.");
        //    }

        //    alreadyVisitedPoints.Add(currentPosition);

        //    for (int i = 0; i < 4; i++)
        //    {
        //        if (currentPosition.Connections[i] != null && !alreadyVisitedPoints.Contains(currentPosition.Connections[i].To))
        //        {
        //            var newScore = currentPosition.Connections[i].Cost;
        //            if (i != currentDirection)
        //            {
        //                newScore += 1000;
        //            }

        //            WalkTheWalk(currentPosition.Connections[i].To, i, currentScore + newScore);
        //        }
        //    }

        //    alreadyVisitedPoints.Remove(currentPosition);
        //}


        private void FindCheapestPath()
        {

            var edgeNodes = new HashSet<Node> { NodeMap[Start.X, Start.Y] };
            var lastEdgeNodes = new HashSet<Node>();
            while (edgeNodes.Any())
            {
                var newEdgeNodes = new HashSet<Node>();

                foreach (var edgeNode in edgeNodes)
                {

                    for (int i = 0; i < 4; i++)
                    {
                        if (edgeNode.Connections[i] == null)
                            continue;

                        //if (lastEdgeNodes.Contains(edgeNode.Connections[i]))
                        //    continue;

                        var cost = edgeNode.CostToMoveTo(i);
                        if ((edgeNode.Connections[i].cheapestByDirection[i] == null || edgeNode.Connections[i].cheapestByDirection[i] > cost))
                        {


                            edgeNode.Connections[i].cheapestByDirection[i] = edgeNode.CostToMoveTo(i);
                            edgeNode.Connections[i].sourceForCheapest[i] = edgeNode;
                            newEdgeNodes.Add(edgeNode.Connections[i]);
                        }
                    }
                }

                lastEdgeNodes = edgeNodes;
                edgeNodes = newEdgeNodes;
            }

        }


        public decimal Part1()
        {
            BuildNodeTreeAndMap();
            FindCheapestPath();

            return (decimal)NodeMap[End.X, End.Y].cheapestByDirection.Min();
        }

        public decimal Part2()
        {
            RecursiveCountUniqueTilesOfCheapestPath(NodeMap[End.X, End.Y]);

            print(NodeMap);
            return uniqueNodesVisited.Count;

        }



        HashSet<Node> uniqueNodesVisited = new HashSet<Node>();
        private void RecursiveCountUniqueTilesOfCheapestPath(Node from)
        {
            uniqueNodesVisited.Add(from);
            //// we have already checked this path, so some looopery is going on
            //if (!uniqueNodesVisited.Add(from))
            //    return;
            //if (from.Point.X == 113 && from.Point.Y == 1)
            //{
            //    Console.Write('e');
            //}


            if (from == NodeMap[Start.X, Start.Y])
                return;

            var minCheapest = from.cheapestByDirection.Min();

            for (int i = 0; i < 4; i++)
            {
                if (from.cheapestByDirection[i] == minCheapest)
                    RecursiveCountUniqueTilesOfCheapestPath(from.sourceForCheapest[i]);
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
