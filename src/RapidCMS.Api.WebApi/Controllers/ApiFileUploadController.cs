﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Models.ApiBridge.Request;
using RapidCMS.Core.Models.Response;

namespace RapidCMS.Api.WebApi.Controllers
{
    [ApiController]
    internal class ApiFileUploadController : ControllerBase
    {
        private readonly THandler _handler;

        public ApiFileUploadController(THandler handler)
        {
            _handler = handler;
        }


        [HttpPost("_rapidcms/{fileHandler}/file/validate")]
        public async Task<ActionResult<FileUploadValidationResponseModel>> ValidateFileAsync(string fileHandler, [FromForm] UploadFileModel fileInfo)
        {
            var messages = await _handler.ValidateFileAsync(fileInfo);
            if (messages.Any())
            {
                return new FileUploadValidationResponseModel { ErrorMessages = messages };
            }

            return NoContent();
        }

        [HttpPost("file")]
        public async Task<ActionResult<FileUploadResponseModel>> SaveFileAsync([FromForm] UploadFileModel fileInfo, [FromForm(Name = "file")] IFormFile file)
        {
            if (ApiFileUploadController<THandler>.DoesFileMatchFileInfo(fileInfo, file, out var downloadedFile))
            {
                using (downloadedFile)
                {
                    try
                    {
                        var result = await _handler.SaveFileAsync(fileInfo, downloadedFile);
                        return new FileUploadResponseModel { Result = result };
                    }
                    catch { }
                }
            }

            return BadRequest();
        }

        private static bool DoesFileMatchFileInfo(IFileInfo fileInfo, IFormFile file, out Stream memoryStream)
        {
            memoryStream = file.OpenReadStream();

            return !(fileInfo.Size != memoryStream.Length || fileInfo.Name != file.FileName);
        }
    }
}
