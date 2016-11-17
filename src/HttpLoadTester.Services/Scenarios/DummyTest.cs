using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttpLoadTester.Entites.Test;
using System.Threading;

namespace HttpLoadTester.Services.Scenarios
{
    public class DummyTest : ITest
    {

        public DummyTest()
        {
            _random = new Random(5000);

        }
        private readonly Random _random;
        public string Name { get { return "Dummy"; } }

        public string DisplayText { get { return "Dummy Tests"; } }

        public async Task Run(TestResult result)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            result.Status = ResultStatusType.Running;
            result.StartDate = DateTime.UtcNow;
            Console.WriteLine($"test started {result.Id}");
            int random = _random.Next(500, 5000);

            await Task.Run(() => 
                    {
                        Thread.Sleep(random);
                        result.Duration = sw.ElapsedMilliseconds;
                        result.Status = random % 10 == 0 ? ResultStatusType.Failed : ResultStatusType.Success;
                });

            Console.WriteLine($"test done {result.Id}");
        }
    }
}
