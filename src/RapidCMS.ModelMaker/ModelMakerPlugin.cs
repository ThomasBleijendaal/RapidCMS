using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Handlers;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.Abstractions.Validation;
using RapidCMS.ModelMaker.DataCollections;
using RapidCMS.ModelMaker.Metadata;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Models.Responses;
using RapidCMS.ModelMaker.Repositories;

namespace RapidCMS.ModelMaker
{
    internal class ModelMakerPlugin : IPlugin
    {
        private readonly IModelMakerConfig _config;
        private readonly ISetupResolver<IButtonSetup, ButtonConfig> _buttonSetupResolver;
        private readonly ICommandHandler<GetAllRequest<ModelEntity>, EntitiesResponse<ModelEntity>> _getAllModelEntitiesCommandHandler;
        private readonly ICommandHandler<GetByAliasRequest<ModelEntity>, EntityResponse<ModelEntity>> _getModelEntityByAliasCommandHandler;

        public ModelMakerPlugin(
            IModelMakerConfig config,
             ISetupResolver<IButtonSetup, ButtonConfig> buttonSetupResolver,
             ICommandHandler<GetAllRequest<ModelEntity>, EntitiesResponse<ModelEntity>> getAllModelEntitiesCommandHandler,
             ICommandHandler<GetByAliasRequest<ModelEntity>, EntityResponse<ModelEntity>> getModelEntityByAliasCommandHandler)
        {
            _config = config;
            _buttonSetupResolver = buttonSetupResolver;
            _getAllModelEntitiesCommandHandler = getAllModelEntitiesCommandHandler;
            _getModelEntityByAliasCommandHandler = getModelEntityByAliasCommandHandler;
        }

        public string CollectionPrefix => "modelmaker";

