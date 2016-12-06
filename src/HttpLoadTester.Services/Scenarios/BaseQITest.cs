//using Newtonsoft.Json;
using HttpLoadTester.Entites.Test;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace HttpLoadTester.Services.Scenarios
{

    public abstract class BaseQITest
    {
        protected readonly string _baseUrl;
        protected readonly TestConfiguration _config;
        protected readonly Random _random;

        public BaseQITest(TestConfiguration config)
        {
            _config = config;
            _userCookies = new Dictionary<string, CookieContainer>();
            _baseUrl = config.BaseUrl;
            _random = new Random();
        }

        private Dictionary<string, CookieContainer> _userCookies;
        private readonly string[] initalCookiePages = new[] {
                                "Config/UserConfig.aspx"
                               ,"api/UserConfig"
                               ,"api/Settings?option=splash"
                               ,"api/User"
                                };

        public abstract Task RunInnerTest(TestResult result, HttpClient client);

        public async Task Run(TestResult result)
        {
            if (_config.UserWaitSeconds > 0)
                Thread.Sleep(_random.Next(1000, 1000 * _config.UserWaitSeconds));

            result.Status = ResultStatusType.Running;
            result.StartDate = DateTime.Now;
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
            catch (SimpleHttpResponseException ex)
            {
                Console.WriteLine(ex.Message);
                result.Exception = ex;
                result.Status = ResultStatusType.Failed;
                result.StatusCode = (int)ex.StatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result.Exception = ex;
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

            var httpClient = new HttpClient(new CustomHttpClientHandler(httpClientHandler), true);
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