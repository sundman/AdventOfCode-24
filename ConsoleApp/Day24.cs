using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using String = System.String;

namespace ConsoleApp
{
    internal class Day24 : IDay
    {


        interface Gate
        {
            public string Name { get; set; }
            public Gate[] Input { get; set; }
            public bool? Output { get; }
            public bool? PureOutput { get; }
            public string Type { get; }
            public Gate OverrideOutput { get; set; }
        }

        class AndGate : Gate
        {
            public Gate OverrideOutput { get; set; }

            public string Type => "AND";
            public string Name { get; set; }
            public Gate[] Input
            {
                get;
                set;
            }

            public bool? PureOutput => Input[0].Output.Value && Input[1].Output.Value;
            public bool? Output => OverrideOutput != null ? OverrideOutput.PureOutput : Input[0].Output.Value && Input[1].Output.Value;
        }

        class OrGate : Gate
        {
            public Gate OverrideOutput { get; set; }
            public string Type => "OR";
            public string Name { get; set; }
            public Gate[] Input
            {
                get;
                set;
            }

            public bool? PureOutput => Input[0].Output.Value || Input[1].Output.Value;
            public bool? Output => OverrideOutput != null ? OverrideOutput.PureOutput : Input[0].Output.Value || Input[1].Output.Value;
        }

        class XorGate : Gate
        {
            public Gate OverrideOutput { get; set; }
            public string Type => "XOR";
            public string Name { get; set; }
            public Gate[] Input { get; set; }

            public bool? PureOutput => Input[0].Output ^ Input[1].Output;
            public bool? Output => OverrideOutput != null ? OverrideOutput.PureOutput : Input[0].Output ^ Input[1].Output;
        }

        class InputGate(bool state) : Gate
        {

            public Gate OverrideOutput { get; set; }
            public string Type => "Input";
            public string Name { get; set; }
            public bool? State = state;
            public Gate[] Input
            {
                get;
                set;
            }
            public bool? PureOutput => State;

            public bool? Output => State;
        }



        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var data = File.ReadAllLines($"{dir}/{GetType().Name}.txt");

            bool handleInputs = true;

            List<List<string>> chainParser = [];
            foreach (var line in data)
            {
                if (string.IsNullOrEmpty(line))
                {
                    handleInputs = false;
                    continue;
                }

                if (handleInputs)
                {
                    var parts = line.Split(':', StringSplitOptions.TrimEntries);

                    var gate = new InputGate(parts[1][0] == '1') { Name = parts[0] };
                    InputGates.Add(gate);
                    Gates[parts[0]] = gate;
                }
                else
                {
                    var parts = line.Split(' ');
                    chainParser.Add(parts.ToList());
                }
            }


            foreach (var part in chainParser)
            {
                var type = part[1];
                var name = part[4];

                switch (type)
                {
                    case "AND":
                        Gates[name] = new AndGate() { Name = name }; break;
                    case "OR":
                        Gates[name] = new OrGate() { Name = name }; break;
                    case "XOR":
                        Gates[name] = new XorGate() { Name = name }; break;
                }
            }

            foreach (var part in chainParser)
            {
                var gate = Gates[part[4]];
                var a = Gates[part[0]];
                var b = Gates[part[2]];
                gate.Input = [a, b];

            }
        }

        Dictionary<string, Gate> Gates = [];
        private List<InputGate> InputGates = [];

        public decimal Part1()
        {
            long result = 0;


            var output = Gates.Where(x => x.Key.StartsWith("z"))
                .OrderBy(x => x.Key).Select(x => x.Value.Output).ToList();




            for (int i = 0; i < output.Count; i++)
            {
                if (output[i].Value)
                {
                    result |= ((long)1 << i); // Set the ith bit if boolArray[i] is true } }

                }
            }

            return result;
        }

        private void SetInput(long x, long y)
        {

            for (int i = 0; i < InputGates.Count / 2; i++)
            {
                ((InputGate)Gates["x" + i.ToString().PadLeft(2, '0')]).State = (x & ((long)1 << i)) != 0;
                ((InputGate)Gates["y" + i.ToString().PadLeft(2, '0')]).State = (y & ((long)1 << i)) != 0;
            }
        }

        private long ReadNumber()
        {
            long result = 0;

            for (int i = 0; i < Output.Count; i++)
            {
                if (Output[i].Output.Value)
                {
                    result |= ((long)1 << i);

                }
            }

            return result;
        }

        private void printInputsLeadingToGate(Gate g1, List<List<(string, string)>> toPrint, int level)
        {
            if (toPrint.Count <= level)
                toPrint.Add([]);

            var gate = g1.OverrideOutput == null ? g1 : g1.OverrideOutput;


            toPrint[level].Add((gate.Name, $"{gate.Name}: {gate?.Input?[0]?.Name} {gate.Type} {gate?.Input?[1]?.Name}" +
                                           $" :{gate?.Input?[0]?.Output} {gate.Type} {gate?.Input?[1]?.Output}"));

            if (gate?.Input?[0] != null)
                printInputsLeadingToGate(gate.Input[0], toPrint, level + 1);

            if (gate?.Input?[1] != null)
                printInputsLeadingToGate(gate.Input[1], toPrint, level + 1);
        }


