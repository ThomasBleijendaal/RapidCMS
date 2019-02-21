using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Interfaces;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;
using TestLibrary;

namespace TestClient.Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var hacky = new RapidCMS.Common.Startup();

            services.AddSingleton<RepositoryA>();
            services.AddSingleton<RepositoryB>();
            services.AddSingleton<RepositoryC>();
            services.AddSingleton<RepositoryD>();
            services.AddSingleton<RepositoryE>();

            void listView(ListViewConfig<TestEntity> listViewConfig)
            {
                listViewConfig
                    .AddDefaultButton(DefaultButtonType.New, "New")
                    .AddListPane(pane =>
                    {
                        pane.AddProperty(x => x.Id);
                        pane.AddProperty(x => x.Name).SetDescription("This is a name");
                        pane.AddDefaultButton(DefaultButtonType.View, string.Empty);
                        pane.AddDefaultButton(DefaultButtonType.Edit, string.Empty);

                    })
                    .AddListPane(pane =>
                    {
                        pane.AddProperty(x => x.Id);
                        pane.AddProperty(x => x.Number).SetValueMapper<IntValueMapper>();
                        pane.AddProperty(x => x.Description).SetDescription("This is a description");
                    });
            }

            void nodeEditor(NodeEditorConfig<TestEntity> nodeEditorConfig)
            {
                nodeEditorConfig
                    .AddDefaultButton(DefaultButtonType.SaveNew)
                    .AddDefaultButton(DefaultButtonType.SaveExisting)
                    .AddDefaultButton(DefaultButtonType.Delete)
                    .AddCustomButton("create-button", () => { }, "Create!")
                    .AddEditorPane(pane =>
                    {
                        pane.AddField(x => x.Id)
                            .SetValueMapper(new IntValueMapper())
                            .SetReadonly(true);

                        pane.AddField(x => x.Name)
                            .SetDescription("This is a name");

                        pane.AddField(x => x.Description)
                            .SetDescription("This is a description")
                            .SetType(EditorType.TextArea);

                        pane.AddField(x => x.Number)
                            .SetDescription("This is a number")
                            .SetValueMapper(new IntValueMapper())
                            .SetType(EditorType.Numeric);
                    });
            }

            void listNodeEditor(ListEditorConfig<TestEntity> listEditorConfig)
            {
                listEditorConfig.AddDefaultButton(DefaultButtonType.New);
                listEditorConfig.SetEditor(editor =>
                {
                    editor.AddDefaultButton(DefaultButtonType.SaveNew);
                    editor.AddDefaultButton(DefaultButtonType.View);
                    editor.AddDefaultButton(DefaultButtonType.Edit);
                    editor.AddDefaultButton(DefaultButtonType.SaveExisting);
                    editor.AddDefaultButton(DefaultButtonType.Delete);

                    editor.AddField(x => x.Id)
                        .SetDescription("This should be readonly")
                        .SetReadonly();

                    editor.AddField(x => x.Name)
                        .SetDescription("This is a name");

                    editor.AddField(x => x.Description)
                        .SetDescription("This is a description")
                        .SetType(EditorType.TextArea);

                    editor.AddField(x => x.Number)
                        .SetDescription("This is a number")
                        .SetValueMapper(new IntValueMapper())
                        .SetType(EditorType.Numeric);
                });
            }

            void nodeEditorWithSubCollection(NodeEditorConfig<TestEntity> nodeEditorConfig)
            {
                nodeEditorConfig
                    .AddDefaultButton(DefaultButtonType.SaveNew)
                    .AddDefaultButton(DefaultButtonType.SaveExisting)
                    .AddDefaultButton(DefaultButtonType.Delete)

                    .AddEditorPane(pane =>
                    {
                        pane.AddField(x => x.Id)
                            .SetValueMapper(new IntValueMapper())
                            .SetReadonly(true);

                        pane.AddField(x => x.Name)
                            .SetDescription("This is a name");

                        pane.AddField(x => x.Description)
                            .SetDescription("This is a description")
                            .SetType(EditorType.TextArea);

                        pane.AddField(x => x.Number)
                            .SetDescription("This is a number")
                            .SetValueMapper(new IntValueMapper())
                            .SetType(EditorType.Numeric);
                    })
                    
                    .AddEditorPane(pane =>
                    {
                        pane.AddSubCollectionListEditor("sub-collection-1", subCollectionListNodeEditor);
                    });
            }

            void nodeEditorWithPolymorphicSubCollection(NodeEditorConfig<TestEntity> nodeEditorConfig)
            {
                nodeEditorConfig
                    .AddDefaultButton(DefaultButtonType.SaveNew)
                    .AddDefaultButton(DefaultButtonType.SaveExisting)
                    .AddDefaultButton(DefaultButtonType.Delete)

                    .AddEditorPane(pane =>
                    {
                        pane.AddField(x => x.Id)
                            .SetValueMapper(new IntValueMapper())
                            .SetReadonly(true);

                        pane.AddField(x => x.Name)
                            .SetDescription("This is a name");

                        pane.AddField(x => x.Description)
                            .SetDescription("This is a description")
                            .SetType(EditorType.TextArea);

                        pane.AddField(x => x.Number)
                            .SetDescription("This is a number")
                            .SetValueMapper(new IntValueMapper())
                            .SetType(EditorType.Numeric);
                    })

                    .AddEditorPane(pane =>
                    {
                        pane.AddSubCollectionListEditor("sub-collection-1", subCollectionListNodeEditor);
                    });
            }

            void subCollectionListNodeEditor(ListEditorConfig<TestEntity> listEditorConfig)
            {
                listEditorConfig.AddDefaultButton(DefaultButtonType.New);
                listEditorConfig.SetEditor(editor =>
                {
                    editor.AddDefaultButton(DefaultButtonType.View);
                    editor.AddDefaultButton(DefaultButtonType.SaveNew);
                    editor.AddDefaultButton(DefaultButtonType.SaveExisting);
                    editor.AddDefaultButton(DefaultButtonType.Delete);

                    editor.AddField(x => x.Id)
                        .SetReadonly();

                    editor.AddField(x => x.Name);

                    editor.AddField(x => x.Description)
                        .SetType(EditorType.TextArea);

                    editor.AddField(x => x.Number)
                        .SetValueMapper(new IntValueMapper())
                        .SetType(EditorType.Numeric);
                });
            }

            services.AddRapidCMS(root =>
            {
                root.AddCollection<TestEntity>("collection-1", "Collection 1", collection =>
                {
                    collection
                        .SetRepository<RepositoryA>()
                        .SetTreeView("Tree 1", ViewType.List, entity => entity.Name)
                        .SetListView(listView)
                        .SetNodeEditor(nodeEditorWithSubCollection)
                        .AddCollection<TestEntity>("sub-collection-1", "Sub Collection 1", subCollection =>
                        {
                            subCollection
                                .SetRepository<RepositoryB>()
                                .SetTreeView("SubTree1", ViewType.List, entity => entity.Name)
                                .SetListView(listView)
                                .SetListEditor(listNodeEditor)
                                .SetNodeEditor(nodeEditor);
                        });
                });

                root.AddCollection<TestEntity>("collection-2", "Collection 2", collection =>
                {
                    collection
                        .SetRepository<RepositoryE>()
                        .SetTreeView("Tree 2", ViewType.List, entity => entity.Name)
                        .SetListView(listView)
                        .SetListEditor(listNodeEditor)
                        .SetNodeEditor(nodeEditor);
                });

                root.AddCollection<TestEntity>("collection-3", "Collection 3", collection =>
                {
                    collection
                        .SetRepository<RepositoryA>()
                        .SetTreeView("Tree 3", ViewType.List, entity => entity.Name)
                        .SetListView(listView)
                        .SetNodeEditor(nodeEditorWithPolymorphicSubCollection)
                        .AddCollection<TestEntity>("sub-collection-2", "Sub Collection 2", subCollection =>
                        {
                            subCollection
                                .SetRepository<RepositoryB>()
                                .SetTreeView("SubTree1", ViewType.List, entity => entity.Name)
                                .SetListView(listView)
                                .SetListEditor(listNodeEditor)
                                .SetNodeEditor(nodeEditor);
                        });
                });
            });

            services.AddRazorComponents<App.Startup>();

            // TODO: 
            hacky.ConfigureServices(services);
            //services.AddRazorComponents<RapidCMS.Common.Startup>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // TODO: 
            // app.UseRazorComponents<RapidCMS.Common.Startup>();
            var root = app.ApplicationServices.GetService<Root>();
            root.MaterializeRepositories(app.ApplicationServices);

            app.UseStaticFiles();
            app.UseRazorComponents<App.Startup>();

        }
    }
}
