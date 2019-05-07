using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;
using RapidCMS.Common.Services;
using RapidCMS.Common.ValueMappers;

namespace RapidCMS.Common.Extensions
{
    // TODO: make code more DRY

    public static class RapidCMSMiddleware
    {
        public static IServiceCollection AddRapidCMS(this IServiceCollection services)
        {
            var root = new Root();

            services.AddSingleton(root);
            services.AddSingleton<ICollectionService, CollectionService>();
            services.AddSingleton<IUIService, UIService>();

            services.AddSingleton<DefaultValueMapper>();
            services.AddSingleton<LongValueMapper>();
            services.AddSingleton(typeof(CollectionValueMapper<>), typeof(CollectionValueMapper<>));

            return services;
        }

        public static IApplicationBuilder UseRapidCMS(this IApplicationBuilder app, Action<Root> configure)
        {
            ServiceLocator.CreateInstance(app.ApplicationServices);

            var root = app.ApplicationServices.GetRequiredService<Root>();

            configure.Invoke(root);

            try
            {
                root.MaterializeRepositories(app.ApplicationServices);

                // TODO: populate value mappers
            }
            catch
            {

            }

            return app;
        }
    }

    public static class ICollectionRootExtensions
    {
        public static ICollectionRoot AddCollection<TEntity>(this ICollectionRoot root, string alias, string name, Action<CollectionConfig<TEntity>> configure)
            where TEntity : IEntity
        {
            var collection = new Collection
            {
                Name = name,
                Alias = alias
            };

            var configReceiver = new CollectionConfig<TEntity>();

            configure.Invoke(configReceiver);

            collection.RepositoryType = configReceiver.RepositoryType;

            if (configReceiver.TreeView != null)
            {
                collection.TreeView = new TreeView
                {
                    EntityVisibility = configReceiver.TreeView.EntityVisibilty,
                    RootVisibility = configReceiver.TreeView.RootVisibility,
                    NameGetter = configReceiver.TreeView.PropertyMetadata.Getter
                };
            }

            if (configReceiver.EntityVariants.Any())
            {
                collection.SubEntityVariants = configReceiver.EntityVariants.ToList(variant => new EntityVariant
                {
                    Alias = variant.Type.Name.ToUrlFriendlyString(),
                    Icon = variant.Icon,
                    Name = variant.Name,
                    Type = variant.Type
                });
            }

            collection.EntityVariant = new EntityVariant
            {
                Alias = typeof(TEntity).Name.ToUrlFriendlyString(),
                Icon = null,
                Name = typeof(TEntity).Name,
                Type = typeof(TEntity)
            };

            if (configReceiver.ListView != null)
            {
                collection.ListView = new ListView
                {
                    Buttons = configReceiver.ListView.Buttons.ToList(button => button switch
                    {
                        DefaultButtonConfig defaultButton => defaultButton.ToDefaultButton(collection.SubEntityVariants, collection.EntityVariant),
                        CustomButtonConfig customButton => customButton.ToCustomButton(),
                        _ => default(Button)
                    }),
                    ViewPane = configReceiver.ListView.ListViewPane == null ? null :
                        new ViewPane
                        {
                            Buttons = configReceiver.ListView.ListViewPane.Buttons.ToList(button => button switch
                            {
                                DefaultButtonConfig defaultButton => defaultButton.ToDefaultButton(collection.SubEntityVariants, collection.EntityVariant),
                                CustomButtonConfig customButton => customButton.ToCustomButton(),
                                _ => default(Button)
                            }),
                            Fields = configReceiver.ListView.ListViewPane.Properties.ToList(property => new Field
                            {
                                Description = property.Description,
                                Name = property.Name,
                                Readonly = true,
                                NodeProperty = property.NodeProperty,
                                ValueMapperType = property.ValueMapperType ?? typeof(DefaultValueMapper)
                            })
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
                        CustomButtonConfig customButton => customButton.ToCustomButton(),
                        _ => default(Button)
                    }),
                    EditorPanes = editors.ToList(editor =>
                    {
                        return new EditorPane<Field>
                        {
                            VariantType = editor.VariantType,
                            Buttons = editor.Buttons.ToList(button => button switch
                            {
                                DefaultButtonConfig defaultButton => defaultButton.ToDefaultButton(collection.SubEntityVariants, collection.EntityVariant),
                                CustomButtonConfig customButton => customButton.ToCustomButton(),
                                _ => default(Button)
                            }),
                            Fields = editor.Fields.ToList(field => field.ToField())
                        };
                    })
                };
            }

            if (configReceiver.NodeEditor != null)
            {
                collection.NodeEditor = new NodeEditor
                {
                    Buttons = configReceiver.NodeEditor.Buttons.ToList(button => button switch
                    {
                        DefaultButtonConfig defaultButton => defaultButton.ToDefaultButton(collection.SubEntityVariants, collection.EntityVariant),
                        CustomButtonConfig customButton => customButton.ToCustomButton(),
                        _ => default(Button)
                    }),

                    BaseType = configReceiver.NodeEditor.BaseType,

                    EditorPanes = configReceiver.NodeEditor.EditorPanes.ToList(pane =>
                    {
                        return new EditorPane<Field>
                        {
                            VariantType = pane.VariantType,

                            Buttons = new List<Button>(),

                            Fields = pane.Fields.ToList(field => field.ToField()),

                            SubCollectionListEditors = pane.SubCollectionListEditors.ToList(listEditor =>
                            {
                                return new SubCollectionListEditor
                                {
                                    Index = listEditor.Index,

                                    CollectionAlias = listEditor.CollectionAlias
                                };
                            })
                        };
                    })
                };


            }

            collection.Collections = configReceiver.Collections;

            root.Collections.Add(collection);

            return root;
        }
    }

    public static class ButtonExtensions
    {
        public static IEnumerable<Button> GetAllButtons(this IEnumerable<Button> buttons)
        {
            // HACK: bit of a hack
            return buttons.SelectMany(x => x.Buttons.Any() ? x.Buttons.AsEnumerable() : new[] { x }).ToList();
        }
    }
}
