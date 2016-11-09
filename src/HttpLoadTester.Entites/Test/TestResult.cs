using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpLoadTester.Entites.Test
{
    public enum ResultStatusType
    {
        ToDo=0
        ,Running=1
        ,Success=2
        ,Failed=3
    }
    public class TestResult
    {
        public ResultStatusType Status { get; set; }
        public int Id { get; set; }
        public DateTime? StartDate { get; set; }
        public long? Duration { get; set; }
    }

}
