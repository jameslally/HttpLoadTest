using System.Collections.Generic;

namespace HttpLoadTester.DTOs
{
    public class TestReport
    {
        public string Name { get; set; }
        public int ProcessedInLastMinute { get; set; }
        public IEnumerable<TestReportRow> Rows { get; set; }
    }
}
