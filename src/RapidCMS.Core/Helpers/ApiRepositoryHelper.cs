using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RapidCMS.Core.Exceptions;

namespace RapidCMS.Core.Helpers
{
    internal class ApiRepositoryHelper
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly string _repositoryAlias;

        public ApiRepositoryHelper(
            IHttpClientFactory httpClientFactory,
            JsonSerializerSettings jsonSerializerSettings,
            string repositoryAlias)
        {
            _httpClientFactory = httpClientFactory;
            _jsonSerializerSettings = jsonSerializerSettings;
            _repositoryAlias = repositoryAlias;
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string url)
        {
            return new HttpRequestMessage(method, url);
        }

        internal HttpRequestMessage CreateRequest<T>(HttpMethod method, string url, T content)
        {
            if (method == HttpMethod.Get)
            {
                throw new InvalidOperationException();
            }

            var request = CreateRequest(method, url);
            var json = JsonConvert.SerializeObject(content, _jsonSerializerSettings);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return request;
        }

        internal async Task<HttpResponseMessage> DoRequestAsync(HttpRequestMessage request)
        {
            var httpClient = _httpClientFactory.CreateClient(_repositoryAlias);
            if (httpClient.BaseAddress == default)
            {
                throw new InvalidOperationException($"Please configure an HttpClient for the repository '{_repositoryAlias}' using " +
                    $".{nameof(RapidCMSMiddleware.AddRapidCMSApiRepository)}([..]) and configure its BaseAddress correctly.");
            }

            var response = await httpClient.SendAsync(request);

            return response.StatusCode switch
            {
                HttpStatusCode.OK => response,
                HttpStatusCode.Unauthorized => throw new UnauthorizedAccessException(),
                HttpStatusCode.Forbidden => throw new UnauthorizedAccessException(),
                HttpStatusCode.NotFound => throw new NotFoundException($"{request.RequestUri} not found."),

                _ => throw new InvalidOperationException()
            };
        }

        internal async Task<TResult?> DoRequestAsync<TResult>(HttpRequestMessage request)
            where TResult : class
        {
            try
            {
                var response = await DoRequestAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TResult>(json, _jsonSerializerSettings);

                return result;
            }
            catch (NotFoundException)
            {
                return default;
            }
        }
    }
}
