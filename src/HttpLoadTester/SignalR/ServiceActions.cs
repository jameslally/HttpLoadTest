using HttpLoadTester.Entites.Test;
using HttpLoadTester.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace HttpLoadTester.SignalR
{
    public class ServiceActions
    {
        public Dictionary<string, ConcurrentBag<TestResult>> Results { get; }
        private readonly Dictionary<string, ScenariosRunner> _runner;
        private readonly IEnumerable<ITest> _tests;

        public ServiceActions (IEnumerable<ITest> tests)
        {
            _tests = tests;
            this.Results = new Dictionary<string, ConcurrentBag<TestResult>>(StringComparer.OrdinalIgnoreCase);
            this._runner = new Dictionary<string, ScenariosRunner>(StringComparer.OrdinalIgnoreCase);
        }

        public void StartService(string testName)
        {
            var test = _tests.FirstOrDefault(t => t.Name.Equals(testName, StringComparison.OrdinalIgnoreCase));
            if (test != null)
            {
                if (!_runner.ContainsKey(testName))
                {
                    var runner = new ScenariosRunner(test, 10);
                    this.Results.Add(testName, new ConcurrentBag<TestResult>());
                    runner.ExecuteTestRun(this.Results[testName]);
                    _runner.Add(testName, runner);
                }
                else
                {
                    var runner = _runner[testName];
                    if (!runner.Active)
                        runner.ExecuteTestRun(this.Results[testName]);
                }                   
            }
        }

        public void StopService(string testName)
        {
            if (_runner.ContainsKey(testName))
            {
                _runner[testName].Active = false;
            }
        }

        public void Status()
        {

        }

    }
}
