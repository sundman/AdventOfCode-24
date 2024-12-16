using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
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
            public Node[] Connections = new Node[4];
        }

        private Node[,] NodeMap;


        private void BuildNodeTreeAndMap()
        {
            NodeMap = new Node[size, size];

            foreach (var point in walkable)
            {
                NodeMap[point.X, point.Y] = new Node();
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
        }

        private void WalkTheWalk(Point currentPosition, HashSet<Point> alreadyVisitedPoints, int currentDirection,
            int currentScore)
        {

        }

        public decimal Part1()
        {
            return 0;
        }

        public decimal Part2()
        {
            return 0;
        }


    }


}
