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

    public class PFMAddNotes : BaseQITest, ITest
    {

        public string Name { get { return "PFMUpdatingPatient"; } }

        public string DisplayText { get { return "Simulating Users Updating Patient in PFM"; } }

        public async override Task RunInnerTest(TestResult result, HttpClient client)
        {
            await client.GetStringAsync("http://192.168.1.12:816/UserInput/UserInput.aspx?ownerTableId=76&episodeList=76,61&module=INPATIENT");
            await client.PostAsync("http://192.168.1.12:816/UserInput/UserInputService.aspx/GetTabs", null);
            await client.PostAsync("http://192.168.1.12:816/UserInput/UserInputService.aspx/GetPatient", null);
            await client.PostAsync("http://192.168.1.12:816/UserInput/UserInputService.aspx/GetTabControls", null);
            await client.PostAsync("http://192.168.1.12:816/UserInput/UserInputService.aspx/ShowEpisodeImportButton", null);
            await client.PostAsync("http://192.168.1.12:816/UserInput/UserInputService.aspx/SavePatient", null);
        }
    }
}