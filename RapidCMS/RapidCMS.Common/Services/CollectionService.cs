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
using RapidCMS.Common.Models.UI;

#nullable enable

namespace RapidCMS.Common.Services
{
    // TODO: split class into Collection and UI service
    // TODO: make UI service intent-aware (new, save existing etc)
    // TODO: make button handling more seperate
    // TODO: rename alias to collectionAlias
    // TODO: check applicability of every crud type (instead of inserting invalid operation exceptions everywhere)

    public interface ICollectionService
    {
        Task<CollectionTreeRootDTO> GetCollectionsAsync();

        Task<ListUI> GetCollectionListViewAsync(string action, string collectionAlias, string? variantAlias, string? parentId);
        Task<ViewCommand> ProcessListActionAsync(string action, string collectionAlias, string? parentId, string actionId);
        Task<ViewCommand> ProcessListActionAsync(string action, string collectionAlias, string? parentId, string id, string actionId, IEntity entity);

        Task<CollectionListEditorDTO> GetCollectionListEditorAsync(string action, string collectionAlias, string variantAlias, string? parentId);
        [Obsolete]
        Task<ViewCommand> ProcessListEditorActionAsync(string collectionAlias, string variantAlias, string? parentId, string? id, CollectionListEditorDTO formValues, string actionId);

        Task<EditorUI> GetNodeEditorAsync(string action, string collectionAlias, string variantAlias, string? parentId, string? id);
        Task<ViewCommand> ProcessNodeEditorActionAsync(string collectionAlias, string variantAlias, string? parentId, string? id, EditorUI editor, string actionId);
    }

    public class CollectionService : ICollectionService
    {
        private readonly Root _root;
        private readonly IUIService _uiService;

        public CollectionService(Root root, IUIService uiService)
        {
            _root = root;
            _uiService = uiService;
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
                    Name = collection.Name,
                    RootVisible = collection.TreeView?.RootVisibility == CollectionRootVisibility.Visible
                };

                if (collection.TreeView?.EntityVisibility == EntityVisibilty.Visible)
                {
                    var entities = await collection.Repository._GetAllAsObjectsAsync(parentId);

                    dto.Nodes = await entities.ToListAsync(async entity =>
                    {
                        var subCollections = collection.Collections.Any()
                            ? await GetTreeViewForCollectionAsync(collection.Collections, entity.Id)
                            : Enumerable.Empty<CollectionTreeCollectionDTO>();

                        var entityVariant = collection.GetEntityVariant(entity);

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

        public async Task<ListUI> GetCollectionListViewAsync(string action, string alias, string? variantAlias, string? parentId)
        {
            var collection = _root.GetCollection(alias);

            var entityVariant = collection.GetEntityVariant(variantAlias);
            var existingEntities = await collection.Repository._GetAllAsObjectsAsync(parentId);
            IEnumerable<EntityIntent> entities;

            if (action == Constants.New)
            {
                var newEntity = await collection.Repository._NewAsync(parentId, entityVariant.Type);

                entities = new[] {
                    new EntityIntent {
                        Entity = newEntity,
                        UsageType = MapActionToUsageType(Constants.New)
                    }
                }.Concat(existingEntities.Select(ent => new EntityIntent
                {
                    Entity = ent,
                    UsageType = MapActionToUsageType(Constants.Edit)
                }));
            }
            else
            {
                entities = existingEntities.Select(ent => new EntityIntent
                {
                    Entity = ent,
                    UsageType = MapActionToUsageType(Constants.Edit)
                });
            }

            var listViewContext = new ViewContext(UsageType.List | MapActionToUsageType(action), entityVariant);

            if (action == Constants.List)
            {
                var editor = _uiService.GenerateListUI(
                    listViewContext,
                    (intent) => new ViewContext(UsageType.View, collection.GetEntityVariant(intent.Entity)),
                    collection.ListView);

                editor.Entities = entities;
                editor.ListType = ListType.TableView;

                return editor;
            }
            else if (action.In(Constants.Edit, Constants.New))
            {
                var editor = _uiService.GenerateListUI(
                    listViewContext,
                    (intent) =>
                    {
                        return new ViewContext(UsageType.Node | intent.UsageType, collection.GetEntityVariant(intent.Entity));
                    },
                    collection.ListEditor);

                editor.Entities = entities;
                editor.ListType = collection.ListEditor.ListEditorType == ListEditorType.Table
                    ? ListType.TableEditor
                    : ListType.BlockEditor;

                return editor;
            }
            else
            {
                throw new NotImplementedException($"Cannot do {action} on GetCollectionListViewAsync");
            }
        }

        public async Task<CollectionListEditorDTO> GetCollectionListEditorAsync(string action, string alias, string variantAlias, string? parentId)
        {
            var collection = _root.GetCollection(alias);

            var listEditor = collection.ListEditor;
            var editors = listEditor.EditorPanes;

            var entityVariant = collection.GetEntityVariant(variantAlias);

            var listViewContext = new ViewContext(UsageType.List | MapActionToUsageType(action), entityVariant);

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
                            Label = button.Label,
                            ShouldConfirm = button.ShouldConfirm
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
                var variant = entityVariant ?? collection.EntityVariant;

                var newEntity = await collection.Repository._NewAsync(parentId, variant.Type);
                var editor = editors.First(x => x.VariantType == newEntity.GetType());

                var editorViewContext = new ViewContext(UsageType.Node | MapActionToUsageType(Constants.New), variant);

                yield return CreateCollectionListEditorNode(newEntity, editorViewContext, parentId, editor);
            }

            if (action.In(Constants.New, Constants.Edit))
            {
                var entities = await collection.Repository._GetAllAsObjectsAsync(parentId);

                foreach (var entity in entities)
                {
                    var entityType = entity.GetType();
                    var variant = collection.GetEntityVariant(entity);

                    var editor = editors.First(x => x.VariantType == entityType);
                    var editorViewContext = new ViewContext(UsageType.Node | MapActionToUsageType(Constants.Edit), variant);

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
                            Label = button.Label,
                            ShouldConfirm = button.ShouldConfirm
                        };
                    }),
                Values = editor.Fields.ToList(field =>
                {
                    var dto = new ValueDTO
                    {
                        Alias = field.Name.ToUrlFriendlyString(),
                        DisplayValue = field.ValueMapper.MapToView(null, field.NodeProperty.Getter.Invoke(entity)),
                        IsReadonly = field.Readonly,
                        Type = field.DataType,

                        // TODO: fix when refactor
                        Value = field.ValueMapper.MapToEditor(null, field.NodeProperty.Getter.Invoke(entity)).ToString()
                    };

                    return dto;
                })
            };
        }

