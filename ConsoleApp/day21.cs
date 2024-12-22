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

            var variation = new List<arrows>();

            if (!disableXBeforeY && (diffx == 0 || diffy == 0))
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

            if (toReturn.Count > 2 && current == arrows.A)
                return toReturn[1];

            return toReturn[0];
        }

        private int X = 0;
        private int Y = 1;

        private int[] UP = [0, -1];
        private int[] DOWN = [1, -1];
        private int[] RIGHT = [0, 1];
        private int[] LEFT = [1, 0];
        private int[] PUSH = [0, 0];



        private enum arrows
        {
            left = '<',
            right = '>',
            up = '^',
            down = 'v',
            A = 'A'
        }

        arrows DicrectionToArrowMap(int[] dir)
        {
            if (dir == LEFT) return arrows.left;
            if (dir == RIGHT) return arrows.right;
            if (dir == UP) return arrows.up;
            if (dir == DOWN) return arrows.down;

            return arrows.A;
        }

        private Dictionary<string, List<string>> MoveTransformationDictionary = [];



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

        private List<arrows> GetArrowPadMovesLevel2Part2(List<arrows> moves)
        {

        }


        private List<arrows> GetArrowPadMovesLevel2(List<arrows> moves)
        {
            var currentPos = arrows.A;

            var variationList = new List<arrows>();
            foreach (var move in moves)
            {
                if (currentPos == move)
                {
                    variationList.Add(arrows.A);
                }
                else
                {
                    variationList.AddRange(MovesToArrowPadPosition(currentPos, move));
                    variationList.Add(arrows.A);

                    currentPos = move;
                }
            }


            return variationList;
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


        private decimal DoNumpadMoves(string code, int additionaLevels = 1)
        {
            var firstLevel = GetArrowPadMovesLevel1(code);


            var variations = BuildAllVariations(firstLevel);

            List<arrows> shortestSecond = null;
            List<arrows> shortestFirst = null;
            foreach (var variation in variations)
            {
                var secondLevel = GetArrowPadMovesLevel2(variation);


                if (shortestSecond == null || shortestSecond.Count > secondLevel.Count)
                {
                    shortestFirst = variation;
                    shortestSecond = secondLevel;
                }
            }

            List<arrows> result = shortestSecond;

            for (int i = 0; i < additionaLevels; i++)
            {
                result = GetArrowPadMovesLevel2(result);
                Console.WriteLine($"After {i}: {result.Count} lenth.");

                Console.WriteLine($"code: {code}: {string.Join("", result.Select(x => (char)x))}");
            }

            var numcode = int.Parse(code.Replace("A", ""));
            //Console.WriteLine($"code: {code}: {string.Join("", shortestFirst.Select(x => (char)x))}");
            //Console.WriteLine($"code: {code}: {string.Join("", shortestSecond.Select(x => (char)x))}");
            //Console.WriteLine($"code: {code}: {string.Join("", thirdLevel.Select(x => (char)x))}");
            //Console.WriteLine($"code: {numcode}*{thirdLevel.Count}={numcode * thirdLevel.Count}");
            return (decimal)numcode * result.Count;
        }

        public decimal Part1()
        {
            return Codes.Sum(x => DoNumpadMoves(x));
        }



        public decimal Part2()
        {


            List<arrows> result = [arrows.down];
            for (int i = 0; i < 25; i++)
            {
                result = GetArrowPadMovesLevel2(result);
                Console.WriteLine($"After {i}: {result.Count} lenth.");

                //  Console.WriteLine($"code: {code}: {string.Join("", result.Select(x => (char)x))}");
            }
            return Codes.Sum(x => DoNumpadMoves(x, 1));

        }





    }


}
