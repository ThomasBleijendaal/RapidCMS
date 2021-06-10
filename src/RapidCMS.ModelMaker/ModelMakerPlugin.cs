using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.Components.Displays;
using RapidCMS.ModelMaker.Components.Sections;
using RapidCMS.ModelMaker.Core.Abstractions.Factories;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;
using RapidCMS.ModelMaker.DataCollections;
using RapidCMS.ModelMaker.Extenstions;
using RapidCMS.ModelMaker.Metadata;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Models.Responses;
using RapidCMS.ModelMaker.Repositories;

namespace RapidCMS.ModelMaker
{
    internal class ModelMakerPlugin : IPlugin
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IModelMakerConfig _config;
        private readonly ISetupResolver<IButtonSetup, ButtonConfig> _buttonSetupResolver;
        private readonly ICommandHandler<GetAllRequest<ModelEntity>, EntitiesResponse<ModelEntity>> _getAllModelEntitiesCommandHandler;
        private readonly ICommandHandler<GetByAliasRequest<ModelEntity>, EntityResponse<ModelEntity>> _getModelEntityByAliasCommandHandler;

        public ModelMakerPlugin(
            IServiceProvider serviceProvider,
            IModelMakerConfig config,
            ISetupResolver<IButtonSetup, ButtonConfig> buttonSetupResolver,
            ICommandHandler<GetAllRequest<ModelEntity>, EntitiesResponse<ModelEntity>> getAllModelEntitiesCommandHandler,
            ICommandHandler<GetByAliasRequest<ModelEntity>, EntityResponse<ModelEntity>> getModelEntityByAliasCommandHandler)
        {
            _serviceProvider = serviceProvider;
            _config = config;
            _buttonSetupResolver = buttonSetupResolver;
            _getAllModelEntitiesCommandHandler = getAllModelEntitiesCommandHandler;
            _getModelEntityByAliasCommandHandler = getModelEntityByAliasCommandHandler;
        }

        public string CollectionPrefix => Constants.CollectionPrefix;

