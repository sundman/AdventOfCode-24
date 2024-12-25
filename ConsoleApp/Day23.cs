using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using String = System.String;

namespace ConsoleApp
{
    internal class Day23 : IDay
    {
        private List<long> StartNumbers;

        public static int ComputerCounter = 0;

        private class Computer
        {
            public readonly int UniqueNumber = ComputerCounter++;
            public string Name;
            public List<Computer> ConnectedTo = [];
        }

        class ComputerList(HashSet<Computer> computers)
        {
            public HashSet<Computer> Computers = computers;
            public override bool Equals(object? obj)
            {
                var toCmp = (ComputerList)obj;

                if (toCmp.Computers.Count != Computers.Count)
                    return false;

                foreach (var c in Computers)
                    if (!toCmp.Computers.Contains(c))
                        return false;

                return true;
            }

            public bool CheckIfComplete(Computer computer)
            {
                if (Computers.Any(x => !x.ConnectedTo.Contains(computer)))
                    return false;

                return true;
            }

            public override int GetHashCode()
            {
                return Computers.Sum(x => x.GetHashCode());
            }
        }

        private Dictionary<string, Computer> ComputerNames = [];

        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var data = File.ReadAllLines($"{dir}/{GetType().Name}.txt");

            foreach (var line in data)
            {
                var parts = line.Split('-');

                Computer c1, c2;


                if (ComputerNames.ContainsKey(parts[0]))
                    c1 = ComputerNames[parts[0]];
                else
                {
                    c1 = new Computer() { Name = parts[0] };
                    ComputerNames[parts[0]] = c1;
                }

                if (ComputerNames.ContainsKey(parts[1]))
                    c2 = ComputerNames[parts[1]];
                else
                {
                    c2 = new Computer() { Name = parts[1] };
                    ComputerNames[parts[1]] = c2;
                }

                c1.ConnectedTo.Add(c2);
                c2.ConnectedTo.Add(c1);
            }
        }



        public decimal Part1()
        {
            long result = 0;


            var t = ComputerNames.Where(c1 =>
                c1.Key.StartsWith("t"));


            HashSet<ComputerList> triples = [];

            foreach (var cm in t)
            {
                var c1 = cm.Value;

                foreach (var c2 in c1.ConnectedTo)
                {
                    foreach (var c3 in c2.ConnectedTo)
                    {
                        if (c3.ConnectedTo.Contains(c1))
                            triples.Add(new ComputerList([c1, c2, c3]));
                    }
                }
            }


            //foreach (var triple in triples)
            //{

            //    Console.WriteLine($"{string.Join('-', triple.Computers.Select(x => x.Name))}");
            //}

            return triples.Count;
        }



        public decimal Part2()
        {
            decimal result = 0;



            var graph = ComputerNames.Select(x => x.Value).ToList();
            var adjacencyList = new HashSet<int>[graph.Count];

            foreach (var c1 in graph)
            {
                adjacencyList[c1.UniqueNumber] = [];
                foreach (var c2 in c1.ConnectedTo)
                    adjacencyList[c1.UniqueNumber].Add(c2.UniqueNumber);
            }

            List<Computer> largestSet = [];

            List<Computer> currentCliqueMax = [];

            // while (graph.Count > largestSet.Count)

            //   Console.WriteLine(($"Current graph size: {graph.Count}, Largest clique found: {currentCliqueMax.Count}"));
            //   var currentNode = graph.First();

            List<List<int>> cliquest = [];
            FindClique([], graph.Select(x => x.UniqueNumber).ToHashSet(), [], cliquest, adjacencyList);
            if (currentCliqueMax.Count > largestSet.Count)
            {
                largestSet.Clear();
                largestSet.AddRange(currentCliqueMax);
            }

            //foreach (var c1 in currentNode.ConnectedTo)
            //{
            //    vertices.Remove((c1.UniqueNumber << 16) + currentNode.UniqueNumber);
            //}
            //graph.Remove(currentNode);



            var maxClique = cliquest.OrderByDescending(x => x.Count).ToList();

            var ordered = maxClique.First().Select(x => ComputerNames.First(c => c.Value.UniqueNumber == x))
                .OrderBy(x => x.Key);


            Console.WriteLine($"{string.Join(',',
                ordered.
                    Select(x => x.Value.Name))}");

            return maxClique.Count;
        }


        void FindClique(List<int> R, HashSet<int> P, HashSet<int> X, List<List<int>> cliques, HashSet<int>[] adjacencyList)

        {

            if (P.Count == 0 && X.Count == 0)
            {
                cliques.Add(new List<int>(R)); 
                return;
            }

            var P2 = new List<int>(P); // Copy P to avoid modification during iteration

            foreach (int v in P2)
            {
                R.Add(v);
                var neighbors = adjacencyList[v];

                var Pnew = P.Where(u => neighbors.Contains(u)).ToHashSet();
                var Xnew = X.Where(u => neighbors.Contains(u)).ToHashSet();

                FindClique(R, Pnew, Xnew, cliques, adjacencyList); 
                R.Remove(v); 
                P.Remove(v); 
                X.Add(v);
            }

        }


    }
}
