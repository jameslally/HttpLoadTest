//using Newtonsoft.Json;
using HttpLoadTester.Entites.Test;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using System;
using System.Collections.Generic;
using PFM.net.Model.JsonPatient;

namespace HttpLoadTester.Services.Scenarios
{

    public class PFMPings : BaseQITest, ITest
    {
        public PFMPings(TestConfiguration config) : base(config)
        {
        }

        public string Name { get { return "PFMJustPings"; } }

        public string DisplayText { get { return "Simulating Users Pings in PFM"; } }

        private readonly string[] testPages = new[] {
                               "api/Ping" };

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