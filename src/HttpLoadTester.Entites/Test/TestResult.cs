using System;

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
        public TestResult()
        {
            Id = Guid.NewGuid();
        }
        public ResultStatusType Status { get; set; }
        public Guid Id { get; }
        public DateTime StartDate { get; set; }
        public long? Duration { get; set; }

        public Exception Exception {get;set;}

        public int StatusCode { get; set; }
    }

}
