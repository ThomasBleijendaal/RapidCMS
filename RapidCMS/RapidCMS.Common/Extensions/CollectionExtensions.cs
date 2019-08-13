using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Data;
using RapidCMS.Common.Exceptions;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;


namespace RapidCMS.Common.Extensions
{
    public static class ICollectionRootExtensions
    {
        public static ICollectionRoot AddCollection<TEntity>(this ICollectionRoot root, string alias, string name, Action<CollectionConfig<TEntity>> configure)
            where TEntity : IEntity
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            if (alias != alias.ToUrlFriendlyString())
            {
                throw new ArgumentException($"Use lowercase, hyphened strings as alias for collections, '{alias.ToUrlFriendlyString()}' instead of '{alias}'.");
            }

            if (!root.IsUnique(alias))
            {
                throw new NotUniqueException(nameof(alias));
            }

            var configReceiver = new CollectionConfig<TEntity>
            {
                Alias = alias ?? throw new ArgumentNullException(nameof(alias)),
                Name = name ?? throw new ArgumentNullException(nameof(name)),
                EntityVariant = new EntityVariantConfig(typeof(TEntity).Name, typeof(TEntity))
            };

            configure.Invoke(configReceiver);

            root.Collections.Add(configReceiver);

            return root;
        }

        public static ICollectionRoot AddSelfAsRecursiveCollection<TEntity>(this CollectionConfig<TEntity> root)
            where TEntity : IEntity
        {
            var configReceiver = new CollectionConfig<TEntity>
            {
                Alias = root.Alias,
                Name = root.Name,
                EntityVariant = root.EntityVariant,

                Recursive = true
            };

            configReceiver.RepositoryType = root.RepositoryType ?? throw new InvalidOperationException("Cannot add self without a Repository, use SetRepository first.");

            root.Collections.Add(configReceiver);

            return root;
        }

        public static List<Collection> ProcessCollections(this ICollectionRoot root)
        {
            var list = new List<Collection>();

            foreach (var configReceiver in root.Collections)
            {
                var variant = new EntityVariant
                {
                    Alias = configReceiver.EntityVariant.Type.FullName.ToUrlFriendlyString(),
                    Icon = null,
                    Name = configReceiver.EntityVariant.Name,
                    Type = configReceiver.EntityVariant.Type
                };

                var collection = new Collection(configReceiver.Name, configReceiver.Alias, variant, configReceiver.RepositoryType, configReceiver.Recursive)
                {
                    DataViews = configReceiver.DataViews,
                    DataViewBuilder = configReceiver.DataViewBuilder
                };

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

                if (configReceiver.ListView != null)
                {
                    var views = configReceiver.ListView.ListViewPanes;

                    collection.ListView = new ListView
                    {
                        PageSize = configReceiver.ListView.PageSize,
                        SearchBarVisible = configReceiver.ListView.SearchBarVisible,
                        Buttons = configReceiver.ListView.Buttons.ToList(button => button switch
                        {
                            DefaultButtonConfig defaultButton => defaultButton.ToDefaultButton(collection.SubEntityVariants, collection.EntityVariant),
                            CustomButtonConfig customButton => customButton.ToCustomButton(),
                            _ => throw new InvalidOperationException("Invalid ListView Button")
                        }),
                        ViewPanes = views.ToList(view =>
                        {
                            return new Pane
                            {
                                CustomAlias = view.CustomAlias,
                                IsVisible = view.IsVisible,
                                VariantType = view.VariantType,
                                Buttons = view.Buttons.ToList(button => button switch
                                {
                                    DefaultButtonConfig defaultButton => defaultButton.ToDefaultButton(collection.SubEntityVariants, collection.EntityVariant),
                                    CustomButtonConfig customButton => customButton.ToCustomButton(),
                                    _ => throw new InvalidOperationException("Invalid ListView ViewPane Button")
                                }),
                                Fields = view.Properties.ToList(x => x.ToField())
                            };
                        })
                    };
                }

                if (configReceiver.ListEditor != null)
                {
                    var editors = configReceiver.ListEditor.ListEditors;

                    collection.ListEditor = new ListEditor
                    {
                        PageSize = configReceiver.ListEditor.PageSize,
                        SearchBarVisible = configReceiver.ListEditor.SearchBarVisible,
                        ListEditorType = configReceiver.ListEditor.ListEditorType,
                        EmptyVariantColumnVisibility = configReceiver.ListEditor.EmptyVariantColumnVisibility,
                        Buttons = configReceiver.ListEditor.Buttons.ToList(button => button switch
                        {
                            DefaultButtonConfig defaultButton => defaultButton.ToDefaultButton(collection.SubEntityVariants, collection.EntityVariant),
                            CustomButtonConfig customButton => customButton.ToCustomButton(),
                            _ => throw new InvalidOperationException("Invalid ListEditor Button")
                        }),
                        EditorPanes = editors.ToList(editor =>
                        {
                            return new Pane
                            {
                                CustomAlias = editor.CustomAlias,
                                IsVisible = editor.IsVisible,
                                VariantType = editor.VariantType,
                                Buttons = editor.Buttons.ToList(button => button switch
                                {
                                    DefaultButtonConfig defaultButton => defaultButton.ToDefaultButton(collection.SubEntityVariants, collection.EntityVariant),
                                    CustomButtonConfig customButton => customButton.ToCustomButton(),
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
                            CustomButtonConfig customButton => customButton.ToCustomButton(),
                            _ => throw new InvalidOperationException("Invalid NodeEditor Button")
                        }),

                        BaseType = configReceiver.NodeView.BaseType,

                        EditorPanes = configReceiver.NodeView.ViewPanes.ToList(config =>
                        {
                            var pane = new Pane
                            {
                                CustomAlias = config.CustomAlias,
                                IsVisible = config.IsVisible,
                                Label = config.Label,
                                VariantType = config.VariantType,
                                Buttons = new List<Button>(),
                                Fields = config.Properties.ToList(x => x.ToField()),
                                SubCollectionLists = config.SubCollectionLists.ToList(x => x.ToSubCollectionList()),
                                RelatedCollectionLists = config.RelatedCollectionLists.ToList(x => x.ToRelatedCollectionList())
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
                            CustomButtonConfig customButton => customButton.ToCustomButton(),
                            _ => throw new InvalidOperationException("Invalid NodeEditor Button")
                        }),

                        BaseType = configReceiver.NodeEditor.BaseType,

                        EditorPanes = configReceiver.NodeEditor.EditorPanes.ToList(config =>
                        {
                            var pane = new Pane
                            {
                                CustomAlias = config.CustomAlias,
                                IsVisible = config.IsVisible,
                                Label = config.Label,
                                VariantType = config.VariantType,
                                Buttons = new List<Button>(),
                                Fields = config.Fields.ToList(x => x.ToField()),
                                SubCollectionLists = config.SubCollectionLists.ToList(x => x.ToSubCollectionList()),
                                RelatedCollectionLists = config.RelatedCollectionLists.ToList(x => x.ToRelatedCollectionList())
                            };

                            return pane;
                        })
                    };
                }

                collection.Collections = configReceiver.ProcessCollections();

                list.Add(collection);
            }

            return list;
        }
    }
}