        public async Task<IResolvedSetup<CollectionSetup>?> GetCollectionAsync(string collectionAlias)
        {
            if (collectionAlias.EndsWith("::property"))
            {
                return new ResolvedSetup<CollectionSetup>(await PropertyConfigurationCollectionAsync(), true);
            }
            else
            {
                var response = await _getModelEntityByAliasCommandHandler.HandleAsync(new GetByAliasRequest<ModelEntity>(collectionAlias));
                if (response.Entity != null)
                {
                    return await ModelCollectionAsync(response.Entity) is CollectionSetup collection
                        ? new ResolvedSetup<CollectionSetup>(collection, false) // TODO: how to bust collection setup cache so this setup can be cached until outdated
                        : default;
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITreeElementSetup>> GetTreeElementsAsync()
        {
            var response = await _getAllModelEntitiesCommandHandler.HandleAsync(new GetAllRequest<ModelEntity>(default));
            return response.Entities
                .Where(IsValidDefintion)
                .Select(model => new TreeElementSetup(model.Alias, model.Name, PageType.Collection));
        }

        public Type? GetRepositoryType(string collectionAlias)
        {
            if (collectionAlias.EndsWith("::models"))
            {
                return typeof(ModelRepository);
            }
            else if (collectionAlias.EndsWith("::property"))
            {
                return typeof(PropertyRepository);
            }

            return typeof(ModelMakerRepository);
        }

        // MODEL MAKER

        private async Task<ICollectionSetup?> ModelCollectionAsync(ModelEntity definition)
        {
            if (!IsValidDefintion(definition))
            {
                return default;
            }

            var entityVariantSetup = new EntityVariantSetup(definition.Alias, default, typeof(ModelMakerEntity), definition.Alias);

            var titleProperty = definition.PublishedProperties.First(x => x.IsTitle);
            var titlePropertyMetadata = CreateExpressionMetadata(titleProperty);

            var collection = new CollectionSetup(
                "Database",
                "Cyan10",
                definition.Name,
                definition.Alias,
                definition.Alias)
            {
                EntityVariant = entityVariantSetup,
                UsageType = UsageType.List,
                TreeView = new TreeViewSetup(
                    EntityVisibilty.Visible,
                    CollectionRootVisibility.Visible,
                    false,
                    false,
                    titlePropertyMetadata)
            };

            collection.ListView = new ListSetup(
                100,
                false,
                false,
                ListType.Table,
                EmptyVariantColumnVisibility.Collapse,
                new List<IPaneSetup>
                {
                    new PaneSetup(
                        default,
                        default,
                        (m, s) => true,
                        typeof(ModelMakerEntity),
                        new List<IButtonSetup>  {
                            await CreateButtonAsync(collection, DefaultButtonType.Edit, true)
                        },
                        new List<IFieldSetup>
                        {
                            CreateExpressionField(DisplayType.Label, EditorType.None, 1, titleProperty.Name, titlePropertyMetadata),
                            CreateCustomExpressionField(typeof(PublishStateDisplay), 2, "", new ExpressionMetadata<ModelMakerEntity>("State", x => x.State.ToString()))
                        },
                        new List<ISubCollectionListSetup>(),
                        new List<IRelatedCollectionListSetup>())
                },
                new List<IButtonSetup>
                {
                    await CreateButtonAsync(collection, DefaultButtonType.New, true)
                });

            collection.NodeEditor = new NodeSetup(
                typeof(ModelMakerEntity),
                await ModelPanesAsync(definition, collection).ToListAsync(),
                new List<IButtonSetup>());

            return collection;
        }

        private async IAsyncEnumerable<IPaneSetup> ModelPanesAsync(ModelEntity definition, CollectionSetup collection)
        {
            yield return
                new PaneSetup(
                    typeof(ModelDetailsSection),
                    default,
                    (m, s) => true,
                    typeof(ModelMakerEntity),
                    new List<IButtonSetup>(),
                    new List<IFieldSetup>(),
                    new List<ISubCollectionListSetup>(),
                    new List<IRelatedCollectionListSetup>());

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
                    await ModelFieldsAsync(definition, collection).ToListAsync(),
                    new List<ISubCollectionListSetup>(),
                    new List<IRelatedCollectionListSetup>());
        }

        private async IAsyncEnumerable<IFieldSetup> ModelFieldsAsync(ModelEntity definition, CollectionSetup collection)
        {
            var i = 0;
            foreach (var property in definition.PublishedProperties)
            {
                var editor = _config.Editors.First(x => x.Alias == property.EditorAlias);

                yield return await CreateCustomPropertyFieldAsync(++i, property, editor, collection);
            }
        }

        private async Task<CustomPropertyFieldSetup> CreateCustomPropertyFieldAsync(int index, PropertyModel property, IPropertyEditorConfig editor, CollectionSetup collection)
        {
            var relationSetup = default(RelationSetup);

            try
            {
                var validator = _config.Validators
                    .SingleOrDefault(x => x.DataCollectionFactory != null && property.Validations.Any(v => v.Config?.IsEnabled == true && v.Alias == x.Alias));

                if (validator != null && _serviceProvider.GetService<IDataCollectionFactory>(validator.DataCollectionFactory!) is IDataCollectionFactory dataCollectionFactory)
                {
                    relationSetup = await dataCollectionFactory.GetModelRelationSetupAsync(property.GetValidation(validator.Alias).Config!);
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("A property can only have 1 enabled validator with DataCollection.", ex);
            }

            var propertyType = _config.Properties.SingleOrDefault(x => x.Alias == property.PropertyAlias)?.Type ?? typeof(object);

            var setup = new CustomPropertyFieldSetup(new FieldConfig
            {
                EditorType = EditorType.Custom,
                Index = index,
                Name = property.Name,
                Property = new PropertyMetadata<ModelMakerEntity>(
                    property.Alias,
                    propertyType,
                    entity => entity.Get(property.Alias),
                    (entity, value) => entity.Set(property.Alias, value),
                    $"{property.EditorAlias}::{property.Alias}")
            },
            editor.Editor)
            {
                Relation = relationSetup
            };

            return setup;
        }

        // MODEL EDITOR

        private async Task<CollectionSetup> PropertyConfigurationCollectionAsync()
        {
            var entityVariantSetup = new EntityVariantSetup("default", default, typeof(PropertyModel), "modelproperty");
            var collection = new CollectionSetup(
                "Database",
                "MagentaPink10",
                "Properties",
                "modelmaker::property",
                "modelmaker::property")
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
                new List<IPaneSetup>
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
                        new List<IFieldSetup>
                        {
                            CreateExpressionField(DisplayType.Label, EditorType.None, 1, "Property name", new ExpressionMetadata<PropertyModel>("Name", x => x.Name))
                        },
                        new List<ISubCollectionListSetup>(),
                        new List<IRelatedCollectionListSetup>())
                },
                new List<IButtonSetup>
                {
                    await CreateButtonAsync(collection, DefaultButtonType.Return, false, "Cancel", "Back"),
                    await CreateButtonAsync(collection, DefaultButtonType.New, true, "Add property", "Add"),
                    await CreateButtonAsync(collection, DefaultButtonType.SaveExisting, false, "Save order", "Save")
                });

            collection.NodeEditor = new NodeSetup(
                typeof(PropertyModel),
                await PropertyPanesAsync().ToListAsync(),
                new List<IButtonSetup>
                {
                    await CreateButtonAsync(collection, DefaultButtonType.Up, false, "Return", "Back"),
                    await CreateButtonAsync(collection, DefaultButtonType.SaveNew, true, "Insert"),
                    await CreateButtonAsync(collection, DefaultButtonType.SaveExisting, true, "Update"),
                    await CreateButtonAsync(collection, DefaultButtonType.Delete, false, "Delete")
                });

            return collection;
        }

