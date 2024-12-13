using System.Diagnostics;

namespace ConsoleApp
{
    internal class Day13 : IDay
    {
        private List<Tuple<decimal[,], decimal[]>> equations;

        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var rows = File.ReadAllLines($"{dir}/{GetType().Name}.txt");
            equations = [];

            int row = 0;

            while (row < rows.Length)
            {

                // Prize: X=8122, Y=6806
                // 
                var a = rows[row++].Substring(12).Split(", Y+", StringSplitOptions.TrimEntries).Select(decimal.Parse).ToArray();
                var b = rows[row++].Substring(12).Split(", Y+", StringSplitOptions.TrimEntries).Select(decimal.Parse).ToArray();
                var result = rows[row++].Substring(9).Split(", Y=", StringSplitOptions.TrimEntries).Select(decimal.Parse).ToArray();
                row++;

                var array = new[,] { { a[0], b[0] }, { a[1], b[1] } };
                equations.Add(new(array, result));
            }
        }

        private decimal[] Gauss(decimal[,] a, decimal[] b)
        {
            var n = a.GetLength(0);

            //Forward Elimination
            for (var k = 0; k < n - 1; k++)
            {
                for (var i = k + 1; i < n; i++)
                {
                    var factor = a[i, k] / a[k, k];
                    for (var j = k + 1; j < n; j++)
                    {
                        a[i, j] -= factor * a[k, j];
                    }
                    b[i] -= factor * b[k];
                }
            }

            //Backward Substitution
            var x = new decimal[n];
            x[n - 1] = b[n - 1] / a[n - 1, n - 1];
            for (var i = n - 2; i >= 0; i--)
            {
                var sum = b[i];
                for (var j = i + 1; j < n; j++)
                {
                    sum -= a[i, j] * x[j];
                }
                x[i] = sum / a[i, i];
            }
            return x;

        }

        public decimal Part1()
        {
            decimal result = 0;
            foreach (var eq in equations)
            {
                var answer = Gauss((decimal[,])eq.Item1.Clone(), (decimal[])eq.Item2.Clone());

                var a = answer[0].AsInteger();
                var b = answer[1].AsInteger();

                if (a.HasValue && b.HasValue)
                {
                    result += a.Value * 3 + b.Value;
                }
            }

            return result;
        }

        public decimal Part2()
        {
            decimal result = 0;
            foreach (var eq in equations)
            {

                decimal[] neweq = [eq.Item2[0] + 10000000000000, eq.Item2[1] + 10000000000000];
                var answer = Gauss(eq.Item1, neweq);

                var a = answer[0].AsInteger();
                var b = answer[1].AsInteger();

                if (a.HasValue && b.HasValue && a > 0 && b > 0)
                {
                    result += a.Value * 3 + b.Value;
                }
            }

            return result;
        }
    }

    static class Extension
    {
        public static decimal? AsInteger(this decimal number)
        {
            return Math.Abs(number - Math.Round(number)) < 1e-8m ? Math.Round(number) : null;
        }
    }
}
