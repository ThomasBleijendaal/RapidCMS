using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Handlers;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.Abstractions.Validation;
using RapidCMS.ModelMaker.DataCollections;
using RapidCMS.ModelMaker.Metadata;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Repositories;

namespace RapidCMS.ModelMaker
{
    internal class ModelMakerPlugin : IPlugin
    {
        private readonly IModelMakerConfig _config;
        private readonly ISetupResolver<IButtonSetup, ButtonConfig> _buttonSetupResolver;

        public ModelMakerPlugin(
            IModelMakerConfig config,
             ISetupResolver<IButtonSetup, ButtonConfig> buttonSetupResolver)
        {
            _config = config;
            _buttonSetupResolver = buttonSetupResolver;
        }

        public string CollectionPrefix => "modelmaker";

        // TODO: async
        public CollectionSetup? GetCollection(string collectionAlias)
        {
            if (collectionAlias == "property")
            {
                return PropertyConfigurationCollection();
            }
            else if (collectionAlias == "validation")
            {
                return ValidationConfigurationCollection();
            }
            else
            {
                var model = ConfigurationExtensions.MODELS.FirstOrDefault(model => model.Alias == collectionAlias);
                if (model != null)
                {
                    return ModelCollection(model);
                }
            }

            return default;
        }

        // TODO: async
        public IEnumerable<ITreeElementSetup> GetTreeElements()
        {
            return ConfigurationExtensions.MODELS.Select(model => new TreeElementSetup($"{CollectionPrefix}::{model.Alias}", PageType.Collection));
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

        private CollectionSetup ModelCollection(ModelEntity definition)
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
                        new List<IButtonSetup>
                        {
                            _buttonSetupResolver.ResolveSetup(new DefaultButtonConfig {
                                ButtonType = DefaultButtonType.Edit,
                                Id = "model-edit",
                                IsPrimary = true,
                                Label = "Edit"
                            }, collection).Setup
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
                    _buttonSetupResolver.ResolveSetup(new DefaultButtonConfig {
                        ButtonType = DefaultButtonType.New,
                        Id = "model-new",
                        IsPrimary = true,
                        Label = "New"
                    }, collection).Setup
                });

            collection.NodeEditor = new NodeSetup(
                typeof(ModelMakerEntity),
                ModelPanes(definition, collection).ToList(),
                new List<IButtonSetup>
                {

                });

            return collection;
        }

        private IEnumerable<PaneSetup> ModelPanes(ModelEntity definition, CollectionSetup collection)
        {
            yield return
                new PaneSetup(
                    default,
                    default,
                    (m, s) => true,
                    typeof(ModelMakerEntity),
                    new List<IButtonSetup>
                    {
                        _buttonSetupResolver.ResolveSetup(new DefaultButtonConfig {
                            ButtonType = DefaultButtonType.SaveExisting,
                            Id = "model-save-existing",
                            IsPrimary = true,
                            Label = "Update"
                        }, collection).Setup,
                        _buttonSetupResolver.ResolveSetup(new DefaultButtonConfig {
                            ButtonType = DefaultButtonType.SaveNew,
                            Id = "model-save-new",
                            IsPrimary = true,
                            Label = "Insert"
                        }, collection).Setup,
                        _buttonSetupResolver.ResolveSetup(new DefaultButtonConfig {
                            ButtonType = DefaultButtonType.Delete,
                            Id = "model-delete",
                            Label = "Delete"
                        }, collection).Setup
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

        private CollectionSetup PropertyConfigurationCollection()
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
                            _buttonSetupResolver.ResolveSetup(new DefaultButtonConfig {
                                ButtonType = DefaultButtonType.Edit,
                                Id = "property-edit"
                            }, collection).Setup
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
                    _buttonSetupResolver.ResolveSetup(new DefaultButtonConfig {
                        ButtonType = DefaultButtonType.Up,
                        Id = "property-up",
                        Icon = "Back",
                        Label = "Return"
                    }, collection).Setup,
                    _buttonSetupResolver.ResolveSetup(new DefaultButtonConfig {
                        ButtonType = DefaultButtonType.SaveNew,
                        Id = "property-save-new",
                        IsPrimary = true,
                        Label = "Insert"
                    }, collection).Setup,
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
    }
}
