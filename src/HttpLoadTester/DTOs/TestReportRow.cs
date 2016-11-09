using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpLoadTester.DTOs
{
    public class TestReportRow
    {
        public string Status { get; set; }
        public int Count { get; set; }
        public int AverageDuration { get; set; }
    }
}
