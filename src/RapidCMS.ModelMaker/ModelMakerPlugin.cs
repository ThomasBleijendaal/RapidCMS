using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Attributes;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Handlers;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.Abstractions.Detail;
using RapidCMS.ModelMaker.DataCollections;
using RapidCMS.ModelMaker.Extenstions;
using RapidCMS.ModelMaker.Metadata;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.UI.Components.Editors;

namespace RapidCMS.ModelMaker
{
    internal class ModelMakerPlugin : IPlugin
    {
        private readonly IModelMakerConfig _config;

        public ModelMakerPlugin(IModelMakerConfig config)
        {
            _config = config;
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
                return _config.ModelRespository;
            }
            else if (collectionAlias.EndsWith("::property"))
            {
                return _config.PropertyRepository;
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
                PropertyPanes().ToList(),
                new List<IButtonSetup>
                {
                    await CreateButtonAsync(collection, DefaultButtonType.Up, false, "Return", "Back"),
                    await CreateButtonAsync(collection, DefaultButtonType.SaveNew, true, "Insert"),
                    await CreateButtonAsync(collection, DefaultButtonType.SaveExisting, true, "Update"),
                    await CreateButtonAsync(collection, DefaultButtonType.Delete, false, "Delete")
                });

            return collection;
        }

        private IEnumerable<IPaneSetup> PropertyPanes()
        {
            var titleField = CreatePropertyField(EditorType.Checkbox,
                2,
                "Use as entity title",
                PropertyMetadataHelper.GetFullPropertyMetadata<PropertyModel, bool>(x => x.IsTitle));

            titleField.IsVisible = (m, s)
                => m is PropertyModel property && !string.IsNullOrEmpty(property.PropertyAlias)
                    && (_config.GetProperty(property.PropertyAlias)?.UsableAsTitle ?? false);

            // TODO: hide when collection is ListEditor
            var listViewField = CreatePropertyField(EditorType.Checkbox,
                3,
                "Include in list view",
                PropertyMetadataHelper.GetFullPropertyMetadata<PropertyModel, bool>(x => x.IncludeInListView));

            listViewField.IsVisible = (m, s) => m is not PropertyModel prop || !prop.IsTitle;

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
                            relation: new DataProviderRelationSetup(typeof(PropertyTypeDataCollection), default)),

                        CreatePropertyField(EditorType.TextBox,
                            1,
                            "Property name",
                            PropertyMetadataHelper.GetFullPropertyMetadata<PropertyModel, string?>(x => x.Name)),

                        titleField,

                        listViewField,

                        CreatePropertyField(EditorType.Checkbox,
                            4,
                            "Required",
                            PropertyMetadataHelper.GetFullPropertyMetadata<PropertyModel, bool>(x => x.IsRequired)),

                        CreatePropertyField(EditorType.Dropdown,
                            5,
                            "Property editor",
                            PropertyMetadataHelper.GetFullPropertyMetadata<PropertyModel, string?>(x => x.EditorAlias),
                           relation: new DataProviderRelationSetup(typeof(PropertyEditorDataCollection), default))
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
                    ValidationFields().ToList(),
                    new List<ISubCollectionListSetup>(),
                    new List<IRelatedCollectionListSetup>());
        }

        private IEnumerable<IFieldSetup> ValidationFields()
        {
            foreach (var validationType in _config.PropertyDetails)
            {
                var relationSetup = default(RelationSetup);

                if (validationType.DataCollection != null)
                {
                    relationSetup = new DataProviderRelationSetup(validationType.DataCollection, default);
                }

                yield return new CustomPropertyFieldSetup(new FieldConfig
                {
                    Description = validationType.Description,
                    EditorType = EditorType.Custom,
                    Index = 1,
                    Name = validationType.Name,
                    IsVisible = validationType.Editor == null
                        ? (m, s) => false
                        : (m, s) => m is PropertyModel property &&
                            property.Details.FirstOrDefault(x => x.Alias == validationType.Alias) is PropertyDetailModel validation &&
                            !string.IsNullOrEmpty(property.PropertyAlias) &&
                            _config.Properties.First(x => x.Alias == property.PropertyAlias).Details.Any(x => x.Alias == validationType.Alias) &&
                            (validation.Config?.IsApplicable(property) ?? false),
                    Property =
                        validationType.ConfigToEditor != null
                        ? validationType.ConfigToEditor.Nest<PropertyModel, PropertyDetailModel>(x => x.Details.FirstOrDefault(x => x.Alias == validationType.Alias))
                        : new PropertyMetadata<PropertyModel>(
                            "Config",
                            typeof(IDetailConfig),
                            x => x.Details.FirstOrDefault(x => x.Alias == validationType.Alias)?.Config,
                            (x, v) =>
                            {
                                if (x.Details.FirstOrDefault(x => x.Alias == validationType.Alias) is PropertyDetailModel validation)
                                {
                                    validation.Config = v as IDetailConfig;
                                }
                            },
                            "config"),
                }, validationType.Editor ?? typeof(TextBoxEditor))
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

        private Task<IButtonSetup> CreateButtonAsync(
            CollectionSetup collection,
            DefaultButtonType type,
            bool isPrimary,
            string? label = default,
            string? icon = default)
        {
            var @default = type.GetCustomAttribute<DefaultIconLabelAttribute>();

            var button = new ButtonSetup
            {
                Buttons = Enumerable.Empty<IButtonSetup>(),

                Label = label ?? @default?.Label ?? "Button",
                Icon = icon ?? @default?.Icon ?? "",
                IsPrimary = isPrimary,

                ButtonId = Guid.NewGuid().ToString(),
                EntityVariant = collection.EntityVariant,

                DefaultButtonType = type,

                ButtonHandlerType = typeof(DefaultButtonActionHandler)
            };

            return Task.FromResult<IButtonSetup>(button);
        }
    }
}
