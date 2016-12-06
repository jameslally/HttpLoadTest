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

    public class PFMAddNotes : BaseQITest, ITest
    {
        public PFMAddNotes(TestConfiguration config) : base(config)
        {
        }

        public string Name { get { return "PFMUpdatingPatient"; } }

        public string DisplayText { get { return "Simulating Users Updating Patient in PFM"; } }

        public async override Task RunInnerTest(TestResult result, HttpClient client)
        {
            await client.GetStringAsync($"{_baseUrl}UserInput/UserInput.aspx?ownerTableId=76&episodeList=76,61&module=INPATIENT");
            await GetTabs(result, client);
            await GetTabsControls(result, client);
            await ShowEpisodeImportButton(result, client);
            var patient = await GetPatient(result, client);
            await SavePatient(result, client, patient);
        }

        private void updatePatient(JsonPatientWrapper patient)
        {
            var random = new Random();
            var ctrlKeys = patient.d.Controls
                                .Where(c => c.ControlType == "TextBox" && !c.IsIntegrated);

            if (ctrlKeys.Count() > 0)
            {
                foreach (var key in ctrlKeys.OrderBy(o => random.Next(10000)).Take(5))
                {
                    key.IsValueChanged = true;
                }

            }

        }
        private async Task SavePatient(TestResult result, HttpClient client, JsonPatientWrapper patient)
        {
            updatePatient(patient);
            var changedJson = JsonConvert.SerializeObject(patient.d).TrimEnd('}').TrimStart('{');
            var requestContent = getContent("{'patient': {'__type': 'PFM.net.Model.JsonPatient'," + changedJson + "}, saveType:'FULL' , category: 'INPATIENT' ,hospital:'eric' , homerUsername:'eric' , homerPassword:'ericPwd' , hospital:'hopsitalA'}");
            var response = await client.PostAsync($"{_baseUrl}UserInput/UserInputService.aspx/SavePatient", requestContent);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
        }

        private async Task<JsonPatientWrapper> GetPatient(TestResult result, HttpClient client)
        {
            var nextEpisodeId = getNextEpisodeId();
            var requestContent = getContent("{episodeId : '" + nextEpisodeId + "', tableName:'INPATIENT'}");

            var response = await client.PostAsync($"{_baseUrl}UserInput/UserInputService.aspx/GetPatient", requestContent);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<JsonPatientWrapper>(content);
        }

        private int getNextEpisodeId()
        {
            var r = new Random();
            var pos = _random.Next(0, _config.EpisodeIDs.Count() - 1);
            return _config.EpisodeIDs[pos];
        }
        private async Task ShowEpisodeImportButton(TestResult result, HttpClient client)
        {
            var requestContent = getContent("{episodeId : '401', tableName:'INPATIENT' , isMobile : 'false'}");

            var response = await client.PostAsync($"{_baseUrl}UserInput/UserInputService.aspx/ShowEpisodeImportButton", requestContent);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
        }

        private async Task GetTabsControls(TestResult result, HttpClient client)
        {
            var requestContent = getContent("{tabId : '1', isMobile : 'false'}");

            var response = await client.PostAsync($"{_baseUrl}UserInput/UserInputService.aspx/GetTabControls", requestContent);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
        }

        private async Task GetTabs(TestResult result, HttpClient client)
        {
            var requestContent = getContent("{pageName : 'INPATIENT', isMobile : 'false'}");

            var response = await client.PostAsync($"{_baseUrl}UserInput/UserInputService.aspx/GetTabs", requestContent);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
        }



        private StringContent getContent(string json)
        {
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}