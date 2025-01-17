﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ConsoleApp
{
    internal class Day14 : IDay
    {

        private class Robot(int[] start, int[] vector)
        {
            public int[] Position = start;
            public int[] Vector = vector;

            public Robot Clone()
            {
                return new Robot([Position[X], Position[Y]], [Vector[X], Vector[Y]]);
            }
        }

        private List<Robot> Robots;


        private int sizeX = Debugger.IsAttached ? 11 : 101;
        private int sizeY = Debugger.IsAttached ? 7 : 103;

        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var rows = File.ReadAllLines($"{dir}/{GetType().Name}.txt");

            Robots = [];
            foreach (var row in rows)
            {

                // p=0,4 v=3,-3

                var parts = row.Split(new[] { "p=", "v=" }, StringSplitOptions.RemoveEmptyEntries);

                var pos = parts[0].Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                var v = parts[1].Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

                Robots.Add(new Robot(pos, v));
            }


        }

        private const int X = 0;
        const int Y = 1;
        private bool doubleSpot;

        private HashSet<int> hasNeighbour = new HashSet<int>();
        public static List<int[]> Directions = [[1, 0], [-1, 0], [0, 1], [0, -1]];


        private HashSet<int> spotsTaken = new HashSet<int>();
        private void MoveRobot(Robot robot, int steps)
        {

            robot.Position[X] = (robot.Position[X] + robot.Vector[X] * steps) % sizeX;
            robot.Position[Y] = (robot.Position[Y] + robot.Vector[Y] * steps) % sizeY;

            if (robot.Position[X] < 0)
                robot.Position[X] += sizeX;
            if (robot.Position[Y] < 0)
                robot.Position[Y] += sizeY;

            var xyBit = (robot.Position[X] << 16) + robot.Position[Y];

            //foreach (var dir in Directions)
            //{
            //    var newBit = xyBit + dir[Y] + (dir[X] << 16);
            //    if (spotsTaken.Contains(newBit))
            //    {
            //        hasNeighbour.Add(xyBit);
            //        hasNeighbour.Add(newBit);
            //    }
            //}

            if (!doubleSpot)
            {
                if (!spotsTaken.Add(xyBit))
                    doubleSpot = true;
            }

        }

        public decimal Part1()
        {

            var robots = Robots.Select(x => x.Clone());

            foreach (var robot in robots)
            {
                MoveRobot(robot, 100);
            }

            var middleX = sizeX / 2;
            var middleY = sizeY / 2;

            var se = robots.Count(robot => robot.Position[X] > middleX && robot.Position[Y] > middleY);
            var sw = robots.Count(robot => robot.Position[X] > middleX && robot.Position[Y] < middleY);
            var ne = robots.Count(robot => robot.Position[X] < middleX && robot.Position[Y] > middleY);
            var nw = robots.Count(robot => robot.Position[X] < middleX && robot.Position[Y] < middleY);

            return se * sw * ne * nw;

        }

      

        public decimal Part2()
        {
            //  return 0;
            decimal counter = 0;
           // decimal maxNeighbour = 0;

            while (true)
            {
                doubleSpot = false;
              //  hasNeighbour.Clear();
                spotsTaken.Clear();
                Robots.ForEach(x => MoveRobot(x, 1));
                counter++;
               
                if (!doubleSpot)
                {
                    return counter;
                }
            }
        }
    }


}
