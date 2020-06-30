using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Enums;
using RapidCMS.Repositories;

namespace RapidCMS.Example.Github
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddAuthorizationCore();

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<LocalStorageRepository<Person>>();

            builder.Services.AddRapidCMSWebAssembly(config =>
            {
                config.AllowAnonymousUser();

                config.AddCollection<Person, LocalStorageRepository<Person>>("person", "Person", collection =>
                {
                    collection
                        .SetTreeView(x => x.Name)
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
                        .SetNodeEditor(editor =>
                        {
                            editor.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                            editor.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);

                            editor.AddDefaultButton(DefaultButtonType.Delete);

                            editor.AddSection(section =>
                            {
                                section.AddField(x => x.Id).SetType(EditorType.Readonly);
                                section.AddField(x => x.Name);
                                section.AddField(x => x.Email);
                            });

                            editor.AddSection(section =>
                            {
                                section.AddField(x => x.Bio).SetType(EditorType.TextArea);
                            });
                        });
                });
            });

            await builder.Build().RunAsync();
        }
    }
}
