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
        public FreeSpace? Last;
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
            freeSpace = null;
            bool file = true;
            int pointer = 0;
            int fileCounter = 0;
            FreeSpace? lastFreeSpace = null;
            for (int i = 0; i < data[0].Length; i++)
            {
                var length = data[0][i] - '0';
                if (file)
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
                    var space = new FreeSpace { Start = pointer, Length = length };
                    if (lastFreeSpace != null)
                    {
                        space.Last = lastFreeSpace;
                        lastFreeSpace.Next = space;
                    }
                    else
                    {
                        freeSpace = space;
                    }

                    lastFreeSpace = space;

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
            var targetMemory = new int[memory.Length];

            var currentFreeSpace = freeSpace;
            var currentFreeSpaceUsage = 0;
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
                            Array.Copy(memory, pointer + currentFileMoved, targetMemory, currStart, length - currentFileMoved);
                            currentFreeSpaceUsage += length - currentFileMoved;
                            currentFileMoved = length;
                           

                        }
                        else // we dont fit, copy part that fits
                        {
                            Array.Copy(memory, pointer + length - currentFileMoved - currLength, targetMemory, currStart, currLength);
                            currentFileMoved += currLength;
                            currentFreeSpace = currentFreeSpace.Next;
                            currentFreeSpaceUsage = 0;
                        }

                    }
                }

                // copy parts that should not be moved
                if (currentFileMoved < length)
                {
                    Array.Copy(memory, pointer, targetMemory, pointer, length - currentFileMoved);
                }
            }

            decimal result = 0;
            for (var i = 0; i < targetMemory.Length; i++)
            {
                result += (decimal)targetMemory[i] * i;

            }

            return result;
        }

        public decimal Part2()
        {
            var targetMemory = new int[memory.Length];

            int noFreeSpaceOfSize = 10;
            for (int i = files.Count - 1; i >= 0; i--)
            {
                var length = files[i].Length;
                var pointer = files[i].Start;

                if (noFreeSpaceOfSize < length)
                {
                    Array.Copy(memory, pointer, targetMemory, pointer, length);
                    continue;
                }

                FreeSpace? firstFreeSpace = null;

                var curr = freeSpace;
                while (curr != null)
                {
                    if (curr.Length >= length)
                    {
                        firstFreeSpace = curr;
                        break;
                    }

                    if (curr.Start > pointer)
                    {
                        noFreeSpaceOfSize = length;
                        break;
                    }

                    curr = curr.Next;
                }

                if (firstFreeSpace == null)
                {
                    Array.Copy(memory, pointer, targetMemory, pointer, length);
                    continue;
                }

                Array.Copy(memory, pointer, targetMemory, firstFreeSpace.Start, length);

                if (length < firstFreeSpace.Length)
                {
                    firstFreeSpace.Start += length;
                    firstFreeSpace.Length -= length;
                }
                else
                {
                    if (firstFreeSpace.Last != null)
                        firstFreeSpace.Last.Next = firstFreeSpace.Next;
                    else
                        freeSpace = firstFreeSpace.Next;

                    if (firstFreeSpace.Next != null)
                        firstFreeSpace.Next.Last = firstFreeSpace.Last;
                }
            }

            decimal result = 0;
            for (int i = 0; i < targetMemory.Length; i++)
            {
                result += (decimal)targetMemory[i] * i;
            }

            return result;

        }
    }
}
