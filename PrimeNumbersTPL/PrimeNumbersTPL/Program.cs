using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace PrimeNumbersTPL
{
    class Program
    {
        const int threadCount = 11;
        const int MaxNum = 100000000;

        static void Main(string[] args)
        {
            var watcher = new Stopwatch();
            watcher.Start();

            var tasks = new Task<HashSet<long>>[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                long start = MaxNum / threadCount * i;
                long end = MaxNum / threadCount * (i + 1);
                tasks[i] = Task.Run(() => CheckRange(start, end));
            }

            var result =
                Task.WhenAll(tasks)
                    .ContinueWith(_ =>
                        tasks.Aggregate(
                            new HashSet<long>(),
                            (r, t) =>
                            {
                                r.UnionWith(t.Result);
                                return r;
                            })
                ).Result;

            watcher.Stop();

            Console.WriteLine($"{Environment.NewLine}Calculation finished in {watcher.Elapsed}");
            Console.WriteLine($"{Environment.NewLine}Found {result.Count} simple numbers.");
        }

        private static HashSet<long> CheckRange(long start, long end)
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} started,  params:{start} {end}");
            var wather = new Stopwatch();
            var result = new HashSet<long>();

            wather.Start();

            for (var i = start; i <= end; i++)
            {
                if (IsPrimeSimple(i))
                    result.Add(i);
            }

            wather.Stop();
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} finished after {wather.Elapsed},  params:{start} {end}, found {result.Count} prime numbers");
            return result;
        }

        private static bool IsPrimeSimple(long num)
        {
            var sqrt = Math.Sqrt(num);

            if (num % 2 == 0) return false;

            for (var i = 3; i < sqrt; i += 2)
            {
                if (num % i == 0) return false;
            }

            return true;
        }
    }
}