        private async IAsyncEnumerable<IPaneSetup> PropertyPanesAsync()
        {
            var titleField = CreatePropertyField(EditorType.Checkbox,
                3,
                "Use as entity title",
                PropertyMetadataHelper.GetFullPropertyMetadata<PropertyModel, bool>(x => x.IsTitle));

            titleField.IsVisible = (m, s)
                => m is PropertyModel property && !string.IsNullOrEmpty(property.PropertyAlias)
                    && (_config.GetProperty(property.PropertyAlias)?.UsableAsTitle ?? false);

            yield return
                new PaneSetup(
                    default,
                    default,
                    (m, s) => true,
                    typeof(PropertyModel),
                    new List<IButtonSetup>
                    {

                    },
                    new List<IFieldSetup>
                    {
                        CreatePropertyField(EditorType.Dropdown,
                            0,
                            "Property type",
                            PropertyMetadataHelper.GetFullPropertyMetadata<PropertyModel, string?>(x => x.PropertyAlias),
                            (m, s) => s == EntityState.IsExisting,
                            relation: new DataProviderRelationSetup(typeof(PropertyTypeDataCollection))),

                        CreatePropertyField(EditorType.TextBox,
                            1,
                            "Property name",
                            PropertyMetadataHelper.GetFullPropertyMetadata<PropertyModel, string?>(x => x.Name)),

                        CreatePropertyField(EditorType.TextBox,
                            2,
                            "Property alias",
                            new PropertyMetadata<PropertyModel>("Alias", typeof(string), x => x.Alias, (x, v) => x.Alias = v as string ?? x.Name?.ToUrlFriendlyString() ?? "", "alias")),

                        titleField,

                        CreatePropertyField(EditorType.Checkbox,
                            4,
                            "Required",
                            PropertyMetadataHelper.GetFullPropertyMetadata<PropertyModel, bool>(x => x.IsRequired)),

                        CreatePropertyField(EditorType.Dropdown,
                            5,
                            "Property editor",
                            PropertyMetadataHelper.GetFullPropertyMetadata<PropertyModel, string?>(x => x.EditorAlias),
                           relation: new DataProviderRelationSetup(typeof(PropertyEditorDataCollection)))
                    },
                    new List<ISubCollectionListSetup>
                    {
                    },
                    new List<IRelatedCollectionListSetup>
                    {

                    });

            yield return
                new PaneSetup(
                    default,
                    default,
                    (m, s) => true,
                    typeof(PropertyModel),
                    new List<IButtonSetup>
                    {

                    },
                    await ValidationFieldsAsync().ToListAsync(),
                    new List<ISubCollectionListSetup>(),
                    new List<IRelatedCollectionListSetup>());
        }

