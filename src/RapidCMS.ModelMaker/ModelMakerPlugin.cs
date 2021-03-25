using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Handlers;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.ModelMaker
{
    internal class ModelMakerPlugin : IPlugin
    {
        public string CollectionPrefix => "modelmaker";

        // TODO: async
        public CollectionSetup? GetCollection(string collectionAlias)
        {
            if (collectionAlias == "dynamic")
            {
                return _dynamic;
            }

            return null;
        }

        // TODO: async
        public IEnumerable<ITreeElementSetup> GetTreeElements()
        {
            return new ITreeElementSetup[]
            {
                // TODO: prefix should be added automatically
                new TreeElementSetup(_dynamic.Alias, PageType.Collection)
                {
                    RootVisibility = CollectionRootVisibility.Visible
                }
            };
        }

        public Type? GetRepositoryType(string collectionAlias)
        {
            return typeof(ModelMakerRepository);
        }

        

        private CollectionSetup _dynamic = new CollectionSetup(
            "Database",
            "Gray100",
            "Dynamic Config",
            $"modelmaker::dynamic",
            $"modelmaker::dynamic",
            false,
            false)
        {
            // TODO: use existing setup resolvers to convert more simple config to complex setup easily

            EntityVariant = new EntityVariantSetup("default", default, typeof(ModelMakerEntity), "modelmaker"),
            UsageType = UsageType.List,
            TreeView = new TreeViewSetup(
                EntityVisibilty.Visible,
                CollectionRootVisibility.Visible,
                false,
                false,
                new ModelMakerEntityExpressionMetadata(
                    "Name",
                    x => x.Get<string>("Name"))),
            NodeEditor = new NodeSetup(
                typeof(ModelMakerEntity),
                new List<PaneSetup>
                {
                    new PaneSetup(
                        default,
                        "Dynamic",
                        (m, s) => true,
                        typeof(ModelMakerEntity),
                        new List<IButtonSetup>
                        {
                            
                        },
                        new List<FieldSetup>
                        {
                            new PropertyFieldSetup(new FieldConfig
                            {
                                Description = "Dynamic Name",
                                EditorType = EditorType.TextBox,
                                Index = 1,
                                IsDisabled = (m, s) => false,
                                IsVisible = (m, s) => true,
                                Name = "Name",
                                Placeholder = "Dynamic name",
                                Property = new ModelMakerEntityPropertyMetadata(
                                    typeof(string),
                                    "Name",
                                    x => x.Get("Name"),
                                    (x, v) => x.Set("Name", v),
                                    "DynamicName")
                            })
                        },
                        new List<SubCollectionListSetup>(),
                        new List<RelatedCollectionListSetup>())
                },
                new List<IButtonSetup>
                {
                    new ButtonSetup
                    {
                        ButtonHandlerType = typeof(DefaultButtonActionHandler),
                        ButtonId = "modelmaker-save-new",
                        Buttons = Enumerable.Empty<IButtonSetup>(),
                        DefaultButtonType = DefaultButtonType.SaveNew,
                        Icon = "Save",
                        IsPrimary = true,
                        Label = "Insert"
                    },
                    new ButtonSetup
                    {
                        ButtonHandlerType = typeof(DefaultButtonActionHandler),
                        ButtonId = "modelmaker-save-existing",
                        Buttons = Enumerable.Empty<IButtonSetup>(),
                        DefaultButtonType = DefaultButtonType.SaveExisting,
                        Icon = "Save",
                        IsPrimary = true,
                        Label = "Update"
                    },
                    new ButtonSetup
                    {
                        ButtonHandlerType = typeof(DefaultButtonActionHandler),
                        ButtonId = "modelmaker-delete",
                        Buttons = Enumerable.Empty<IButtonSetup>(),
                        DefaultButtonType = DefaultButtonType.Delete,
                        Icon = "Delete",
                        Label = "Delete"
                    }
                }),
            ListEditor = new ListSetup(
                100,
                false,
                false,
                ListType.Table,
                EmptyVariantColumnVisibility.Collapse,
                new List<PaneSetup>
                {
                    new PaneSetup(
                        default,
                        "Dynamic",
                        (m, s) => true,
                        typeof(ModelMakerEntity),
                        new List<IButtonSetup>
                        {
                            new ButtonSetup
                            {
                                ButtonHandlerType = typeof(DefaultButtonActionHandler),
                                ButtonId = "modelmaker-save-new",
                                Buttons = Enumerable.Empty<IButtonSetup>(),
                                DefaultButtonType = DefaultButtonType.SaveNew,
                                Icon = "Save",
                                IsPrimary = true,
                                Label = "Insert"
                            },
                            new ButtonSetup
                            {
                                ButtonHandlerType = typeof(DefaultButtonActionHandler),
                                ButtonId = "modelmaker-save-existing",
                                Buttons = Enumerable.Empty<IButtonSetup>(),
                                DefaultButtonType = DefaultButtonType.SaveExisting,
                                Icon = "Save",
                                IsPrimary = true,
                                Label = "Update"
                            },
                            new ButtonSetup
                            {
                                ButtonHandlerType = typeof(DefaultButtonActionHandler),
                                ButtonId = "modelmaker-edit",
                                Buttons = Enumerable.Empty<IButtonSetup>(),
                                DefaultButtonType = DefaultButtonType.Edit,
                                Icon = "Edit",
                                Label = "Edit"
                            },
                        },
                        new List<FieldSetup>
                        {
                            new PropertyFieldSetup(new FieldConfig
                            {
                                Description = "Dynamic Name",
                                EditorType = EditorType.TextBox,
                                Index = 1,
                                IsDisabled = (m, s) => false,
                                IsVisible = (m, s) => true,
                                Name = "Name",
                                Placeholder = "Dynamic name",
                                Property = new ModelMakerEntityPropertyMetadata(
                                    typeof(string),
                                    "Name",
                                    x => x.Get("Name"),
                                    (x, v) => x.Set("Name", v),
                                    "DynamicName")
                            })
                        },
                        new List<SubCollectionListSetup>(),
                        new List<RelatedCollectionListSetup>())
                },
                new List<IButtonSetup>
                {
                    new ButtonSetup
                    {
                        ButtonHandlerType = typeof(DefaultButtonActionHandler),
                        ButtonId = "modelmaker-new",
                        Buttons = Enumerable.Empty<IButtonSetup>(),
                        DefaultButtonType = DefaultButtonType.New,
                        Icon = "Add",
                        IsPrimary = true,
                        Label = "New",

                        // TODO: reuse
                        EntityVariant = new EntityVariantSetup("default", default, typeof(ModelMakerEntity), "modelmaker")
                    }
                })
        };
    }
}
