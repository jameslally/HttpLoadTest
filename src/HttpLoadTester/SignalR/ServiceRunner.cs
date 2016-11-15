using HttpLoadTester.DTOs;
using HttpLoadTester.Entites.Test;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
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
        private int _totalCount = 0;
        

        public ServiceRunner(IConnectionManager connectionManager, IConfiguration configuration, ServiceActions statusService)
        {
            _hubContext = connectionManager.GetHubContext<DashboardHub>();
            _statusService = statusService;
            _sourceUrl = configuration["HttpSourceLocation"];
            
        }

        public bool Active { get; set; }

        public void DoWork()
        {
            Active = true;

            while (Active)
            {
                var json = GetStatusJson();
                _hubContext.Clients.All.displayFromHub(json).Wait();

                Thread.Sleep(1000);
            }
        }

   

        private string GetStatusJson()
        {
            foreach (var result in _statusService.Results.Values)
            {
                int totalCount = 0;
                var results = new TestReport();
                var rows = new List<TestReportRow>();
                foreach (var group in result.GroupBy(g => g.Status))
                {
                    var count = group.Count();
                    var avg = group.Average(g => g.Duration) ?? 0;
                    Console.WriteLine($"{group.Key} - {count} items averaging {avg}ms per test");
                    rows.Add(new TestReportRow() { AverageDuration = (int)avg, Count = count, Status = group.Key.ToString() });

                    if (group.Key == ResultStatusType.Failed || group.Key == ResultStatusType.Success)
                        totalCount += count;
                }
                results.Rows = rows;
                
                results.ProcessedInLastMinute = totalCount - _totalCount;
                _totalCount = totalCount;

                return JsonConvert.SerializeObject(results);
            }
            return "";
            //var results = new TestReport();
            //var rows = new List<TestReportRow>();
            //rows.Add(new TestReportRow() { AverageDuration = 12, Count = 55, Status = "Successful" });
            //rows.Add(new TestReportRow() { AverageDuration = 33, Count = 8, Status = "Failed" });
            //rows.Add(new TestReportRow() { AverageDuration = 0, Count = 3, Status = "InProgress" });
            //results.Rows = rows;
            //return JsonConvert.SerializeObject(results);
        }
    }
}
