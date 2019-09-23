using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Example.Components;
using RapidCMS.Example.Data;
using RapidCMS.Repositories;

namespace RapidCMS.Example
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddScoped<InMemoryRepository<Person>>();
            services.AddScoped<InMemoryRepository<Country>>();
            services.AddScoped<InMemoryRepository<User>>();


            services.AddRapidCMS(config =>
            {
                config.AllowAnonymousUser();

                // CRUD editor for simple POCO.
                config.AddCollection<Person>("person", "Person", collection =>
                {
                    collection
                        .SetTreeView(x => x.Name)
                        .SetRepository<InMemoryRepository<Person>>()
                        .SetListView(view =>
                        {
                            view.AddDefaultButton(DefaultButtonType.New);

                            view.AddRow(row =>
                            {
                                row.AddField(p => p.Id.ToString()).SetName("ID");
                                row.AddField(p => p.Name);

                                row.AddDefaultButton(DefaultButtonType.Edit);
                            });
                        })
                        .SetListEditor(editor =>
                        {
                            editor.AddDefaultButton(DefaultButtonType.New);

                            editor.AddSection(row =>
                            {
                                row.AddField(p => p.Id);
                                row.AddField(p => p.Name);

                                row.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                                row.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);
                                row.AddDefaultButton(DefaultButtonType.View);
                                row.AddDefaultButton(DefaultButtonType.Edit);
                            });
                        })
                        .SetNodeEditor(editor =>
                        {
                            editor.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                            editor.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);

                            editor.AddDefaultButton(DefaultButtonType.Delete);

                            editor.AddSection(section =>
                            {
                                section.AddField(x => x.Id).SetReadonly();
                                section.AddField(x => x.Name);
                                section.AddField(x => x.Email);
                            });

                            editor.AddSection(section =>
                            {
                                section.AddField(x => x.Bio).SetType(EditorType.TextArea);
                            });
                        })
                        .SetNodeView(view =>
                        {
                            view.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                            view.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);

                            view.AddDefaultButton(DefaultButtonType.Delete);

                            view.AddSection(section =>
                            {
                                section.AddField(x => x.Id.ToString()).SetName("ID");
                                section.AddField(x => x.Name);
                                section.AddField(x => x.Email);
                            });

                            view.AddSection(section =>
                            {
                                section.AddField(x => x.Bio);
                            });
                        });
                });

                // CRUD editor with support for one-to-many relation + validation
                config.AddCollection<Country>("country", "Country", collection =>
                {
                    collection
                        .SetTreeView(x => x.Name)
                        .SetRepository<InMemoryRepository<Country>>()
                        .SetListView(view =>
                        {
                            view.AddDefaultButton(DefaultButtonType.New);

                            view
                                .AddRow(row =>
                                {
                                    row.AddField(p => p.Id.ToString()).SetName("ID");
                                    row.AddField(p => p.Name);

                                    row.AddDefaultButton(DefaultButtonType.Edit);
                                });
                        })
                        .SetNodeEditor(editor =>
                        {
                            editor
                                .AddDefaultButton(DefaultButtonType.SaveExisting)
                                .AddDefaultButton(DefaultButtonType.SaveNew);

                            editor.AddSection(section =>
                            {
                                section.AddField(x => x.Name);

                                // basic One-To-Many editor
                                section.AddField(x => x.People)
                                    .SetType(EditorType.MultiSelect)
                                    .SetCollectionRelation<Person, int>(
                                        people => people.Select(p => p.Id),
                                        "person",
                                        relation =>
                                        {
                                            relation
                                                .SetElementIdProperty(x => x.Id)
                                                .SetElementDisplayProperties(x => x.Name, x => x.Email);
                                        });
                            });
                        });
                });

                // CURD editor with validation attributes, custom editor and custom button panes
                config.AddCollection<User>("user", "User", collection =>
                {
                    collection
                        .SetTreeView(EntityVisibilty.Hidden, x => x.Name)
                        .SetRepository<InMemoryRepository<User>>()
                        .SetListEditor(editor =>
                        {
                            editor.AddDefaultButton(DefaultButtonType.Return);
                            editor.AddDefaultButton(DefaultButtonType.New);
                            editor.AddPaneButton(typeof(ResetAllPane), "Reset all passwords", "trash");

                            editor
                                .AddSection(section =>
                                {
                                    section.AddField(x => x.Name);
                                    section.AddField(x => x.Password).SetType(typeof(PasswordEditor));

                                    section.AddDefaultButton(DefaultButtonType.SaveExisting);
                                    section.AddDefaultButton(DefaultButtonType.SaveNew);
                                });
                        });
                });

                // Do not touch this setting if you do not know what you are doing :)
                // Due to InMemoryRepository uses other InMemoryRepositories while saving, this limit must be increased, otherwise the repos will deadlock during save.
                // Please do not use this setting unless you run into issues with repo-repo deadlocking.
                config.DangerouslyFiddleWithSemaphoreSettings(2);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
