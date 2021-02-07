using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Pipeline;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using RapidCMS.Api.Core.Abstractions;
using RapidCMS.Api.Core.Models;
using RapidCMS.Api.Functions.Models;

namespace RapidCMS.Api.Functions.Functions
{
    public class ApiFunctions
    {
        private readonly IApiHandlerResolver _apiHandlerResolver;

        public ApiFunctions(IApiHandlerResolver apiHandlerResolver)
        {
            _apiHandlerResolver = apiHandlerResolver;
        }

        [FunctionName(nameof(GetByIdAsync))]
        public async Task<HttpResponseData> GetByIdAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/entity/{id}")] HttpRequestData req, FunctionExecutionContext context)
        {
            if (req.Params.TryGetValue("repositoryAlias", out var repositoryAlias) && req.Params.TryGetValue("id", out var id))
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).GetByIdAsync(new ApiRequestModel { Id = id, Body = JsonConvert.DeserializeObject< JsonRequestWrapper>(req.Body).Json });
                return new HttpResponseData(response.StatusCode, response.ResponseBody);
            }
            else
            {
                return new HttpResponseData(HttpStatusCode.BadRequest);
            }
        }

        [FunctionName(nameof(GetAllAsync))]
        public async Task<HttpResponseData> GetAllAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/all")] HttpRequestData req)
        {
            if (req.Params.TryGetValue("repositoryAlias", out var repositoryAlias))
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).GetAllAsync(new ApiRequestModel { Body = JsonConvert.DeserializeObject<JsonRequestWrapper>(req.Body).Json });
                return new HttpResponseData(response.StatusCode, response.ResponseBody);
            }
            else
            {
                return new HttpResponseData(HttpStatusCode.BadRequest);
            }
        }

        [FunctionName(nameof(GetAllRelatedAsync))]
        public async Task<HttpResponseData> GetAllRelatedAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/all/related")] HttpRequestData req)
        {
            if (req.Params.TryGetValue("repositoryAlias", out var repositoryAlias))
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).GetAllRelatedAsync(new ApiRequestModel { Body = JsonConvert.DeserializeObject<JsonRequestWrapper>(req.Body).Json });
                return new HttpResponseData(response.StatusCode, response.ResponseBody);
            }
            else
            {
                return new HttpResponseData(HttpStatusCode.BadRequest);
            }
        }

        [FunctionName(nameof(GetAllNonRelatedAsync))]
        public async Task<HttpResponseData> GetAllNonRelatedAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/all/nonrelated")] HttpRequestData req)
        {
            if (req.Params.TryGetValue("repositoryAlias", out var repositoryAlias))
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).GetAllNonRelatedAsync(new ApiRequestModel { Body = JsonConvert.DeserializeObject<JsonRequestWrapper>(req.Body).Json });
                return new HttpResponseData(response.StatusCode, response.ResponseBody);
            }
            else
            {
                return new HttpResponseData(HttpStatusCode.BadRequest);
            }
        }

        [FunctionName(nameof(NewAsync))]
        public async Task<HttpResponseData> NewAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/new")] HttpRequestData req)
        {
            if (req.Params.TryGetValue("repositoryAlias", out var repositoryAlias))
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).NewAsync(new ApiRequestModel { Body = JsonConvert.DeserializeObject<JsonRequestWrapper>(req.Body).Json });
                return new HttpResponseData(response.StatusCode, response.ResponseBody);
            }
            else
            {
                return new HttpResponseData(HttpStatusCode.BadRequest);
            }
        }

        [FunctionName(nameof(InsertAsync))]
        public async Task<HttpResponseData> InsertAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/entity")] HttpRequestData req)
        {
            if (req.Params.TryGetValue("repositoryAlias", out var repositoryAlias))
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).InsertAsync(new ApiRequestModel { Body = JsonConvert.DeserializeObject<JsonRequestWrapper>(req.Body).Json });
                return new HttpResponseData(response.StatusCode, response.ResponseBody);
            }
            else
            {
                return new HttpResponseData(HttpStatusCode.BadRequest);
            }
        }

        [FunctionName(nameof(UpdateAsync))]
        public async Task<HttpResponseData> UpdateAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "_rapidcms/{repositoryAlias}/entity/{id}")] HttpRequestData req)
        {
            if (req.Params.TryGetValue("repositoryAlias", out var repositoryAlias) && req.Params.TryGetValue("id", out var id))
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).UpdateAsync(new ApiRequestModel { Id = id, Body = JsonConvert.DeserializeObject<JsonRequestWrapper>(req.Body).Json });
                return new HttpResponseData(response.StatusCode, response.ResponseBody);
            }
            else
            {
                return new HttpResponseData(HttpStatusCode.BadRequest);
            }
        }

        [FunctionName(nameof(DeleteAsync))]
        public async Task<HttpResponseData> DeleteAsync([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "_rapidcms/{repositoryAlias}/entity/{id}")] HttpRequestData req)
        {
            if (req.Params.TryGetValue("repositoryAlias", out var repositoryAlias) && req.Params.TryGetValue("id", out var id))
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).DeleteAsync(new ApiRequestModel { Id = id, Body = JsonConvert.DeserializeObject<JsonRequestWrapper>(req.Body).Json });
                return new HttpResponseData(response.StatusCode, response.ResponseBody);
            }
            else
            {
                return new HttpResponseData(HttpStatusCode.BadRequest);
            }
        }

        [FunctionName(nameof(AddRelationAsync))]
        public async Task<HttpResponseData> AddRelationAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/relate")] HttpRequestData req)
        {
            if (req.Params.TryGetValue("repositoryAlias", out var repositoryAlias))
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).AddRelationAsync(new ApiRequestModel { Body = JsonConvert.DeserializeObject<JsonRequestWrapper>(req.Body).Json });
                return new HttpResponseData(response.StatusCode, response.ResponseBody);
            }
            else
            {
                return new HttpResponseData(HttpStatusCode.BadRequest);
            }
        }

        [FunctionName(nameof(RemoveRelationAsync))]
        public async Task<HttpResponseData> RemoveRelationAsync([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "_rapidcms/{repositoryAlias}/relate")] HttpRequestData req)
        {
            if (req.Params.TryGetValue("repositoryAlias", out var repositoryAlias))
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).RemoveRelationAsync(new ApiRequestModel { Body = JsonConvert.DeserializeObject<JsonRequestWrapper>(req.Body).Json });
                return new HttpResponseData(response.StatusCode, response.ResponseBody);
            }
            else
            {
                return new HttpResponseData(HttpStatusCode.BadRequest);
            }
        }

        [FunctionName(nameof(ReorderAsync))]
        public async Task<HttpResponseData> ReorderAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/reorder")] HttpRequestData req)
        {
            if (req.Params.TryGetValue("repositoryAlias", out var repositoryAlias))
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).ReorderAsync(new ApiRequestModel { Body = JsonConvert.DeserializeObject<JsonRequestWrapper>(req.Body).Json });
                return new HttpResponseData(response.StatusCode, response.ResponseBody);
            }
            else
            {
                return new HttpResponseData(HttpStatusCode.BadRequest);
            }
        }
    }
}
