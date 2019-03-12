using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Interfaces;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.DTOs;

#nullable enable

namespace RapidCMS.Common.Services
{
    // TODO: int id?

    // TODO: split class into Collection and UI service
    // TODO: make UI service intent-aware (new, save existing etc)
    // TODO: make button handling more seperate
    // TODO: rename alias to collectionAlias

    public interface ICollectionService
    {
        Task<CollectionTreeRootDTO> GetCollectionsAsync();

        Task<CollectionListViewDTO> GetCollectionListViewAsync(string action, string collectionAlias, string variantAlias, string? parentId);
        Task<ViewCommand> ProcessListViewActionAsync(string collectionAlias, string? parentId, string actionId);
        Task<ViewCommand> ProcessListViewActionAsync(string collectionAlias, string? parentId, string id, string actionId);

        Task<CollectionListEditorDTO> GetCollectionListEditorAsync(string action, string collectionAlias, string variantAlias, string? parentId);
        Task<ViewCommand> ProcessListEditorActionAsync(string collectionAlias, string? parentId, string actionId);
        Task<ViewCommand> ProcessListEditorActionAsync(string collectionAlias, string variantAlias, string? parentId, string? id, CollectionListEditorDTO formValues, string actionId);

        Task<NodeEditorDTO> GetNodeEditorAsync(string action, string collectionAlias, string variantAlias, string? parentId, string? id);
        Task<ViewCommand> ProcessNodeEditorActionAsync(string collectionAlias, string variantAlias, string? parentId, string? id, NodeEditorDTO formValues, string actionId);
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

        // TODO: make tree configuration aware of editors and viewers
        public async Task<CollectionTreeRootDTO> GetCollectionsAsync()
        {
            var result = new CollectionTreeRootDTO
            {
                Collections = await GetTreeViewForCollectionAsync(_root.Collections, null)
            };

            return result;
        }

        private static async Task<List<CollectionTreeCollectionDTO>> GetTreeViewForCollectionAsync(IEnumerable<Collection> collections, string? parentId)
        {
            var result = new List<CollectionTreeCollectionDTO>();

            foreach (var collection in collections)
            {
                var dto = new CollectionTreeCollectionDTO
                {
                    Alias = collection.Alias,
                    Name = collection.Name
                };

                if (collection.TreeView.EntityViewType == ViewType.Tree)
                {
                    var entities = await collection.Repository._GetAllAsObjectsAsync(parentId);

                    dto.Nodes = await entities.ToListAsync(async entity =>
                    {
                        var subCollections = collection.Collections.Any()
                            ? await GetTreeViewForCollectionAsync(collection.Collections, entity.Id)
                            : Enumerable.Empty<CollectionTreeCollectionDTO>();

                        var entityVariant = collection.EntityVariants.First(x => x.Type == entity.GetType());

                        return new CollectionTreeNodeDTO
                        {
                            Id = entity.Id,
                            Name = collection.TreeView.NameGetter.Invoke(entity) as string,
                            Path = UriHelper.Node(Constants.Edit, collection.Alias, entityVariant, parentId, entity.Id),
                            Collections = subCollections.ToList()
                        };
                    });
                }

                dto.Path = new List<CollectionTreePathDTO>();

                if (collection.ListView != null)
                {
                    dto.Path.Add(new CollectionTreePathDTO
                    {
                        Icon = "list",
                        Path = UriHelper.Collection(Constants.List, collection.Alias, parentId)
                    });
                }

                if (collection.ListEditor != null)
                {
                    dto.Path.Add(new CollectionTreePathDTO
                    {
                        Icon = "list-rich",
                        Path = UriHelper.Collection(Constants.Edit, collection.Alias, parentId)
                    });
                }

                result.Add(dto);
            }

            return result;
        }

