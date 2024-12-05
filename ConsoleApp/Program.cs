

using System.Diagnostics;
using ConsoleApp;

var day = new Day5();

Stopwatch timer = new Stopwatch();  
timer.Start();
Console.WriteLine($"Part1: {day.Part1()}");
Console.WriteLine($"Elapsed MS:{timer.ElapsedMilliseconds}");
Console.WriteLine($"Part2: {day.Part2()}");
Console.WriteLine($"Elapsed MS:{timer.ElapsedMilliseconds}");
Console.ReadLine();