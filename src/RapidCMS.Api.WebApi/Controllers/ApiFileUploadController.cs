using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RapidCMS.Api.Core.Abstractions;
using RapidCMS.Api.WebApi.Extensions;
using RapidCMS.Core.Models.ApiBridge.Request;
using RapidCMS.Core.Models.Response;

namespace RapidCMS.Api.WebApi.Controllers
{
    [ApiController]
    public class ApiFileUploadController : ControllerBase
    {
        private readonly IFileHandlerResolver _fileHandlerResolver;

        public ApiFileUploadController(IFileHandlerResolver fileHandlerResolver)
        {
            _fileHandlerResolver = fileHandlerResolver;
        }

        [HttpPost("_rapidcms/{fileHandlerAlias}/file/validate")]
        public async Task<ActionResult<FileUploadValidationResponseModel>> ValidateFileAsync(string fileHandlerAlias, [FromForm] UploadFileModel model)
        {
            try
            {
                var response = await _fileHandlerResolver.GetFileHandler(fileHandlerAlias).ValidateFileAsync(model);
                return response.ToContentResult();
            }
            catch { }

            return BadRequest();
        }

        [HttpPost("_rapidcms/{fileHandlerAlias}/file")]
        public async Task<ActionResult<FileUploadResponseModel>> SaveFileAsync(string fileHandlerAlias, [FromForm] UploadFileModel model, [FromForm(Name = "file")] IFormFile file)
        {
            try
            {
                var response = await _fileHandlerResolver.GetFileHandler(fileHandlerAlias).SaveFileAsync(model, file.OpenReadStream());
                return response.ToContentResult();
            }
            catch { }

            return BadRequest();
        }
    }
}