        public async Task<EditorUI> GetNodeEditorAsync(string action, string alias, string variantAlias, string? parentId, string? id)
        {
            var collection = _root.GetCollection(alias);

            var entityVariant = collection.GetEntityVariant(variantAlias);

            var entity = action switch
            {
                Constants.View => await collection.Repository._GetByIdAsync(id, parentId),
                Constants.Edit => await collection.Repository._GetByIdAsync(id, parentId),
                Constants.New => await collection.Repository._NewAsync(parentId, entityVariant.Type),
                _ => null
            };

            if (entity == null)
            {
                throw new Exception("Failed to get entity for given parameters.");
            }

            if (action != Constants.New)
            {
                entityVariant = collection.GetEntityVariant(entity);
            }

            var viewContext = new ViewContext(UsageType.Node | MapActionToUsageType(action), entityVariant);
            var nodeEditor = collection.NodeEditor;

            var editor = _uiService.GenerateNodeUI(viewContext, nodeEditor);

            editor.Entity = entity;

            return editor;
        }

        public async Task<ViewCommand> ProcessNodeEditorActionAsync(string collectionAlias, string variantAlias, string? parentId, string? id, EditorUI editor, string actionId)
        {
            var collection = _root.GetCollection(collectionAlias);

            var entityVariant = collection.GetEntityVariant(variantAlias);

            var nodeEditor = collection.NodeEditor;
            var button = nodeEditor.Buttons.GetAllButtons().First(x => x.ButtonId == actionId);
            var buttonCrudType = button.GetCrudType();

            // TODO: relations must not be simply set but must be added using seperate IRepository call after update to allow for better support
            // TODO: must track which releation(s) have been broken and which have been made to allow for absolute control
            var updatedEntity = editor.Entity;



            // TODO: what to do with this action
            if (button is CustomButton customButton)
            {
                await customButton.HandleActionAsync(parentId, id);
            }

            switch (buttonCrudType)
            {
                case CrudType.View:
                    return new NavigateCommand { Uri = UriHelper.Node(Constants.View, collectionAlias, entityVariant, parentId, id) };

                case CrudType.Read:
                    return new NavigateCommand { Uri = UriHelper.Node(Constants.Edit, collectionAlias, entityVariant, parentId, id) };

                case CrudType.Update:
                    await collection.Repository._UpdateAsync(id, parentId, updatedEntity);
                    return new ReloadCommand();

                case CrudType.Insert:
                    var entity = await collection.Repository._InsertAsync(parentId, updatedEntity);
                    return new NavigateCommand { Uri = UriHelper.Node(Constants.Edit, collectionAlias, entityVariant, parentId, entity.Id) };

                case CrudType.Delete:
                    await collection.Repository._DeleteAsync(id, parentId);
                    return new NavigateCommand { Uri = UriHelper.Collection(Constants.List, collectionAlias, parentId) };

                case CrudType.None:
                    return new NullOperationCommand();

                case CrudType.Refresh:
                    return new ReloadCommand();

                case CrudType.Create:
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<ViewCommand> ProcessListActionAsync(string action, string collectionAlias, string? parentId, string actionId)
        {
            var collection = _root.GetCollection(collectionAlias);

            var buttons = action == Constants.List ? collection.ListView.Buttons : collection.ListEditor.Buttons;
            var button = buttons.GetAllButtons().First(x => x.ButtonId == actionId);
            var buttonCrudType = button.GetCrudType();

            // TODO: what to do with this action
            if (button is CustomButton customButton)
            {
                await customButton.HandleActionAsync(parentId, null);
            }

            switch (buttonCrudType)
            {
                case CrudType.Create:
                    if (!(button.Metadata is EntityVariant entityVariant))
                    {
                        throw new InvalidOperationException();
                    }

                    if (action == Constants.List)
                    {
                        return new NavigateCommand { Uri = UriHelper.Node(Constants.New, collectionAlias, entityVariant, parentId, null) };
                    }
                    else
                    {
                        return new UpdateParameterCommand
                        {
                            Action = Constants.New,
                            CollectionAlias = collectionAlias,
                            VariantAlias = entityVariant.Alias,
                            ParentId = parentId,
                            Id = null
                        };
                    }

                case CrudType.None:
                    return new NullOperationCommand();

                case CrudType.Refresh:
                    return new ReloadCommand();

                case CrudType.View:
                case CrudType.Read:
                case CrudType.Insert:
                case CrudType.Update:
                case CrudType.Delete:
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<ViewCommand> ProcessListActionAsync(string action, string collectionAlias, string? parentId, string id, string actionId, IEntity updatedEntity)
        {
            var collection = _root.GetCollection(collectionAlias);

            var buttons = action == Constants.List
                ? collection.ListView.ViewPane.Buttons
                : collection.ListEditor.EditorPanes.SelectMany(pane => pane.Buttons);
            var button = buttons.GetAllButtons().First(x => x.ButtonId == actionId);
            var buttonCrudType = button.GetCrudType();

            //// since the id is known, get the entity variant from the entity
            //var entity = await collection.Repository._GetByIdAsync(id, parentId);

            // TODO: relations must not be simply set but must be added using seperate IRepository call after update to allow for better support
            // TODO: must track which releation(s) have been broken and which have been made to allow for absolute control
            var entityVariant = collection.GetEntityVariant(updatedEntity);


            // TODO: what to do with this action
            if (button is CustomButton customButton)
            {
                await customButton.HandleActionAsync(parentId, id);
            }

            switch (buttonCrudType)
            {
                case CrudType.View:
                    return new NavigateCommand { Uri = UriHelper.Node(Constants.View, collectionAlias, entityVariant, parentId, id) };

                case CrudType.Read:
                    return new NavigateCommand { Uri = UriHelper.Node(Constants.Edit, collectionAlias, entityVariant, parentId, id) };

                case CrudType.Update:
                    await collection.Repository._UpdateAsync(id, parentId, updatedEntity);
                    return new ReloadCommand();

                case CrudType.Insert:
                    updatedEntity = await collection.Repository._InsertAsync(parentId, updatedEntity);
                    return new UpdateParameterCommand
                    {
                        Action = Constants.New,
                        CollectionAlias = collectionAlias,
                        VariantAlias = entityVariant.Alias,
                        ParentId = parentId,
                        Id = updatedEntity.Id
                    };

                case CrudType.Delete:

                    await collection.Repository._DeleteAsync(id, parentId);
                    return new ReloadCommand();

                case CrudType.None:
                    return new NullOperationCommand();

                case CrudType.Refresh:
                    return new ReloadCommand();

                case CrudType.Create:
                default:
                    throw new InvalidOperationException();
            }
        }

        [Obsolete]
        public async Task<ViewCommand> ProcessListEditorActionAsync(string collectionAlias, string variantAlias, string? parentId, string? id, CollectionListEditorDTO formValues, string actionId)
        {
            var collection = _root.GetCollection(collectionAlias);

            var entityVariant = collection.GetEntityVariant(variantAlias);

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
                await customButton.HandleActionAsync(parentId, id);
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

                case CrudType.None:
                    return new NullOperationCommand();

                case CrudType.Refresh:
                    return new ReloadCommand();

                case CrudType.Create:
                default:
                    throw new InvalidOperationException();
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
