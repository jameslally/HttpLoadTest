using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HttpLoadTester.Services
{
    public class CustomHttpClientHandler : DelegatingHandler
    {
        public CustomHttpClientHandler(HttpMessageHandler innerHandler) : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                else
                {
                    throw new SimpleHttpResponseException(response.StatusCode, $"Exception calling url '{request.RequestUri}'");
                }
            }
            catch (TaskCanceledException ex)
            {
                throw new TimeoutException($"Timeout calling url '{request.RequestUri}'",ex);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
