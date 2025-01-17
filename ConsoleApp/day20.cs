﻿using System.Collections.Generic;
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



                    if (win >= goalGain)
                    {
                        toReturn++;

                        count.TryAdd(win, 0);
                        count[win]++;
                    }
                }
            }


            return toReturn;
        }


        public decimal Part1()
        {
            BuildNodeTree();
            FindStepsFromGoal();

            return FindCheatsGainingAtLeast(100);

        }

        int DistanceBetweenNodes(Node a, Node b)
        {
            return Math.Abs(a.Point.X - b.Point.X) + Math.Abs(a.Point.Y - b.Point.Y);
        }


        public decimal Part2()
        {
            int maxCheat = 20;
            int countCheatsOfAtLeast = 100;
            decimal toReturn = 0;
            var roads = Nodes.Where(x => x.Type == Type.Road)
                .OrderBy(x => x.distanceFromGoal).ToArray();

            for (int i = 0; i < roads.Length - countCheatsOfAtLeast - 2; i++)
            {
                var start = roads[i];

                var startDistanceFromGoal = start.distanceFromGoal.Value;

                for (int j = i + countCheatsOfAtLeast + 2; j < roads.Length; j++)
                {
                    var target = roads[j];
                    var distance = DistanceBetweenNodes(start, target);
                    if (distance > maxCheat)
                        continue;

                    var gains = target.distanceFromGoal.Value - startDistanceFromGoal - distance;

                    if (gains >= countCheatsOfAtLeast)
                        toReturn++;

                }
            }


            return toReturn;

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
