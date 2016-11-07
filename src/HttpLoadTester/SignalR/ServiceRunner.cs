using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Threading;

namespace HttpLoadTester.SignalR
{
    public class ServiceRunner
    {
        private readonly IHubContext _hubContext;
        private readonly ServiceActions _statusService;
        private readonly string _sourceUrl;

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

                //if (!StatusUpdatePerformed(json))
                //{
                    _hubContext.Clients.All.displayFromHub(json).Wait();
                //}

                Thread.Sleep(1000);
            }
        }

        private bool StatusUpdatePerformed(string json)
        {
            //dynamic stuff = JsonConvert.DeserializeObject(json);
            //foreach (var org in stuff)
            //{
            //    string code = org.Code;
            //    int batch = org.PatientCount;
            //    int processed = org.Processed;
            //    int unprocessed = org.Unprocessed;
            //    string state = org.State;

            //    if (!string.IsNullOrWhiteSpace(state) && state.StartsWith("running", StringComparison.OrdinalIgnoreCase)
            //        && batch == processed && batch > 0 && unprocessed == 0)
            //    {
            //        _statusService.UpdateStatus(code, "AwaitingPostProcess");
            //        return true;
            //    }
            //}

            //return false;

            return true;
        }

        private string GetStatusJson()
        {
            return "{test:bcd}";// JsonConvert.SerializeObject(_statusService.Status());
        }
    }
}
