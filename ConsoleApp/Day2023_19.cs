using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Data.SqlTypes;
using System.Diagnostics;

namespace ConsoleApp
{
    internal class Day2023_19
    {
        public static Dictionary<string, WorkFlow> workflows = [];

        private List<int[]> Inputs = [];
        public void ReadInput()
        {
            var dir = Debugger.IsAttached ? "Example" : "Input";
            var rows = File.ReadAllLines($"{dir}/{GetType().Name}.txt");


            int index = 0;
            while (!string.IsNullOrEmpty(rows[index]))
            {
                var part1 = rows[index].Split('{');

                var name = part1[0];
                // px{a<2006:qkq,m>2090:A,rfg}

                // cdz{s>3074:kzg,m>2365:ckf,s<2573:zxl,sqj}

                var part2 = part1[1].Split(',');

                var rule = createRule(part1[1]);

                var flow = new WorkFlow()
                {
                    Name = name,
                    Rule = rule
                };

                workflows[name] = flow;

                index++;
            }

            index++; // empty row

            while (index < rows.Length)
            {
                // {x=1272,m=2035,a=996,s=123}

                var remove = rows[index].Substring(1, rows[index].Length - 2);

                var parts = remove.Split(',');

                var ints = parts.Select(x => int.Parse(x.Substring(2))).ToArray();
                Inputs.Add(ints);
                index++;
            }

            In = workflows["in"];
        }

        private WorkFlow In;


        private Rule createRule(string s)
        {
            var toReturn = new Rule();

            if (s[0] == 'A')
                return Accept;

            if (s[0] == 'R')
                return Reject;

            switch (s[0])
            {
                case 'x':
                    toReturn.Variable = variable.x; break;
                case 'm':
                    toReturn.Variable = variable.m; break;
                case 'a':
                    toReturn.Variable = variable.a; break;
                case 's':
                    toReturn.Variable = variable.s; break;
            }


            var ch = s[1];
            if (ch == '<')
                toReturn.Operator = oper.lesserThan;
            else if (ch == '>')
                toReturn.Operator = oper.inputGreaterThan;




            if (!s.Contains(":"))
            {
                if (toReturn.Operator == oper.redirect)
                {
                    var target = s.Replace("}", "");
                    return new Redirect() { WorkFlowTarget = target };
                }

                toReturn.Value = int.Parse(s.Substring(2));
                combos.Add((toReturn.Variable, /*toReturn.Operator, */toReturn.Value));
                return toReturn;

            }

            var colon = s.IndexOf(":");
            var comma = s.IndexOf(",");

            toReturn.Value = int.Parse(s.Substring(2, colon - 2));

            combos.Add((toReturn.Variable, /*toReturn.Operator, */toReturn.Value));


            toReturn.IfTrue = createRule(s.Substring(colon + 1, comma - colon - 1));
            toReturn.IfFalse = createRule(s.Substring(comma + 1));
            return toReturn;
        }

        HashSet<(variable, /*oper,*/ int)> combos = [];


        public class WorkFlow
        {
            public string Name;
            public Rule Rule;
        }





        public class Range
        {
            public int Start;
            public int End;

            public static readonly Range StartRange = new Range { Start = 1, End = 4000 };

            public bool Valid => Start < End;

            public (Range yes, Range no) Split(oper op, int number)
            {
                if (op == oper.inputGreaterThan)
                {
                    var yes = new Range() { Start = number + 1, End = End };
                    var no = new Range() { Start = Start, End = number };

                    return (yes, no);
                }

                return (new Range() { Start = Start, End = number - 1 },
                    new Range { Start = number, End = End });
            }
        }

        private static readonly Redirect Accept = new Redirect();
        private static readonly Redirect Reject = new Redirect();

        public class Redirect : Rule
        {
            public string WorkFlowTarget;

            public List<Range[]> ReachedWith = [];

            public override Redirect Followrule(int[] input)
            {
                return this;
            }

            public override void Followrule(Range[] input)
            {
                if (WorkFlowTarget != null)
                {
                    workflows[WorkFlowTarget].Rule.Followrule(input);
                }
                else
                {
                    ReachedWith.Add(input);
                }
            }
        }


        public class Rule
        {
            public Rule IfTrue;
            public Rule IfFalse;

            public oper Operator;
            public variable Variable;

            public int Value;

            public virtual Redirect Followrule(int[] input)
            {

                var variable = input[(int)Variable];

                if (Operator == oper.lesserThan)
                {
                    if (variable < Value)
                        return IfTrue.Followrule(input);
                    else
                        return IfFalse.Followrule(input);
                }


                if (variable > Value)
                    return IfTrue.Followrule(input);
                else
                    return IfFalse.Followrule(input);
            }

            public virtual void Followrule(Range[] input)
            {
                var variable = input[(int)Variable];

                var (yes, no) = variable.Split(Operator, Value);

                if (yes.Valid)
                {
                    var newInput = input.Select(x => x).ToArray();
                    newInput[(int)Variable] = yes;
                    IfTrue.Followrule(newInput);
                }

                if (no.Valid)
                {
                    var newInput = input.Select(x => x).ToArray();
                    newInput[(int)Variable] = no;
                    IfFalse.Followrule(newInput);
                }



            }

        }

        public enum variable
        {
            x,
            m,
            a,
            s
        }

        public enum oper
        {
            redirect,
            inputGreaterThan,
            lesserThan,
            Accept,
            Reject
        }


        public bool FollowRules(int[] input)
        {
            var currentWorkflow = In;
            while (true)
            {
                var result = currentWorkflow.Rule.Followrule(input);

                if (result == Accept)
                    return true;
                if (result == Reject)
                    return false;

                currentWorkflow = workflows[result.WorkFlowTarget];
            }
        }



        public decimal Part1()
        {
            return Inputs.Sum(x => FollowRules(x) ? x.Sum() : 0);

        }

        public decimal Part2()
        {

            var currentWorkflow = In;

            var input = new Range[]
            {
                Range.StartRange, Range.StartRange, Range.StartRange, Range.StartRange
            };

            currentWorkflow.Rule.Followrule(input);


            Console.WriteLine($"Accept reached with {Accept.ReachedWith.Count} ranges.");

            decimal result = 0;
            foreach (var range in Accept.ReachedWith)
            {
                decimal toAdd = 1;
                foreach (var range1 in range)
                {
                    toAdd *= range1.End - range1.Start+1;
                }

                result += toAdd;
            }


            return result;
        }
    }
}
