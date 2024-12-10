using System.Diagnostics;

namespace ConsoleApp
{
    class Files
    {
        public int Name;
        public int Start;
        public int Length;
    }

    class FreeMemorySlot
    {
        public int Start;
        public int Length;
        public FreeMemorySlot? Next;
    }

    internal class Day09 : IDay
    {
        private List<Files> files;
        private FreeMemorySlot? freeSpace;

        private string[] data;

        public void ReadInput()
        {
            files = [];

            var dir = Debugger.IsAttached ? "Example" : "Input";
            data = File.ReadAllLines($"{dir}/{GetType().Name}.txt");

            freeSpace = new FreeMemorySlot() { Start = -1, Length = -1 }
                    ;
            bool file = true;
            int pointer = 0;
            int fileCounter = 0;
            FreeMemorySlot? lastFreeSpace = freeSpace;
            for (int i = 0; i < data[0].Length; i++)
            {
                var length = data[0][i] - '0';
                if (file)
                {
                    // only create file if it has size (note that fileCounter must increase regardless though)
                    if (length > 0)
                    {
                        files.Add(new Files()
                        {
                            Name = fileCounter,
                            Start = pointer,
                            Length = length
                        });
                    }

                    fileCounter++;
                }
                else
                {
                    // add to linked list of memory if it has size
                    if (length > 0)
                    {
                        var space = new FreeMemorySlot { Start = pointer, Length = length };

                        lastFreeSpace.Next = space;
                        lastFreeSpace = space;
                    }

                }

                pointer += length;
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

                // we did not move this part, add it to result from original location
                if (currentFileMoved < length)
                {
                    var l = length - currentFileMoved;
                    result += (decimal)files[i].Name * (pointer * l + (l * (l - 1)) / 2);

                }
            }


            return result;
        }

        void recalcPointersToFirstFreeeLocation(FreeMemorySlot[] pointers, int maxPointer)
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

        }

        public decimal Part2()
        {
            decimal result = 0;

            var pointerToFirstFreeLocationOfSize = new FreeMemorySlot?[10];

            // init a list of pointers to first free memory slot of size x
            for (int x = 1; x < 10; x++)
            {
                var c = freeSpace;

                while (c != null && c.Length < x)
                    c = c.Next;

                pointerToFirstFreeLocationOfSize[x] = c;
            }

            for (int i = files.Count - 1; i >= 0; i--)
            {
                var length = files[i].Length;
                var pointer = files[i].Start;
                var firstFreeSpace = pointerToFirstFreeLocationOfSize[length];

                if (firstFreeSpace == null || firstFreeSpace.Start > pointer)
                {
                    // we do not move this part, count it from original location
                    result += (decimal)files[i].Name * (pointer * length + (length * (length - 1)) / 2);
                    continue;
                }
                
                // we moved this to the found free space, add to result
                result += (decimal)files[i].Name * (firstFreeSpace.Start * length + (length * (length - 1)) / 2);

                // reduce size and start of current free space
                firstFreeSpace.Start += length;
                firstFreeSpace.Length -= length;

                // update pointers to free memory
                recalcPointersToFirstFreeeLocation(pointerToFirstFreeLocationOfSize, pointer);
            }

            return result;

        }
    }
}
