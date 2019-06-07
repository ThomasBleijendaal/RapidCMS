using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;
using RapidCMS.Common.ValueMappers;

#nullable enable

namespace RapidCMS.Common.Extensions
{
    public static class ICollectionRootExtensions
    {
        public static ICollectionRoot AddCollection<TEntity>(this ICollectionRoot root, string alias, string name, Action<CollectionConfig<TEntity>> configure)
            where TEntity : IEntity
        {
            var configReceiver = new CollectionConfig<TEntity>
            {
                Alias = alias,
                Name = name,
                EntityVariant = new EntityVariantConfig
                {
                    Icon = null,
                    Name = typeof(TEntity).Name,
                    Type = typeof(TEntity)
                }
            };

            configure.Invoke(configReceiver);

            root.Collections.Add(configReceiver);

            return root;
        }

        // TODO: loose the serviceProvider
        public static List<Collection> ProcessCollections(this ICollectionRoot root, IServiceProvider serviceProvider)
        {
            var list = new List<Collection>();

            foreach (var configReceiver in root.Collections)
            {
                var collection = new Collection
                {
                    Alias = configReceiver.Alias,
                    Name = configReceiver.Name
                };

                collection.RepositoryType = configReceiver.RepositoryType;

                if (configReceiver.TreeView != null)
                {
                    collection.TreeView = new TreeView
                    {
                        EntityVisibility = configReceiver.TreeView.EntityVisibilty,
                        RootVisibility = configReceiver.TreeView.RootVisibility,
                        Name = configReceiver.TreeView.Name
                    };
                }

                if (configReceiver.SubEntityVariants.Any())
                {
                    collection.SubEntityVariants = configReceiver.SubEntityVariants.ToList(variant => new EntityVariant
                    {
                        Alias = variant.Type.FullName.ToUrlFriendlyString(),
                        Icon = variant.Icon,
                        Name = variant.Name,
                        Type = variant.Type
                    });
                }

                collection.EntityVariant = new EntityVariant
                {
                    Alias = configReceiver.EntityVariant.Type.FullName.ToUrlFriendlyString(),
                    Icon = null,
                    Name = configReceiver.EntityVariant.Name,
                    Type = configReceiver.EntityVariant.Type
                };

                if (configReceiver.ListView != null)
                {
                    collection.ListView = new ListView
                    {
                        Buttons = configReceiver.ListView.Buttons.ToList(button => button switch
                        {
                            DefaultButtonConfig defaultButton => defaultButton.ToDefaultButton(collection.SubEntityVariants, collection.EntityVariant),
                            CustomButtonConfig customButton => customButton.ToCustomButton(serviceProvider),
                            _ => throw new InvalidOperationException("Invalid ListView Button")
                        }),
                        ViewPane = configReceiver.ListView.ListViewPane == null ? null :
                            new ViewPane
                            {
                                Buttons = configReceiver.ListView.ListViewPane.Buttons.ToList(button => button switch
                                {
                                    DefaultButtonConfig defaultButton => defaultButton.ToDefaultButton(collection.SubEntityVariants, collection.EntityVariant),
                                    CustomButtonConfig customButton => customButton.ToCustomButton(serviceProvider),
                                    _ => throw new InvalidOperationException("Invalid ListView ViewPane Button")
                                }),
                                Fields = configReceiver.ListView.ListViewPane.Properties.ToList(x => x.ToField())
                            }
                    };
                }

                if (configReceiver.ListEditor != null)
                {
                    var editors = configReceiver.ListEditor.ListEditors;

                    collection.ListEditor = new ListEditor
                    {
                        ListEditorType = configReceiver.ListEditor.ListEditorType,
                        Buttons = configReceiver.ListEditor.Buttons.ToList(button => button switch
                        {
                            DefaultButtonConfig defaultButton => defaultButton.ToDefaultButton(collection.SubEntityVariants, collection.EntityVariant),
                            CustomButtonConfig customButton => customButton.ToCustomButton(serviceProvider),
                            _ => throw new InvalidOperationException("Invalid ListEditor Button")
                        }),
                        EditorPanes = editors.ToList(editor =>
                        {
                            return new Pane
                            {
                                VariantType = editor.VariantType,
                                Buttons = editor.Buttons.ToList(button => button switch
                                {
                                    DefaultButtonConfig defaultButton => defaultButton.ToDefaultButton(collection.SubEntityVariants, collection.EntityVariant),
                                    CustomButtonConfig customButton => customButton.ToCustomButton(serviceProvider),
                                    _ => throw new InvalidOperationException("Invalid ListEditor EditorPanes Button")
                                }),
                                Fields = editor.Fields.ToList(field => field.ToField())
                            };
                        })
                    };
                }

                if (configReceiver.NodeView != null)
                {
                    collection.NodeView = new Node
                    {
                        Buttons = configReceiver.NodeView.Buttons.ToList(button => button switch
                        {
                            DefaultButtonConfig defaultButton => defaultButton.ToDefaultButton(collection.SubEntityVariants, collection.EntityVariant),
                            CustomButtonConfig customButton => customButton.ToCustomButton(serviceProvider),
                            _ => throw new InvalidOperationException("Invalid NodeEditor Button")
                        }),

                        BaseType = configReceiver.NodeView.BaseType,

                        EditorPanes = configReceiver.NodeView.ViewPanes.ToList(config =>
                        {
                            var pane = new Pane
                            {
                                CustomAlias = config.CustomAlias,
                                Label = config.Label,
                                VariantType = config.VariantType,
                                Buttons = new List<Button>(),
                                Fields = config.Properties.ToList(x => x.ToField()),
                                SubCollectionLists = config.SubCollectionLists.ToList(x => x.ToSubCollectionList())
                            };

                            return pane;
                        })
                    };
                }

                if (configReceiver.NodeEditor != null)
                {
                    collection.NodeEditor = new Node
                    {
                        Buttons = configReceiver.NodeEditor.Buttons.ToList(button => button switch
                        {
                            DefaultButtonConfig defaultButton => defaultButton.ToDefaultButton(collection.SubEntityVariants, collection.EntityVariant),
                            CustomButtonConfig customButton => customButton.ToCustomButton(serviceProvider),
                            _ => throw new InvalidOperationException("Invalid NodeEditor Button")
                        }),

                        BaseType = configReceiver.NodeEditor.BaseType,

                        EditorPanes = configReceiver.NodeEditor.EditorPanes.ToList(config =>
                        {
                            var pane = new Pane
                            {
                                CustomAlias = config.CustomAlias,
                                Label = config.Label,
                                VariantType = config.VariantType,
                                Buttons = new List<Button>(),
                                Fields = config.Fields.ToList(x => x.ToField()),
                                SubCollectionLists = config.SubCollectionLists.ToList(x => x.ToSubCollectionList())
                            };

                            return pane;
                        })
                    };
                }

                collection.Collections = configReceiver.ProcessCollections(serviceProvider);

                list.Add(collection);
            }

            return list;
        }
    }
}
