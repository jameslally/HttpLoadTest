using System;
using System.Collections.Concurrent;
using HttpLoadTester.Entites.Test;
using HttpLoadTester.Services;

namespace ConsoleTestRunner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var test = new HttpLoadTester.Services.Scenarios.PFMAddNotes(new TestConfiguration() { BaseUrl = "http://localhost:56999/" });
            var testRunner = new HttpLoadTester.Services.ScenariosRunner(test , 1);

            var results = new ConcurrentBag<TestResult> ();
            testRunner.ExecuteTestRun(results);

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }
    }
}
