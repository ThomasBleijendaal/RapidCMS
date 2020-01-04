using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Request;
using RapidCMS.Core.Models.Response;

namespace RapidCMS.Core.Services.Persistence
{
    internal class PersistenceService : IPersistenceService
    {
        private readonly IDispatcher<GetEntityRequestModel, EntityResponseModel> _getEntityDispatcher;

        public PersistenceService(
            IDispatcher<GetEntityRequestModel, EntityResponseModel> getEntityDispatcher)
        {
            _getEntityDispatcher = getEntityDispatcher;
        }

        public async Task<EditContext> GetEntityAsync(UsageType usageType, string collectionAlias, string? variantAlias, ParentPath? parentPath, string? id)
        {
            var response = await _getEntityDispatcher.InvokeAsync(new GetEntityRequestModel
            {
                CollectionAlias = collectionAlias,
                Id = id,
                ParentPath = parentPath,
                UsageType = usageType,
                VariantAlias = variantAlias
            });

            return response.EditContext;
        }
    }
}
