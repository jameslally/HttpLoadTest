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
