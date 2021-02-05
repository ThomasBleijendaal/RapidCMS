using System.IO;
using System.Threading.Tasks;
using RapidCMS.Api.Core.Models;
using RapidCMS.Core.Models.ApiBridge.Request;

namespace RapidCMS.Api.Core.Abstractions
{
    public interface IFileHandler
    {
        Task<ApiResponseModel> ValidateFileAsync(UploadFileModel request);
        Task<ApiResponseModel> SaveFileAsync(UploadFileModel request, Stream fileStream);
    }
}
