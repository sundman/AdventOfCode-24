

using System.Diagnostics;
using ConsoleApp;

var day = new Day1();

Stopwatch timer = new Stopwatch();  
timer.Start();
Console.WriteLine(day.Part1());
Console.WriteLine(timer.ElapsedMilliseconds);
Console.WriteLine(day.Part2());
Console.WriteLine(timer.ElapsedMilliseconds);
Console.ReadLine();