        private void guessWrongLeadingGates(Gate gate)
        {
            List<Gate> almostInputGates = [];
            List<(string gate, string type)> inputGates = [];

            Queue<Gate> queue = [];
            queue.Enqueue(gate);

            Console.WriteLine($"Checking {gate.Name}...");
            while (queue.TryDequeue(out var current))
            {
                if (current.OverrideOutput != null)
                    current = current.OverrideOutput;

                if (current.Input[0].Type == "Input")
                {
                    almostInputGates.Add(current);
                    inputGates.Add((current.Input[0].Name, current.Type));
                    inputGates.Add((current.Input[1].Name, current.Type));
                }

                else
                {
                    queue.Enqueue(current.Input[0]);
                    queue.Enqueue(current.Input[1]);

                }
            }

            var num = int.Parse(gate.Name.Substring(1));

            var xstr = "x" + num.ToString().PadLeft(2, '0');
            var ystr = "x" + num.ToString().PadLeft(2, '0');

            if (inputGates.Count(x => x.gate == xstr && x.type == "XOR") != 1)
            {
                Console.WriteLine($"wrong number of x{num} XOR inputs");
            }
            if (inputGates.Count(x => x.gate == ystr && x.type == "XOR") != 1)
            {
                Console.WriteLine($"wrong number of y{num} XOR inputs");
            }

            for (int i = 0; i < num; i++)
            {

                xstr = "x" + i.ToString().PadLeft(2, '0');
                ystr = "x" + i.ToString().PadLeft(2, '0');
                if (inputGates.Count(x => x.gate == xstr && x.type == "AND") != 1)
                {
                    Console.WriteLine($"wrong number of x{i} AND inputs");
                }
                if (inputGates.Count(x => x.gate == ystr && x.type == "AND") != 1)
                {
                    Console.WriteLine($"wrong number of y{i} AND inputs");
                }

                if (i > 0)
                {
                    if (inputGates.Count(x => x.gate == xstr && x.type == "XOR") != 1)
                    {
                        Console.WriteLine($"wrong number of x{i} XOR inputs");
                    }
                    if (inputGates.Count(x => x.gate == ystr && x.type == "XOR") != 1)
                    {
                        Console.WriteLine($"wrong number of y{i} XOR inputs");
                    }
                }
            }
        }
        private IList<Gate> Output;
        public decimal Part2()
        {
            return 0;
            Output = Gates.Where(x => x.Key.StartsWith("z"))
                 .OrderBy(x => x.Key).Select(x => x.Value).ToList();

            Console.WriteLine($"Unmodified: {ReadNumber()}");

            SetInput(0, 0);
            Console.WriteLine($"Empty input: {ReadNumber()}");






            //swap(Gates["dwp"], Gates["kfm"]);
            //swap(Gates["z22"], Gates["gjh"]);
            //swap(Gates["z31"], Gates["jdr"]);
            //swap(Gates["ffj"], Gates["z08"]);
            // guessWrongLeadingGates(Gates["z34"]);




            for (int i = 0; i < 45; i++)
            {
                SetInput((long)1 << i, 0);
                var res = ReadNumber();
                Console.WriteLine($"{res == (long)1 << i} {i}+0:\t {ReadNumber()} \t= {Convert.ToString(res, 2)}");

                SetInput(0, (long)1 << i);

                res = ReadNumber();
                Console.WriteLine($"{res == (long)1 << i} 0+{i}:\t {ReadNumber()} \t= {Convert.ToString(res, 2)}");
                
                SetInput((long)1 << i, (long)1 << i);
                res = ReadNumber();
                Console.WriteLine($"{res == (long)2 << i} {i}+{i}:\t {ReadNumber()} \t= {Convert.ToString(res, 2)}");
            }

            foreach (var gate in Output)
                guessWrongLeadingGates(gate);




            //guessWrongLeadingGates(Gates["z15"]);
            List<List<(string, string)>> print = [];
              printInputsLeadingToGate(Gates["z35"], print, 0);
              printInputsLeadingToGate(Gates["z22"], print, 0);

            for (int i = 0; i < print.Count; i++)
            {
                var dict = print[i].OrderBy(x => x.Item1);
                foreach (var kvp in dict)
                    Console.WriteLine($"Level {i}: {kvp.Item2}");
            }

            print = [];
            // printInputsLeadingToGate(Gates["z08"], print, 0);
            //  printInputsLeadingToGate(Gates["z22"], print, 0);

            //for (int i = 0; i < print.Count; i++)
            //{
            //    var dict = print[i].OrderBy(x => x.Item1);
            //    foreach (var kvp in dict)
            //        Console.WriteLine($"Level {i}: {kvp.Item2}");
            //}
            //guessWrongLeadingGates(Gates["z15"]);

            decimal result = 0;
            return 0;
        }

        private void swap(Gate g1, Gate g2)
        {
            g1.OverrideOutput = g2;
            g2.OverrideOutput = g1;





        }
    }
}
