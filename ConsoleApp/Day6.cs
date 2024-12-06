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

            var toReturn = new List<Point>();
            var visited = new bool[size, size];
            var direction = 3;

            var currX = startPosition.X;
            var currY = startPosition.Y;

            while (true)
            {



                if (!visited[currX, currY])
                {
                    visited[currX, currY] = true;
                    toReturn.Add(new Point(currX, currY));
                }

                var newX = currX + dirs[direction].X;
                var newY = currY + dirs[direction].Y;

                if (!(newX >= 0 && newX < size && newY >= 0 && newY < size))
                    break;

                if (obstacles[newX, newY])
                {
                    // turn
                    direction = (direction + 1) % 4;
                    continue;
                }

                // walk
                currX = newX;
                currY = newY;


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
                if (IsLoop(obstacles, startPosition.X, startPosition.Y, size))
                    result += 1;

                obstacles[currPos.X, currPos.Y] = false;
            }

            return result;
        }

        private bool IsLoop(bool[,] obstacles, int startX, int startY, int size)
        {
            var dirs = new List<Point>
            {
                new(1, 0), new(0, 1), new(-1, 0), new(0, -1)
            };

            var currX = startX;
            var currY = startY;
            var visited = new int[size, size];
            var direction = 3;

            while (true)
            {
                 var newX = currX + dirs[direction].X;
                 var newY = currY + dirs[direction].Y;

                if (!(newX >= 0 && newX < size && newY >= 0 && newY < size))
                    break;

                if (obstacles[newX, newY])
                {
                    // have we been here facing this direction before?
                    if ((visited[currX, currY] & (1 << direction)) != 0)
                        return true;

                    visited[currX, currY] |= (1 << direction);

                    // turn
                    direction = (direction + 1) % 4;
                    continue;
                }

                // walk
                currX = newX;
                currY = newY;
            }

            return false;
        }



    }
}
