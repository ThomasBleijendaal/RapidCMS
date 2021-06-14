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
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.Core.Abstractions.Factories;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;
using RapidCMS.ModelMaker.DataCollections;
using RapidCMS.ModelMaker.Extenstions;
using RapidCMS.ModelMaker.Metadata;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Repositories;

namespace RapidCMS.ModelMaker
{
    internal class ModelMakerPlugin : IPlugin
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IModelMakerConfig _config;
        private readonly ISetupResolver<IButtonSetup, ButtonConfig> _buttonSetupResolver;

        public ModelMakerPlugin(
            IServiceProvider serviceProvider,
            IModelMakerConfig config,
            ISetupResolver<IButtonSetup, ButtonConfig> buttonSetupResolver)
        {
            _serviceProvider = serviceProvider;
            _config = config;
            _buttonSetupResolver = buttonSetupResolver;
        }

        public string CollectionPrefix => Constants.CollectionPrefix;

        public async Task<IResolvedSetup<CollectionSetup>?> GetCollectionAsync(string collectionAlias)
        {
            if (collectionAlias.EndsWith("::property"))
            {
                return new ResolvedSetup<CollectionSetup>(await PropertyConfigurationCollectionAsync(), true);
            }

            return default;
        }

        public Task<IEnumerable<ITreeElementSetup>> GetTreeElementsAsync() 
            => Task.FromResult(Enumerable.Empty<ITreeElementSetup>());

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

            return default;
        }

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
    }
}
