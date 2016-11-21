//using Newtonsoft.Json;
using HttpLoadTester.Entites.Test;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HttpLoadTester.Services.Scenarios
{

    public class PFMViewingDashboard : BaseQITest, ITest
    {
        
        private readonly string _baseUrl;
        public PFMViewingDashboard(string baseUrl = "http://192.168.1.12:816/" ) : base(baseUrl)
        {
            _baseUrl = baseUrl;
        }
        private readonly string[] testPages = new[] {
                               "api/SummaryCount"
                               ,"api/MOTD"
                               ,"api/Grid"
                               ,"api/GridColumn?gridCategory=INPATIENT"
                               ,"api/Ping" };
        public string Name { get { return "PFMViewingDashboard"; } }

        public string DisplayText { get { return "Simulating Users Viewing PFM Dashboard"; } }

        public async override Task RunInnerTest(TestResult result, HttpClient client)
        {
            foreach (var page in testPages)
            {
                var response = await client.GetAsync(_baseUrl + page);
                response.EnsureSuccessStatusCode();
                var s = await response.Content.ReadAsStringAsync();
            }
        }

        

        
    }
}