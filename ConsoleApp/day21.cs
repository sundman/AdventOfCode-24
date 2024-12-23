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

        private List<List<arrows>> MovesToNumpadPosition(int currentnum, int goal)
        {
            var toReturn = new List<List<arrows>>();

            var curr = numpad[currentnum];
            var goalp = numpad[goal];

            // special rule to not step out of bounds
            bool disableXbeforeY = (currentnum == 0 || currentnum == 10) && goalp[X] == 0;

            var diffy = curr[Y] - goalp[Y];
            var absy = Math.Abs(diffy);
            var diffx = curr[X] - goalp[X];
            var absx = Math.Abs(diffx);


            var subList = new List<arrows>();
            if (!disableXbeforeY && (diffx != 0 && diffy != 0))
            {
                for (int i = 0; i < absx; i++)
                {
                    subList.Add(diffx > 0 ? arrows.left : arrows.right);
                }
                for (int i = 0; i < absy; i++)
                {
                    subList.Add(diffy > 0 ? arrows.up : arrows.down);
                }
                toReturn.Add(subList);
            }

            subList = new List<arrows>();
            for (int i = 0; i < absy; i++)
            {
                subList.Add(diffy > 0 ? arrows.up : arrows.down);
            }
            for (int i = 0; i < absx; i++)
            {
                subList.Add(diffx > 0 ? arrows.left : arrows.right);
            }

            toReturn.Add(subList);


            return toReturn;
        }


        private List<arrows> MovesToArrowPadPosition(arrows current, arrows goal)
        {
            var toReturn = new List<List<arrows>>();
            var curr = arrowPad[current];
            var goalp = arrowPad[goal];

            var diffy = curr[Y] - goalp[Y];
            var absy = Math.Abs(diffy);
            var diffx = curr[X] - goalp[X];
            var absx = Math.Abs(diffx);


            bool disableXBeforeY = curr[Y] == 0 && goalp[X] == 0;
            bool disableYBeforeX = curr[X] == 0;

            var variation = new List<arrows>();


            if (current == arrows.right && goal == arrows.up)
                return [arrows.left, arrows.up];

            if (!disableXBeforeY)
            {
                for (int i = 0; i < absx; i++)
                {
                    variation.Add(diffx > 0 ? arrows.left : arrows.right);
                }

                for (int i = 0; i < absy; i++)
                {
                    variation.Add(diffy > 0 ? arrows.up : arrows.down);
                }

                toReturn.Add(variation);
            }

            if (!disableYBeforeX || (diffx == 0 || diffy == 0))
            {
                variation = [];
                for (int i = 0; i < absy; i++)
                {
                    variation.Add(diffy > 0 ? arrows.up : arrows.down);
                }
                for (int i = 0; i < absx; i++)
                {
                    variation.Add(diffx > 0 ? arrows.left : arrows.right);
                }
                toReturn.Add(variation);
            }

            //     if (toReturn.Count > 2 && current == arrows.A)
            //         return toReturn[1];

            return toReturn[0];
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


        List<List<arrows>> BuildAllVariations(List<List<List<arrows>>> clickVariationsList)
        {
            List<List<arrows>> toReturn = [];

            // fuck it, this should be done recursive but lets do it grunt stile for now
            foreach (var button1 in clickVariationsList[0])
                foreach (var button2 in clickVariationsList[1])
                    foreach (var button3 in clickVariationsList[2])
                        foreach (var button4 in clickVariationsList[3])
                        {
                            var clicks = new List<arrows>();
                            clicks.AddRange(button1);
                            clicks.AddRange(button2);
                            clicks.AddRange(button3);
                            clicks.AddRange(button4);
                            toReturn.Add(clicks);
                        }

            return toReturn;

        }


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

        private List<List<List<arrows>>> GetArrowPadMovesLevel1(string code)
        {
            List<List<List<arrows>>> clicks = [];
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

                foreach (var list in moves)
                    list.Add(arrows.A);
                clicks.Add(moves);
            }

            return clicks;
        }


        private List<arrows> MovesAfterStep2(string code)
        {
            var firstLevel = GetArrowPadMovesLevel1(code);


            var variations = BuildAllVariations(firstLevel);

            List<arrows> shortestSecond = null;
            foreach (var variation in variations)
            {
                var secondLevel = GetArrowPadMovesLevel2(variation);

                //    Console.WriteLine($"{code} variation: {string.Join("", secondLevel.Select(x => (char)x))}");
                //    Console.WriteLine($"Will turn into {GetArrowPadMovesLevel2(secondLevel).Count} and then {GetArrowPadMovesLevel2(GetArrowPadMovesLevel2(secondLevel)()).Count} ");
                if (shortestSecond == null || shortestSecond.Count > secondLevel.Count)
                {
                    shortestSecond = secondLevel;
                }
            }

            return shortestSecond;
        }

        private List<arrows> MovesAfterStep1(string code)
        {
            var firstLevel = GetArrowPadMovesLevel1(code);


            var variations = BuildAllVariations(firstLevel);

            List<arrows> shortestSecond = null;
            foreach (var variation in variations)
            {
                var secondLevel = GetArrowPadMovesLevel2(variation);


                if (shortestSecond == null || shortestSecond.Count > secondLevel.Count)
                {
                    shortestSecond = secondLevel;
                }
            }

            return shortestSecond;
        }

        private decimal DoNumpadMoves(string code)
        {
            var firstLevel = GetArrowPadMovesLevel1(code);


            var variations = BuildAllVariations(firstLevel);

            List<arrows> shortestSecond = null;
            foreach (var variation in variations)
            {
                var secondLevel = GetArrowPadMovesLevel2(variation);


                if (shortestSecond == null || shortestSecond.Count > secondLevel.Count)
                {
                    shortestSecond = secondLevel;
                }
            }

            List<arrows> result = shortestSecond;

            result = GetArrowPadMovesLevel2(result);


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
            foreach (var code in Codes)
            {
                var firstLevel = GetArrowPadMovesLevel1(code);
                var variations = BuildAllVariations(firstLevel);

                //foreach (var variation in variations)
                //{

                //    //var startMoves = MovesAfterStep2(code); 
                //    Console.WriteLine();
                //    Console.WriteLine($"{code} variation: {string.Join("", variation.Select(x => (char)x))}");
                //    var input = variation;
                //    for (int i = 1; i < 4; i++)
                //    {
                //        input = GetArrowPadMovesLevel2(input);
                //        UniqueSubstringCount.Clear();
                //        GetArrowPadMovesLevel2Part2(variation, i);

                //        Console.WriteLine($" {UniqueSubstringCount.Sum(x => x.Value * x.Key.Length)}: " +
                //                          $"{string.Join("", input.Select(x => (char)x))}");
                //    }

                //    var numcode = int.Parse(code.Replace("A", ""));

                //    var num = UniqueSubstringCount.Sum(x => x.Value * x.Key.Length);
                //    totalCount += num * numcode;
                //}
            }

            return totalCount;


            // Here are all the counts for 029A all the way to 25: 4, 12, 28, 68, 164, 404, 998, 2482, 6166, 15340, 38154, 94910, 236104, 587312, 1461046, 3634472, 9041286, 22491236, 55949852, 139182252, 346233228, 861298954, 2142588658, 5329959430, 13258941912, 32983284966, 82050061710

            // 866 685 207 341 too low
            // 1 305 271 414 768 too low
            // 3 265 640 606 594

            // 723400511121604 wrong
        }





    }


}
