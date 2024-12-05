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
