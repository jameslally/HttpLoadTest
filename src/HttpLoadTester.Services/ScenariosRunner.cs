using HttpLoadTester.Entites.Test;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HttpLoadTester.Services
{
    public class ScenariosRunner
    {
        private readonly ITest _test;
        private readonly int _parallelTests;
        private readonly int _numberOfTests;
        private List<Task> _workerThreads;
        public ScenariosRunner(ITest test , int parallelTests , int numberOfTests)
        {
            _test = test;
            _parallelTests = parallelTests;
            _numberOfTests = numberOfTests;
            _workerThreads = new List<Task>();
            
        }

        public bool Active { get; set; }

        public void ExecuteTestRun (ConcurrentBag<TestResult> results)
        {
            if (results == null)
                throw new ArgumentNullException();

            Active = true;
            for (int i = 0; i < _parallelTests; i++)
            {
                var task = Task.Run(() => workerLoop(results));
                _workerThreads.Add(task);
            }
 
        }

        private void workerLoop(ConcurrentBag<TestResult> results)
        {
            while (Active)
            {
                var result = new TestResult();
                results.Add(result);
                _test.Run(result).Wait();
            }
        }
    }
}
