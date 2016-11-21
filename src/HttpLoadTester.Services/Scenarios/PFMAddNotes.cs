//using Newtonsoft.Json;
using HttpLoadTester.Entites.Test;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Dynamic;
using System.Text;

namespace HttpLoadTester.Services.Scenarios
{

    public class PFMAddNotes : BaseQITest, ITest
    {
        private readonly string _baseUrl;
        public PFMAddNotes(string baseUrl = "http://localhost:56999/" ) : base(baseUrl)
        {
            _baseUrl = baseUrl;
        }
        
        public string Name { get { return "PFMUpdatingPatient"; } }

        public string DisplayText { get { return "Simulating Users Updating Patient in PFM"; } }

        public async override Task RunInnerTest(TestResult result, HttpClient client)
        {
            await client.GetStringAsync($"{_baseUrl}UserInput/UserInput.aspx?ownerTableId=76&episodeList=76,61&module=INPATIENT");
            GetTabs(result,client);
            await client.PostAsync($"{_baseUrl}UserInput/UserInputService.aspx/GetPatient", null);
            await client.PostAsync($"{_baseUrl}UserInput/UserInputService.aspx/GetTabControls", null);
            await client.PostAsync($"{_baseUrl}UserInput/UserInputService.aspx/ShowEpisodeImportButton", null);
            await client.PostAsync($"{_baseUrl}UserInput/UserInputService.aspx/SavePatient", null);
        }

        private async void GetTabs(TestResult result, HttpClient client)
        {
            var requestContent = getContent("{pageName : 'INPATIENT', isMobile : 'false'}");

            var response = await client.PostAsync($"{_baseUrl}UserInput/UserInputService.aspx/GetTabs", requestContent);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
        }



        private StringContent getContent (string json)
        {
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}