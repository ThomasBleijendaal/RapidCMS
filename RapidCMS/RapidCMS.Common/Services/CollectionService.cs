using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.DTOs;
using RapidCMS.Common.Models.UI;

#nullable enable

namespace RapidCMS.Common.Services
{

    internal class CollectionService : ICollectionService
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
        // TODO: convert to proper UI model
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

            var subEntityVariant = collection.GetEntityVariant(variantAlias);
            var existingEntities = await collection.Repository._GetAllAsObjectsAsync(parentId);
            IEnumerable<UISubject> entities;

            if (action == Constants.New)
            {
                var newEntity = await collection.Repository._NewAsync(parentId, subEntityVariant.Type);

                entities = new[] {
                    new UISubject {
                        Entity = newEntity,
                        UsageType = MapActionToUsageType(Constants.New)
                    }
                }.Concat(existingEntities.Select(ent => new UISubject
                {
                    Entity = ent,
                    UsageType = MapActionToUsageType(Constants.Edit)
                }));
            }
            else
            {
                entities = existingEntities.Select(ent => new UISubject
                {
                    Entity = ent,
                    UsageType = MapActionToUsageType(Constants.Edit)
                });
            }

            var listViewContext = new ViewContext(UsageType.List | MapActionToUsageType(action), collection.EntityVariant);

            if (action == Constants.List)
            {
                var editor = _uiService.GenerateListUI(
                    listViewContext,
                    (subject) => new ViewContext(UsageType.View | UsageType.Node, collection.GetEntityVariant(subject.Entity)),
                    collection.ListView);

                editor.Entities = entities;
                editor.ListType = ListType.TableView;

                return editor;
            }
            else if (action.In(Constants.Edit, Constants.New))
            {
                var editor = _uiService.GenerateListUI(
                    listViewContext,
                    (subject) =>
                    {
                        return new ViewContext(UsageType.Node | subject.UsageType, collection.GetEntityVariant(subject.Entity));
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

        public async Task<NodeUI> GetNodeEditorAsync(string action, string alias, string variantAlias, string? parentId, string? id)
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

            var node = _uiService.GenerateNodeUI(viewContext, nodeEditor);

            node.Subject = new UISubject
            {
                Entity = entity,
                UsageType = MapActionToUsageType(action)
            };

            return node;
        }

        public async Task<ViewCommand> ProcessNodeEditorActionAsync(string collectionAlias, string variantAlias, string? parentId, string? id, NodeUI node, string actionId, object? customData)
        {
            var collection = _root.GetCollection(collectionAlias);

            var entityVariant = collection.GetEntityVariant(variantAlias);

            var nodeEditor = collection.NodeEditor;
            var button = nodeEditor.Buttons.GetAllButtons().First(x => x.ButtonId == actionId);
            var buttonCrudType = button.GetCrudType();

            // TODO: relations must not be simply set but must be added using seperate IRepository call after update to allow for better support
            // TODO: must track which releation(s) have been broken and which have been made to allow for absolute control
            var updatedEntity = node.Subject.Entity;



            // TODO: what to do with this action
            if (button is CustomButton customButton)
            {
                await customButton.HandleActionAsync(parentId, id, customData);
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

        public async Task<ViewCommand> ProcessListActionAsync(string action, string collectionAlias, string? parentId, string actionId, object? customData)
        {
            var collection = _root.GetCollection(collectionAlias);

            var buttons = action == Constants.List ? collection.ListView.Buttons : collection.ListEditor.Buttons;
            var button = buttons.GetAllButtons().First(x => x.ButtonId == actionId);
            var buttonCrudType = button.GetCrudType();

            // TODO: what to do with this action
            if (button is CustomButton customButton)
            {
                await customButton.HandleActionAsync(parentId, null, customData);
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

        public async Task<ViewCommand> ProcessListActionAsync(string action, string collectionAlias, string? parentId, string id, string actionId, IEntity updatedEntity, object? customData)
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
                await customButton.HandleActionAsync(parentId, id, customData);
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
    }
}
