using System.Collections.Generic;

namespace HttpLoadTester.DTOs
{
    public class TestDurationReport
    {
        public string Name { get; set; }
        public IEnumerable<TestDurationReportItem> Items { get; set; }
    }
}