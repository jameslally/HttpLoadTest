﻿//using Newtonsoft.Json;
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

    public class PFMUserTest : ITest
    {
        public PFMUserTest ()
        {
            _userCookies = new Dictionary<string, CookieContainer>();
        }
        private Dictionary<string, CookieContainer> _userCookies;

        public bool ResponsibleFor(string name)
        {
            return "PFMUser".Equals(name, StringComparison.OrdinalIgnoreCase);
        }

        private readonly string[] initalCookiePages = new[] {
                                "http://192.168.1.12:816/Config/UserConfig.aspx"
                               ,"http://192.168.1.12:816/api/UserConfig"
                               ,"http://192.168.1.12:816/api/Settings?option=splash"
                               ,"http://192.168.1.12:816/api/User"
                                };

        private readonly string[] testPages = new[] {
                               "http://192.168.1.12:816/api/SummaryCount"
                               ,"http://192.168.1.12:816/api/MOTD"
                               ,"http://192.168.1.12:816/api/Grid"
                               ,"http://192.168.1.12:816/api/GridColumn?gridCategory=INPATIENT"
                               ,"http://192.168.1.12:816/api/Ping" };
        
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
                    foreach (var page in testPages)
                    {
                        if (cookies.Count > 0)
                        {
                            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Persistent - Auth", "true");
                            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Expires", "-1");
                        }

                        var response = await httpClient.GetAsync(page);
                        response.EnsureSuccessStatusCode();
                        var s = await response.Content.ReadAsStringAsync();
                    }
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
                                var response = httpClient.GetAsync(page).Result;
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