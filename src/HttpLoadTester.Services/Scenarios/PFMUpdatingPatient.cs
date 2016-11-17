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

    public class PFMUpdatingPatient : BaseQITest, ITest
    {
        private readonly string[] testPages = new[] {
                               "http://192.168.1.12:816/api/SummaryCount"
                               ,"http://192.168.1.12:816/api/MOTD"
                               ,"http://192.168.1.12:816/api/Grid"
                               ,"http://192.168.1.12:816/api/GridColumn?gridCategory=INPATIENT"
                               ,"http://192.168.1.12:816/api/Ping" };
        public string Name { get { return "PFMUpdatingPatient"; } }

        public string DisplayText { get { return "Simulating Users Updating Patient in PFM"; } }

        public async override Task RunInnerTest(TestResult result, HttpClient client)
        {
            foreach (var page in testPages)
            {
                var response = await client.GetAsync(page);
                response.EnsureSuccessStatusCode();
                var s = await response.Content.ReadAsStringAsync();
            }
        }

        

        
    }
}