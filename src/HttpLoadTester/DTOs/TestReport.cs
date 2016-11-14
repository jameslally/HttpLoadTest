using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpLoadTester.DTOs
{
    public class TestReport
    {
        public int ProcessedInLastMinute { get; set; }
        public IEnumerable<TestReportRow> Rows { get; set; }
    }
}
