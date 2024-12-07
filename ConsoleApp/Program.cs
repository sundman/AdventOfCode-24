using System.Diagnostics;
using ConsoleApp;

var day = new Day7();

var total = new Stopwatch();
var timer = new Stopwatch();
total.Start();
timer.Start();
day.ReadInput();
Console.WriteLine($"Input read+parsed:{timer.ElapsedTicks} ticks (Total: {total.ElapsedMilliseconds} ms)");
timer.Restart();
Console.WriteLine($"Part1 answer: {day.Part1()}");
Console.WriteLine($"Part1 time: {timer.ElapsedTicks} ticks (Total: {total.ElapsedMilliseconds} ms)");
timer.Restart();

Console.WriteLine($"Part2 answer: {day.Part2()}");
Console.WriteLine($"Part2 time: {timer.ElapsedTicks} ticks (Total: {total.ElapsedMilliseconds} ms)");
Console.ReadLine();