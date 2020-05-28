using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazor.FileReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Models.Request.Api;
using RapidCMS.Core.Models.Response;

namespace RapidCMS.Core.Controllers
{
    [ApiController]
    internal class ApiFileUploadController<THandler> : ControllerBase
        where THandler : IFileUploadHandler
    {
        private readonly THandler _handler;

        public ApiFileUploadController(THandler handler)
        {
            _handler = handler;
        }

        [HttpPost("file/validate")]
        public async Task<ActionResult<FileUploadValidationResponseModel>> ValidateFileAsync([FromForm] UploadFileRequestModel fileInfo)
        {
            var messages = await _handler.ValidateFileAsync(fileInfo);
            if (messages.Any())
            {
                return new FileUploadValidationResponseModel { ErrorMessages = messages };
            }

            return NoContent();
        }

        [HttpPost("file")]
        public async Task<ActionResult<FileUploadResponseModel>> SaveFileAsync([FromForm] UploadFileRequestModel fileInfo, [FromForm(Name = "file")] IFormFile file)
        {
            if (DoesFileMatchFileInfo(fileInfo, file, out var downloadedFile))
            {
                try
                {
                    var result = await _handler.SaveFileAsync(fileInfo, downloadedFile);
                    return new FileUploadResponseModel { Result = result };
                }
                catch { }
            }

            return BadRequest();
        }

        // TODO
        private bool DoesFileMatchFileInfo(IFileInfo fileInfo, IFormFile file, out Stream memoryStream)
        {
            memoryStream = file.OpenReadStream();

            return true;
        }
    }
}
