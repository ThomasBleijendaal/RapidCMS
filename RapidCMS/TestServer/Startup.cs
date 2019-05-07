using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.WindowsAzure.Storage;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;
using RapidCMS.Common.ValueMappers;
using TestLibrary;
using TestLibrary.DataProvider;
using TestLibrary.Entities;
using TestLibrary.Repositories;
using TestServer.ActionHandlers;

namespace TestServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<RepositoryA>();
            services.AddSingleton<RepositoryB>();
            services.AddSingleton<RepositoryC>();
            services.AddSingleton<RepositoryD>();
            services.AddSingleton<RepositoryE>();
            services.AddSingleton<RepositoryF>();
            services.AddSingleton<VariantRepository>();

            services.AddSingleton<RelationRepository>();

            services.AddSingleton(CloudStorageAccount.DevelopmentStorageAccount);
            services.AddSingleton<AzureTableStorageRepository>();

            services.AddTransient<CreateButtonActionHandler>();

            services.AddTransient<DummyDataProvider>();

            services.AddRapidCMS();

            services.AddMvc()
                .AddNewtonsoftJson();

            services.AddRazorComponents();

            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region done

            void listView(ListViewConfig<TestEntity> listViewConfig)
            {
                listViewConfig
                    .AddDefaultButton(DefaultButtonType.New, "New", isPrimary: true)
                    .SetListPane(pane =>
                    {
                        pane.AddProperty(x => x._Id);
                        pane.AddProperty(x => x.Name).SetDescription("This is a name");
                        pane.AddProperty(x => x.Description).SetDescription("This is a description");
                        pane.AddDefaultButton(DefaultButtonType.View, string.Empty);
                        pane.AddDefaultButton(DefaultButtonType.Edit, string.Empty);

                    });
            }

            void listViewWithPolymorphism(ListViewConfig<TestEntity> listViewConfig)
            {
                listViewConfig
                    .AddDefaultButton(DefaultButtonType.New, "New", isPrimary: true)
                    .SetListPane(pane =>
                    {
                        pane.AddProperty(x => x._Id);
                        pane.AddProperty(x => x.Name).SetDescription("This is a name");
                        pane.AddProperty(x => x.Description).SetDescription("This is a description");
                        pane.AddDefaultButton(DefaultButtonType.View, string.Empty);
                        pane.AddDefaultButton(DefaultButtonType.Edit, string.Empty);

                    });
            }

            void nodeEditor(NodeEditorConfig<TestEntity> nodeEditorConfig)
            {
                nodeEditorConfig
                    .AddDefaultButton(DefaultButtonType.Edit)
                    .AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true)
                    .AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true)
                    .AddDefaultButton(DefaultButtonType.Delete)
                    .AddEditorPane(pane =>
                    {
                        pane.AddField(x => x._Id)
                            .SetValueMapper<LongValueMapper>()
                            .SetReadonly(true);

                        pane.AddField(x => x.Name)
                            .SetDescription("This is a name");

                        pane.AddField(x => x.Description)
                            .SetDescription("This is a description")
                            .SetType(EditorType.TextArea);

                        pane.AddField(x => x.Number)
                            .SetDescription("This is a number")
                            .SetValueMapper<LongValueMapper>()
                            .SetType(EditorType.Numeric);
                    });
            }

            void listNodeEditor(ListEditorConfig<TestEntity> listEditorConfig)
            {
                listEditorConfig.AddDefaultButton(DefaultButtonType.New);
                listEditorConfig.AddEditor(editor =>
                {
                    editor.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);
                    editor.AddDefaultButton(DefaultButtonType.View);
                    editor.AddDefaultButton(DefaultButtonType.Edit);
                    editor.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                    editor.AddDefaultButton(DefaultButtonType.Delete);

                    editor.AddField(x => x._Id)
                        .SetDescription("This should be readonly")
                        .SetReadonly();

                    editor.AddField(x => x.Name)
                        .SetDescription("This is a name");

                    editor.AddField(x => x.Description)
                        .SetDescription("This is a description")
                        .SetType(EditorType.TextArea);

                    editor.AddField(x => x.Number)
                        .SetDescription("This is a number")
                        .SetValueMapper<LongValueMapper>()
                        .SetType(EditorType.Numeric);
                });
            }

            void subListNodeEditor(ListEditorConfig<TestEntity> listEditorConfig)
            {
                listEditorConfig.AddDefaultButton(DefaultButtonType.New);
                listEditorConfig.AddEditor(editor =>
                {
                    editor.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);
                    editor.AddDefaultButton(DefaultButtonType.View);
                    editor.AddDefaultButton(DefaultButtonType.Edit);
                    editor.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                    editor.AddDefaultButton(DefaultButtonType.Delete);

                    editor.AddField(x => x._Id)
                        .SetDescription("This should be readonly")
                        .SetReadonly();

                    editor.AddField(x => x.Name)
                        .SetDescription("This is a name");

                    editor.AddField(x => x.Description)
                        .SetDescription("This is a description")
                        .SetType(EditorType.TextArea);

                    editor.AddField(x => x.Number)
                        .SetDescription("This is a number")
                        .SetValueMapper<LongValueMapper>()
                        .SetType(EditorType.Numeric);
                });
            }

            void nodeEditorWithSubCollection(NodeEditorConfig<TestEntity> nodeEditorConfig)
            {
                nodeEditorConfig
                    .AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true)
                    .AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true)
                    .AddDefaultButton(DefaultButtonType.Delete)

                    .AddEditorPane(pane =>
                    {
                        pane.AddField(x => x._Id)
                            .SetValueMapper<LongValueMapper>()
                            .SetReadonly(true);

                        pane.AddField(x => x.Name)
                            .SetDescription("This is a name");

                        pane.AddField(x => x.Description)
                            .SetDescription("This is a description")
                            .SetType(EditorType.TextArea);

                        pane.AddField(x => x.Number)
                            .SetDescription("This is a number")
                            .SetValueMapper<LongValueMapper>()
                            .SetType(EditorType.Numeric);
                    })

                    .AddEditorPane(pane =>
                    {
                        pane.AddSubCollectionListEditor("sub-collection-1");
                    })

                    .AddEditorPane(pane =>
                    {
                        pane.AddSubCollectionListEditor("sub-collection-2");
                    });
            }

            void nodeEditorWithPolymorphicSubCollection(NodeEditorConfig<TestEntity> nodeEditorConfig)
            {
                nodeEditorConfig
                    .AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true)
                    .AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true)
                    .AddDefaultButton(DefaultButtonType.Delete)

                    .AddEditorPane(pane =>
                    {
                        pane.AddField(x => x._Id)
                            .SetValueMapper<LongValueMapper>()
                            .SetReadonly(true);

                        pane.AddField(x => x.Name)
                            .SetDescription("This is a name");

                        pane.AddField(x => x.Description)
                            .SetDescription("This is a description")
                            .SetType(EditorType.TextArea);

                        pane.AddField(x => x.Number)
                            .SetDescription("This is a number")
                            .SetValueMapper<LongValueMapper>()
                            .SetType(EditorType.Numeric);
                    })

                    .AddEditorPane(pane =>
                    {
                        pane.AddSubCollectionListEditor("sub-collection-3");
                    });
            }

            void nodeEditorWithPolymorphism(NodeEditorConfig<TestEntity> nodeEditorConfig)
            {
                nodeEditorConfig
                    .AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true)
                    .AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true)
                    .AddDefaultButton(DefaultButtonType.Delete)

                    .AddEditorPane(pane =>
                    {
                        pane.AddField(x => x._Id)
                            .SetValueMapper<LongValueMapper>()
                            .SetReadonly(true);

                        pane.AddField(x => x.Name)
                            .SetDescription("This is a name");

                        pane.AddField(x => x.Description)
                            .SetDescription("This is a description")
                            .SetType(EditorType.TextArea);

                        pane.AddField(x => x.Number)
                            .SetDescription("This is a number")
                            .SetValueMapper<LongValueMapper>()
                            .SetType(EditorType.Numeric);
                    })

                    .AddEditorPane<TestEntityVariantA>(pane =>
                    {
                        pane.AddField(x => x.Title)
                            .SetDescription("This is a title");
                    })

                    .AddEditorPane<TestEntityVariantB>(pane =>
                    {
                        pane.AddField(x => x.Image)
                            .SetDescription("This is an image");
                    })

                    .AddEditorPane<TestEntityVariantC>(pane =>
                    {
                        pane.AddField(x => x.Quote)
                            .SetDescription("This is a quote");
                    });
            }

            void listNodeEditorWithPolymorphism(ListEditorConfig<TestEntity> listEditorConfig)
            {
                listEditorConfig.AddDefaultButton(DefaultButtonType.New, isPrimary: true);
                listEditorConfig.AddEditor<TestEntityVariantA>(editor =>
                {
                    editor.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);
                    editor.AddDefaultButton(DefaultButtonType.View);
                    editor.AddDefaultButton(DefaultButtonType.Edit);
                    editor.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                    editor.AddDefaultButton(DefaultButtonType.Delete);

                    editor.AddField(x => x._Id)
                        .SetDescription("This should be readonly")
                        .SetReadonly();

                    editor.AddField(x => x.Name)
                        .SetDescription("This is a name");

                    editor.AddField(x => x.Description)
                        .SetDescription("This is a description")
                        .SetType(EditorType.TextArea);

                    editor.AddField(x => x.Number)
                        .SetDescription("This is a number")
                        .SetValueMapper<LongValueMapper>()
                        .SetType(EditorType.Numeric);

                    editor.AddField(x => x.Title)
                        .SetDescription("This is a title");
                });

                listEditorConfig.AddEditor<TestEntityVariantB>(editor =>
                {
                    editor.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);
                    editor.AddDefaultButton(DefaultButtonType.View);
                    editor.AddDefaultButton(DefaultButtonType.Edit);
                    editor.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                    editor.AddDefaultButton(DefaultButtonType.Delete);

                    editor.AddField(x => x._Id)
                        .SetDescription("This should be readonly")
                        .SetReadonly();

                    editor.AddField(x => x.Name)
                        .SetDescription("This is a name");

                    editor.AddField(x => x.Description)
                        .SetDescription("This is a description")
                        .SetType(EditorType.TextArea);

                    editor.AddField(x => x.Number)
                        .SetDescription("This is a number")
                        .SetValueMapper<LongValueMapper>()
                        .SetType(EditorType.Numeric);

                    editor.AddField(x => x.Image)
                        .SetDescription("This is an image");
                });

                listEditorConfig.AddEditor<TestEntityVariantC>(editor =>
                {
                    editor.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);
                    editor.AddDefaultButton(DefaultButtonType.View);
                    editor.AddDefaultButton(DefaultButtonType.Edit);
                    editor.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                    editor.AddDefaultButton(DefaultButtonType.Delete);

                    editor.AddField(x => x._Id)
                        .SetDescription("This should be readonly")
                        .SetReadonly();

                    editor.AddField(x => x.Name)
                        .SetDescription("This is a name");

                    editor.AddField(x => x.Description)
                        .SetDescription("This is a description")
                        .SetType(EditorType.TextArea);

                    editor.AddField(x => x.Number)
                        .SetDescription("This is a number")
                        .SetValueMapper<LongValueMapper>()
                        .SetType(EditorType.Numeric);

                    editor.AddField(x => x.Quote)
                        .SetDescription("This is a quote");
                });
            }

            #endregion

            void AzureTableStorageListView(ListViewConfig<AzureTableStorageEntity> config)
            {
                config
                    .AddDefaultButton(DefaultButtonType.New, isPrimary: true)
                    .AddCustomButton<CreateButtonActionHandler>("create-button", "Custom create!");

                config.SetListPane(listPaneConfig =>
                {
                    listPaneConfig.AddProperty(x => x.Id);
                    listPaneConfig.AddProperty(x => x.Title);
                    listPaneConfig.AddProperty(x => x.Description);

                    listPaneConfig.AddDefaultButton(DefaultButtonType.Edit, isPrimary: true);
                    listPaneConfig.AddDefaultButton(DefaultButtonType.Delete);
                    listPaneConfig.AddCustomButton<CreateButtonActionHandler>("create-button", "Custom create!");
                });
            }

            void AzureTableStorageEditor(NodeEditorConfig<AzureTableStorageEntity> config)
            {
                config.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                config.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);
                config.AddDefaultButton(DefaultButtonType.Delete);

                config.AddEditorPane(editorPaneConfig =>
                {
                    editorPaneConfig.AddField(x => x.Title);
                    editorPaneConfig.AddField(x => x.Description);


                });
            }

            void RelationListView(ListViewConfig<RelationEntity> config)
            {
                config
                    .AddDefaultButton(DefaultButtonType.New);

                config.SetListPane(listPaneConfig =>
                {
                    listPaneConfig.AddProperty(x => x.Id);
                    listPaneConfig.AddProperty(x => x.Name);
                    listPaneConfig.AddProperty(x => x.AzureTableStorageEntityId);
                    listPaneConfig.AddProperty(x => x.AzureTableStorageEntityIds)
                        .SetValueMapper<CollectionValueMapper<string>>();

                    listPaneConfig.AddDefaultButton(DefaultButtonType.Edit, isPrimary: true);
                    listPaneConfig.AddDefaultButton(DefaultButtonType.Delete);
                });
            }

            void RelationEditor(NodeEditorConfig<RelationEntity> config)
            {
                config.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                config.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);
                config.AddDefaultButton(DefaultButtonType.Delete);

                config.AddEditorPane(editorPaneConfig =>
                {
                    editorPaneConfig.AddField(x => x.Name);

                    editorPaneConfig.AddField(x => x.Location)
                        .SetType(EditorType.Dropdown)
                        .SetOneToManyRelation<DummyDataProvider>();

                    editorPaneConfig.AddField(x => x.AzureTableStorageEntityId)
                        .SetType(EditorType.Select)
                        .SetOneToManyRelation<AzureTableStorageEntity>("collection-10", relation =>
                        {
                            relation
                                .SetIdProperty(x => x.Id)
                                .SetDisplayProperty(x => x.Description);
                        });

                    editorPaneConfig.AddField(x => x.AzureTableStorageEntityIds)
                        .SetType(EditorType.MultiSelect)
                        .SetValueMapper<CollectionValueMapper<string>>()
                        .SetOneToManyRelation<AzureTableStorageEntity>("collection-10", relation =>
                        {
                            relation
                                .SetIdProperty(x => x.Id)
                                .SetDisplayProperty(x => x.Description);
                        });
                });
            }

            app.UseRapidCMS(root =>
            {
                root.AddCollection<RelationEntity>("collection-11", "Azure Table Storage Collecation with relations", collection =>
                {
                    collection
                        .SetRepository<RelationRepository>()
                        .SetTreeView(EntityVisibilty.Visible, entity => entity.Name)
                        .SetListView(RelationListView)
                        .SetNodeEditor(RelationEditor);
                });

                root.AddCollection<AzureTableStorageEntity>("collection-10", "Azure Table Storage Collection", collection =>
                {
                    collection
                        .SetRepository<AzureTableStorageRepository>()
                        .SetTreeView(EntityVisibilty.Visible, entity => entity.Title)
                        .SetListView(AzureTableStorageListView)
                        .SetNodeEditor(AzureTableStorageEditor)

                        .AddCollection<AzureTableStorageEntity>("collection-10-a", "Sub collection", subCollection =>
                        {
                            subCollection
                                .SetRepository<AzureTableStorageRepository>()
                                .SetTreeView(EntityVisibilty.Visible, entity => entity.Title)
                                .SetListView(AzureTableStorageListView)
                                .SetNodeEditor(AzureTableStorageEditor);
                        });
                });

                root.AddCollection<TestEntity>("collection-6", "Variant collection as blocks", collection =>
                {
                    collection
                        .SetRepository<VariantRepository>()
                        .SetTreeView(EntityVisibilty.Hidden, entity => entity.Name)
                        .AddEntityVariant<TestEntityVariantA>("Variant A", "align-left")
                        .AddEntityVariant<TestEntityVariantB>("Variant B", "align-center")
                        .AddEntityVariant<TestEntityVariantC>("Variant C", "align-right")
                        .SetListEditor(ListEditorType.Block, listNodeEditorWithPolymorphism)
                        .SetNodeEditor(nodeEditorWithPolymorphism);
                });

                root.AddCollection<TestEntity>("collection-5", "Collections with variant sub collection", collection =>
                {
                    collection
                        .SetRepository<RepositoryF>()
                        .SetTreeView(EntityVisibilty.Visible, entity => entity.Name)
                        .SetListView(listView)
                        .SetNodeEditor(nodeEditorWithPolymorphicSubCollection)
                        .AddCollection<TestEntity>("sub-collection-3", "Sub Collection 3", subCollection =>
                        {
                            subCollection
                                .SetRepository<VariantRepository>()
                                .SetTreeView(EntityVisibilty.Visible, CollectionRootVisibility.Hidden, entity => entity.Name)
                                .AddEntityVariant<TestEntityVariantA>("Variant A", "align-left")
                                .AddEntityVariant<TestEntityVariantB>("Variant B", "align-center")
                                .AddEntityVariant<TestEntityVariantC>("Variant C", "align-right")
                                .SetListView(listViewWithPolymorphism)
                                .SetListEditor(ListEditorType.Table, listNodeEditorWithPolymorphism)
                                .SetNodeEditor(nodeEditorWithPolymorphism);
                        });
                });

                root.AddCollection<TestEntity>("collection-4", "Collection with sub collections", collection =>
                {
                    collection
                        .SetRepository<RepositoryA>()
                        .SetTreeView(entity => entity.Name)
                        .SetListView(listView)
                        .SetNodeEditor(nodeEditorWithSubCollection)
                        .AddCollection<TestEntity>("sub-collection-1", "Sub Collection 1", subCollection =>
                        {
                            subCollection
                                .SetRepository<RepositoryB>()
                                //.SetTreeView(EntityVisibilty.Hidden, CollectionRootVisibility.Hidden, entity => entity.Name)
                                .SetListView(listView)
                                .SetListEditor(ListEditorType.Table, subListNodeEditor)
                                .SetNodeEditor(nodeEditor);
                        })
                        .AddCollection<TestEntity>("sub-collection-2", "Sub Collection 2", subCollection =>
                        {
                            subCollection
                                .SetRepository<RepositoryC>()
                                //.SetTreeView(EntityVisibilty.Hidden, CollectionRootVisibility.Hidden, entity => entity.Name)
                                .SetListEditor(ListEditorType.Block, subListNodeEditor)
                                .SetNodeEditor(nodeEditor);
                        });
                });

                root.AddCollection<TestEntity>("collection-3", "Variant collection", collection =>
                {
                    collection
                        .SetRepository<VariantRepository>()
                        .SetTreeView(EntityVisibilty.Visible, entity => entity.Name)
                        .AddEntityVariant<TestEntityVariantA>("Variant A", "align-left")
                        .AddEntityVariant<TestEntityVariantB>("Variant B", "align-center")
                        .AddEntityVariant<TestEntityVariantC>("Variant C", "align-right")
                        .SetListView(listView)
                        .SetNodeEditor(nodeEditorWithPolymorphism);
                });

                root.AddCollection<TestEntity>("collection-2", "List editor collection", collection =>
                {
                    collection
                        .SetRepository<RepositoryD>()
                        .SetTreeView(EntityVisibilty.Hidden, entity => entity.Name)
                        .SetListEditor(ListEditorType.Table, listNodeEditor)
                        .SetNodeEditor(nodeEditor);
                });

                root.AddCollection<TestEntity>("collection-1", "Simple collection", collection =>
                {
                    collection
                        .SetRepository<RepositoryE>()
                        .SetTreeView(EntityVisibilty.Visible, entity => entity.Name)
                        .SetListView(listView)
                        .SetNodeEditor(nodeEditor);
                });
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting(routes =>
            {
                routes.MapRazorPages();
                routes.MapComponentHub<Components.App>("app");
            });
        }
    }
}
