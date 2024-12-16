using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using MathNet.Numerics.RootFinding;

namespace ConsoleApp
{
    internal class Day15 : IDay
    {

        enum Tile
        {
            Empty,
            Wall,
            Box,
            LeftBox,
            RightBox
        }

        private Tile[,] Map;
        private Tile[,] MapPart2;

        private int[] currentPositionPart1;
        private int[] currentPositionPart2;

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

        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var rows = File.ReadAllLines($"{dir}/{GetType().Name}.txt");

            var y = 0;

            Map = new Tile[rows[0].Length, rows[0].Length];
            MapPart2 = new Tile[rows[0].Length * 2, rows[0].Length];
            while (!string.IsNullOrEmpty(rows[y]))
            {
                for (var x = 0; x < rows[y].Length; x++)
                {
                    var ch = rows[y][x];

                    switch (ch)
                    {
                        case 'O':
                            Map[x, y] = Tile.Box;
                            MapPart2[x * 2, y] = Tile.LeftBox;
                            MapPart2[x * 2 + 1, y] = Tile.RightBox;
                            break;
                        case '.':
                            // empty is the default value
                            break;
                        case '@':
                            currentPositionPart1 = [x, y];
                            currentPositionPart2 = [x * 2, y];
                            break;
                        case '#':
                            Map[x, y] = Tile.Wall;
                            MapPart2[x * 2, y] = Tile.Wall;
                            MapPart2[x * 2 + 1, y] = Tile.Wall;
                            break;
                    }
                }

                y++;
            }

            y++;

            // <vv>^<v
            for (; y < rows.Length; y++)
            {
                for (int x = 0; x < rows[y].Length; x++)
                {
                    var ch = rows[y][x];

                    switch (ch)
                    {
                        case '<':
                            Moves.Add(Direction.West);
                            break;
                        case '>':
                            Moves.Add(Direction.East);
                            break;

                        case '^':
                            Moves.Add(Direction.North);
                            break;
                        case 'v':
                            Moves.Add(Direction.South);
                            break;
                    }
                }
            }
        }
        public decimal Part1()
        {
            foreach (var move in Moves)
            {
                if (CanWalkInDirectionPart1(currentPositionPart1, move))
                    MoveAndPushPart1(currentPositionPart1, move);
            }

            return CalculateGPS(Map);
        }

        public decimal Part2()
        {
            foreach (var move in Moves)
            {
                if (CanWalkInDirectionPart2(move))
                    MoveAndPushPart2(move);
            }

            return CalculateGPS(MapPart2);
        }

        private decimal CalculateGPS(Tile[,] map)
        {
            decimal score = 0;
            for (int y = 0; y < map.GetLength(1); y++)
            {

                for (int x = 0; x < map.GetLength(0); x++)
                {
                    if (map[x, y] == Tile.Box || map[x, y] == Tile.LeftBox)
                        score += 100 * y + x;
                }
            }
            return score;
        }

        // debugging friend
        private decimal print(Tile[,] map, int[] currPos, Direction? move = null)
        {
            Console.WriteLine();
            decimal score = 0;
            for (int y = 0; y < map.GetLength(1); y++)
            {

                for (int x = 0; x < map.GetLength(0); x++)
                {
                    if (map[x, y] == Tile.LeftBox)
                        Console.Write("[");
                    if (map[x, y] == Tile.RightBox)
                        Console.Write("]");
                    if (map[x, y] == Tile.Box)
                        Console.Write("O");
                    if (map[x, y] == Tile.Wall)
                        Console.Write("#");
                    if (x == currPos[X] && y == currPos[Y])
                    {
                        switch (move)
                        {
                            case Direction.West:
                                Console.Write('<');
                                break;
                            case Direction.East:
                                Console.Write('>');
                                break;

                            case Direction.North:
                                Console.Write('^');
                                break;
                            case Direction.South:
                                Console.Write('v');
                                break;
                            default:
                                Console.Write('@');
                                break;
                        }

                    }

                    else if (map[x, y] == Tile.Empty)
                        Console.Write(".");
                }
                Console.WriteLine();
            }
            return score;
        }

        #region part1 methods

        private bool CanWalkInDirectionPart1(int[] position, Direction direction)
        {
            // we can wak in an 
            var moves = 1;
            while (true)
            {
                var newX = position[X] + Directions[(int)direction][X] * moves;
                var newY = position[Y] + Directions[(int)direction][Y] * moves;
                if (Map[newX, newY] == Tile.Empty)
                    return true;

                if (Map[newX, newY] == Tile.Wall)
                    return false;
                moves++;
            }
        }
        private void MoveAndPushPart1(int[] position, Direction direction)
        {
            int moves = 1;

            var newX = position[X] + Directions[(int)direction][X] * moves;
            var newY = position[Y] + Directions[(int)direction][Y] * moves;

            currentPositionPart1[X] += Directions[(int)direction][X];
            currentPositionPart1[Y] += Directions[(int)direction][Y];

            // push
            if (Map[newX, newY] != Tile.Empty)
            {
                Map[newX, newY] = Tile.Empty;
                while (true)
                {
                    newX = position[X] + Directions[(int)direction][X] * moves;
                    newY = position[Y] + Directions[(int)direction][Y] * moves;
                    if (Map[newX, newY] == Tile.Empty)
                    {
                        Map[newX, newY] = Tile.Box;
                        return;
                    }

                    moves++;
                }
            }
        }
        #endregion
        #region part2 methods
        private bool CanWalkInDirectionPart2(Direction direction)
        {
            // fuck it, split by direction. 
            if (direction == Direction.East || direction == Direction.West)
            {
                return CanWalkInDirectionEastWest(direction);
            }

            return CanWalkInDirectionNorthSouth(direction);
        }

