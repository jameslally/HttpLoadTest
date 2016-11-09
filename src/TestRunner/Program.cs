using HttpLoadTester.Entites.Test;
using HttpLoadTester.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestRunner
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var r = new HttpLoadTester.Services.Scenarios.PFMUserTest();
            var runner = new ScenariosRunner(r, 10, 500);

            var results = new ConcurrentBag<TestResult>();
            runner.ExecuteTestRun(results);
            
            foreach (var group in results.GroupBy(g => g.Status))
            {
                var count = group.Count();
                var avg = group.Average(g => g.Duration);
                Console.WriteLine($"{group.Key} - {count} items averaging {avg}ms per test");
            }

            Console.ReadLine();
        }
    }
}
