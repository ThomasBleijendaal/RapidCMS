using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Handlers;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;
using RapidCMS.ModelMaker.Abstractions.Config;

namespace RapidCMS.ModelMaker
{
    internal class ModelMakerPlugin : IPlugin
    {
        private readonly IModelMakerConfig _config;

        public ModelMakerPlugin(IModelMakerConfig config)
        {
            _config = config;
        }

        public string CollectionPrefix => "modelmaker";

        // TODO: async?
        public CollectionSetup? GetCollection(string collectionAlias)
        {
            //if (collectionAlias == "dynamic")
            //{
            //    return _dynamic;
            //}

            if (collectionAlias == "property")
            {
                return PropertyConfigurationCollection();
            }
            else if (collectionAlias == "validation")
            {
                return ValidationConfigurationCollection();
            }

            return null;
        }

        // TODO: async?
        public IEnumerable<ITreeElementSetup> GetTreeElements()
        {
            return new ITreeElementSetup[]
            {
                // TODO: prefix should be added automatically
                //new TreeElementSetup(_dynamic.Alias, PageType.Collection)
                //{
                //    RootVisibility = CollectionRootVisibility.Visible
                //}

                //new TreeElementSetup(ModelConfigurationCollection().Alias, PageType.Collection)
                //{
                //    RootVisibility = CollectionRootVisibility.Visible
                //}
            };
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
                        typeof(PropertyValidationModel),
                        new List<IButtonSetup>
                        {

                        },
                        new List<FieldSetup>
                        {
                            new PropertyFieldSetup(new FieldConfig
                            {
                                EditorType = EditorType.TextBox,
                                Index = 1,
                                IsDisabled = (m, s) => false,
                                IsVisible = (m, s) => true,
                                Name = "Alias",
                                Property = new PropertyMetadata<PropertyValidationModel, string>(
                                    "Alias",
                                    x => x.Alias,
                                    (x, v) => x.Alias = v,
                                    "name")
                            }),
                            new CustomPropertyFieldSetup(new FieldConfig
                            {
                                Description = validationType.Description,
                                EditorType = EditorType.Custom,
                                Index = 2,
                                IsDisabled = (m, s) => false,
                                IsVisible = (m, s) => true,
                                Name = validationType.Name,
                                Property = new PropertyMetadata<PropertyValidationModel, IValidatorConfig>(
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
            return new CollectionSetup(
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
                    new ExpressionMetadata<ModelEntity>("Name", x => x.Name ?? string.Empty)),
                ListEditor = new ListSetup(
                    100,
                    false,
                    false,
                    ListType.Block,
                    EmptyVariantColumnVisibility.Collapse,
                    PropertyPanes().ToList(),
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
                    })
            };
        }

        private IEnumerable<PaneSetup> PropertyPanes()
        {
            // TODO: property alias selector when property is new 

            foreach (var propertyType in _config.Properties)
            {
                yield return
                    new PaneSetup(
                        default,
                        default,
                        (m, s) => s == EntityState.IsExisting &&
                            m is PropertyModel p && p.PropertyAlias == propertyType.Alias,
                        typeof(PropertyModel),
                        new List<IButtonSetup>
                        {

                        },
                        new List<FieldSetup>
                        {
                            new PropertyFieldSetup(new FieldConfig
                            {
                                Description = propertyType.Name,
                                EditorType = EditorType.TextBox,
                                Index = 1,
                                IsDisabled = (m, s) => false,
                                IsVisible = (m, s) => true,
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
                                IsDisabled = (m, s) => false,
                                IsVisible = (m, s) => true,
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


        //private CollectionSetup _dynamic = new CollectionSetup(
        //    "Database",
        //    "Gray100",
        //    "Dynamic Config",
        //    $"modelmaker::dynamic",
        //    $"modelmaker::dynamic",
        //    false,
        //    false)
        //{
        //    // TODO: use existing setup resolvers to convert more simple config to complex setup easily

        //    EntityVariant = new EntityVariantSetup("default", default, typeof(ModelMakerEntity), "modelmaker"),
        //    UsageType = UsageType.List,
        //    TreeView = new TreeViewSetup(
        //        EntityVisibilty.Visible,
        //        CollectionRootVisibility.Visible,
        //        false,
        //        false,
        //        new ModelMakerEntityExpressionMetadata(
        //            "Name",
        //            x => x.Get<string>("Name"))),
        //    NodeEditor = new NodeSetup(
        //        typeof(ModelMakerEntity),
        //        new List<PaneSetup>
        //        {
        //            new PaneSetup(
        //                default,
        //                "Dynamic",
        //                (m, s) => true,
        //                typeof(ModelMakerEntity),
        //                new List<IButtonSetup>
        //                {

        //                },
        //                new List<FieldSetup>
        //                {
        //                    new PropertyFieldSetup(new FieldConfig
        //                    {
        //                        Description = "Dynamic Name",
        //                        EditorType = EditorType.TextBox,
        //                        Index = 1,
        //                        IsDisabled = (m, s) => false,
        //                        IsVisible = (m, s) => true,
        //                        Name = "Name",
        //                        Placeholder = "Dynamic name",
        //                        Property = new ModelMakerEntityPropertyMetadata(
        //                            typeof(string),
        //                            "Name",
        //                            x => x.Get("Name"),
        //                            (x, v) => x.Set("Name", v),
        //                            "DynamicName")
        //                    })
        //                },
        //                new List<SubCollectionListSetup>(),
        //                new List<RelatedCollectionListSetup>())
        //        },
        //        new List<IButtonSetup>
        //        {
        //            new ButtonSetup
        //            {
        //                ButtonHandlerType = typeof(DefaultButtonActionHandler),
        //                ButtonId = "modelmaker-save-new",
        //                Buttons = Enumerable.Empty<IButtonSetup>(),
        //                DefaultButtonType = DefaultButtonType.SaveNew,
        //                Icon = "Save",
        //                IsPrimary = true,
        //                Label = "Insert"
        //            },
        //            new ButtonSetup
        //            {
        //                ButtonHandlerType = typeof(DefaultButtonActionHandler),
        //                ButtonId = "modelmaker-save-existing",
        //                Buttons = Enumerable.Empty<IButtonSetup>(),
        //                DefaultButtonType = DefaultButtonType.SaveExisting,
        //                Icon = "Save",
        //                IsPrimary = true,
        //                Label = "Update"
        //            },
        //            new ButtonSetup
        //            {
        //                ButtonHandlerType = typeof(DefaultButtonActionHandler),
        //                ButtonId = "modelmaker-delete",
        //                Buttons = Enumerable.Empty<IButtonSetup>(),
        //                DefaultButtonType = DefaultButtonType.Delete,
        //                Icon = "Delete",
        //                Label = "Delete"
        //            }
        //        }),
        //    ListEditor = new ListSetup(
        //        100,
        //        false,
        //        false,
        //        ListType.Table,
        //        EmptyVariantColumnVisibility.Collapse,
        //        new List<PaneSetup>
        //        {
        //            new PaneSetup(
        //                default,
        //                "Dynamic",
        //                (m, s) => true,
        //                typeof(ModelMakerEntity),
        //                new List<IButtonSetup>
        //                {
        //                    new ButtonSetup
        //                    {
        //                        ButtonHandlerType = typeof(DefaultButtonActionHandler),
        //                        ButtonId = "modelmaker-save-new",
        //                        Buttons = Enumerable.Empty<IButtonSetup>(),
        //                        DefaultButtonType = DefaultButtonType.SaveNew,
        //                        Icon = "Save",
        //                        IsPrimary = true,
        //                        Label = "Insert"
        //                    },
        //                    new ButtonSetup
        //                    {
        //                        ButtonHandlerType = typeof(DefaultButtonActionHandler),
        //                        ButtonId = "modelmaker-save-existing",
        //                        Buttons = Enumerable.Empty<IButtonSetup>(),
        //                        DefaultButtonType = DefaultButtonType.SaveExisting,
        //                        Icon = "Save",
        //                        IsPrimary = true,
        //                        Label = "Update"
        //                    },
        //                    new ButtonSetup
        //                    {
        //                        ButtonHandlerType = typeof(DefaultButtonActionHandler),
        //                        ButtonId = "modelmaker-edit",
        //                        Buttons = Enumerable.Empty<IButtonSetup>(),
        //                        DefaultButtonType = DefaultButtonType.Edit,
        //                        Icon = "Edit",
        //                        Label = "Edit"
        //                    },
        //                },
        //                new List<FieldSetup>
        //                {
        //                    new PropertyFieldSetup(new FieldConfig
        //                    {
        //                        Description = "Dynamic Name",
        //                        EditorType = EditorType.TextBox,
        //                        Index = 1,
        //                        IsDisabled = (m, s) => false,
        //                        IsVisible = (m, s) => true,
        //                        Name = "Name",
        //                        Placeholder = "Dynamic name",
        //                        Property = new ModelMakerEntityPropertyMetadata(
        //                            typeof(string),
        //                            "Name",
        //                            x => x.Get("Name"),
        //                            (x, v) => x.Set("Name", v),
        //                            "DynamicName")
        //                    })
        //                },
        //                new List<SubCollectionListSetup>(),
        //                new List<RelatedCollectionListSetup>())
        //        },
        //        new List<IButtonSetup>
        //        {
        //            new ButtonSetup
        //            {
        //                ButtonHandlerType = typeof(DefaultButtonActionHandler),
        //                ButtonId = "modelmaker-new",
        //                Buttons = Enumerable.Empty<IButtonSetup>(),
        //                DefaultButtonType = DefaultButtonType.New,
        //                Icon = "Add",
        //                IsPrimary = true,
        //                Label = "New",

        //                // TODO: reuse
        //                EntityVariant = new EntityVariantSetup("default", default, typeof(ModelMakerEntity), "modelmaker")
        //            }
        //        })
        //};
    }
}
