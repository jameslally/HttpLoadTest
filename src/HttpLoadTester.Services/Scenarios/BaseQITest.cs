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

    public abstract class BaseQITest 
    {
        private readonly string _baseUrl;

        public BaseQITest(string baseUrl)
        {
            _userCookies = new Dictionary<string, CookieContainer>();
            _baseUrl = baseUrl;
        }

        private Dictionary<string, CookieContainer> _userCookies;        
        private readonly string[] initalCookiePages = new[] {
                                "Config/UserConfig.aspx"
                               ,"api/UserConfig"
                               ,"api/Settings?option=splash"
                               ,"api/User"
                                };

        public abstract Task RunInnerTest(TestResult result , HttpClient client);

        public async Task Run(TestResult result)
        {
            result.Status = ResultStatusType.Running;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var cookies = GetUserCookies("hiq\\jamesl");

                Console.WriteLine($"test started {result.Id}");
                using (var httpClient = createClient(cookies))
                {
                    await RunInnerTest(result, httpClient);
                }
                Console.WriteLine($"test done {result.Id} - {sw.ElapsedMilliseconds}ms");
                result.Status = ResultStatusType.Success;
            }
            catch
            {
                Console.WriteLine($"test exception {result.Id} - {sw.ElapsedMilliseconds}ms");
                result.Status = ResultStatusType.Failed;
            }
            finally
            {
                result.Duration = sw.ElapsedMilliseconds;
            }
        }

        private HttpClient createClient(CookieContainer cookies)
        {
            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = cookies,
                UseDefaultCredentials = true
            };
            
            var httpClient = new HttpClient(httpClientHandler, true);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Persistent - Auth", "true");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Expires", "-1");

            return httpClient;
        }

        private CookieContainer GetUserCookies(string userName)
        {
            if (!_userCookies.ContainsKey(userName))
            {
                lock (_userCookies)
                {
                    if (!_userCookies.ContainsKey(userName))
                    {
                        Console.WriteLine($"loading cookies for {userName}");

                        var cookies = new CookieContainer();
                        using (var httpClient = createClient(cookies))
                        {
                            foreach (var page in initalCookiePages)
                            {
                                var response = httpClient.GetAsync(_baseUrl + page).Result;
                                response.EnsureSuccessStatusCode();
                            }
                            _userCookies.Add(userName, cookies);
                            Console.WriteLine($"got cookies for {userName}");

                        }
                    }
                }

            }
            return _userCookies[userName];
        }
    }
}