        private async IAsyncEnumerable<IFieldSetup> ValidationFieldsAsync()
        {
            foreach (var validationType in _config.Validators)
            {
                var relationSetup = default(RelationSetup);

                if (validationType.DataCollectionFactory != null &&
                    _serviceProvider.GetService<IDataCollectionFactory>(validationType.DataCollectionFactory) is IDataCollectionFactory dataCollectionFactory)
                {
                    relationSetup = await dataCollectionFactory.GetModelEditorRelationSetupAsync();
                }

                yield return new CustomPropertyFieldSetup(new FieldConfig
                {
                    Description = validationType.Description,
                    EditorType = EditorType.Custom,
                    Index = 1,
                    Name = validationType.Name,
                    IsVisible = (m, s) =>
                    {
                        return m is PropertyModel property &&
                            property.Validations.FirstOrDefault(x => x.Alias == validationType.Alias) is PropertyValidationModel validation &&
                            !string.IsNullOrEmpty(property.PropertyAlias) &&
                            _config.Properties.First(x => x.Alias == property.PropertyAlias).Validators.Any(x => x.Alias == validationType.Alias) &&
                            (validation.Config?.IsApplicable(property) ?? false);
                    },
                    Property =
                        validationType.ConfigToEditor != null
                        ? validationType.ConfigToEditor.Nest<PropertyModel, PropertyValidationModel>(x => x.Validations.FirstOrDefault(x => x.Alias == validationType.Alias))
                        : new PropertyMetadata<PropertyModel>(
                            "Config",
                            typeof(IValidatorConfig),
                            x => x.Validations.FirstOrDefault(x => x.Alias == validationType.Alias)?.Config,
                            (x, v) =>
                            {
                                if (x.Validations.FirstOrDefault(x => x.Alias == validationType.Alias) is PropertyValidationModel validation)
                                {
                                    validation.Config = v as IValidatorConfig;
                                }
                            },
                            "config"),
                }, validationType.Editor)
                {
                    Relation = relationSetup
                };
            }
        }

        // COMMON

        private static ExpressionFieldSetup CreateExpressionField(
            DisplayType displayType,
            EditorType editorType,
            int index,
            string name,
            IExpressionMetadata expression)
            => new ExpressionFieldSetup(
                new FieldConfig
                {
                    DisplayType = displayType,
                    EditorType = editorType,
                    Index = index,
                    Name = name
                },
                expression);

        private static ExpressionFieldSetup CreateCustomExpressionField(
            Type displayComponent,
            int index,
            string name,
            IExpressionMetadata expression)
            => new CustomExpressionFieldSetup(
                new FieldConfig
                {
                    DisplayType = DisplayType.Custom,
                    EditorType = EditorType.None,
                    Index = index,
                    Name = name
                },
                expression, displayComponent);

        private static PropertyFieldSetup CreatePropertyField(
            EditorType editorType,
            int index,
            string name,
            IFullPropertyMetadata metadata,
            Func<object, EntityState, bool>? isDisabled = default,
            Func<object, EntityState, bool>? isVisible = default,
            RelationSetup? relation = default)
        {
            return new PropertyFieldSetup(new FieldConfig
            {
                EditorType = editorType,
                Index = index,
                IsDisabled = isDisabled ?? ((m, s) => false),
                IsVisible = isVisible ?? ((m, s) => true),
                Name = name,
                Property = metadata
            })
            {
                Relation = relation
            };
        }

        private async Task<IButtonSetup> CreateButtonAsync(
            CollectionSetup collection,
            DefaultButtonType type,
            bool isPrimary,
            string? label = default,
            string? icon = default)
        {
            var response = await _buttonSetupResolver.ResolveSetupAsync(new DefaultButtonConfig
            {
                ButtonType = type,
                Icon = icon,
                Id = $"{collection.Alias}::{type}",
                IsPrimary = isPrimary,
                Label = label,

            }, collection);

            return response.Setup;
        }

        private bool IsValidDefintion(ModelEntity entity)
            => entity.PublishedProperties.Any(x => x.IsTitle);

        private static ExpressionMetadata<ModelMakerEntity> CreateExpressionMetadata(PropertyModel property)
            => new ExpressionMetadata<ModelMakerEntity>(property.Name, x => x.Get<string>(property.Alias));
    }
}
