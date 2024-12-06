using System.Diagnostics;

namespace ConsoleApp
{
    internal class Day6
    {
        enum Tile
        {
            Empty,
            Obstacle,
            Lava
        }

        private Tile[,] map;
        private int startX;
        private int startY;

        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var data = File.ReadAllLines($"{dir}/{GetType().Name}.txt");
            var size = data.Length;

            map = new Tile[size + 2, size + 2];
            int startX = 0, startY = 0;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    var c = data[y][x];

                    switch (c)
                    {
                        case '#': map[x + 1, y + 1] = Tile.Obstacle; break;
                        case '^': startX = x + 1; startY = y + 1; ; break;
                    }
                }
            }

            // lets dig a pit of lava around the map to make edge-checks easier (yes, the input is a square)
            for (int x = 0; x < size; x++)
            {
                map[x, 0] = Tile.Lava;
                map[x, size + 1] = Tile.Lava;
                map[0, x] = Tile.Lava;
                map[size + 1, x] = Tile.Lava;
            }

            this.map = map;
            this.startX = startX;
            this.startY = startY;
            
        }

        readonly Tuple<int, int>[] directions =
        [
            new(1, 0), new(0, 1), new(-1, 0), new(0, -1)
        ];



        public decimal Part1()
        {
            return part1Walk().Count;
        }


        private List<Tuple<int, int>> part1Walk()
        {
             var size = map.GetLength(0);


            var toReturn = new List<Tuple<int, int>>();
            var visited = new bool[size, size];
            var direction = 3; // 3 = UP

            var currX = startX;
            var currY = startY;

            while (map[currX, currY] != Tile.Lava)
            {
                if (!visited[currX, currY])
                {
                    visited[currX, currY] = true;
                    toReturn.Add(new(currX, currY));
                }

                var newX = currX + directions[direction].Item1;
                var newY = currY + directions[direction].Item2;

                if (map[newX, newY] == Tile.Obstacle)
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
            
            var tasks = new List<Task<bool>>();

            foreach (var currPos in part1Walk())
            {
                // this case doesn't seem to be needed, but the task says you are not allowed to place an obstruction here...
                if (currPos.Item1 == startX && currPos.Item2 == startY)
                    continue;

                tasks.Add(Task.Run(() => IsLoop(map, startX, startY, currPos.Item1, currPos.Item2)));
                
            }

            return Task.WhenAll(tasks).Result.Count(x => x);

        }


        private bool IsLoop(Tile[,] map, int startX, int startY, int extraX, int extraY)
        {
            var currX = startX;
            var currY = startY;
            var size = map.GetLength(0);
            var visited = new int[size, size];
            var direction = 3; // 3 is up

            while (map[currX, currY] != Tile.Lava)
            {
                var newX = currX + directions[direction].Item1;
                var newY = currY + directions[direction].Item2;

                if (map[newX, newY] == Tile.Obstacle || newX == extraX && newY == extraY)
                {
                    // have we been here facing this direction before? We only check this on turns to save on comparisons.
                    // It is faster even though we might walk a bit longer than necessary before realising we are looping.
                    if ((visited[currX, currY] & (1 << direction)) != 0)
                        return true;

                    // add current location and direction to visited
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
