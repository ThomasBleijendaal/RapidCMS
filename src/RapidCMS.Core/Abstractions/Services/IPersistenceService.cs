using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Abstractions.Services
{
    public interface IPersistenceService
    {
        Task<EditContext> GetEntityAsync(UsageType usageType, string collectionAlias, string? variantAlias, ParentPath? parentPath, string? id);

        // TODO: remove parentPath
        //Task<ViewCommand> ProcessEntityActionAsync(UsageType usageType, string collectionAlias, ParentPath? parentPath, string? id, EditContext editContext, string actionId, object? customData);

        //Task<EditContext> GetRootAsync(UsageType usageType, string collectionAlias, ParentPath? parentPath);

        //Task<List<EditContext>> GetEntitiesAsync(UsageType usageType, string collectionAlias, ParentPath? parentPath, Query query);
        //Task<ViewCommand> ProcessListActionAsync(UsageType usageType, string collectionAlias, ParentPath? parentPath, IEnumerable<EditContext> editContexts, string actionId, object? customData);

        //// TODO: remove parentPath
        //Task<ViewCommand> ProcessListActionAsync(UsageType usageType, string collectionAlias, ParentPath? parentPath, string? id, EditContext editContext, string actionId, object? customData);

        //Task<List<EditContext>> GetRelatedEntitiesAsync(UsageType usageType, string collectionAlias, IEntity relatedEntity, Query query);
        //Task<ViewCommand> ProcessRelationActionAsync(UsageType usageType, string collectionAlias, IEntity relatedEntity, IEnumerable<EditContext> editContexts, string actionId, object? customData);
        //Task<ViewCommand> ProcessRelationActionAsync(UsageType usageType, string collectionAlias, IEntity relatedEntity, string? id, EditContext editContext, string actionId, object? customData);
    }
}
