using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GCD
{
    class Program
    {
        static void Main(string[] args)
        {
            List<uint> numbers = GenerateList(17, 1000000);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            uint result = GCD.CalculateSequential(numbers);
            sw.Stop();
            Console.WriteLine($"Sequential GCD: {result}");
            Console.WriteLine($"Time (ms): {sw.ElapsedMilliseconds}");
            sw.Reset();
            sw.Start();
            result = GCD.CalculateParallel(numbers, 15000);
            sw.Stop();
            Console.WriteLine($"Parallel GCD: {result}");
            Console.WriteLine($"Time (ms): {sw.ElapsedMilliseconds}");
            Console.ReadKey();
        }

        //Accepts a seed value up to 3571, the 500th prime.  Past that will be out
        //of range for a uint unless a smaller maxRange is specified.
        private static List<uint> GenerateList(int seed, int count, int maxRange = 1200000)
        {
            List<uint> values = new List<uint>(count);
            Random rnd = new Random();
            for (int i = 0; i < count; i++)
                values.Add((uint)(seed * rnd.Next(maxRange)));
            return values;
        }
    }

    public static class GCD
    {
        public static uint CalculateParallel(List<uint> numbers, int taskLoad)
        {
            int totalTasks = numbers.Count / taskLoad;
            Task<uint>[] gcdTasks = new Task<uint>[totalTasks];
            for (int i = 0; i < totalTasks; i++)
            {
                int startIndex = i * taskLoad;
                int endIndex = (i * taskLoad) + (taskLoad - 1);
                gcdTasks[i] = Task.Run(() => CollapseGCDRange(numbers, startIndex, endIndex));
            }

            uint gcd = 0;
            int remainingNumbers = numbers.Count % taskLoad;
            if (remainingNumbers > 0)
            {
                int startIndex = totalTasks * taskLoad;
                gcd = CollapseGCDRange(numbers, startIndex, --remainingNumbers);
            }

            Task.WaitAll(gcdTasks);
            foreach (Task<uint> task in gcdTasks)
                gcd = BinaryEuclidGCD(gcd, task.Result);
            return gcd;
        }

        private static uint CollapseGCDRange(List<uint> numbers, int startIndex, int endIndex)
        {
            uint gcd = numbers[startIndex];
            for (int j = startIndex + 1; j < endIndex; j++)
                gcd = BinaryEuclidGCD(gcd, numbers[j]);
            return gcd;
        }

        public static uint CalculateSequential(List<uint> numbers)
        {
            uint gcd = numbers[0];
            for (int i = 1; i < numbers.Count; i++)
                gcd = BinaryEuclidGCD(gcd, numbers[i]);
            return gcd;
        }

        public static uint BinaryEuclidGCD(uint firstNumber, uint secondNumber)
        {
            if (firstNumber == 0)
                return secondNumber;
            if (secondNumber == 0 || firstNumber == secondNumber)
                return firstNumber;

            //Cases where at least one number is even
            if ((firstNumber & 1) == 0)
            {
                if ((secondNumber & 1) == 0)
                    return BinaryEuclidGCD(firstNumber >> 1, secondNumber >> 1) << 1;
                else
                    return BinaryEuclidGCD(firstNumber >> 1, secondNumber);
            }
            if ((secondNumber & 1) == 0)
                return BinaryEuclidGCD(firstNumber, secondNumber >> 1);

            //Cases where both numbers are odd
            if (firstNumber > secondNumber)
                return BinaryEuclidGCD((firstNumber - secondNumber) >> 1, secondNumber);
            return BinaryEuclidGCD((secondNumber - firstNumber) >> 1, firstNumber);
        }
    }
}
