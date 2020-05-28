using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Blazor.FileReader;
using Newtonsoft.Json;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Controllers;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Response;

namespace RapidCMS.Core.Handlers
{
    public static class ApiFileUploadHandler
    {
        public static string GetFileUploaderAlias(Type handlerType)
        {
            var type = (handlerType.IsGenericType && handlerType.GetGenericTypeDefinition().In(typeof(ApiFileUploadHandler<>), typeof(ApiFileUploadController<>)))
                ? handlerType.GetGenericArguments().FirstOrDefault()
                : handlerType;

            return type?.Name.ToUrlFriendlyString() ?? "unknown-file-handler";
        }
    }

    public class ApiFileUploadHandler<THandler> : IFileUploadHandler
        where THandler : IFileUploadHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _handlerAlias;

        public ApiFileUploadHandler(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _handlerAlias = ApiFileUploadHandler.GetFileUploaderAlias(typeof(THandler));
        }

        public async Task<object> SaveFileAsync(IFileInfo fileInfo, Stream stream)
        {
            var response = await DoRequestAsync<FileUploadResponseModel>(CreateRequest("file", fileInfo, stream));
            return response?.Result ?? new object();
        }

        public async Task<IEnumerable<string>> ValidateFileAsync(IFileInfo fileInfo)
        {
            var response = await DoRequestAsync<FileUploadValidationResponseModel>(CreateRequest("file/validate", fileInfo));
            return response?.ErrorMessages ?? Enumerable.Empty<string>();
        }

        private HttpRequestMessage CreateRequest(string url, IFileInfo fileInfo, Stream? stream = default)
        {
            var content = new MultipartFormDataContent
            {
                { new StringContent(fileInfo.LastModified?.ToString()), nameof(IFileInfo.LastModified) },
                { new StringContent(fileInfo.Name?.ToString()), nameof(IFileInfo.Name) },
                { new StringContent(fileInfo.Size.ToString()), nameof(IFileInfo.Size) },
                { new StringContent(fileInfo.Type?.ToString()), nameof(IFileInfo.Type) }
            };

            if (stream != null)
            {
                content.Add(new StreamContent(stream), "file", fileInfo.Name);
            }

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            return request;
        }

        private async Task<HttpResponseMessage> DoRequestAsync(HttpRequestMessage request)
        {
            var httpClient = _httpClientFactory.CreateClient(_handlerAlias);
            if (httpClient.BaseAddress == default)
            {
                throw new InvalidOperationException($"Please configure an HttpClient for the file handler '{_handlerAlias}' using " +
                    $".AddRapidCMSFileUploadApiHttpClient<THandler>([..]) and configure its BaseAddress correctly.");
            }

            var response = await httpClient.SendAsync(request);
            return response.StatusCode switch
            {
                HttpStatusCode.OK => response,
                HttpStatusCode.NoContent => response,
                HttpStatusCode.Unauthorized => throw new UnauthorizedAccessException(),
                HttpStatusCode.Forbidden => throw new UnauthorizedAccessException(),

                _ => throw new InvalidOperationException("Please configure the Api correctly.")
            };
        }

        private async Task<TResult?> DoRequestAsync<TResult>(HttpRequestMessage request)
            where TResult : class
        {
            var response = await DoRequestAsync(request);
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return default;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResult>(json);
        }
    }
}