        public async Task<CollectionSetup?> GetCollectionAsync(string collectionAlias)
        {
            if (collectionAlias == "property")
            {
                return await PropertyConfigurationCollectionAsync();
            }
            else if (collectionAlias == "validation")
            {
                return ValidationConfigurationCollection();
            }
            else
            {
                var response = await _getModelEntityByAliasCommandHandler.HandleAsync(new GetByAliasRequest<ModelEntity>(collectionAlias));
                if (response.Entity != null)
                {
                    return await ModelCollectionAsync(response.Entity);
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITreeElementSetup>> GetTreeElementsAsync()
        {
            var response = await _getAllModelEntitiesCommandHandler.HandleAsync(new GetAllRequest<ModelEntity>());
            return response.Entities.Select(model => new TreeElementSetup($"{CollectionPrefix}::{model.Alias}", PageType.Collection));
        }

        public Type? GetRepositoryType(string collectionAlias)
        {
            if (collectionAlias == "models")
            {
                return typeof(ModelRepository);
            }
            else if (collectionAlias == "property")
            {
                return typeof(PropertyRepository);
            }
            else if (collectionAlias == "validation")
            {
                return typeof(ValidationRepository);
            }

            return typeof(ModelMakerRepository);
        }

        private async Task<CollectionSetup> ModelCollectionAsync(ModelEntity definition)
        {
            var entityVariantSetup = new EntityVariantSetup(definition.Alias, default, typeof(ModelMakerEntity), definition.Alias);
            var collection = new CollectionSetup(
                "Database",
                "Cyan10",
                definition.Name,
                $"modelmaker::{definition.Alias}",
                $"modelmaker::{definition.Alias}",
                false,
                false)
            {
                EntityVariant = entityVariantSetup,
                UsageType = UsageType.List,
                TreeView = new TreeViewSetup(
                    EntityVisibilty.Visible,
                    CollectionRootVisibility.Visible,
                    false,
                    false,
                    new ExpressionMetadata<ModelMakerEntity>("Name", x => x.Id))
            };

            var titleProperty = definition.Properties.First(x => x.IsTitle);

            collection.ListView = new ListSetup(
                100,
                false,
                false,
                ListType.Table,
                EmptyVariantColumnVisibility.Collapse,
                new List<PaneSetup>
                {
                    new PaneSetup(
                        default,
                        default,
                        (m, s) => true,
                        typeof(ModelMakerEntity),
                        new List<IButtonSetup>  {
                            await CreateButtonAsync(collection, DefaultButtonType.Edit, true)
                        },
                        new List<FieldSetup>
                        {
                            new ExpressionFieldSetup(new FieldConfig
                            {
                                DisplayType = DisplayType.Label,
                                Name = titleProperty.Name
                            }, new ExpressionMetadata<ModelMakerEntity>(titleProperty.Name, x => x.Get<string>(titleProperty.Alias)))
                        },
                        new List<SubCollectionListSetup>(),
                        new List<RelatedCollectionListSetup>())
                },
                new List<IButtonSetup>
                {
                    (await _buttonSetupResolver.ResolveSetupAsync(new DefaultButtonConfig {
                        ButtonType = DefaultButtonType.New,
                        Id = "model-new",
                        IsPrimary = true,
                        Label = "New"
                    }, collection)).Setup
                });

            collection.NodeEditor = new NodeSetup(
                typeof(ModelMakerEntity),
                await ModelPanesAsync(definition, collection).ToListAsync(),
                new List<IButtonSetup>
                {

                });

            return collection;
        }



        private async IAsyncEnumerable<PaneSetup> ModelPanesAsync(ModelEntity definition, CollectionSetup collection)
        {
            yield return
                new PaneSetup(
                    default,
                    default,
                    (m, s) => true,
                    typeof(ModelMakerEntity),
                    new List<IButtonSetup>
                    {
                        await CreateButtonAsync(collection, DefaultButtonType.SaveExisting, true, "Update"),
                        await CreateButtonAsync(collection, DefaultButtonType.SaveNew, true, "Insert"),
                        await CreateButtonAsync(collection, DefaultButtonType.Delete, false)
                    },
                    ModelFields(definition).ToList(),
                    new List<SubCollectionListSetup>(),
                    new List<RelatedCollectionListSetup>());
        }

        private IEnumerable<FieldSetup> ModelFields(ModelEntity definition)
        {
            var i = 0;
            foreach (var property in definition.Properties)
            {
                var editor = _config.Editors.First(x => x.Alias == property.EditorAlias);

                yield return new CustomPropertyFieldSetup(new FieldConfig
                {
                    EditorType = EditorType.Custom,
                    Index = ++i,
                    Name = property.Name,
                    Property = new PropertyMetadata<ModelMakerEntity, object?>(
                        property.Alias,
                        entity => entity.Get(property.Alias),
                        (entity, value) => entity.Set(property.Alias, value),
                        $"{property.EditorAlias}::{property.Alias}")
                }, editor.Editor);
            }
        }

        private CollectionSetup ValidationConfigurationCollection()
        {
            var entityVariantSetup = new EntityVariantSetup("default", default, typeof(PropertyValidationModel), "modelpropertyvalidation");
            return new CollectionSetup(
                "Database",
                "MagentaPink20",
                "Validation",
                "modelmaker::validation",
                "modelmaker::validation",
                false,
                false)
            {
                EntityVariant = entityVariantSetup,
                UsageType = UsageType.List,
                TreeView = new TreeViewSetup(
                    EntityVisibilty.Visible,
                    CollectionRootVisibility.Visible,
                    false,
                    false,
                    new ExpressionMetadata<PropertyValidationModel>("Alias", x => x.Alias)),
                ListEditor = new ListSetup(
                    100,
                    false,
                    false,
                    ListType.Block,
                    EmptyVariantColumnVisibility.Collapse,
                    ValidationPanes().ToList(),
                    new List<IButtonSetup>
                    {

                    })
            };
        }

        private IEnumerable<PaneSetup> ValidationPanes()
        {
            foreach (var validationType in _config.Validators)
            {
                yield return
                    new PaneSetup(
                        default,
                        default,
                        (m, s) => s == EntityState.IsExisting &&
                            m is PropertyValidationModel v && v.Alias == validationType.Alias,
                        typeof(PropertyValidationModel<>).MakeGenericType(validationType.Config),
                        new List<IButtonSetup>
                        {

                        },
                        new List<FieldSetup>
                        {
                            new CustomPropertyFieldSetup(new FieldConfig
                            {
                                Description = validationType.Description,
                                EditorType = EditorType.Custom,
                                Index = 1,
                                Name = validationType.Name,
                                Property = validationType.ConfigToEditor
                                    ?? new PropertyMetadata<PropertyValidationModel, IValidatorConfig>(
                                        "Config",
                                         x => x.Config,
                                        (x, v) => x.Config = v,
                                        "config"),
                            }, validationType.Editor),
                        },
                        new List<SubCollectionListSetup>(),
                        new List<RelatedCollectionListSetup>());
            }
        }

        private async Task<CollectionSetup> PropertyConfigurationCollectionAsync()
        {
            var entityVariantSetup = new EntityVariantSetup("default", default, typeof(PropertyModel), "modelproperty");
            var collection = new CollectionSetup(
                "Database",
                "MagentaPink10",
                "Properties",
                "modelmaker::property",
                "modelmaker::property",
                false,
                false)
            {
                EntityVariant = entityVariantSetup,
                UsageType = UsageType.List,
                TreeView = new TreeViewSetup(
                    EntityVisibilty.Visible,
                    CollectionRootVisibility.Visible,
                    false,
                    false,
                    new ExpressionMetadata<ModelEntity>("Name", x => x.Name ?? string.Empty))
            };
            collection.ListEditor = new ListSetup(
                100,
                false,
                true,
                ListType.Table,
                EmptyVariantColumnVisibility.Collapse,
                new List<PaneSetup>
                {
                    new PaneSetup(
                        default,
                        default,
                        (m, s) => true,
                        typeof(PropertyModel),
                        new List<IButtonSetup>
                        {
                            await CreateButtonAsync(collection, DefaultButtonType.Edit, true)
                        },
                        new List<FieldSetup>
                        {
                            new ExpressionFieldSetup(new FieldConfig
                            {
                                DisplayType = DisplayType.Label,
                                EditorType = EditorType.None,
                                Index = 1,
                                Name = "Property name"
                            }, new ExpressionMetadata<PropertyModel>("Name", x => x.Name)),
                        },
                        new List<SubCollectionListSetup>(),
                        new List<RelatedCollectionListSetup>())
                },
                new List<IButtonSetup>
                {
                    new ButtonSetup
                    {
                        ButtonHandlerType = typeof(DefaultButtonActionHandler),
                        ButtonId = "property-return",
                        DefaultButtonType = DefaultButtonType.Return,
                        Icon = "Back",
                        Label = "Cancel",
                        Buttons = Enumerable.Empty<IButtonSetup>(),
                        EntityVariant = entityVariantSetup
                    },
                    new ButtonSetup
                    {
                        ButtonHandlerType = typeof(DefaultButtonActionHandler),
                        ButtonId = "property-new",
                        DefaultButtonType = DefaultButtonType.New,
                        Icon = "Add",
                        IsPrimary = true,
                        Label = "Add property",
                        Buttons = Enumerable.Empty<IButtonSetup>(),
                        EntityVariant = entityVariantSetup
                    }
                });

            collection.NodeEditor = new NodeSetup(
                typeof(PropertyModel),
                PropertyPanes().ToList(),
                new List<IButtonSetup>
                {
                    await CreateButtonAsync(collection, DefaultButtonType.Up, false, "Return", "Back"),
                    await CreateButtonAsync(collection, DefaultButtonType.SaveNew, true, "Insert")
                });

            return collection;
        }

        private IEnumerable<PaneSetup> PropertyPanes()
        {
            yield return
                new PaneSetup(
                    default,
                    default,
                    (m, s) => true,
                    typeof(PropertyModel),
                    new List<IButtonSetup>
                    {

                    },
                    new List<FieldSetup>
                    {
                        new PropertyFieldSetup(new FieldConfig
                        {
                            EditorType = EditorType.Dropdown,
                            Index = 0,
                            IsDisabled = (m, s) => s == EntityState.IsExisting,
                            IsVisible = (m, s) => true,
                            Name = "Property type",
                            Property = new PropertyMetadata<PropertyModel, string>(
                                "PropertyAlias",
                                x => x.PropertyAlias,
                                (x, v) => x.PropertyAlias = v,
                                "propertyalias")
                        })
                        {
                            Relation = new DataProviderRelationSetup(typeof(PropertyTypeDataCollection))
                        },
                        new PropertyFieldSetup(new FieldConfig
                        {
                            EditorType = EditorType.TextBox,
                            Index = 1,
                            Name = "Property name",
                            Property = new PropertyMetadata<PropertyModel, string>(
                                "Name",
                                x => x.Name,
                                (x, v) => x.Name = v,
                                "name")
                        }),
                        new PropertyFieldSetup(new FieldConfig
                        {
                            EditorType = EditorType.Dropdown,
                            Index = 2,
                            Name = "Property editor",
                            Property = new PropertyMetadata<PropertyModel, string>(
                                "EditorAlias",
                                x => x.EditorAlias,
                                (x, v) => x.EditorAlias = v,
                                "editoralias")
                        })
                        {
                            Relation = new DataProviderRelationSetup(typeof(PropertyEditorDataCollection))
                        }
                    },
                    new List<SubCollectionListSetup>
                    {
                        new SubCollectionListSetup(3, "modelmaker::validation")
                    },
                    new List<RelatedCollectionListSetup>
                    {

                    });

        }

        private async Task<IButtonSetup> CreateButtonAsync(CollectionSetup collection, DefaultButtonType type, bool isPrimary, string? label = default, string? icon = default)
        {
            var response = (await _buttonSetupResolver.ResolveSetupAsync(new DefaultButtonConfig
            {
                ButtonType = type,
                Icon = icon,
                Id = Guid.NewGuid().ToString(),
                IsPrimary = isPrimary,
                Label = label,

            }, collection));

            return response.Setup;
        }
    }
}
