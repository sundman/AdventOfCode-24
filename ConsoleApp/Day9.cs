using System.Diagnostics;

namespace ConsoleApp
{
    class Files
    {
        public int Name;
        public int Start;
        public int Length;
    }

    class FreeSpace
    {
        public int Start;
        public int Length;
        public FreeSpace? Next;
        public FreeSpace? Prev;
    }

    internal class Day9 : IDay
    {
        private int[] memory;
        private List<Files> files;
        private FreeSpace? freeSpace;

        private string[] data;

        public void ReadInput()
        {
            files = [];

            var dir = Debugger.IsAttached ? "Example" : "Input";
            data = File.ReadAllLines($"{dir}/{GetType().Name}.txt");

            int memLenght = 0;
            for (int i = 0; i < data[0].Length; i++)
            {
                memLenght += data[0][i] - '0';
            }

            memory = new int[memLenght];
            freeSpace = new FreeSpace() { Start = -1, Length = -1 }
                    ;
            bool file = true;
            int pointer = 0;
            int fileCounter = 0;
            FreeSpace? lastFreeSpace = freeSpace;
            for (int i = 0; i < data[0].Length; i++)
            {
                var length = data[0][i] - '0';
                if (file && length > 0)
                {
                    files.Add(new Files()
                    {
                        Name = fileCounter,
                        Start = pointer,
                        Length = length
                    });
                }
                else
                {
                    if (length > 0)
                    {
                        var space = new FreeSpace { Start = pointer, Length = length };

                        lastFreeSpace.Next = space;
                        lastFreeSpace = space;
                    }

                }

                var toWrite = file ? fileCounter++ : 0;


                for (int pos = 0; pos < length; pos++)
                {
                    memory[pointer++] = toWrite;
                }

                file = !file;
            }
        }


        public decimal Part1()
        {

            var currentFreeSpace = freeSpace.Next;
            var currentFreeSpaceUsage = 0;
            decimal result = 0;

            for (int i = files.Count - 1; i >= 0; i--)
            {
                int length = files[i].Length;
                int pointer = files[i].Start;
                int currentFileMoved = 0;

                if (currentFreeSpace != null && currentFreeSpace.Start <= pointer)
                {

                    while (currentFileMoved < length)
                    {
                        if (currentFreeSpace == null || currentFreeSpace.Start + currentFreeSpaceUsage > pointer)
                            break;


                        int currStart = currentFreeSpace.Start + currentFreeSpaceUsage;
                        int currLength = currentFreeSpace.Length - currentFreeSpaceUsage;


                        // we fit, copy remainder of file
                        if (length - currentFileMoved <= currLength)
                        {

                            var l = length - currentFileMoved;
                            result += (decimal)files[i].Name * (currStart * l + (l * (l - 1)) / 2);
                            currentFreeSpaceUsage += length - currentFileMoved;
                            currentFileMoved = length;


                        }
                        else // we dont fit, copy part that fits
                        {

                            var l = currLength;
                            result += (decimal)files[i].Name * (currStart * l + (l * (l - 1)) / 2);
                            currentFileMoved += currLength;
                            currentFreeSpace = currentFreeSpace.Next;
                            currentFreeSpaceUsage = 0;
                        }

                    }
                }

                if (currentFileMoved < length)
                {
                    var l = length - currentFileMoved;
                    result += (decimal)files[i].Name * (pointer * l + (l * (l - 1)) / 2);

                }
            }


            return result;
        }

        FreeSpace[] recalcPointersToFirstFreeeLocation(FreeSpace[] pointers, int maxPointer)
        {
            for (int x = 1; x < 10; x++)
            {
                var c = pointers[x];

                while (c != null && c.Length < x)
                    c = c.Next;

                if (c == null || c.Start > maxPointer)
                    pointers[x] = null;

                pointers[x] = c;
            }

            return pointers;
        }

        public decimal Part2()
        {

            decimal result = 0;

            var pointerToFirstFreeLocation = new FreeSpace?[10];

            for (int x = 1; x < 10; x++)
            {
                var c = freeSpace;

                while (c != null && c.Length < x)
                    c = c.Next;

                pointerToFirstFreeLocation[x] = c;
            }

            for (int i = files.Count - 1; i >= 0; i--)
            {
                var length = files[i].Length;
                var pointer = files[i].Start;
                FreeSpace? firstFreeSpace = pointerToFirstFreeLocation[length];


                if (firstFreeSpace == null || firstFreeSpace.Start > pointer)
                {
                    result += (decimal)files[i].Name * (pointer * length + (length * (length - 1)) / 2);
                    continue;
                }
                result += (decimal)files[i].Name * (firstFreeSpace.Start * length + (length * (length - 1)) / 2);


                firstFreeSpace.Start += length;
                firstFreeSpace.Length -= length;

                recalcPointersToFirstFreeeLocation(pointerToFirstFreeLocation, pointer);
            }

            return result;

        }
    }
}
