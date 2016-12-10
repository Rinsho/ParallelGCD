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
            //Change seed and/or count, or set to a custom list
            List<int> numbers = GenerateList(17, 1000000);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int result = GCD.CalculateSequential(numbers);
            sw.Stop();
            Console.WriteLine($"Sequential GCD: {result}");
            Console.WriteLine($"Time (ms): {sw.ElapsedMilliseconds}");
            sw.Reset();
            sw.Start();
            result = GCD.CalculateParallel(numbers, 10000);
            sw.Stop();
            Console.WriteLine($"Parallel GCD: {result}");
            Console.WriteLine($"Time (ms): {sw.ElapsedMilliseconds}");
            Console.ReadKey();
        }

        //Accepts a seed value up to 3571, the 500th prime.
        //Past that will be out of the bounds of an integer.
        private static List<int> GenerateList(int seed, int count)
        {
            List<int> values = new List<int>(count);
            Random rnd = new Random();
            for (int i = 0; i < count; i++)
                values.Add(seed * rnd.Next(601367));
            return values;
        }
    }

    public static class GCD
    {
        public static int CalculateParallel(List<int> numbers, int taskLoad)
        {
            int totalTasks = numbers.Count / taskLoad;
            Task<int>[] gcdTasks = new Task<int>[totalTasks];
            for (int i = 0; i < totalTasks; i++)
            {
                int startIndex = i * taskLoad;
                int endIndex = (i * taskLoad) + (taskLoad - 1);
                gcdTasks[i] = Task.Run(() => CollapseGCDRange(numbers, startIndex, endIndex));
            }

            int gcd = 0;
            int remainingNumbers = numbers.Count % taskLoad;
            if (remainingNumbers > 0)
            {
                int startIndex = totalTasks * taskLoad;
                gcd = CollapseGCDRange(numbers, startIndex, --remainingNumbers);
            }

            Task.WaitAll(gcdTasks);
            foreach (Task<int> task in gcdTasks)
                gcd = BinaryEuclidGCD(gcd, task.Result);
            return gcd;
        }

        private static int CollapseGCDRange(List<int> numbers, int startIndex, int endIndex)
        {
            int gcd = numbers[startIndex];
            for (int j = startIndex + 1; j < endIndex; j++)
                gcd = BinaryEuclidGCD(gcd, numbers[j]);
            return gcd;
        }

        public static int CalculateSequential(List<int> numbers)
        {
            int gcd = numbers[0];
            for (int i = 1; i < numbers.Count; i++)
                gcd = BinaryEuclidGCD(gcd, numbers[i]);
            return gcd;
        }

        public static int BinaryEuclidGCD(int firstNumber, int secondNumber)
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
