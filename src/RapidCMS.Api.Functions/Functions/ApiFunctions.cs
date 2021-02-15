using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Pipeline;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RapidCMS.Api.Core.Abstractions;
using RapidCMS.Api.Core.Models;
using RapidCMS.Api.Functions.Abstractions;
using RapidCMS.Api.Functions.Models;

namespace RapidCMS.Api.Functions.Functions
{
    public class ApiFunctions
    {
        private readonly IApiHandlerResolver _apiHandlerResolver;
        private readonly IFunctionExecutionContextAccessor _functionExecutionContextAccessor;
        private readonly ILogger<ApiFunctions> _logger;

        public ApiFunctions(
            IApiHandlerResolver apiHandlerResolver,
            IFunctionExecutionContextAccessor functionExecutionContextAccessor,
            ILogger<ApiFunctions> logger)
        {
            _apiHandlerResolver = apiHandlerResolver;
            _functionExecutionContextAccessor = functionExecutionContextAccessor;
            _logger = logger;
        }

        [FunctionName(nameof(GetByIdAsync))]
        public async Task<HttpResponseData> GetByIdAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/entity/{id}")] HttpRequestData req, FunctionExecutionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

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
        public async Task<HttpResponseData> GetAllAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/all")] HttpRequestData req, FunctionExecutionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

            if(req.Params.TryGetValue("repositoryAlias", out var repositoryAlias))
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
        public async Task<HttpResponseData> GetAllRelatedAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/all/related")] HttpRequestData req, FunctionExecutionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

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
        public async Task<HttpResponseData> GetAllNonRelatedAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/all/nonrelated")] HttpRequestData req, FunctionExecutionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

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
        public async Task<HttpResponseData> NewAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/new")] HttpRequestData req, FunctionExecutionContext context)
        {
            _logger.LogWarning("Got into function");
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

            _logger.LogWarning("Context set");
            if (req.Params.TryGetValue("repositoryAlias", out var repositoryAlias))
            {
                _logger.LogWarning("Repo alias is: " + repositoryAlias);

                var handler = _apiHandlerResolver.GetApiHandler(repositoryAlias);

                _logger.LogWarning("Handler found");

                _logger.LogWarning("Body is");

                _logger.LogWarning(req.Body);

                var json = JsonConvert.DeserializeObject<JsonRequestWrapper>(req.Body).Json;

                _logger.LogWarning("Json in body is");

                _logger.LogWarning(json);

                var response = await handler.NewAsync(new ApiRequestModel { Body = json });

                _logger.LogWarning("Response received");
                return new HttpResponseData(response.StatusCode, response.ResponseBody);
            }
            else
            {
                return new HttpResponseData(HttpStatusCode.BadRequest);
            }
        }

        [FunctionName(nameof(InsertAsync))]
        public async Task<HttpResponseData> InsertAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/entity")] HttpRequestData req, FunctionExecutionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

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
        public async Task<HttpResponseData> UpdateAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "_rapidcms/{repositoryAlias}/entity/{id}")] HttpRequestData req, FunctionExecutionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

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
        public async Task<HttpResponseData> DeleteAsync([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "_rapidcms/{repositoryAlias}/entity/{id}")] HttpRequestData req, FunctionExecutionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

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
        public async Task<HttpResponseData> AddRelationAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/relate")] HttpRequestData req, FunctionExecutionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

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
        public async Task<HttpResponseData> RemoveRelationAsync([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "_rapidcms/{repositoryAlias}/relate")] HttpRequestData req, FunctionExecutionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

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
        public async Task<HttpResponseData> ReorderAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/reorder")] HttpRequestData req, FunctionExecutionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

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
