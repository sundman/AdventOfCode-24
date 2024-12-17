using System.Diagnostics;
using System.Drawing;
using System.Security.Cryptography;

namespace ConsoleApp
{
    internal class Day17 : IDay
    {
        private const int A = 0;
        const int B = 1;
        const int C = 2;

        private long[] Register = new long[3];

        private List<int> Output = [];

        private List<Action<int>> operators;

        private void adv(int input)
        {
            var num = Register[A];
            var denom = (int)Math.Pow(2, combo(input));

            Register[A] = num / denom;
        }

        private void bxl(int input)
        {
            Register[B] ^= input;
        }

        private void bst(int input)
        {
            Register[B] = combo(input) % 8;
        }

        private void jnz(int input)
        {
            if (Register[A] == 0)
                return;

            CurrentInstructionPointer = input - 1;
        }

        private void bxc(int input)
        {
            Register[B] ^= Register[C];
        }

        private void outt(int input)
        {
            Output.Add((int)(combo(input) % 8));
        }

        private void bdv(int input)
        {
            var num = Register[A];
            var denom = (int)Math.Pow(2, combo(input));

            Register[B] = num / denom;
        }

        private void cdv(int input)
        {
            var num = Register[A];
            var denom = (int)Math.Pow(2, combo(input));

            Register[C] = num / denom;
        }

        private Int64 combo(int input)
        {
            if (input < 4)
                return input;

            return Register[input - 4];
        }

        private int CurrentInstructionPointer = 0;

        private List<Tuple<Action<int>, int>> Instructions = [];

        private List<int> RawInstructions;

        public void ReadInput()
        {
            operators = new List<Action<int>>()
            {
                adv,
                bxl,
                bst,
                jnz,
                bxc,
                outt,
                bdv,
                cdv

            };
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var rows = File.ReadAllLines($"{dir}/{GetType().Name}.txt");


            Register[A] = int.Parse(rows[0].Substring(12));
            Register[B] = int.Parse(rows[1].Substring(12));
            Register[C] = int.Parse(rows[2].Substring(12));

            RawInstructions = rows[4].Substring(9).Split(',').Select(int.Parse).ToList();

            int ptr = 0;
            while (ptr < RawInstructions.Count())
            {
                Instructions.Add(new(operators[RawInstructions[ptr++]], RawInstructions[ptr++]));
            }
        }

        private void RunProgramPart1()
        {
            while (CurrentInstructionPointer < Instructions.Count)
            {
                Instructions[CurrentInstructionPointer].Item1(Instructions[CurrentInstructionPointer].Item2);
                CurrentInstructionPointer++;
            }
        }

        private List<int> RunProgramPart2(Int64 a)
        {
            Register[A] = a;
            Register[B] = 0;
            Register[C] = 0;

            Output = new List<int>();
            CurrentInstructionPointer = 0;
            RunProgramPart1();
            return Output;
        }

        private Int64? FoundA;

        private void FindFirstPossibleA(Int64 alreadyFoundPart)
        {
            if (FoundA.HasValue)
            {
                return;
            }

            for (int i = 0; i < 8; i++)
            {
                // dont allow more leading 0, that leads nowhere
                if (alreadyFoundPart == 0 && i == 0)
                    continue;

                var thisNum = (alreadyFoundPart << 3) + i;

                var result = RunProgramPart2(thisNum);

                if (TailCompare(result, RawInstructions))
                {
                    if (result.Count == RawInstructions.Count)
                    {
                        FoundA = thisNum;
                        return;
                    }

                    FindFirstPossibleA(thisNum);
                }
            }
        }

        private bool TailCompare(List<int> a, List<int> b)
        {
            var indexDiff = b.Count - a.Count;

            for (int i = 0; i < a.Count; i++)
            {
                if (a[i] != b[i + indexDiff])
                    return false;
            }

            return true;
        }

        public decimal Part1()
        {
            RunProgramPart1();

            decimal result = 0;
            for (int i = 0; i < Output.Count; i++)
            {
                result = result * 10 + Output[i];
            }

            return result;
        }

        public decimal Part2()
        {
            FindFirstPossibleA(0);

            return FoundA.Value;
        }


    }


}