        // more or less a copy of part1: we can move east/west if we see an empty space before a wall
        private bool CanWalkInDirectionEastWest(Direction direction)
        {
            int moves = 1;

            while (true)
            {
                var newX = currentPositionPart2[X] + Directions[(int)direction][X] * moves;
                var newY = currentPositionPart2[Y] + Directions[(int)direction][Y] * moves;
                if (MapPart2[newX, newY] == Tile.Empty)
                    return true;

                if (MapPart2[newX, newY] == Tile.Wall)
                    return false;
                moves++;
            }
        }

        /*
         * When moving north/south we put pressure on positons on the next row.
         * This causes those pressure-points to in turn put pressure on their next rows..
         * This continues until all pressure points hits empty space, or a single wall.
         * If anything hits a wall, we cant move.
         */
        private bool CanWalkInDirectionNorthSouth(Direction direction)
        {

            var pressurePoints = new List<int> { currentPositionPart2[X] };
            var newY = currentPositionPart2[Y];
            while (true)
            {
                newY += Directions[(int)direction][Y];
                var newPressurePoints = new List<int>();

                foreach (var pressurePoint in pressurePoints)
                {
                    if (MapPart2[pressurePoint, newY] == Tile.Empty)
                        continue;

                    if (MapPart2[pressurePoint, newY] == Tile.Wall)
                        return false;


                    if (MapPart2[pressurePoint, newY] == Tile.LeftBox)
                    {
                        newPressurePoints.Add(pressurePoint);
                        newPressurePoints.Add(pressurePoint + 1);
                    }
                    if (MapPart2[pressurePoint, newY] == Tile.RightBox)
                    {
                        newPressurePoints.Add(pressurePoint);
                        newPressurePoints.Add(pressurePoint - 1);
                    }
                }

                if (!newPressurePoints.Any())
                    return true;

                pressurePoints = newPressurePoints;

            }
        }

        private void MoveAndPushPart2(Direction direction)
        {
            // fuck it, split by direction. 
            if (direction == Direction.East || direction == Direction.West)
            {
                MoveAndPushEastWest(currentPositionPart2, direction);
            }
            else

                MoveAndPushNorthSouth(currentPositionPart2, direction);
        }

        /*
         * Each change in y position we put pressure on new points.
         * Put these in the list of things we need to write to the next row,
         * and write what we have in the list to the current row.
         */
        private void MoveAndPushNorthSouth(int[] position, Direction direction)
        {
            var thingsToMoveToNextRow = new List<Tuple<int, Tile>>();
            
            currentPositionPart2[Y] += Directions[(int)direction][Y];
            var currX = currentPositionPart2[X];
            var currY = currentPositionPart2[Y];

            // special case for first iteration, we only want to move the dude which means an empty space
            thingsToMoveToNextRow.Add(new(currX, Tile.Empty));

            while (thingsToMoveToNextRow.Any())
            {
                var newPressurePoints = new List<Tuple<int, Tile>>();
                foreach (var toMove in thingsToMoveToNextRow)
                {
                    currX = toMove.Item1;
                    if (MapPart2[currX, currY] == Tile.LeftBox)
                    {
                        newPressurePoints.Add(new(currX, Tile.LeftBox));
                        newPressurePoints.Add(new(currX + 1, Tile.RightBox));
                        MapPart2[currX, currY] = Tile.Empty;
                        MapPart2[currX + 1, currY] = Tile.Empty;
                    }
                    else if (MapPart2[currX, currY] == Tile.RightBox)
                    {
                        newPressurePoints.Add(new(currX, Tile.RightBox));
                        newPressurePoints.Add(new(currX - 1, Tile.LeftBox));
                        MapPart2[currX, currY] = Tile.Empty;
                        MapPart2[currX - 1, currY] = Tile.Empty;
                    }

                    MapPart2[currX, currY] = toMove.Item2;
                }

                thingsToMoveToNextRow = newPressurePoints;
                currY += Directions[(int)direction][Y];
            }
        }

        // Pretty much like part1, but we need to replace left tile box with right and vice versa
        private void MoveAndPushEastWest(int[] position, Direction direction)
        {

            int moves = 1;
            var newX = position[X] + Directions[(int)direction][X];
            var newY = position[Y] + Directions[(int)direction][Y];

            currentPositionPart2[X] += Directions[(int)direction][X];
            currentPositionPart2[Y] += Directions[(int)direction][Y];

            // push
            if (MapPart2[newX, newY] != Tile.Empty)
            {
                MapPart2[newX, newY] = Tile.Empty;
                while (true)
                {
                    newX = position[X] + Directions[(int)direction][X] * moves;
                    newY = position[Y] + Directions[(int)direction][Y] * moves;
                    if (MapPart2[newX, newY] == Tile.Empty)
                    {
                        // we know what new box to put at the end based on the direction we are moving
                        MapPart2[newX, newY] = direction == Direction.East ? Tile.RightBox : Tile.LeftBox;
                        return;
                    }

                    // while in box-territory, replace each box with their couterpart
                    MapPart2[newX, newY] = MapPart2[newX, newY] == Tile.LeftBox ? Tile.RightBox : Tile.LeftBox;

                    moves++;
                }
            }
        }
        #endregion

    }


}
