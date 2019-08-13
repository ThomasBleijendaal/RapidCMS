using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Models.Commands;

namespace RapidCMS.Common.Services
{
    // TODO: limit the parameters more
    public interface IEditContextService
    {
        Task<EditContext> GetEntityAsync(UsageType usageType, string collectionAlias, string? variantAlias, string? parentId, string? id);
        Task<ViewCommand> ProcessEntityActionAsync(UsageType usageType, string collectionAlias, string? parentId, string? id, EditContext editContext, string actionId, object? customData);

        Task<EditContext> GetRootAsync(UsageType usageType, string collectionAlias, string? parentId);

        Task<List<EditContext>> GetEntitiesAsync(UsageType usageType, string collectionAlias, string? parentId, Query query);
        Task<ViewCommand> ProcessListActionAsync(UsageType usageType, string collectionAlias, string? parentId, IEnumerable<EditContext> editContexts, string actionId, object? customData);
        Task<ViewCommand> ProcessListActionAsync(UsageType usageType, string collectionAlias, string? parentId, string id, EditContext editContext, string actionId, object? customData);

        Task<List<EditContext>> GetRelatedEntitiesAsync(UsageType usageType, string collectionAlias, IEntity relatedEntity, Query query);
        Task<ViewCommand> ProcessRelationActionAsync(UsageType usageType, string collectionAlias, IEntity relatedEntity, IEnumerable<EditContext> editContexts, string actionId, object? customData);
        Task<ViewCommand> ProcessRelationActionAsync(UsageType usageType, string collectionAlias, IEntity relatedEntity, string id, EditContext editContext, string actionId, object? customData);
    }
}
