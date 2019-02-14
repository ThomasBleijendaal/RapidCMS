using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Services;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Interfaces;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.DTOs;

namespace RapidCMS.Common.Services
{
    // TODO: int id?

    // TODO: split class into Collection and UI service
    // TODO: make UI service intent-aware (new, save existing etc)

    public interface ICollectionService
    {
        Task<CollectionTreeRootDTO> GetCollectionsAsync();

        Task<CollectionListViewDTO> GetCollectionListViewAsync(string alias, int? parentId);

        Task<NodeEditorDTO> GetNodeEditorAsync(string action, string alias, int? parentId, int id);

        Task<ViewCommand> ProcessListViewActionAsync(string alias, int? parentId, string actionId);

        Task<ViewCommand> ProcessNodeEditorActionAsync(string action, string alias, int? parentId, int id, NodeEditorDTO editor, string actionId);
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
                       Path = $"node/{Constants.Edit}/{collection.Alias}{(parentId.HasValue ? $"/{parentId.Value}" : "")}/{entity.Id}",
                       Collections = subCollections.ToList()
                   };
               }));

                var dto = new CollectionTreeCollectionDTO
                {
                    Alias = collection.Alias,
                    Name = collection.Name,
                    Path = $"collection/{Constants.List}/{collection.Alias}{(parentId.HasValue ? $"/{parentId.Value}" : "")}",
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
                Buttons = listView.Buttons.ToList(button =>
                {
                    return new ButtonDTO
                    {
                        Icon = button.Icon,
                        Id = button.Id,
                        Label = button.Label
                    };
                }),
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
                            entities.ToList(entity => prop.ValueMapper.MapToView(null, prop.NodeProperty.Getter.Invoke(entity)))))
                    };
                })
            };
        }

        public async Task<NodeEditorDTO> GetNodeEditorAsync(string action, string alias, int? parentId, int id)
        {
            var viewContext = new ViewContext
            {
                Action = action
            };

            var collection = _root.GetCollection(alias);

            var entity = action switch
            {
                Constants.Edit => await collection.Repository.GetByIdAsync(id, parentId),
                Constants.New => await collection.Repository.NewAsync(parentId),
                _ => null
            };

            if (entity == null)
            {
                return null;
            }

            var nodeEditor = collection.NodeEditor;

            return new NodeEditorDTO
            {
                Buttons = nodeEditor.Buttons
                    .Where(button => button.IsCompatibleWithView(viewContext))
                    .ToList(button =>
                    {
                        return new ButtonDTO
                        {
                            Icon = button.Icon,
                            Id = button.Id,
                            Label = button.Label
                        };
                    }),
                EditorPanes = nodeEditor.EditorPanes.ToList(pane =>
                {
                    return new NodeEditorPaneDTO
                    {
                        Fields = pane.Fields.ToList(field =>
                        {
                            var editor = (
                                new LabelDTO
                                {
                                    Name = field.Name,
                                    Description = field.Description
                                },
                                new EditorDTO
                                {
                                    Type = field.DataType,
                                    Value = field.ValueMapper.MapToEditor(null, field.NodeProperty.Getter(entity))
                                });

                            return editor;
                        })
                    };
                })
            };
        }

        public async Task<ViewCommand> ProcessNodeEditorActionAsync(string action, string alias, int? parentId, int id, NodeEditorDTO editor, string actionId)
        {
            var collection = _root.GetCollection(alias);

            var entity = action switch
            {
                Constants.Edit => await collection.Repository.GetByIdAsync(id, parentId),
                Constants.New => await collection.Repository.NewAsync(parentId),
                _ => null
            };

            var nodeEditor = collection.NodeEditor;

            // TODO: process view to entity automagically
            var nameValue = editor.EditorPanes.First().Fields.ElementAt(1).editor.Value;
            var descValue = editor.EditorPanes.First().Fields.Last().editor.Value;

            var element1 = nodeEditor.EditorPanes.First().Fields.ElementAt(1);
            var element2 = nodeEditor.EditorPanes.First().Fields.Last();

            element1.NodeProperty.Setter.Invoke(entity, element1.ValueMapper.MapFromEditor(null, nameValue));
            element2.NodeProperty.Setter.Invoke(entity, element2.ValueMapper.MapFromEditor(null, descValue));


            var button = nodeEditor.Buttons.First(x => x.Id == actionId);

            if (button is DefaultButton defaultButton)
            {
                switch (defaultButton.DefaultButtonType)
                {
                    case DefaultButtonType.SaveNew:

                        entity = await collection.Repository.InsertAsync(id, parentId, entity);

                        return new NavigateCommand { Uri = $"/node/{Constants.Edit}/{alias}{(parentId.HasValue ? $"/{parentId.Value}" : "")}/{entity.Id}" };

                    case DefaultButtonType.SaveExisting:
                    case DefaultButtonType.SaveNewAndExisting:

                        await collection.Repository.UpdateAsync(id, parentId, entity);

                        break;
                    case DefaultButtonType.Delete:

                        await collection.Repository.DeleteAsync(id, parentId);

                        return new NavigateCommand { Uri = $"/collection/{Constants.List}/{alias}{(parentId.HasValue ? $"/{parentId.Value}" : "")}" };

                    case DefaultButtonType.New:
                    default:
                        break;
                }
            }
            else
            {

            }

            return null;
        }

        public async Task<ViewCommand> ProcessListViewActionAsync(string alias, int? parentId, string actionId)
        {
            var collection = _root.GetCollection(alias);

            var listView = collection.ListView;

            var button = listView.Buttons.First(x => x.Id == actionId);

            if (button is DefaultButton defaultButton)
            {
                switch (defaultButton.DefaultButtonType)
                {
                    case DefaultButtonType.New:

                        return new NavigateCommand { Uri = $"/node/{Constants.New}/{alias}/{(parentId.HasValue ? $"{parentId.Value}/" : "")}" };

                    default:
                        break;
                }
            }

            return null;
        }
    }
}
