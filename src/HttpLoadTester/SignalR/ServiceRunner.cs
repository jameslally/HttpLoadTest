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
            var resultsList = new List<TestReport>();
            foreach (var key in _statusService.Results.Keys)
            {
                int totalCount = 0;
                var results = new TestReport();
                results.Name = key;

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
                results.ProcessedInLastMinute = totalCount - _totalCount;
                _totalCount = totalCount;

                resultsList.Add(results);
            }

            return JsonConvert.SerializeObject(resultsList);
        }
    }
}
