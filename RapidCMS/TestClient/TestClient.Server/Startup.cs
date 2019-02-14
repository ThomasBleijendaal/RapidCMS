using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Common.Data;
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
                        pane.AddProperty(x => x.Description).SetDescription("This is a description");
                    });
            }

            void nodeEditor(NodeEditorConfig<TestEntity> nodeEditorConfig)
            {
                nodeEditorConfig
                    .AddDefaultButton(DefaultButtonType.SaveNew)
                    .AddDefaultButton(DefaultButtonType.SaveExisting)
                    .AddDefaultButton(DefaultButtonType.Delete)
                    .AddEditorPane(pane =>
                    {
                        pane.AddField(x => x.Id)
                            .SetValueMapper(new IntValueMapper());

                        pane.AddField(x => x.Name)
                            .SetDescription("This is a name");

                        pane.AddField(x => x.Description)
                            .SetDescription("This is a description")
                            .SetType(EditorType.TextArea);
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
                        .SetNodeEditor(nodeEditor)
                        .AddCollection<TestEntity>("sub-collection-1", "Sub Collection 1", subCollection =>
                        {
                            subCollection
                                .SetRepository<RepositoryB>()
                                .SetTreeView("SubTree1", ViewType.List, entity => entity.Name)
                                .SetListView(listView)
                                .SetNodeEditor(nodeEditor)
                                .AddCollection<TestEntity>("sub-sub-collection", "Sub Sub Collection", subSubCollection =>
                                {
                                    subSubCollection
                                        .SetRepository<RepositoryC>()
                                        .SetTreeView("SubSubTree", ViewType.List, entity => entity.Name)
                                        .SetListView(listView)
                                        .SetNodeEditor(nodeEditor);
                                });
                        })
                        .AddCollection<TestEntity>("sub-collection-2", "Sub Collection 2", subCollection =>
                        {
                            subCollection
                                .SetRepository<RepositoryD>()
                                .SetTreeView("SubTree2", ViewType.List, entity => entity.Name)
                                .SetListView(listView)
                                .SetNodeEditor(nodeEditor);
                        });
                });

                root.AddCollection<TestEntity>("collection-2", "Collection 2", collection =>
                {
                    collection
                        .SetRepository<RepositoryE>()
                        .SetTreeView("Tree 2", ViewType.List, entity => entity.Name)
                        .SetListView(listView)
                        .SetListEditor(list =>
                        {
                            list.AddDefaultButton(DefaultButtonType.New);
                            list.SetEditor(editor =>
                            {
                                editor.AddDefaultButton(DefaultButtonType.View);
                                editor.AddDefaultButton(DefaultButtonType.Edit);
                                editor.AddDefaultButton(DefaultButtonType.Delete);

                                editor.AddField(x => x.Id)
                                    .SetDescription("This should be readonly");

                                editor.AddField(x => x.Name)
                                    .SetDescription("This is a name");

                                editor.AddField(x => x.Description)
                                    .SetDescription("This is a description")
                                    .SetType(EditorType.TextArea);
                            });
                        })
                        .SetNodeEditor(nodeEditor);
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
