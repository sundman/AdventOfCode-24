using System.Data.Common;
using Microsoft.VisualBasic;

namespace ConsoleApp
{
    internal class Day5
    {
        private (Dictionary<int, HashSet<int>>, List<List<int>>) ReadInput()
        {
            var data = File.ReadAllLines($"Input/{GetType().Name}.txt");
            var dict = new Dictionary<int, HashSet<int>>();
            var list = new List<List<int>>();

            foreach (var line in data)
            {
                if (line.Contains('|'))
                {
                    var parts = line.Split('|').Select(int.Parse).ToList();
                    if (!dict.ContainsKey(parts[0]))
                        dict[parts[0]] = [];

                    dict[parts[0]].Add(parts[1]);
                }

                if (line.Contains(','))
                {
                    var ints = line.Split(',').Select(int.Parse).ToList();
                    list.Add(ints);
                }
            }

            return (dict, list);
        }

        // alternative way to do it, albeit not that effective. But fun. You can sort them by the number of times they appear on the left
        // side on those rules where both sides are in the list. 
        public void Party()
        {

            var data = File.ReadAllLines($"Input/{GetType().Name}.txt");

            var rules = data.Where(x => x.Contains('|')).Select(x => x.Split('|')).ToList();
            var lines = data.Where(x => x.Contains(',')).Select(x => x.Split(',')).ToList();

            decimal totalPart1 = 0;
            decimal totalPart2 = 0;
            foreach (var line in lines)
            {
                var rulesThatApply = rules.Where(x => line.Contains(x[0]) && line.Contains(x[1]));

                var correctOrder = line.Select(x => x)
                    .OrderByDescending(sort => rulesThatApply.Count(rule => rule[0] == sort)).ToList();

                if (correctOrder.SequenceEqual(line))
                    totalPart1 += int.Parse(correctOrder[correctOrder.Count / 2]);
                else
                    totalPart2 += int.Parse(correctOrder[correctOrder.Count / 2]);
            }

            Console.WriteLine($"Part1: {totalPart1}, Part2: {totalPart2}");

        }

        public decimal Part1()
        {
            decimal total = 0;
            var (dict, lists) = ReadInput();

            foreach (var list in lists)
            {
                var correct = true;
                for (int i = 0; i < list.Count - 1; i++)
                {
                    var after = list.Skip(i + 1).ToHashSet();

                    if (!after.IsSubsetOf(dict[list[i]]))
                    {
                        correct = false;
                        break;
                    }
                }

                if (correct)
                {
                    total += list[list.Count / 2];
                }
            }
            return total;
        }

        public decimal Part2()
        {
            decimal total = 0;
            var (dict, lists) = ReadInput();

            foreach (var list in lists)
            {
                for (int i = 0; i < list.Count - 1; i++)
                {
                    var after = list.Skip(i + 1).ToHashSet();

                    if (!after.IsSubsetOf(dict[list[i]]))
                    {
                        list.Sort((i, ii) => dict[i].Contains(ii) ? 1 : -1);

                        total += list[list.Count / 2];
                        break;
                    }
                }
            }
            return total;
        }

        // original way to sort...
        private List<int> sortList(List<int> list, Dictionary<int, HashSet<int>> dict)
        {
            List<int> toReturn = [];
            var numsLeftToHandle = list.ToList();

            while (numsLeftToHandle.Any())
            {
                // select the number which none of the others have in their dict-list
                var next = numsLeftToHandle.First(x =>
                    !dict.Where(kvp => numsLeftToHandle.Contains(kvp.Key)).Any(kvp => kvp.Value.Contains(x)));

                numsLeftToHandle.Remove(next);
                toReturn.Insert(0, next);
            }

            return toReturn;
        }
    }
}
