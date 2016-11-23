using System;

namespace HttpLoadTester.DTOs
{
    public class TestExceptionReportItem
    {
        public string TestName {get;set;}
        public DateTime StartDate {get;set;}

        public string Time { get { return StartDate.ToString("HH:mm"); } }
        public string ResponseCode {get;set;}
        public string Message {get;set;}
    }
}