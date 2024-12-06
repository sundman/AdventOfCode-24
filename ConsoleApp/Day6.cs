using System.Diagnostics;

namespace ConsoleApp
{
    public class Point(int x, int y)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;
        public static Point operator +(Point p1, Point p2) { return new Point(p1.X + p2.X, p1.Y + p2.Y); }

        public override bool Equals(object? obj)
        {
            var toCheck = obj as Point;

            return toCheck != null && Equals(toCheck);
        }

        protected bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }

    internal class Day6
    {


        private (Point, bool[,]) readInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var data = File.ReadAllLines($"{dir}/{GetType().Name}.txt");
            var size = data.Length;

            var obstacles = new bool[size, size];
            Point? startPosition = null;

            for (int y = 0; y < size; y++)
            {

                for (int x = 0; x < size; x++)
                {
                    if (data[y][x] == '#')
                        obstacles[x, y] = true;

                    if (data[y][x] == '^')
                        startPosition = new Point(x, y);
                }
            }

            return (startPosition, obstacles);
        }

        public decimal Part1()
        {
            return part1Walk().Count;
        }

        private List<Point> part1Walk()
        {
            var (startPosition, obstacles) = readInput();
            var size = obstacles.GetLength(0);
            var dirs = new List<Point>
            {
                new(1, 0), new(0, 1), new(-1, 0), new(0, -1)
            };

            var currentPosition = startPosition;
            var toReturn = new List<Point>();
            var visited = new bool[size, size];
            var direction = 3;

            while (true)
            {
                if (!visited[currentPosition.X, currentPosition.Y])
                {
                    visited[currentPosition.X, currentPosition.Y] = true;
                    toReturn.Add(currentPosition);
                }



                var newPos = currentPosition + dirs[direction];

                if (!(newPos.X >= 0 && newPos.X < size && newPos.Y >= 0 && newPos.Y < size))
                    break;

                if (obstacles[newPos.X, newPos.Y])
                {
                    direction = (direction + 1) % 4;
                    continue;
                }

                currentPosition = newPos;


            }

            return toReturn;

        }

        public decimal Part2()
        {
            decimal result = 0;

            var (startPosition, obstacles) = readInput();
            var size = obstacles.GetLength(0);

            foreach (var currPos in part1Walk())
            {
                if (currPos.Equals(startPosition))
                    continue;

                obstacles[currPos.X, currPos.Y] = true;
                if (IsLoop(obstacles, startPosition, size))
                    result += 1;

                obstacles[currPos.X, currPos.Y] = false;
            }

            return result;
        }

        private bool IsLoop(bool[,] obstacles, Point startPosition, int size)
        {
            var dirs = new List<Point>
            {
                new(1, 0), new(0, 1), new(-1, 0), new(0, -1)
            };

            var currentPosition = startPosition;
            var visited = new int[size, size];
            var direction = 3;

            while (true)
            {
                var newPos = currentPosition + dirs[direction];

                if (!(newPos.X >= 0 && newPos.X < size && newPos.Y >= 0 && newPos.Y < size))
                    break;

                if (obstacles[newPos.X, newPos.Y])
                {
                    // have we been here facing this direction before?
                    if ((visited[currentPosition.X, currentPosition.Y] & (1 << direction)) != 0)
                        return true;

                    visited[currentPosition.X, currentPosition.Y] |= (1 << direction);


                    direction = (direction + 1) % 4;
                    continue;
                }

                currentPosition = newPos;
            }

            return false;
        }



    }
}
