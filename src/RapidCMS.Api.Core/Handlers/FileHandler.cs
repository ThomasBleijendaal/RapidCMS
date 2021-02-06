using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using RapidCMS.Api.Core.Abstractions;
using RapidCMS.Api.Core.Models;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Models.ApiBridge.Request;
using RapidCMS.Core.Models.Response;

namespace RapidCMS.Api.Core.Handlers
{
    internal class FileHandler<THandler> : IFileHandler
        where THandler : IFileUploadHandler
    {
        private readonly THandler _handler;

        public FileHandler(THandler handler)
        {
            _handler = handler;
        }

        public async Task<ApiResponseModel> SaveFileAsync(UploadFileModel request, Stream fileStream)
        {
            if (fileStream.Length == request.Size)
            {
                using (fileStream)
                {
                    try
                    {
                        var result = await _handler.SaveFileAsync(request, fileStream);
                        return new ApiResponseModel(HttpStatusCode.OK, new FileUploadResponseModel { Result = result });
                    }
                    catch { }
                }
            }

            return new ApiResponseModel(HttpStatusCode.BadRequest);
        }

        public async Task<ApiResponseModel> ValidateFileAsync(UploadFileModel request)
        {
            var messages = await _handler.ValidateFileAsync(request);
            if (messages.Any())
            {
                return new ApiResponseModel(HttpStatusCode.OK, new FileUploadValidationResponseModel { ErrorMessages = messages });
            }

            return new ApiResponseModel(HttpStatusCode.NoContent);
        }
    }
}
