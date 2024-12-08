using System.Diagnostics;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;

namespace ConsoleApp
{
    internal class Day9 : IDay
    {
        private Dictionary<char, List<Point>> antennas;
        private int mapSize;
        public void ReadInput()
        {
            var dir = /*Debugger.IsAttached ? "Example" :*/ "Input";
            var data = File.ReadAllLines($"{dir}/{GetType().Name}.txt");
           
        }


        public decimal Part1()
        {
            decimal result = 0;

            return result;
        }
      
        public decimal Part2()
        {

            return 0;

        }
    }
}
