using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models.DTOs;
using RapidCMS.Common.Models.UI;

#nullable enable

namespace RapidCMS.Common.Services
{
    // TODO: rename NodeEditor to Node
    // TODO: make UI service subject-aware (new, save existing etc)
    // TODO: make button handling more seperate
    // TODO: why variantAlias for ProcessNodeEditorAction and not ProcessListAction?

    public interface ICollectionService
    {
        // TODO: refactor this to UI
        Task<CollectionTreeRootDTO> GetCollectionsAsync();

        Task<ListUI> GetCollectionListViewAsync(string action, string collectionAlias, string? variantAlias, string? parentId);
        Task<ViewCommand> ProcessListActionAsync(string action, string collectionAlias, string? parentId, string actionId, object? customData);
        Task<ViewCommand> ProcessListActionAsync(string action, string collectionAlias, string? parentId, string id, string actionId, NodeUI node, object? customData);
        
        Task<NodeUI> GetNodeEditorAsync(string action, string collectionAlias, string variantAlias, string? parentId, string? id);
        Task<ViewCommand> ProcessNodeEditorActionAsync(string collectionAlias, string variantAlias, string? parentId, string? id, NodeUI node, string actionId, object? customData);
    }
}
