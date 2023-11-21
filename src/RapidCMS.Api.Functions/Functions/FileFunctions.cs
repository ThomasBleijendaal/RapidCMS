using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HttpMultipartParser;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using RapidCMS.Api.Core.Abstractions;
using RapidCMS.Api.Functions.Abstractions;
using RapidCMS.Api.Functions.Extensions;
using RapidCMS.Core.Models.ApiBridge.Request;

namespace RapidCMS.Api.Functions.Functions;

public class FileUploadFunctions
{
    private readonly IFileHandlerResolver _fileHandlerResolver;
    private readonly IFunctionContextAccessor _functionExecutionContextAccessor;

    public FileUploadFunctions(
        IFileHandlerResolver fileHandlerResolver,
        IFunctionContextAccessor functionExecutionContextAccessor)
    {
        _fileHandlerResolver = fileHandlerResolver;
        _functionExecutionContextAccessor = functionExecutionContextAccessor;
    }

    [Function(nameof(ValidateFileAsync))]
    public async Task<HttpResponseData> ValidateFileAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{fileHandlerAlias}/file/validate")] HttpRequestData req,
        string fileHandlerAlias,
        FunctionContext context)
    {
        _functionExecutionContextAccessor.FunctionExecutionContext = context;

        try
        {
            var (model, file) = await ReadBodyAsFormDataAsync(req);
            if (file != null)
            {
                throw new InvalidOperationException();
            }

            var response = await _fileHandlerResolver.GetFileHandler(fileHandlerAlias).ValidateFileAsync(model);
            return req.CreateResponse(response);
        }
        catch { }

        return req.CreateResponse(HttpStatusCode.BadRequest);
    }

    [Function(nameof(SaveFileAsync))]
    public async Task<HttpResponseData> SaveFileAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{fileHandlerAlias}/file")] HttpRequestData req,
        string fileHandlerAlias,
        FunctionContext context)
    {
        _functionExecutionContextAccessor.FunctionExecutionContext = context;

        try
        {
            var (model, file) = await ReadBodyAsFormDataAsync(req);
            if (file == null)
            {
                throw new InvalidOperationException();
            }

            var response = await _fileHandlerResolver.GetFileHandler(fileHandlerAlias).SaveFileAsync(model, file);
            return req.CreateResponse(response);
        }
        catch { }

        return req.CreateResponse(HttpStatusCode.BadRequest);
    }

    private static async Task<(UploadFileModel model, Stream? file)> ReadBodyAsFormDataAsync(HttpRequestData req)
    {
        var result = await MultipartFormDataParser.ParseAsync(req.Body);
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
