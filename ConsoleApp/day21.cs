using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace ConsoleApp
{
    internal class Day21 : IDay
    {

        private Dictionary<int, int[]> numpad = [];

        private Dictionary<arrows, int[]> arrowPad = [];

        private List<string> Codes;

        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            Codes = File.ReadAllLines($"{dir}/{GetType().Name}.txt").ToList();

            numpad[7] = [0, 0];
            numpad[8] = [1, 0];
            numpad[9] = [2, 0];

            numpad[4] = [0, 1];
            numpad[5] = [1, 1];
            numpad[6] = [2, 1];

            numpad[1] = [0, 2];
            numpad[2] = [1, 2];
            numpad[3] = [2, 2];


            numpad[0] = [1, 3];
            numpad[10] = [2, 3];


            arrowPad[arrows.up] = [1, 0];
            arrowPad[arrows.A] = [2, 0];
            arrowPad[arrows.left] = [0, 1];
            arrowPad[arrows.down] = [1, 1];
            arrowPad[arrows.right] = [2, 1];
        }

        private List<arrows> MovesToNumpadPosition(int currentnum, int goal)
        {

            var curr = numpad[currentnum];
            var goalp = numpad[goal];

            bool disableXbeforeY = (currentnum == 0 || currentnum == 10) && goalp[X] == 0;

            var diffy = curr[Y] - goalp[Y];
            var absy = Math.Abs(diffy);
            var diffx = curr[X] - goalp[X];
            var absx = Math.Abs(diffx);

            if (diffx < 0)
            {
                disableXbeforeY = true;
            }

            if (curr[X] == 0 && goalp[Y] == 3)
                disableXbeforeY = false;


            var moves = new List<arrows>();
            if (!disableXbeforeY)
            {
                for (int i = 0; i < absx; i++)
                {
                    moves.Add(diffx > 0 ? arrows.left : arrows.right);
                }
                for (int i = 0; i < absy; i++)
                {
                    moves.Add(diffy > 0 ? arrows.up : arrows.down);
                }

                return moves;
            }

            for (int i = 0; i < absy; i++)
            {
                moves.Add(diffy > 0 ? arrows.up : arrows.down);
            }
            for (int i = 0; i < absx; i++)
            {
                moves.Add(diffx > 0 ? arrows.left : arrows.right);
            }

            return moves;


        }


        private List<arrows> MovesToArrowPadPosition(arrows current, arrows goal)
        {
            var curr = arrowPad[current];
            var goalp = arrowPad[goal];

            var diffy = goalp[Y] - curr[Y];
            var absy = Math.Abs(diffy);
            var diffx = goalp[X] - curr[X];
            var absx = Math.Abs(diffx);


            bool disableXBeforeY = curr[Y] == 0 && goalp[X] == 0;



            if (diffx > 0 && current != arrows.left)
                disableXBeforeY = true;


            var moves = new List<arrows>();
            if (!disableXBeforeY)
            {
                for (int i = 0; i < absx; i++)
                {
                    moves.Add(diffx > 0 ? arrows.right : arrows.left);
                }

                for (int i = 0; i < absy; i++)
                {
                    moves.Add(diffy > 0 ? arrows.down : arrows.up);
                }

                return moves;
            }

            for (int i = 0; i < absy; i++)
            {
                moves.Add(diffy > 0 ? arrows.down : arrows.up);
            }
            for (int i = 0; i < absx; i++)
            {
                moves.Add(diffx > 0 ? arrows.right : arrows.left);
            }

            return moves;

        }

        private int X = 0;
        private int Y = 1;

        private enum arrows
        {
            left = '<',
            right = '>',
            up = '^',
            down = 'v',
            A = 'A'
        }

        private Dictionary<string, List<string>> MoveTransformationDictionary = [];

        private Dictionary<string, decimal> UniqueSubstringCount = [];


        List<string> AddToMoveTransformationDictionary(string start)
        {
            List<string> toReturn = [];

            var moves = start.Select(x => (arrows)x).ToList();

            moves = GetArrowPadMovesLevel2(moves);


            var rest = moves;
            while (rest.Any())
            {
                var indexOfA = rest.IndexOf(arrows.A);
                var firstPart = rest.Take(indexOfA + 1);
                rest = rest.Skip(indexOfA + 1).ToList();
                var str = new string(firstPart.Select(x => (char)x).ToArray());
                toReturn.Add(str);
            }

            MoveTransformationDictionary[start] = toReturn;

            return toReturn;
        }


        private void GetArrowPadMovesLevel2Part2(List<arrows> moves, int iterations)
        {
            var rest = moves;
            while (rest.Any())
            {

                var indexOfA = rest.IndexOf(arrows.A);
                var firstPart = rest.Take(indexOfA + 1).ToList();
                rest = rest.Skip(indexOfA + 1).ToList();

                var str = new string(firstPart.Select(x => (char)x).ToArray());

                UniqueSubstringCount.TryAdd(str, 0);
                UniqueSubstringCount[str] += 1;
            }

            for (int i = 0; i < iterations; i++)
            {
                Dictionary<string, decimal> nextDictionary = [];

                foreach (var kvp in UniqueSubstringCount)
                {
                    List<string> dictionaryHit = null;

                    if (!MoveTransformationDictionary.TryGetValue(kvp.Key, out dictionaryHit))
                    {
                        dictionaryHit = AddToMoveTransformationDictionary(kvp.Key);
                    }


                    foreach (var newPart in dictionaryHit)
                    {
                        nextDictionary.TryAdd(newPart, 0);
                        nextDictionary[newPart] += UniqueSubstringCount[kvp.Key];
                    }
                }

                UniqueSubstringCount = nextDictionary;
            }

        }

        private List<arrows> GetArrowPadMovesLevel2(List<arrows> moves)
        {
            var currentPos = arrows.A;

            var movesList = new List<arrows>();
            foreach (var move in moves)
            {
                if (currentPos == move)
                {
                    movesList.Add(arrows.A);
                }
                else
                {
                    movesList.AddRange(MovesToArrowPadPosition(currentPos, move));
                    movesList.Add(arrows.A);

                    currentPos = move;
                }
            }

            return movesList;
        }

        private List<arrows> GetArrowPadMovesLevel1(string code)
        {
            List<arrows> clicks = [];
            int currNum = 10;
            foreach (var ch in code)
            {
                int newNum = 0;
                if (ch == 'A')
                {
                    newNum = 10;
                }
                else
                {
                    newNum = ch - '0';
                }

                var moves = MovesToNumpadPosition(currNum, newNum);
                currNum = newNum;

                clicks.AddRange(moves);
                clicks.Add(arrows.A);
            }

            return clicks;
        }



        private decimal DoNumpadMoves(string code)
        {
            var firstLevel = GetArrowPadMovesLevel1(code);

            var secondLevel = GetArrowPadMovesLevel2(firstLevel);

            var result = GetArrowPadMovesLevel2(secondLevel);

            var numcode = int.Parse(code.Replace("A", ""));

            return (decimal)numcode * result.Count;
        }

        public decimal Part1()
        {
            return Codes.Sum(x => DoNumpadMoves(x));
        }


        public decimal Part2()
        {
            decimal totalCount = 0;

           // debug();

            foreach (var code in Codes)
            {
                var moves = GetArrowPadMovesLevel1(code);

                UniqueSubstringCount.Clear();
                GetArrowPadMovesLevel2Part2(moves, 25);

                var numcode = int.Parse(code.Replace("A", ""));

                var num = UniqueSubstringCount.Sum(x => x.Value * x.Key.Length);
                totalCount += num * numcode;

            }

            return totalCount;


        }

        public void debug()
        {
            foreach (var from in Enum.GetValues<arrows>())
            {
                foreach (var to in Enum.GetValues<arrows>())
                {
                    var moves = MovesToArrowPadPosition(from, to);
                    Console.WriteLine($"{from} -> {to}: {string.Join(',', moves)}");
                }
            }

            foreach (var from in numpad)
            {
                foreach (var to in numpad)
                {
                    var moves = MovesToNumpadPosition(from.Key, to.Key);
                    Console.WriteLine($"{from.Key} -> {to.Key}: {string.Join(',', moves)}");
                }
            }

            foreach (var code in Codes)
            {
                var moves = GetArrowPadMovesLevel1(code);

                Console.WriteLine();
                Console.WriteLine($"{code} moves: {string.Join("", moves.Select(x => (char)x))}");

                for (int i = 1; i < 5; i++)
                {
                    UniqueSubstringCount.Clear();
                    GetArrowPadMovesLevel2Part2(moves, i);
                    Console.WriteLine($" {UniqueSubstringCount.Sum(x => x.Value * x.Key.Length)}");
                }



            }
        }

    }
}
