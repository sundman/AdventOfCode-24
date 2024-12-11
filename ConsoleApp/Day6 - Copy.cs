using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace ConsoleApp
{
    internal class Day06Play 
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


        private class Portal
        {
            public Portal exitPoint;
            //  public IList<Portal> sources;

            public bool LeadsToExit;

            public bool LeadsToTempPortal;

            public int visitCheck;

            public Portal()
            {
                //       sources = new List<Portal>();
            }
        }

        private Portal[,,] portalMap;

        private List<Portal> portals;

        private HashSet<int> obstacles;

        private class Obstacle(int x, int y)
        {
            public readonly int x = x;
            public readonly int y = y;

            public override int GetHashCode()
            {
                return (x << 16) + y;
            }
        }

        private List<Tuple<Portal, Portal>> TempRemapConnectoinsThatHitsObstacle(WalkPath currPos, Portal[] tempPortals)
        {

            int x = currPos.X;
            int y = currPos.Y;

            var toReturn = new List<Tuple<Portal, Portal>>();

            // check south
            while (map[x, y] != Tile.Lava)
            {
                if (portalMap[x, y, 2] != null)
                {
                    var source = portalMap[x, y, 2];
                    toReturn.Add(new(source, source.exitPoint));
                    //   source.exitPoint?.sources.Remove(source);
                    source.exitPoint = tempPortals[3]; // portalMap[currPos.X, currPos.Y + 1, 3];
                }

                y++;
            }

            y = currPos.Y;
            // check north
            while (map[x, y] != Tile.Lava)
            {
                if (portalMap[x, y, 0] != null)
                {
                    var source = portalMap[x, y, 0];
                    toReturn.Add(new(source, source.exitPoint));
                    //    source.exitPoint?.sources.Remove(source);
                    source.exitPoint = tempPortals[1]; // portalMap[currPos.X, currPos.Y - 1, 1];
                }

                y--;
            }

            y = currPos.Y;

            // check east
            while (map[x, y] != Tile.Lava)
            {
                if (portalMap[x, y, 1] != null)
                {
                    var source = portalMap[x, y, 1];
                    toReturn.Add(new(source, source.exitPoint));
                    //    source.exitPoint?.sources.Remove(source);
                    source.exitPoint = tempPortals[2]; // portalMap[currPos.X + 1, currPos.Y, 2];
                }

                x++;
            }

            x = currPos.X;
            // check west
            while (map[x, y] != Tile.Lava)
            {
                if (portalMap[x, y, 3] != null)
                {
                    var source = portalMap[x, y, 3];
                    toReturn.Add(new(source, source.exitPoint));
                    //   source.exitPoint?.sources.Remove(source);
                    source.exitPoint = tempPortals[0]; // portalMap[currPos.X - 1, currPos.Y, 0];
                }

                x--;
            }

            return toReturn;
        }


        private void RemoveTempObstacle(WalkPath currPos, List<Tuple<Portal, Portal>> portalsToRestore)
        {
            foreach (var portal in portalsToRestore)
            {
                portal.Item1.exitPoint = portal.Item2;
                //   portal.Item2?.sources.Add(portal.Item1);
            }

        }

        private void InsertTempObstacleInPortalMap(WalkPath currPos, Portal[] tempPortals)
        {
            int x = currPos.X;
            int y = currPos.Y;


            // north->east
            // .#.
            // .^X
            var currX = x + 1;
            var currY = y + 1;
            while (map[currX, currY] != Tile.Lava)
            {
                if (portalMap[currX, currY, 0] != null)
                {

                    var source = tempPortals[3]; //portalMap[x, y + 1, 3];
                    var target = portalMap[currX, currY, 0];
                    source.exitPoint = target;
                    //  target.sources.Add(source);
                    break;
                }

                ++currX;
            }

            // east->south
            // .>#
            // .X.
            currX = x - 1;
            currY = y + 1;
            while (map[currX, currY] != Tile.Lava)
            {
                if (portalMap[currX, currY, 1] != null)
                {
                    var source = tempPortals[0]; //portalMap[x - 1, y, 0];
                    var target = portalMap[currX, currY, 1];
                    source.exitPoint = target;
                    //  target.sources.Add(source);
                    break;
                }

                ++currY;
            }

            // south->west
            // Xv.
            // .#.
            currX = x - 1;
            currY = y - 1;
            while (map[currX, currY] != Tile.Lava)
            {
                if (portalMap[currX, currY, 2] != null)
                {
                    var source = tempPortals[1]; //portalMap[x, y - 1, 1];
                    var target = portalMap[currX, currY, 2];
                    source.exitPoint = target;
                    //   target.sources.Add(source);
                    break;
                }

                --currX;
            }

            // west-north
            // .X.
            // #<.
            currX = x + 1;
            currY = y - 1;
            while (map[currX, currY] != Tile.Lava)
            {
                if (portalMap[currX, currY, 3] != null)
                {
                    var source = tempPortals[2]; //portalMap[x + 1, y, 2];
                    var target = portalMap[currX, currY, 3];
                    source.exitPoint = target;
                    //  target.sources.Add(source);
                    break;
                }

                --currY;
            }
        }


        private void BuildPortalMap()
        {
            portalMap = new Portal[size + 2, size + 2, 4];
            portals = new List<Portal>();

            // init all portal entries
            foreach (var obstacle in obstacles)
            {
                int x = obstacle >> 16;
                int y = obstacle & 0xFFFF;



                portalMap[x, y + 1, 3] = new Portal(); // moving north
                portalMap[x - 1, y, 0] = new Portal(); // moving  east
                portalMap[x, y - 1, 1] = new Portal(); // moving south
                portalMap[x + 1, y, 2] = new Portal(); // moving west

                portals.Add(portalMap[x, y + 1, 3]);
                portals.Add(portalMap[x - 1, y, 0]);
                portals.Add(portalMap[x, y - 1, 1]);
                portals.Add(portalMap[x + 1, y, 2]);
            }

            // calculate all portal exits
            foreach (var obstacle in obstacles)
            {
                int x = obstacle >> 16;
                int y = obstacle & 0xFFFF;

                // todo fix this later. too tired to figure out a loop for the indexes

                // north->east
                // .#.
                // .^X
                var currX = x;
                var currY = y + 1;
                while (map[currX, currY] != Tile.Lava)
                {
                    if (portalMap[currX, currY, 0] != null)
                    {
                        var source = portalMap[x, y + 1, 3];
                        var target = portalMap[currX, currY, 0];
                        source.exitPoint = target;
                        //   target.sources.Add(source);
                        break;
                    }

                    ++currX;
                }

                // east->south
                // .>#
                // .X.
                currX = x - 1;
                currY = y;
                while (map[currX, currY] != Tile.Lava)
                {
                    if (portalMap[currX, currY, 1] != null)
                    {
                        var source = portalMap[x - 1, y, 0];
                        var target = portalMap[currX, currY, 1];
                        source.exitPoint = target;
                        //   target.sources.Add(source);
                        break;
                    }

                    ++currY;
                }

                // south->west
                // Xv.
                // .#.
                currX = x;
                currY = y - 1;
                while (map[currX, currY] != Tile.Lava)
                {
                    if (portalMap[currX, currY, 2] != null)
                    {
                        var source = portalMap[x, y - 1, 1];
                        var target = portalMap[currX, currY, 2];
                        source.exitPoint = target;
                        //  target.sources.Add(source);
                        break;
                    }

                    --currX;
                }

                // west-north
                // .X.
                // #<.
                currX = x + 1;
                currY = y;
                while (map[currX, currY] != Tile.Lava)
                {
                    if (portalMap[currX, currY, 3] != null)
                    {
                        var source = portalMap[x + 1, y, 2];
                        var target = portalMap[currX, currY, 3];
                        source.exitPoint = target;
                        //    target.sources.Add(source);
                        break;
                    }

                    --currY;
                }
            }
            //   Console.WriteLine($"Portal Map done with {portals.Count} portals having an total of {portals.Sum(x => x.sources.Count)} connections.");

            //foreach (var portal in portals)
            //{
            //    if (portal.exitPoint == null)
            //    {
            //        MarkAllSourcesAsLeadingToExit(portal);
            //    }
            //}

            //    Console.WriteLine($"All exit points marked. Leading to exits: {portals.Count(x => x.LeadsToExit)}");
        }

        //private void MarkAllSourcesAsLeadingToExit(Portal portal)
        //{
        //    portal.LeadsToExit = true;
        //    foreach (var source in portal.sources)
        //    {
        //        MarkAllSourcesAsLeadingToExit(source);
        //    }
        //}

        private int size;

        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var data = File.ReadAllLines($"{dir}/{GetType().Name}.txt");
            size = data.Length;

            obstacles = new HashSet<int>();

            map = new Tile[size + 2, size + 2];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    var c = data[y][x];

                    switch (c)
                    {
                        case '#': map[x + 1, y + 1] = Tile.Obstacle; obstacles.Add((x + 1 << 16) + y + 1); break;
                        case '^': startX = x + 1; startY = y + 1; ; break;
                    }
                }
            }

            // lets dig a pit of lava around the map to make edge-checks easier (yes, the input is a square)
            for (int x = 0; x <= size; x++)
            {
                map[x, 0] = Tile.Lava;
                map[x, size + 1] = Tile.Lava;
                map[0, x] = Tile.Lava;
                map[size + 1, x] = Tile.Lava;
            }


        }

        enum Direction
        {
            East,
            South,
            West,
            North
        }

        readonly Tuple<int, int>[] directions =
        [
            new(1, 0), new(0, 1), new(-1, 0), new(0, -1)
        ];



        public decimal Part1()
        {
            return part1Walk().Count;
        }

        class WalkPath
        {
            public int X;
            public int Y;
            public int direction;
        }

        // returns the path and the direction you entered it for the first time. This will be important if an obstacle is placed there!
        private List<WalkPath> part1Walk()
        {
            var size = map.GetLength(0);


            var toReturn = new List<WalkPath>();
            var visited = new bool[size, size];
            var direction = 3; // 3 = UP

            var currX = startX;
            var currY = startY;

            while (map[currX, currY] != Tile.Lava)
            {
                if (!visited[currX, currY])
                {
                    visited[currX, currY] = true;
                    toReturn.Add(new WalkPath() { X = currX, Y = currY, direction = direction });
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

            BuildPortalMap();

            var tasks = new List<Task<bool>>();

            Portal[] tempObstacle = new Portal[4];
            for (int i = 0; i < 4; i++)
            {
                tempObstacle[i] = new Portal();
            }


            // skip the first position since we shouldnt place an object there and did not come from anywhere
            int x = 0;
            foreach (var currPos in part1Walk().Skip(1))
            {
                x++;
                if (x > 136)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        tempObstacle[i] = new Portal();
                    }
                    InsertTempObstacleInPortalMap(currPos, tempObstacle);
                    var toRestore = TempRemapConnectoinsThatHitsObstacle(currPos, tempObstacle);
                    //      Console.WriteLine($"{toRestore.Count} routes temporarily remapped to go through obstacle");
                    var portal = tempObstacle[currPos.direction];
                    var visitChecker = (currPos.X << 8) + (currPos.Y << 8) + currPos.direction;

                    while (portal != null && portal.visitCheck != visitChecker)
                    {
                        portal.visitCheck = visitChecker;
                        portal = portal.exitPoint;
                    }

                    var fromX = currPos.X - directions[currPos.direction].Item1;
                    var fromY = currPos.Y - directions[currPos.direction].Item2;

                    var old = IsLoop(map, fromX, fromY, currPos.direction, currPos.X, currPos.Y);

                    if (old != (portal != null))
                    {
                        Console.WriteLine($"Corrupt {x}: {fromX} {fromY}");
                    }

                    if (portal != null)
                        result++;

                    RemoveTempObstacle(currPos, toRestore);
                }

                //// take one step back and start from there
                //var fromX = currPos.X - directions[currPos.direction].Item1;
                //var fromY = currPos.Y - directions[currPos.direction].Item2;

                //tasks.Add(Task.Run(() => IsLoop(map, fromX, fromY, currPos.direction, currPos.X, currPos.Y)));


            }

            return result;
            //  return Task.WhenAll(tasks).Result.Count(x => x);
        }



        private bool IsLoop(Tile[,] map, int startX, int startY, int direction, int extraX, int extraY)
        {
            var currX = startX;
            var currY = startY;
            var size = map.GetLength(0);
            var visited = new int[size, size];

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
