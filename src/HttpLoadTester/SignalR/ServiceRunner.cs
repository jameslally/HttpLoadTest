using HttpLoadTester.DTOs;
using HttpLoadTester.Entites.Test;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HttpLoadTester.SignalR
{
    public class ServiceRunner
    {
        private readonly IHubContext _hubContext;
        private readonly ServiceActions _statusService;
        private readonly string _sourceUrl;
        private Dictionary<string, int> _totalCount;
        

        public ServiceRunner(IConnectionManager connectionManager, IConfiguration configuration, ServiceActions statusService)
        {
            _hubContext = connectionManager.GetHubContext<DashboardHub>();
            _statusService = statusService;
            _sourceUrl = configuration["HttpSourceLocation"];
            _totalCount = new Dictionary<string, int>();
        }

        public bool Active { get; set; }

        public void DoQuickReports()
        {
            Active = true;

            while (Active)
            {
                var json = GetStatusJson();
                _hubContext.Clients.All.displayFromHub(json).Wait();

                Thread.Sleep(5000);
            }
        }

        public void DoLongRunningReports()
        {
            Active = true;
            while (Active)
            {
                var json = GetDurationJson();
                _hubContext.Clients.All.displayDurationReportFromHub(json).Wait();

                var exceptionJson = GetExceptionJson();
                _hubContext.Clients.All.displayExceptionReportFromHub(exceptionJson).Wait();
                Thread.Sleep(60000);
            }
        }

        private string GetExceptionJson(){
            var resultsList = new List<TestExceptionReportItem>();
            foreach (var key in _statusService.Results.Keys)
            {
                var results = new TestDurationReport();
                results.Name = key;
                
                var events = _statusService.Results[key].Where(s => s.Status == ResultStatusType.Failed)
                                                        .OrderByDescending(s => s.StartDate)
                                                        .Take(50);
                var reportItems = events.Select(e => new TestExceptionReportItem() {TestName = key 
                                                                                    , StartDate = e.StartDate 
                                                                                    , ResponseCode = e.StatusCode == 0 ? "" : e.StatusCode.ToString()                                                                                     
                                                                                    , Message = e.Exception.Message
                                                                                    , Duration = e.Duration.Value});
                resultsList.AddRange(reportItems);
            }

            var report = new TestExceptionReport() ;
            report.Exceptions = resultsList.OrderByDescending(s => s.StartDate).Take(50);
            return JsonConvert.SerializeObject(report);
        }
        private string GetDurationJson()
        {
            var resultsList = new List<TestDurationReport>();
            foreach (var key in _statusService.Results.Keys)
            {
                var results = new TestDurationReport();
                results.Name = key;

                var events = _statusService.Results[key].Where(r => r.Duration.HasValue && r.StartDate > DateTime.Now.AddHours(-1)).ToList();;

                var rows = new List<TestDurationReportItem>();
                foreach (var group in events.GroupBy(g => g.StartDate.ToString("HH:mm")))
                {
                    
                    var item = new TestDurationReportItem()
                    {
                        AverageDuration =  group.Average(g => g.Duration.Value),
                        SuccessfulRequests = group.Where( g => g.Status == ResultStatusType.Success).Count(),
                        FailedRequests = group.Where( g => g.Status == ResultStatusType.Failed).Count(),
                        EventTime = group.Key
                    };

                    rows.Add(item);
                    
                }
                results.Items = rows;
                if (rows.Count > 0 && rows.Count < 60)
                {
                    var minDate = events.Min(e => e.StartDate);
                    var paddedRows = new List<TestDurationReportItem>();
                    for (int i = 60 - rows.Count; i >  0; i--)
                    {
                        paddedRows.Add(new TestDurationReportItem() { EventTime = minDate.AddMinutes(-1 - i).ToString("HH:mm") });
                    }
                    paddedRows.AddRange(rows.OrderBy(r => r.EventTime));
                    
                    results.Items = paddedRows;
                }

                resultsList.Add(results);
            }

            return JsonConvert.SerializeObject(resultsList);
        }

        private string GetStatusJson()
        {
            var resultsList = new List<TestReport>();
            foreach (var key in _statusService.Results.Keys)
            {
                int totalCount = 0;
                var results = new TestReport();
                results.Name = key;
                if (!_totalCount.ContainsKey(key))
                    _totalCount.Add(key, 0);

                var rows = new List<TestReportRow>();
                foreach (var group in _statusService.Results[key].GroupBy(g => g.Status))
                {
                    var count = group.Count();
                    var avg = group.Average(g => g.Duration) ?? 0;
                    Console.WriteLine($"{group.Key} - {count} items averaging {avg}ms per test");
                    rows.Add(new TestReportRow() { AverageDuration = (int)avg, Count = count, Status = group.Key.ToString() });

                    if (group.Key == ResultStatusType.Failed || group.Key == ResultStatusType.Success)
                        totalCount += count;
                }
                results.Rows = rows;
                results.ProcessedInLastMinute = totalCount - _totalCount[key];
                _totalCount[key] = totalCount;

                resultsList.Add(results);
            }

            return JsonConvert.SerializeObject(resultsList);
        }
    }
}
