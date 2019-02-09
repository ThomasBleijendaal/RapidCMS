using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.DTOs;

namespace RapidCMS.Common.Services
{
    // TODO: int id?

    public interface ICollectionService
    {
        Task<CollectionTreeRootDTO> GetCollectionsAsync();

        Task<CollectionListViewDTO> GetCollectionListViewAsync(string alias, int? parentId);

        Task<NodeEditorDTO> GetNodeEditorAsync(string alias, int? parentId, int id);
    }

    public class CollectionService : ICollectionService
    {
        private readonly Root _root;

        public CollectionService(Root root)
        {
            _root = root;
        }

        public async Task<CollectionTreeRootDTO> GetCollectionsAsync()
        {
            var result = new CollectionTreeRootDTO
            {
                Collections = await GetTreeViewForCollectionAsync(_root.Collections, null)
            };

            return result;
        }

        private static async Task<List<CollectionTreeCollectionDTO>> GetTreeViewForCollectionAsync(IEnumerable<Collection> collections, int? parentId)
        {
            var result = new List<CollectionTreeCollectionDTO>();

            foreach (var collection in collections)
            {
                var entities = await collection.Repository.GetAllAsObjectsAsync(parentId);

                var nodes = await Task.WhenAll(entities.Select(async entity =>
               {
                   var subCollections = collection.Collections.Any()
                       ? await GetTreeViewForCollectionAsync(collection.Collections, entity.Id)
                       : Enumerable.Empty<CollectionTreeCollectionDTO>();

                   return new CollectionTreeNodeDTO
                   {
                       Id = entity.Id,
                       Name = collection.TreeView.NameGetter.Invoke(entity) as string,
                       Collections = subCollections.ToList()
                   };
               }));

                var dto = new CollectionTreeCollectionDTO
                {
                    Alias = collection.Alias,
                    Name = collection.Name,
                    Nodes = nodes.ToList()
                };

                result.Add(dto);
            }

            return result;
        }

        public async Task<CollectionListViewDTO> GetCollectionListViewAsync(string alias, int? parentId)
        {
            var collection = _root.GetCollection(alias);

            var entities = await collection.Repository.GetAllAsObjectsAsync(parentId);

            var listView = collection.ListView;

            return new CollectionListViewDTO()
            {
                ViewPanes = listView.ViewPanes.ToList(pane =>
                {
                    return new CollectionListViewPaneDTO
                    {
                        Properties = pane.Properties.ToList(prop => (
                            new PropertyDTO
                            {
                                Name = prop.Name,
                                Description = prop.Description
                            },
                            entities.ToList(entity => prop.Formatter.Invoke(prop.Getter.Invoke(entity)))))
                    };
                })
            };
        }

        public async Task<NodeEditorDTO> GetNodeEditorAsync(string alias, int? parentId, int id)
        {
            var collection = _root.GetCollection(alias);

            var entity = await collection.Repository.GetByIdAsync(id, parentId);

            if (entity == null)
            {
                return null;
            }

            var nodeEditor = collection.NodeEditor;

            return new NodeEditorDTO
            {
                EditorPanes = nodeEditor.EditorPanes.ToList(pane =>
                {
                    return new NodeEditorPaneDTO
                    {
                        Fields = pane.Fields.ToList(field =>
                        {
                            var editor =  (
                                new LabelDTO
                                {
                                    Name = field.Name,
                                    Description = field.Description
                                },
                                new EditorDTO
                                {
                                    Type = field.DataType,
                                    Value = field.Getter(entity)
                                });

                            return editor;
                        })
                    };
                })
            };
        }
    }
}
