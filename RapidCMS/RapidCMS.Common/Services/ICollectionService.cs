using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Models.Commands;
using RapidCMS.Common.Models.UI;

namespace RapidCMS.Common.Services
{
    // TODO: this service should only return Entities / EditContexts. UI processing should be outside the scope of this service
    // TODO: why variantAlias for ProcessNodeEditorAction and not ProcessListAction? -> remove variantAlias on all Process* functions since it is found in EditContext

    public interface ICollectionService
    {
        Task<ListUI> GetCollectionListViewAsync(string action, string collectionAlias, string? variantAlias, string? parentId);
        Task<ViewCommand> ProcessListActionAsync(string action, string collectionAlias, string? parentId, string actionId, object? customData);
        Task<ViewCommand> ProcessListActionAsync(string action, string collectionAlias, string? parentId, string id, EditContext editContext, string actionId, object? customData);
        
        Task<NodeUI> GetNodeEditorAsync(string action, string collectionAlias, string variantAlias, string? parentId, string? id);
        Task<ViewCommand> ProcessNodeEditorActionAsync(string collectionAlias, string variantAlias, string? parentId, string? id, EditContext editContext, string actionId, object? customData);

        Task<ListUI> GetRelationListViewAsync(string action, string collectionAlias, IEntity relatedEntity);
        Task<ViewCommand> ProcessRelationActionAsync(string action, string collectionAlias, IEntity relatedEntity, string actionId, object? customData);
        Task<ViewCommand> ProcessRelationActionAsync(string action, string collectionAlias, IEntity relatedEntity, string id, EditContext editContext, string actionId, object? customData);
    }
}
