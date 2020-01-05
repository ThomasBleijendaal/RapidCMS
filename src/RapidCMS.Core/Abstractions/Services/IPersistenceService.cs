using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Commands;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Abstractions.Services
{
    public interface IPersistenceService
    {
        Task<EditContext> GetEntityAsync(
            UsageType usageType, 
            string collectionAlias,
            string? variantAlias,
            ParentPath? parentPath,
            string? id);

        Task<ViewCommand> ProcessEntityActionAsync(
            EditContext editContext, 
            string actionId, 
            object? customData);

        Task<List<EditContext>> GetEntitiesAsync(
            UsageType usageType, 
            string collectionAlias, 
            ParentPath? parentPath, 
            Query query);

        Task<ViewCommand> ProcessListActionAsync(
            UsageType usageType,
            string collectionAlias,
            ParentPath? parentPath,
            // TODO: transform into ListContext (allow to insert UsageType and stuff in ListContext)
            IEnumerable<EditContext> editContexts, 
            string actionId, 
            object? customData);

        Task<ViewCommand> ProcessListActionAsync(
            EditContext editContext, 
            string actionId, 
            object? customData);

        Task<ViewCommand> ProcessRelationActionAsync(
            UsageType usageType,
            string collectionAlias,
            // TODO: transform into ListContext (allow to insert UsageType and stuff in ListContext)
            IEnumerable<EditContext> editContexts,
            string actionId,
            object? customData);

        Task<ViewCommand> ProcessRelationActionAsync(
            IRelated related,
            EditContext editContext,
            string actionId, 
            object? customData);
    }
}
