using System.Diagnostics;

namespace ConsoleApp
{
    internal class Day2022_11 : IDay
    {

        class Monkey
        {
            public string Name;

            public int Test;

            public int IndexTrue;
            public int IndexFalse;

            public List<long> Items = [];

            public Operation Operation;

            public int OperationInput;

            public int InspectedItems;

            public (long item, int toMonkey) InspectItem(bool part1, long? gcd)
            {
                InspectedItems++;

                var item = Items[0];
                Items.RemoveAt(0);

                var newItem = PerformOperation(item);

                if (part1)
                    newItem /= 3;

                if (gcd.HasValue)
                    newItem = newItem % gcd.Value;

                var toIndex = newItem % Test == 0 ? IndexTrue : IndexFalse;

                return (newItem, toIndex);

            }

            private long PerformOperation(long item)
            {
                switch (Operation)
                {
                    case Operation.add:
                        return (item + OperationInput);
                        break;
                    case Operation.mult:
                        return (item * OperationInput);
                        break;
                    case Operation.exp:
                        return (item * item);

                }

                return -1;
            }
        }

        enum Operation
        {
            add,
            mult,
            exp
        }

        private List<Monkey> Monkeys = [];
        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var rows = File.ReadAllLines($"{dir}/{GetType().Name}.txt");


            int index = 0;
            while (index < rows.Length)
            {
                var monkey = new Monkey();
                monkey.Name = rows[index++];
                monkey.Items = rows[index++].Substring(18).Split(',', StringSplitOptions.TrimEntries)
                    .Select(long.Parse)
                    .ToList();

                var operation = rows[index++].Substring(23).Split(' ');

                if (operation[1] == "old")
                    monkey.Operation = Operation.exp;
                else
                {
                    if (operation[0] == "+")
                        monkey.Operation = Operation.add;
                    else
                        monkey.Operation = Operation.mult;

                    monkey.OperationInput = int.Parse(operation[1]);
                }

                monkey.Test = int.Parse(rows[index++].Substring(21));

                monkey.IndexTrue = rows[index++].Last() - '0';
                monkey.IndexFalse = rows[index++].Last() - '0';

                Monkeys.Add(monkey);
                index++;
            }

        }



        public decimal Part1()
        {

            for (int i = 0; i < 20; i++)
            {
                foreach (var monkey in Monkeys)
                {
                    while (monkey.Items.Any())
                    {
                        var (item, toMonkey) = monkey.InspectItem(true, null);
                        Monkeys[toMonkey].Items.Add(item);
                    }
                }
            }

            var order = Monkeys.OrderByDescending(x => x.InspectedItems).ToList();
            return order[0].InspectedItems * order[1].InspectedItems;
        }

        public decimal Part2()
        {
            Monkeys.Clear();
            ReadInput();

            var gcd = 1;
            foreach (var monkey in Monkeys)
            {
                gcd *= monkey.Test;
            }

            for (int i = 0; i < 10000; i++)
            {
                foreach (var monkey in Monkeys)
                {
                    while (monkey.Items.Any())
                    {
                        var (item, toMonkey) = monkey.InspectItem(false, gcd);
                        Monkeys[toMonkey].Items.Add(item);
                    }
                }
            }

          

            var order = Monkeys.OrderByDescending(x => x.InspectedItems).ToList();
            return (decimal)order[0].InspectedItems * order[1].InspectedItems;

        }
    }
}
