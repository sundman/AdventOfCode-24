using System.Diagnostics;
using ConsoleApp;

var day = new Day7();

var total = new Stopwatch();
var timer = new Stopwatch();
total.Start();
timer.Start();
day.ReadInput();
timer.Stop();
Console.WriteLine($"Input read+parsed:{timer.ElapsedMilliseconds} ms {timer.ElapsedTicks} ticks");
timer.Restart();
var part1 = day.Part1();
timer.Stop();
Console.WriteLine($"Part1 answer: {part1}");
Console.WriteLine($"Part1 time: {timer.ElapsedMilliseconds} ms {timer.ElapsedTicks} ticks");
timer.Restart();
var part2 = day.Part2();
timer.Stop();
Console.WriteLine($"Part2 answer: {part2}");
Console.WriteLine($"Part2 time: {timer.ElapsedMilliseconds} ms {timer.ElapsedTicks} ticks");
Console.WriteLine($"Total time including writing these messages: {total.ElapsedMilliseconds} ms");

Console.ReadLine();