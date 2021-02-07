using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HttpMultipartParser;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using RapidCMS.Api.Core.Abstractions;
using RapidCMS.Api.Functions.Models;
using RapidCMS.Core.Models.ApiBridge.Request;

namespace RapidCMS.Api.Functions.Functions
{
    public class FileUploadFunctions
    {
        private readonly IFileHandlerResolver _fileHandlerResolver;

        public FileUploadFunctions(IFileHandlerResolver fileHandlerResolver)
        {
            _fileHandlerResolver = fileHandlerResolver;
        }

        [FunctionName(nameof(ValidateFileAsync))]
        public async Task<HttpResponseData> ValidateFileAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{fileHandlerAlias}/file/validate")] HttpRequestData req)
        {
            if (req.Params.TryGetValue("fileHandlerAlias", out var fileHandlerAlias))
            {
                try
                {
                    var (model, file) = await ReadBodyAsFormDataAsync(req);
                    if (file != null)
                    {
                        throw new InvalidOperationException();
                    }

                    var response = await _fileHandlerResolver.GetFileHandler(fileHandlerAlias).ValidateFileAsync(model);
                    return new HttpResponseData(response.StatusCode, response.ResponseBody);
                }
                catch { }
            }

            return new HttpResponseData(HttpStatusCode.BadRequest);
        }

        [FunctionName(nameof(SaveFileAsync))]
        public async Task<HttpResponseData> SaveFileAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{fileHandlerAlias}/file")] HttpRequestData req)
        {
            if (req.Params.TryGetValue("fileHandlerAlias", out var fileHandlerAlias))
            {
                try
                {
                    var (model, file) = await ReadBodyAsFormDataAsync(req);
                    if (file == null)
                    {
                        throw new InvalidOperationException();
                    }

                    var response = await _fileHandlerResolver.GetFileHandler(fileHandlerAlias).SaveFileAsync(model, file);
                    return new HttpResponseData(response.StatusCode, response.ResponseBody);
                }
                catch { }
            }

            return new HttpResponseData(HttpStatusCode.BadRequest);
        }

        private static async Task<(UploadFileModel model, Stream? file)> ReadBodyAsFormDataAsync(HttpRequestData req)
        {
            var base64Bytes = JsonConvert.DeserializeObject<BytesRequestWrapper>(req.Body).Bytes;

            using var memoryStream = new MemoryStream(Convert.FromBase64String(base64Bytes));
            var result = await MultipartFormDataParser.ParseAsync(memoryStream);

            var model = new UploadFileModel()
            {
                Name = result.Parameters.FirstOrDefault(x => x.Name == nameof(UploadFileModel.Name))?.Data!,
                Size = long.Parse(result.Parameters.FirstOrDefault(x => x.Name == nameof(UploadFileModel.Size))?.Data!),
                Type = result.Parameters.FirstOrDefault(x => x.Name == nameof(UploadFileModel.Type))?.Data!,
                LastModified = long.Parse(result.Parameters.FirstOrDefault(x => x.Name == nameof(UploadFileModel.LastModified))?.Data!)
            };

            Validator.ValidateObject(model, new ValidationContext(model));

            return (model, result.Files.FirstOrDefault(x => x.Name == "file")?.Data);
        }
    }
}
