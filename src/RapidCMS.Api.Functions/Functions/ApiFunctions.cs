using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using RapidCMS.Api.Core.Abstractions;
using RapidCMS.Api.Core.Models;
using RapidCMS.Api.Functions.Abstractions;
using RapidCMS.Api.Functions.Extensions;

namespace RapidCMS.Api.Functions.Functions
{
    public class ApiFunctions
    {
        private readonly IApiHandlerResolver _apiHandlerResolver;
        private readonly IFunctionContextAccessor _functionExecutionContextAccessor;

        public ApiFunctions(
            IApiHandlerResolver apiHandlerResolver,
            IFunctionContextAccessor functionExecutionContextAccessor)
        {
            _apiHandlerResolver = apiHandlerResolver;
            _functionExecutionContextAccessor = functionExecutionContextAccessor;
        }

        [Function(nameof(GetByIdAsync))]
        public async Task<HttpResponseData> GetByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/entity/{id}")] HttpRequestData req,
            string repositoryAlias,
            string id,
            FunctionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

            var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).GetByIdAsync(new ApiRequestModel { Id = id, Body = await req.ReadAsStringAsync() });
            return req.CreateResponse(response);
        }

        [Function(nameof(GetAllAsync))]
        public async Task<HttpResponseData> GetAllAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/all")] HttpRequestData req,
            string repositoryAlias,
             FunctionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

            var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).GetAllAsync(new ApiRequestModel { Body = await req.ReadAsStringAsync() });
            return req.CreateResponse(response);
        }

        [Function(nameof(GetAllRelatedAsync))]
        public async Task<HttpResponseData> GetAllRelatedAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/all/related")] HttpRequestData req,
            string repositoryAlias,
             FunctionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

            var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).GetAllRelatedAsync(new ApiRequestModel { Body = await req.ReadAsStringAsync() });
            return req.CreateResponse(response);
        }

        [Function(nameof(GetAllNonRelatedAsync))]
        public async Task<HttpResponseData> GetAllNonRelatedAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/all/non-related")] HttpRequestData req,
            string repositoryAlias,
            FunctionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

            var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).GetAllNonRelatedAsync(new ApiRequestModel { Body = await req.ReadAsStringAsync() });
            return req.CreateResponse(response);
        }

        [Function(nameof(NewAsync))]
        public async Task<HttpResponseData> NewAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/new")] HttpRequestData req,
            string repositoryAlias,
            FunctionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

            var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).NewAsync(new ApiRequestModel { Body = await req.ReadAsStringAsync() });
            return req.CreateResponse(response);
        }

        [Function(nameof(InsertAsync))]
        public async Task<HttpResponseData> InsertAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/entity")] HttpRequestData req,
            string repositoryAlias,
            FunctionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

            var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).InsertAsync(new ApiRequestModel { Body = await req.ReadAsStringAsync() });
            return req.CreateResponse(response);
        }

        [Function(nameof(UpdateAsync))]
        public async Task<HttpResponseData> UpdateAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "_rapidcms/{repositoryAlias}/entity/{id}")] HttpRequestData req,
            string repositoryAlias,
            string id,
            FunctionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

            var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).UpdateAsync(new ApiRequestModel { Id = id, Body = await req.ReadAsStringAsync() });
            return req.CreateResponse(response);
        }

        [Function(nameof(DeleteAsync))]
        public async Task<HttpResponseData> DeleteAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "_rapidcms/{repositoryAlias}/entity/{id}")] HttpRequestData req,
            string repositoryAlias,
            string id,
            FunctionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

            var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).DeleteAsync(new ApiRequestModel { Id = id, Body = await req.ReadAsStringAsync() });
            return req.CreateResponse(response);
        }

        [Function(nameof(AddRelationAsync))]
        public async Task<HttpResponseData> AddRelationAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/relate")] HttpRequestData req,
            string repositoryAlias,
            FunctionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

            var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).AddRelationAsync(new ApiRequestModel { Body = await req.ReadAsStringAsync() });
            return req.CreateResponse(response);
        }

        [Function(nameof(RemoveRelationAsync))]
        public async Task<HttpResponseData> RemoveRelationAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "_rapidcms/{repositoryAlias}/relate")] HttpRequestData req,
            string repositoryAlias,
            FunctionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

            var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).RemoveRelationAsync(new ApiRequestModel { Body = await req.ReadAsStringAsync() });
            return req.CreateResponse(response);
        }

        [Function(nameof(ReorderAsync))]
        public async Task<HttpResponseData> ReorderAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/reorder")] HttpRequestData req,
            string repositoryAlias,
            FunctionContext context)
        {
            _functionExecutionContextAccessor.FunctionExecutionContext = context;

            var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).ReorderAsync(new ApiRequestModel { Body = await req.ReadAsStringAsync() });
            return req.CreateResponse(response);
        }
    }
}
