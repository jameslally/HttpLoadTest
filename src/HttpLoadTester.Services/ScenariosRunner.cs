using HttpLoadTester.Entites.Test;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpLoadTester.Services
{
    public class ScenariosRunner
    {
        private readonly ITest _test;
        private readonly int _parallelTests;
        private readonly int _numberOfTests;
        public ScenariosRunner(ITest test , int parallelTests , int numberOfTests)
        {
            _test = test;
            _parallelTests = parallelTests;
            _numberOfTests = numberOfTests;
        }

        public void ExecuteTestRun (ConcurrentBag<TestResult> results)
        {
            if (results == null)
                throw new ArgumentNullException();

            Parallel.For(0, _numberOfTests,
                    new ParallelOptions() { MaxDegreeOfParallelism = _parallelTests },
                    i =>
                    {
                        var result = new TestResult() { Id = i + 1};
                        results.Add(result);
                        _test.Run(result).Wait();
                    });
        }
    }
}
