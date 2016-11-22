namespace HttpLoadTester.DTOs
{
    public class TestDurationReportItem
    {
        public string EventTime {get;set;}
        public double AverageDuration {get;set;}
        public double MedianDuration {get;set;}
        public int FailedRequests {get;set;}
        public int SuccessfulRequests {get;set;}
    }
}