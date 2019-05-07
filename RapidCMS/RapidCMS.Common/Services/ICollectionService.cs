using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models.DTOs;
using RapidCMS.Common.Models.UI;

#nullable enable

namespace RapidCMS.Common.Services
{
    // TODO: split class into Collection and UI service
    // TODO: make UI service subject-aware (new, save existing etc)
    // TODO: make button handling more seperate
    // TODO: rename alias to collectionAlias
    // TODO: check applicability of every crud type (instead of inserting invalid operation exceptions everywhere)

    public interface ICollectionService
    {
        Task<CollectionTreeRootDTO> GetCollectionsAsync();

        Task<ListUI> GetCollectionListViewAsync(string action, string collectionAlias, string? variantAlias, string? parentId);
        Task<ViewCommand> ProcessListActionAsync(string action, string collectionAlias, string? parentId, string actionId, object? customData);
        Task<ViewCommand> ProcessListActionAsync(string action, string collectionAlias, string? parentId, string id, string actionId, IEntity entity, object? customData);
        
        Task<EditorUI> GetNodeEditorAsync(string action, string collectionAlias, string variantAlias, string? parentId, string? id);
        Task<ViewCommand> ProcessNodeEditorActionAsync(string collectionAlias, string variantAlias, string? parentId, string? id, EditorUI editor, string actionId, object? customData);
    }
}