        public async Task<CollectionListViewDTO> GetCollectionListViewAsync(string action, string alias, string variantAlias, string? parentId)
        {
            var collection = _root.GetCollection(alias);
            var listView = collection.ListView;

            var entityVariant = collection.EntityVariants.FirstOrDefault(x => x.Alias == variantAlias);

            var viewContext = new ViewContext
            {
                Usage = UsageType.List | MapActionToUsageType(action),
                EntityVariant = entityVariant
            };

            var entities = await collection.Repository._GetAllAsObjectsAsync(parentId);

            return new CollectionListViewDTO()
            {
                Buttons = listView.Buttons
                    .GetAllButtons()
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
                        Buttons = pane.Buttons
                            .GetAllButtons()
                            .ToList(button =>
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
                                Alias = prop.Name.ToUrlFriendlyString(),
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
                                    Alias = prop.Name.ToUrlFriendlyString(),
                                    DisplayValue = prop.ValueMapper.MapToView(null, prop.NodeProperty.Getter.Invoke(entity))
                                })
                            })
                    };
                })
            };
        }

        public async Task<CollectionListEditorDTO> GetCollectionListEditorAsync(string action, string alias, string variantAlias, string? parentId)
        {
            var collection = _root.GetCollection(alias);

            var listEditor = collection.ListEditor;
            var editors = listEditor.EditorPanes;

            var entityVariant = collection.EntityVariants.FirstOrDefault(x => x.Alias == variantAlias);

            var listViewContext = new ViewContext
            {
                Usage = UsageType.List | MapActionToUsageType(action),
                EntityVariant = entityVariant
            };

            return new CollectionListEditorDTO
            {
                ListEditorType = listEditor.ListEditorType,
                Buttons = listEditor.Buttons
                    .GetAllButtons()
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
                    Properties = editors
                        .SelectMany(x => x.Fields.ToList(field =>
                            new PropertyDTO
                            {
                                Alias = field.Name.ToUrlFriendlyString(),
                                Name = field.Name,
                                Description = field.Description
                            }))
                        .GroupBy(x => x.Alias)
                        .Select(x => x.First())
                        .ToList(),
                    Nodes = await GetCollectionListEditorNodesAsync(action, parentId, collection, entityVariant, editors).ToListAsync()
                }
            };
        }

        private async IAsyncEnumerable<NodeDTO> GetCollectionListEditorNodesAsync(string action, string? parentId, Collection collection, EntityVariant entityVariant, List<EditorPane<Field>> editors)
        {
            if (action == Constants.New)
            {
                var variant = entityVariant ?? collection.EntityVariants.First();

                var newEntity = await collection.Repository._NewAsync(parentId, variant.Type);
                var editor = editors.First(x => x.VariantType == newEntity.GetType());

                var editorViewContext = new ViewContext
                {
                    Usage = UsageType.Node | MapActionToUsageType(Constants.New),
                    EntityVariant = variant
                };

                yield return CreateCollectionListEditorNode(newEntity, editorViewContext, parentId, editor);
            }

            if (action.In(Constants.New, Constants.Edit))
            {
                var entities = await collection.Repository._GetAllAsObjectsAsync(parentId);

                foreach (var entity in entities)
                {
                    var entityType = entity.GetType();

                    var editor = editors.First(x => x.VariantType == entityType);
                    var editorViewContext = new ViewContext
                    {
                        Usage = UsageType.Node | MapActionToUsageType(Constants.Edit),
                        EntityVariant = collection.EntityVariants.First(x => x.Type == entityType)
                    };

                    yield return CreateCollectionListEditorNode(entity, editorViewContext, parentId, editor);
                }
            }
        }

        private static NodeDTO CreateCollectionListEditorNode(IEntity entity, ViewContext viewContext, string? parentId, EditorPane<Field> editor)
        {
            return new NodeDTO
            {
                Id = entity.Id,
                ParentId = parentId,
                VariantAlias = viewContext.EntityVariant.Alias,
                Buttons = editor.Buttons
                    .GetAllButtons()
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
                    Alias = field.Name.ToUrlFriendlyString(),
                    DisplayValue = field.ValueMapper.MapToView(null, field.NodeProperty.Getter.Invoke(entity)),
                    IsReadonly = field.Readonly,
                    Type = field.DataType,
                    Value = field.ValueMapper.MapToEditor(null, field.NodeProperty.Getter.Invoke(entity)),
                })
            };
        }

        public async Task<NodeEditorDTO> GetNodeEditorAsync(string action, string alias, string variantAlias, string? parentId, string? id)
        {
            var collection = _root.GetCollection(alias);

            var nodeType = typeof(IEntity);

            if (!string.IsNullOrEmpty(variantAlias))
            {
                nodeType = collection.EntityVariants.First(variant => variant.Alias == variantAlias).Type;
            }

            var entity = action switch
            {
                Constants.View => await collection.Repository._GetByIdAsync(id, parentId),
                Constants.Edit => await collection.Repository._GetByIdAsync(id, parentId),
                Constants.New => await collection.Repository._NewAsync(parentId, nodeType),
                _ => null
            };

            if (entity == null)
            {
                return null;
            }

            var nodeEditor = collection.NodeEditor;

            if (collection.EntityVariants.Count > 1 && action != Constants.New)
            {
                nodeType = entity.GetType();
            }

            var viewContext = new ViewContext
            {
                Usage = UsageType.Node | MapActionToUsageType(action),
                EntityVariant = null // TODO: null
            };

            var editor = new NodeEditorDTO
            {
                Buttons = nodeEditor.Buttons
                    .GetAllButtons()
                    .Where(button => button.IsCompatibleWithView(viewContext))
                    .ToList(button =>
                    {
                        return new ButtonDTO
                        {
                            Icon = button.Icon,
                            ButtonId = button.ButtonId,
                            Label = button.Label,
                            Alias = (button is CustomButton customButton) ? customButton.Alias : null
                        };
                    }),
                EditorPanes = nodeEditor.EditorPanes
                    // allow for the specialized type, or the base type
                    .Where(pane => pane.VariantType == nodeType || pane.VariantType == nodeEditor.BaseType)
                    .ToList(pane =>
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

            return editor;
        }

        public async Task<ViewCommand> ProcessNodeEditorActionAsync(string collectionAlias, string variantAlias, string? parentId, string? id, NodeEditorDTO formValues, string actionId)
        {
            var collection = _root.GetCollection(collectionAlias);

            var entityVariant = !string.IsNullOrEmpty(variantAlias)
                ? collection.EntityVariants.First(variant => variant.Alias == variantAlias)
                : collection.EntityVariants.First();

            var nodeEditor = collection.NodeEditor;
            var button = nodeEditor.Buttons.GetAllButtons().First(x => x.ButtonId == actionId);
            var buttonCrudType = button.GetCrudType();

            var entity = buttonCrudType switch
            {
                CrudType.Insert => await collection.Repository._NewAsync(parentId, entityVariant.Type),
                _ => await collection.Repository._GetByIdAsync(id, parentId)
            };

            UpdateEntityWithFormData(formValues, entity, nodeEditor, entityVariant.Type);

            // TODO: what to do with this action
            if (button is CustomButton customButton)
            {
                customButton.Action.Invoke();
            }

            switch (buttonCrudType)
            {
                case CrudType.View:
                    return new NavigateCommand { Uri = UriHelper.Node(Constants.View, collectionAlias, entityVariant, parentId, id) };

                case CrudType.Read:
                    return new NavigateCommand { Uri = UriHelper.Node(Constants.Edit, collectionAlias, entityVariant, parentId, id) };

                case CrudType.Update:
                    await collection.Repository._UpdateAsync(id, parentId, entity);
                    return new ReloadCommand();

                case CrudType.Insert:
                    entity = await collection.Repository._InsertAsync(parentId, entity);
                    return new NavigateCommand { Uri = UriHelper.Node(Constants.Edit, collectionAlias, entityVariant, parentId, entity.Id) };

                case CrudType.Delete:
                    await collection.Repository._DeleteAsync(id, parentId);
                    return new NavigateCommand { Uri = UriHelper.Collection(Constants.List, collectionAlias, parentId) };

                default:
                    return null;
            }
        }

        public Task<ViewCommand> ProcessListViewActionAsync(string collectionAlias, string? parentId, string actionId)
        {
            var collection = _root.GetCollection(collectionAlias);

            var listView = collection.ListView;
            var button = listView.Buttons.GetAllButtons().First(x => x.ButtonId == actionId);
            var buttonCrudType = button.GetCrudType();
            var entityVariant = button.Metadata as EntityVariant;

            // TODO: what to do with this action
            if (button is CustomButton customButton)
            {
                customButton.Action.Invoke();
            }

            switch (buttonCrudType)
            {
                case CrudType.Create:
                    return Task.FromResult(new NavigateCommand { Uri = UriHelper.Node(Constants.New, collectionAlias, entityVariant, parentId, null) } as ViewCommand);

                default:
                    return Task.FromResult(default(ViewCommand));
            }

        }

        public async Task<ViewCommand> ProcessListViewActionAsync(string collectionAlias, string? parentId, string id, string actionId)
        {
            var collection = _root.GetCollection(collectionAlias);

            var listView = collection.ListView;
            var button = listView.ViewPanes.SelectMany(x => x.Buttons).GetAllButtons().First(x => x.ButtonId == actionId);
            var buttonCrudType = button.GetCrudType();

            // since the id is known, get the entity variant from the entity
            var entity = await collection.Repository._GetByIdAsync(id, parentId);
            var entityType = entity.GetType();
            var entityVariant = collection.EntityVariants.First(variant => variant.Type == entityType);

            // TODO: what to do with this action
            if (button is CustomButton customButton)
            {
                customButton.Action.Invoke();
            }

            switch (buttonCrudType)
            {
                case CrudType.View:
                    return new NavigateCommand { Uri = UriHelper.Node(Constants.View, collectionAlias, entityVariant, parentId, id) };

                case CrudType.Read:
                    return new NavigateCommand { Uri = UriHelper.Node(Constants.Edit, collectionAlias, entityVariant, parentId, id) };

                default:
                    return null;
            }
        }

        public Task<ViewCommand> ProcessListEditorActionAsync(string collectionAlias, string? parentId, string actionId)
        {
            var collection = _root.GetCollection(collectionAlias);

            var listEditor = collection.ListEditor;
            var button = listEditor.Buttons.GetAllButtons().First(x => x.ButtonId == actionId);
            var buttonCrudType = button.GetCrudType();
            var entityVariant = button.Metadata as EntityVariant;

            // TODO: what to do with this action
            if (button is CustomButton customButton)
            {
                customButton.Action.Invoke();
            }

            switch (buttonCrudType)
            {
                case CrudType.View:
                    return Task.FromResult(new UpdateParameterCommand
                    {
                        Action = Constants.View,
                        CollectionAlias = collectionAlias,
                        VariantAlias = entityVariant.Alias,
                        ParentId = parentId,
                        Id = null
                    } as ViewCommand);

                case CrudType.Read:
                    return Task.FromResult(new UpdateParameterCommand
                    {
                        Action = Constants.Edit,
                        CollectionAlias = collectionAlias,
                        VariantAlias = entityVariant.Alias,
                        ParentId = parentId,
                        Id = null
                    } as ViewCommand);

                case CrudType.Create:
                    return Task.FromResult(new UpdateParameterCommand
                    {
                        Action = Constants.New,
                        CollectionAlias = collectionAlias,
                        VariantAlias = entityVariant.Alias,
                        ParentId = parentId,
                        Id = null
                    } as ViewCommand);

                default:
                    return Task.FromResult(default(ViewCommand));
            }
        }

        public async Task<ViewCommand> ProcessListEditorActionAsync(string collectionAlias, string variantAlias, string? parentId, string? id, CollectionListEditorDTO formValues, string actionId)
        {
            var collection = _root.GetCollection(collectionAlias);

            var entityVariant = !string.IsNullOrEmpty(variantAlias)
                ? collection.EntityVariants.First(variant => variant.Alias == variantAlias)
                : collection.EntityVariants.First();

            var listEditor = collection.ListEditor;
            var button = listEditor.EditorPanes.SelectMany(x => x.Buttons).GetAllButtons().First(x => x.ButtonId == actionId);
            var buttonCrudType = button.GetCrudType();

            var entity = buttonCrudType switch
            {
                CrudType.Insert => await collection.Repository._NewAsync(parentId, entityVariant.Type),
                _ => await collection.Repository._GetByIdAsync(id, parentId)
            };

            // TODO: what is id when inserting??
            var nodeFormValues = formValues.Editor.Nodes.First(x => x.Id == id);

            UpdateEntityWithFormData(nodeFormValues, entity, listEditor);

            // TODO: what to do with this action
            if (button is CustomButton customButton)
            {
                customButton.Action.Invoke();
            }

            switch (buttonCrudType)
            {
                case CrudType.View:
                    return new NavigateCommand { Uri = UriHelper.Node(Constants.View, collectionAlias, entityVariant, parentId, id) };

                case CrudType.Read:
                    return new NavigateCommand { Uri = UriHelper.Node(Constants.Edit, collectionAlias, entityVariant, parentId, id) };

                case CrudType.Update:
                    await collection.Repository._UpdateAsync(id, parentId, entity);
                    return new ReloadCommand();

                case CrudType.Insert:
                    entity = await collection.Repository._InsertAsync(parentId, entity);
                    return new UpdateParameterCommand
                    {
                        Action = Constants.New,
                        CollectionAlias = collectionAlias,
                        VariantAlias = entityVariant.Alias,
                        ParentId = parentId,
                        Id = entity.Id
                    };

                case CrudType.Delete:
                    await collection.Repository._DeleteAsync(id, parentId);
                    return new ReloadCommand();

                default:
                    return null;
            }
        }

        // TODO: these functions contain a huge flaw which requires the form to ALWAYS have EVERY property
        // TODO: create the reciprocal function to load the form with entity data
        // TODO: refactor the shit out of this function
        private static void UpdateEntityWithFormData(NodeDTO formValues, IEntity entity, ListEditor listEditor)
        {
            var formValueForNode = formValues.Values.Select((x, fieldIndex) => (fieldIndex, x.Value));
            var setters = listEditor.EditorPanes.First(x => x.VariantType == entity.GetType()).Fields
                .Select((x, fieldIndex) => x.Readonly ? (fieldIndex, null, null) : (fieldIndex, x.NodeProperty.Setter, x.ValueMapper))
                .ToDictionary(x => x.fieldIndex, x => (x.Setter, x.ValueMapper));

            foreach (var (fieldIndex, value) in formValueForNode)
            {
                var (setter, valueMapper) = setters[fieldIndex];

                setter?.Invoke(entity, valueMapper?.MapFromEditor(null, value));
            }
        }

        // TODO: these functions contain a huge flaw which requires the form to ALWAYS have EVERY property
        // TODO: create the reciprocal function to load the form with entity data
        // TODO: refactor the shit out of this function
        private static void UpdateEntityWithFormData(NodeEditorDTO formValues, IEntity entity, NodeEditor nodeEditor, Type variantType)
        {
            var enteredFormValues = formValues.EditorPanes
                .SelectMany((x, paneIndex) => x.Fields.Select((y, fieldIndex) => (paneIndex, fieldIndex, y.value.Value)));

            var panes = variantType == null
                ? nodeEditor.EditorPanes
                : nodeEditor.EditorPanes.Where(x => x.VariantType == variantType || x.VariantType == nodeEditor.BaseType);

            var setters = panes
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
