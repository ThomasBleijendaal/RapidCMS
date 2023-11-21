using System.Threading.Tasks;
using RapidCMS.Api.Core.Models;

namespace RapidCMS.Api.Core.Abstractions;

public interface IApiHandler
{
    string RepositoryAlias { get; }

    Task<ApiResponseModel> GetByIdAsync(ApiRequestModel request);
    Task<ApiResponseModel> GetAllAsync(ApiRequestModel request);
    Task<ApiResponseModel> GetAllRelatedAsync(ApiRequestModel request);
    Task<ApiResponseModel> GetAllNonRelatedAsync(ApiRequestModel request);
    Task<ApiResponseModel> NewAsync(ApiRequestModel request);
    Task<ApiResponseModel> InsertAsync(ApiRequestModel request);
    Task<ApiResponseModel> UpdateAsync(ApiRequestModel request);
    Task<ApiResponseModel> DeleteAsync(ApiRequestModel request);
    Task<ApiResponseModel> AddRelationAsync(ApiRequestModel request);
    Task<ApiResponseModel> RemoveRelationAsync(ApiRequestModel request);
    Task<ApiResponseModel> ReorderAsync(ApiRequestModel request);
}
