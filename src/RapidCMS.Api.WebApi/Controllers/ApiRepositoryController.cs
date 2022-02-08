using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RapidCMS.Api.Core.Abstractions;
using RapidCMS.Api.Core.Models;
using RapidCMS.Api.WebApi.Extensions;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Models.ApiBridge;
using RapidCMS.Core.Models.ApiBridge.Response;

namespace RapidCMS.Api.WebApi.Controllers
{
    [ApiController]
    public class ApiRepositoryController : ControllerBase
    {
        private readonly IApiHandlerResolver _apiHandlerResolver;

        public ApiRepositoryController(IApiHandlerResolver apiHandlerResolver)
        {
            _apiHandlerResolver = apiHandlerResolver;
        }

        [HttpPost("_rapidcms/{repositoryAlias}/entity/{id}")]
        public async Task<ActionResult<EntityModel<IEntity>>> GetByIdAsync(string repositoryAlias, string id)
        {
            try
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).GetByIdAsync(new ApiRequestModel { Id = id, Body = await ReadBodyAsStringAsync() });
                return response.ToContentResult();
            }
            catch { }

            return BadRequest();
        }

        [HttpPost("_rapidcms/{repositoryAlias}/all")]
        public async Task<ActionResult<EntitiesModel<IEntity>>> GetAllAsync(string repositoryAlias)
        {
            try
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).GetAllAsync(new ApiRequestModel { Body = await ReadBodyAsStringAsync() });
                return response.ToContentResult();
            }
            catch { }

            return BadRequest();
        }

        [HttpPost("_rapidcms/{repositoryAlias}/all/related")]
        public async Task<ActionResult<EntitiesModel<IEntity>>> GetAllRelatedAsync(string repositoryAlias)
        {
            try
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).GetAllRelatedAsync(new ApiRequestModel { Body = await ReadBodyAsStringAsync() });
                return response.ToContentResult();
            }
            catch { }

            return BadRequest();
        }

        [HttpPost("_rapidcms/{repositoryAlias}/all/non-related")]
        public async Task<ActionResult<EntitiesModel<IEntity>>> GetAllNonRelatedAsync(string repositoryAlias)
        {
            try
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).GetAllNonRelatedAsync(new ApiRequestModel { Body = await ReadBodyAsStringAsync() });
                return response.ToContentResult();
            }
            catch { }

            return BadRequest();
        }

        [HttpPost("_rapidcms/{repositoryAlias}/new")]
        public async Task<ActionResult<EntityModel<IEntity>>> NewAsync(string repositoryAlias)
        {
            try
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).NewAsync(new ApiRequestModel { Body = await ReadBodyAsStringAsync() });
                return response.ToContentResult();
            }
            catch { }

            return BadRequest();
        }

        [HttpPost("_rapidcms/{repositoryAlias}/entity")]
        public async Task<ActionResult<EntityModel<IEntity>?>> InsertAsync(string repositoryAlias)
        {
            try
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).InsertAsync(new ApiRequestModel { Body = await ReadBodyAsStringAsync() });
                return response.ToContentResult();
            }
            catch { }

            return BadRequest();
        }

        [HttpPut("_rapidcms/{repositoryAlias}/entity/{id}")]
        public async Task<ActionResult> UpdateAsync(string repositoryAlias, string id)
        {
            try
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).UpdateAsync(new ApiRequestModel { Id = id, Body = await ReadBodyAsStringAsync() });
                return response.ToContentResult();
            }
            catch { }

            return BadRequest();
        }

        [HttpDelete("_rapidcms/{repositoryAlias}/entity/{id}")]
        public async Task<ActionResult> DeleteAsync(string repositoryAlias, string id)
        {
            try
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).DeleteAsync(new ApiRequestModel { Id = id, Body = await ReadBodyAsStringAsync() });
                return response.ToContentResult();
            }
            catch { }

            return BadRequest();
        }

        [HttpPost("_rapidcms/{repositoryAlias}/relate")]
        public async Task<ActionResult> AddRelationAsync(string repositoryAlias)
        {
            try
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).AddRelationAsync(new ApiRequestModel { Body = await ReadBodyAsStringAsync() });
                return response.ToContentResult();
            }
            catch { }

            return BadRequest();
        }

        [HttpDelete("_rapidcms/{repositoryAlias}/relate")]
        public async Task<ActionResult> RemoveRelationAsync(string repositoryAlias)
        {
            try
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).RemoveRelationAsync(new ApiRequestModel { Body = await ReadBodyAsStringAsync() });
                return response.ToContentResult();
            }
            catch { }

            return BadRequest();
        }

        [HttpPost("_rapidcms/{repositoryAlias}/reorder")]
        public async Task<ActionResult> ReorderAsync(string repositoryAlias)
        {
            try
            {
                var response = await _apiHandlerResolver.GetApiHandler(repositoryAlias).ReorderAsync(new ApiRequestModel { Body = await ReadBodyAsStringAsync() });
                return response.ToContentResult();
            }
            catch { }

            return BadRequest();
        }

        private async Task<string> ReadBodyAsStringAsync()
        {
            using var streamReader = new StreamReader(Request.Body);
            return await streamReader.ReadToEndAsync();
        }
    }
}
