using System.Collections.Generic;

namespace HttpLoadTester.DTOs
{
    public class TestExceptionReport
    {
        public IEnumerable<TestExceptionReportItem> Exceptions { get; set; }
    }
}