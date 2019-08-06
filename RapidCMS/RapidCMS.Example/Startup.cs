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

            services.AddRapidCMS(config =>
            {
                config.AllowAnonymousUser();

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
                                row.AddProperty(p => p.Id.ToString()).SetName("ID");
                                row.AddProperty(p => p.Name);

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
                                section.AddProperty(x => x.Id.ToString()).SetName("ID");
                                section.AddProperty(x => x.Name);
                                section.AddProperty(x => x.Email);
                            });

                            view.AddSection(section =>
                            {
                                section.AddProperty(x => x.Bio);
                            });
                        });
                });
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
