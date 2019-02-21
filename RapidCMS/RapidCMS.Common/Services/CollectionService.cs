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
    // TODO: make button handling more seperate

    public interface ICollectionService
    {
        Task<CollectionTreeRootDTO> GetCollectionsAsync();

        Task<CollectionListViewDTO> GetCollectionListViewAsync(string action, string alias, int? parentId);

        Task<CollectionListEditorDTO> GetCollectionListEditorAsync(string action, string alias, int? parentId);

        Task<NodeEditorDTO> GetNodeEditorAsync(string action, string alias, int? parentId, int id);

        Task<ViewCommand> ProcessListViewActionAsync(string action, string alias, int? parentId, string actionId);
        Task<ViewCommand> ProcessListEditorActionAsync(string action, string alias, int? parentId, string actionId);

        Task<ViewCommand> ProcessListViewActionAsync(string action, string alias, int id, int? parentId, string actionId);
        Task<ViewCommand> ProcessListEditorActionAsync(string action, string alias, int id, int? parentId, CollectionListEditorDTO formValues, string actionId);

        Task<ViewCommand> ProcessNodeEditorActionAsync(string action, string alias, int? parentId, int id, NodeEditorDTO formValues, string actionId);
    }

    public class CollectionService : ICollectionService
    {
        private readonly Root _root;

        public CollectionService(Root root)
        {
            _root = root;
        }

        private UsageType MapActionToUsageType(string action)
        {
            return action switch
            {
                Constants.Edit => UsageType.Edit,
                Constants.New => UsageType.New,
                Constants.View => UsageType.View,
                _ => (UsageType)0
            };
        }

        // TODO: make tree configuration aware for editors and viewers
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

        public async Task<CollectionListViewDTO> GetCollectionListViewAsync(string action, string alias, int? parentId)
        {
            var viewContext = new ViewContext
            {
                Usage = UsageType.List | MapActionToUsageType(action)
            };

            var collection = _root.GetCollection(alias);

            var entities = await collection.Repository.GetAllAsObjectsAsync(parentId);

            var listView = collection.ListView;

            return new CollectionListViewDTO()
            {
                Buttons = listView.Buttons
                    .Where(button => button.IsCompatibleWithView(viewContext))
                    .ToList(button =>
                    {
                        return new ButtonDTO
                        {
                            Icon = button.Icon,
                            ButtonId = button.ButtonId,
                            Label = button.Label
                        };
                    }),
                ViewPanes = listView.ViewPanes.ToList(pane =>
                {
                    return new CollectionListViewPaneDTO
                    {
                        Buttons = pane.Buttons.ToList(button =>
                        {
                            return new ButtonDTO
                            {
                                Icon = button.Icon,
                                ButtonId = button.ButtonId,
                                Label = button.Label
                            };
                        }),
                        Properties = pane.Properties.ToList(prop =>
                            new PropertyDTO
                            {
                                Name = prop.Name,
                                Description = prop.Description
                            }),
                        Nodes = entities.ToList(entity =>
                            new NodeDTO
                            {
                                Id = entity.Id,
                                ParentId = parentId,
                                Values = pane.Properties.ToList(prop => new ValueDTO
                                {
                                    DisplayValue = prop.ValueMapper.MapToView(null, prop.NodeProperty.Getter.Invoke(entity))
                                })
                            })
                    };
                })
            };
        }

        public async Task<CollectionListEditorDTO> GetCollectionListEditorAsync(string action, string alias, int? parentId)
        {
            var listViewContext = new ViewContext
            {
                Usage = UsageType.List | MapActionToUsageType(action)
            };

            var collection = _root.GetCollection(alias);

            var listEditor = collection.ListEditor;
            var editor = listEditor.EditorPane;

            return new CollectionListEditorDTO
            {
                Buttons = listEditor.Buttons
                    .Where(button => button.IsCompatibleWithView(listViewContext))
                    .ToList(button =>
                    {
                        return new ButtonDTO
                        {
                            Icon = button.Icon,
                            ButtonId = button.ButtonId,
                            Label = button.Label
                        };
                    }),
                Editor = new CollectionListEditorPaneDTO
                {
                    Properties = editor.Fields.ToList(field =>
                        new PropertyDTO
                        {
                            Name = field.Name,
                            Description = field.Description
                        }),
                    Nodes = await GetCollectionListEditorNodesAsync(action, parentId, collection, editor).ToListAsync()
                }
            };
        }

        private async IAsyncEnumerable<NodeDTO> GetCollectionListEditorNodesAsync(string action, int? parentId, Collection collection, EditorPane<Field> editor)
        {
            if (action == Constants.New)
            {
                var editorViewContext = new ViewContext
                {
                    Usage = UsageType.Node | MapActionToUsageType(Constants.New)
                };

                var newEntity = await collection.Repository.NewAsync(parentId);

                yield return CreateCollectionListEditorNode(newEntity, editorViewContext, parentId, editor);
            }

            if (action.In(Constants.New, Constants.Edit))
            {
                var editorViewContext = new ViewContext
                {
                    Usage = UsageType.Node | MapActionToUsageType(Constants.Edit)
                };

                var entities = await collection.Repository.GetAllAsObjectsAsync(parentId);

                foreach (var entity in entities)
                {
                    yield return CreateCollectionListEditorNode(entity, editorViewContext, parentId, editor);
                }
            }
        }

        private static NodeDTO CreateCollectionListEditorNode(IEntity entity, ViewContext viewContext, int? parentId, EditorPane<Field> editor)
        {
            return new NodeDTO
            {
                Id = entity.Id,
                ParentId = parentId,
                Buttons = editor.Buttons
                    .Where(button => button.IsCompatibleWithView(viewContext))
                    .ToList(button =>
                    {
                        return new ButtonDTO
                        {
                            Icon = button.Icon,
                            ButtonId = button.ButtonId,
                            Label = button.Label
                        };
                    }),
                Values = editor.Fields.ToList(field => new ValueDTO
                {
                    DisplayValue = field.ValueMapper.MapToView(null, field.NodeProperty.Getter.Invoke(entity)),
                    IsReadonly = field.Readonly,
                    Type = field.DataType,
                    Value = field.ValueMapper.MapToEditor(null, field.NodeProperty.Getter.Invoke(entity)),
                })
            };
        }

        public async Task<NodeEditorDTO> GetNodeEditorAsync(string action, string alias, int? parentId, int id)
        {
            var viewContext = new ViewContext
            {
                Usage = UsageType.Node | MapActionToUsageType(action)
            };

            var collection = _root.GetCollection(alias);

            var entity = action switch
            {
                Constants.View => await collection.Repository.GetByIdAsync(id, parentId),
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
                            ButtonId = button.ButtonId,
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
                                new ValueDTO
                                {
                                    DisplayValue = field.ValueMapper.MapToView(null, field.NodeProperty.Getter(entity)),
                                    IsReadonly = field.Readonly,
                                    Type = field.DataType,
                                    Value = field.ValueMapper.MapToEditor(null, field.NodeProperty.Getter(entity))
                                });

                            return editor;
                        }),
                        SubCollectionListEditors = action == Constants.New 
                            ? new List<SubCollectionListEditorDTO>()
                            : pane.SubCollectionListEditors.ToList(listEditor =>
                                {
                                    return new SubCollectionListEditorDTO
                                    {
                                        CollectionAlias = listEditor.CollectionAlias,
                                        Action = Constants.Edit
                                    };
                                })
                    };
                })
            };
        }

        public async Task<ViewCommand> ProcessNodeEditorActionAsync(string action, string alias, int? parentId, int id, NodeEditorDTO formValues, string actionId)
        {
            var collection = _root.GetCollection(alias);

            var entity = action switch
            {
                Constants.Edit => await collection.Repository.GetByIdAsync(id, parentId),
                Constants.New => await collection.Repository.NewAsync(parentId),
                _ => null
            };

            var nodeEditor = collection.NodeEditor;

            UpdateEntityWithFormData(formValues, entity, nodeEditor);

            var button = nodeEditor.Buttons.First(x => x.ButtonId == actionId);

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

                        return new ReloadCommand();

                    case DefaultButtonType.Delete:

                        await collection.Repository.DeleteAsync(id, parentId);

                        return new NavigateCommand { Uri = $"/collection/{Constants.List}/{alias}{(parentId.HasValue ? $"/{parentId.Value}" : "")}" };

                    default:
                        break;
                }
            }
            else
            {

            }

            return null;
        }

        public async Task<ViewCommand> ProcessListViewActionAsync(string action, string alias, int? parentId, string actionId)
        {
            var collection = _root.GetCollection(alias);

            var listView = collection.ListView;

            var button = listView.Buttons.First(x => x.ButtonId == actionId);

            if (button is DefaultButton defaultButton)
            {
                switch (defaultButton.DefaultButtonType)
                {
                    case DefaultButtonType.New:

                        return new NavigateCommand { Uri = $"/node/{Constants.New}/{alias}/{(parentId.HasValue ? $"{parentId.Value}" : "")}" };

                    default:
                        break;
                }
            }

            return null;
        }

        public async Task<ViewCommand> ProcessListEditorActionAsync(string action, string alias, int? parentId, string actionId)
        {
            var collection = _root.GetCollection(alias);

            var listEditor = collection.ListEditor;

            var button = listEditor.Buttons.First(x => x.ButtonId == actionId);

            if (button is DefaultButton defaultButton)
            {
                switch (defaultButton.DefaultButtonType)
                {
                    case DefaultButtonType.New:

                        return new UpdateParameterCommand
                        {
                            Action = Constants.New,
                            Alias = alias,
                            ParentId = parentId,
                            Id = null
                        };

                        // return new NavigateCommand { Uri = $"/collection/{Constants.New}/{alias}/{(parentId.HasValue ? $"{parentId.Value}" : "")}" };

                    default:
                        break;
                }
            }

            return null;
        }

        public async Task<ViewCommand> ProcessListViewActionAsync(string action, string alias, int id, int? parentId, string actionId)
        {
            var collection = _root.GetCollection(alias);

            var listView = collection.ListView;

            var button = listView.ViewPanes.SelectMany(x => x.Buttons).First(x => x.ButtonId == actionId);

            if (button is DefaultButton defaultButton)
            {
                switch (defaultButton.DefaultButtonType)
                {
                    case DefaultButtonType.View:

                        return new NavigateCommand { Uri = $"/node/{Constants.View}/{alias}/{(parentId.HasValue ? $"{parentId.Value}/" : "")}{id}" };

                    case DefaultButtonType.Edit:

                        return new NavigateCommand { Uri = $"/node/{Constants.Edit}/{alias}/{(parentId.HasValue ? $"{parentId.Value}/" : "")}{id}" };

                    default:
                        break;
                }
            }

            return null;
        }

        public async Task<ViewCommand> ProcessListEditorActionAsync(string action, string alias, int id, int? parentId, CollectionListEditorDTO formValues, string actionId)
        {
            var collection = _root.GetCollection(alias);

            var entity = action switch
            {
                Constants.Edit => await collection.Repository.GetByIdAsync(id, parentId),
                Constants.New => await collection.Repository.NewAsync(parentId),
                _ => null
            };

            var listEditor = collection.ListEditor;
            var nodeFormValues = formValues.Editor.Nodes.First(x => x.Id == id);

            UpdateEntityWithFormData(nodeFormValues, entity, listEditor);

            var button = listEditor.EditorPane.Buttons.First(x => x.ButtonId == actionId);

            if (button is DefaultButton defaultButton)
            {
                switch (defaultButton.DefaultButtonType)
                {
                    case DefaultButtonType.View:

                        return new NavigateCommand { Uri = $"/node/{Constants.View}/{alias}/{(parentId.HasValue ? $"{parentId.Value}/" : "")}{id}" };

                    case DefaultButtonType.Edit:

                        return new NavigateCommand { Uri = $"/node/{Constants.Edit}/{alias}/{(parentId.HasValue ? $"{parentId.Value}/" : "")}{id}" };

                    case DefaultButtonType.SaveNew:

                        entity = await collection.Repository.InsertAsync(id, parentId, entity);

                        return new UpdateParameterCommand
                        {
                            Action = Constants.New,
                            Alias = alias,
                            ParentId = parentId,
                            Id = entity.Id
                        };

                    case DefaultButtonType.SaveExisting:
                    case DefaultButtonType.SaveNewAndExisting:

                        await collection.Repository.UpdateAsync(id, parentId, entity);

                        return new ReloadCommand();

                    case DefaultButtonType.Delete:

                        await collection.Repository.DeleteAsync(id, parentId);

                        return new ReloadCommand();

                    default:
                        break;
                }
            }

            return null;
        }

        // TODO: these functions contain a huge flaw which requires the form to ALWAYS have EVERY property
        // TODO: create the reciprocal function to load the form with entity data
        private static void UpdateEntityWithFormData(NodeDTO formValues, IEntity entity, ListEditor listEditor)
        {
            var formValueForNode = formValues.Values.Select((x, fieldIndex) => (fieldIndex, x.Value));
            var setters = listEditor.EditorPane.Fields
                .Select((x, fieldIndex) => x.Readonly ? (fieldIndex, null, null) : (fieldIndex, x.NodeProperty.Setter, x.ValueMapper))
                .ToDictionary(x => x.fieldIndex, x => (x.Setter, x.ValueMapper));

            foreach (var (fieldIndex, value) in formValueForNode)
            {
                var (setter, valueMapper) = setters[fieldIndex];

                setter?.Invoke(entity, valueMapper?.MapFromEditor(null, value));
            }
        }

        // TODO: create the reciprocal function to load the form with entity data
        private static void UpdateEntityWithFormData(NodeEditorDTO formValues, IEntity entity, NodeEditor nodeEditor)
        {
            var enteredFormValues = formValues.EditorPanes
                .SelectMany((x, paneIndex) => x.Fields.Select((y, fieldIndex) => (paneIndex, fieldIndex, y.value.Value)));
            var setters = nodeEditor.EditorPanes
                .Select((x, paneIndex) => (paneIndex, fields: x.Fields.Select((y, fieldIndex) => y.Readonly ? (fieldIndex, null, null) : (fieldIndex, y.ValueMapper, y.NodeProperty.Setter))))
                .ToDictionary(
                    x => x.paneIndex,
                    x => x.fields.ToDictionary(
                        y => y.fieldIndex,
                        y => (y.Setter, y.ValueMapper)));

            foreach (var (paneIndex, fieldIndex, value) in enteredFormValues)
            {
                var (setter, valueMapper) = setters[paneIndex][fieldIndex];

                setter?.Invoke(entity, valueMapper?.MapFromEditor(null, value));
            }
        }
    }
}
