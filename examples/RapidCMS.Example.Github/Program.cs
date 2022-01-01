using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Enums;
using RapidCMS.Example.Github.Components;
using RapidCMS.Example.Github.DataViewBuilders;
using RapidCMS.Example.Github.Entities;
using RapidCMS.Repositories;

namespace RapidCMS.Example.Github
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddAuthorizationCore();

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<LocalStorageRepository<Person>>();
            builder.Services.AddScoped<LocalStorageRepository<Country>>();
            builder.Services.AddScoped<LocalStorageRepository<ValidationPerson>>();
            builder.Services.AddScoped<LocalStorageRepository<ConventionPerson>>();
            builder.Services.AddScoped<LocalStorageRepository<CountryPerson>>();
            builder.Services.AddScoped<LocalStorageRepository<RelatablePerson>>();
            builder.Services.AddScoped<LocalStorageRepository<Relatable2Person>>();

            builder.Services.AddScoped<CountryDataViewBuilder>();

            builder.Services.AddRapidCMSWebAssembly(config =>
            {
                config.AllowAnonymousUser();

                config.Dashboard.AddSection(typeof(Dashboard));

                config.AddCollection<Person, LocalStorageRepository<Person>>("person", "FabricUserFolder", "Red10", "Person", collection =>
                {
                    collection
                        .SetTreeView(x => x.Name)
                        .SetListView(view =>
                        {
                            view.AddDefaultButton(DefaultButtonType.New);

                            view.AddRow(row =>
                            {
                                row.AddField(p => p.Id).SetName("ID");
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

                            editor.AddSection(typeof(SimplePersonCollection));
                        });
                });

                config.AddCollection<Person, LocalStorageRepository<Person>>("sort-person", "Sort", "Green10", "Sortable Person", collection =>
                {
                    collection
                        .SetTreeView(x => x.Name)
                        .SetListView(view =>
                        {
                            view.AddDefaultButton(DefaultButtonType.New);

                            view.AddRow(row =>
                            {
                                row.AddField(p => p.Id).SetName("ID");
                                row.AddField(p => p.Name).SetOrderByExpression(x => x.Name, OrderByType.Descending);
                                row.AddField(p => p.Email).SetOrderByExpression(x => x.Email);

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

                            editor.AddSection(typeof(SortPersonCollection));
                        });
                });

                config.AddCollection<Country, LocalStorageRepository<Country>>("country", "MapPin", "GreenCyan10", "Countries", collection =>
                {
                    collection
                        .SetTreeView(EntityVisibilty.Hidden, x => x.Name)
                        .SetListEditor(editor =>
                        {
                            editor.AllowReordering(true);

                            editor.AddDefaultButton(DefaultButtonType.New);
                            editor.AddDefaultButton(DefaultButtonType.Return);
                            editor.AddDefaultButton(DefaultButtonType.SaveExisting);

                            editor.AddSection(row =>
                            {
                                row.AddField(p => p.Id).SetType(DisplayType.Label);
                                row.AddField(p => p.Name);

                                row.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                                row.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);
                                row.AddDefaultButton(DefaultButtonType.View, "View code");
                                row.AddDefaultButton(DefaultButtonType.Delete);
                            });

                        })
                        .SetNodeView(view =>
                        {
                            view.AddDefaultButton(DefaultButtonType.Up);

                            view.AddSection(typeof(SimpleCountryCollection));
                        });
                });

                config.AddCollection<ValidationPerson, LocalStorageRepository<ValidationPerson>>("validation-person", "UserFollowed", "Magenta10", "Validation Person", collection =>
                {
                    collection
                        .SetTreeView(x => x.Name)
                        .SetListView(view =>
                        {
                            view.AddDefaultButton(DefaultButtonType.New);

                            view.AddRow(row =>
                            {
                                row.AddField(p => p.Id).SetName("ID");
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

                            editor.AddSection(typeof(SimplePersonCollection));
                        });
                });

                config.AddCollection<Person, LocalStorageRepository<Person>>("person-with-inline-countries", "FabricUserFolder", "RedOrange10", "Person With Inline Countries", collection =>
                {
                    collection
                        .SetTreeView(x => x.Name)
                        .SetListView(view =>
                        {
                            view.AddDefaultButton(DefaultButtonType.New);

                            view.AddRow(row =>
                            {
                                row.AddField(p => p.Id).SetName("ID");
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

                            editor.AddSection(section =>
                            {
                                section.VisibleWhen((person, state) => state == EntityState.IsExisting);

                                section.AddSubCollectionList<Country, LocalStorageRepository<Country>>(subCollection =>
                                {
                                    subCollection.SetListEditor(listEditor =>
                                    {
                                        listEditor.AddDefaultButton(DefaultButtonType.New);
                                        listEditor.AddDefaultButton(DefaultButtonType.Return);

                                        listEditor.AddSection(row =>
                                        {
                                            row.AddField(p => p.Id).SetType(DisplayType.Label);
                                            row.AddField(p => p.Name);

                                            row.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                                            row.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);
                                            row.AddDefaultButton(DefaultButtonType.Delete);
                                        });
                                    });
                                });
                            });

                            editor.AddSection(typeof(PersonCountryCollection));
                        });
                });

                config.AddCollection<ConventionPerson, LocalStorageRepository<ConventionPerson>>("recursive-person", "UserSync", "Cyan30", "Recursive Person - By Convention", collection =>
                {
                    collection
                        .SetTreeView(x => x.Name)
                        .ConfigureByConvention(CollectionConvention.ListViewNodeEditor);

                    collection.AddSelfAsRecursiveCollection();
                });

                config.AddCollection<ConventionPerson, LocalStorageRepository<ConventionPerson>>("recursive-person-view", "UserSync", "YellowGreen10", "Recursive Person - By Convention (read-only)", collection =>
                {
                    collection
                        .SetTreeView(x => x.Name)
                        .ConfigureByConvention(CollectionConvention.ListViewNodeView);

                    collection.AddSelfAsRecursiveCollection();
                });

                config.AddCollection<CountryPerson, LocalStorageRepository<CountryPerson>>("person-with-countries", "FabricUserFolder", "Cyan10", "Person With Countries", collection =>
                {
                    collection
                        .SetTreeView(x => x.Name)
                        .SetListView(view =>
                        {
                            view.AddDefaultButton(DefaultButtonType.New);

                            view.AddRow(row =>
                            {
                                row.AddField(p => p.Id).SetName("ID");
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

                                section.AddField(x => x.FavouriteCountryId1)
                                    .SetName("Favourite country (based on sub collection)")
                                    .SetType(EditorType.Select)
                                    .SetCollectionRelation<Country, LocalStorageRepository<Country>>(field =>
                                    {
                                        field.SetElementDisplayProperties(x => x.Name);
                                        field.SetElementIdProperty(x => x.Id);
                                        field.SetEntityAsParent();
                                    });

                                section.AddField(x => x.FavouriteCountryId2)
                                    .SetName("Favourite country (based on country collection)")
                                    .SetType(EditorType.Select)
                                    .SetCollectionRelation<Country, LocalStorageRepository<Country>>(field =>
                                    {
                                        field.SetElementDisplayProperties(x => x.Name);
                                        field.SetElementIdProperty(x => x.Id);
                                    });
                            });

                            editor.AddSection(section =>
                            {
                                section.AddField(x => x.Bio).SetType(EditorType.TextArea);
                            });

                            editor.AddSection(section =>
                            {
                                section.VisibleWhen((person, state) => state == EntityState.IsExisting);

                                section.AddSubCollectionList("countries-under-person");
                            });

                            editor.AddSection(typeof(PersonCountry2Collection));
                        });

                    collection.AddSubCollection<Country, LocalStorageRepository<Country>>("countries-under-person", "Countries", config =>
                    {
                        config.SetTreeView(EntityVisibilty.Hidden, x => x.Name);

                        config.SetListEditor(listEditor =>
                        {
                            listEditor.AddDefaultButton(DefaultButtonType.New);
                            listEditor.AddDefaultButton(DefaultButtonType.Return);

                            listEditor.AddSection(row =>
                            {
                                row.AddField(p => p.Id).SetType(DisplayType.Label);
                                row.AddField(p => p.Name);

                                row.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                                row.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);
                                row.AddDefaultButton(DefaultButtonType.Delete);
                            });
                        });
                    });
                });

                config.AddCollection<Country, LocalStorageRepository<Country>>("static-data-view-country", "TabTwoColumn", "PinkRed10", "Countries With Static Data Views", collection =>
                {
                    collection
                        .SetTreeView(EntityVisibilty.Hidden, x => x.Name)
                        .AddDataView("Countries A-K", x => Regex.IsMatch(x.Name ?? string.Empty, "^[A-K]", RegexOptions.IgnoreCase))
                        .AddDataView("Countries L-Z", x => Regex.IsMatch(x.Name ?? string.Empty, "^[L-Z]", RegexOptions.IgnoreCase))
                        .SetListEditor(editor =>
                        {
                            editor.AddDefaultButton(DefaultButtonType.New);
                            editor.AddDefaultButton(DefaultButtonType.Return);

                            editor.AddSection(row =>
                            {
                                row.AddField(p => p.Id).SetType(DisplayType.Label);
                                row.AddField(p => p.Name);

                                row.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                                row.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);
                                row.AddDefaultButton(DefaultButtonType.View, "View code");
                                row.AddDefaultButton(DefaultButtonType.Delete);
                            });

                        })
                        .SetNodeView(view =>
                        {
                            view.AddDefaultButton(DefaultButtonType.Up);

                            view.AddSection(typeof(StaticDataViewCountryCollection));
                        });
                });

                config.AddCollection<Country, LocalStorageRepository<Country>>("dynamic-data-view-country", "TabThreeColumn", "Red20", "Countries With Dynamic Data Views", collection =>
                {
                    collection
                        .SetTreeView(EntityVisibilty.Hidden, x => x.Name)
                        .SetDataViewBuilder<CountryDataViewBuilder>()
                        .SetListEditor(editor =>
                        {
                            editor.AddDefaultButton(DefaultButtonType.New);
                            editor.AddDefaultButton(DefaultButtonType.Return);

                            editor.AddSection(row =>
                            {
                                row.AddField(p => p.Id).SetType(DisplayType.Label);
                                row.AddField(p => p.Name);

                                row.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                                row.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);
                                row.AddDefaultButton(DefaultButtonType.View, "View code");
                                row.AddDefaultButton(DefaultButtonType.Delete);
                            });

                        })
                        .SetNodeView(view =>
                        {
                            view.AddDefaultButton(DefaultButtonType.Up);

                            view.AddSection(typeof(DynamicDataViewCountryCollection));
                        });
                });

                config.AddCollection<RelatablePerson, LocalStorageRepository<RelatablePerson>>("person-relation", "Relationship", "BlueMagenta30", "Person With Relations", collection =>
                {
                    collection
                        .SetTreeView(x => x.Name)
                        .SetListView(view =>
                        {
                            view.AddDefaultButton(DefaultButtonType.New);

                            view.AddRow(row =>
                            {
                                row.AddField(p => p.Id).SetName("ID");
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

                            editor.AddSection(section =>
                            {
                                section.VisibleWhen((entity, state) => state == EntityState.IsExisting);

                                section.AddField(x => x.Countries)
                                    .SetType(EditorType.MultiSelect)
                                    .SetCollectionRelation<Country, string>(
                                        countries => countries.Select(x => x.Id ?? string.Empty),
                                        "country",
                                        relation =>
                                        {
                                            relation.SetElementDisplayProperties(x => x.Name);
                                            relation.SetElementIdProperty(x => x.Id);
                                        });
                            });

                            editor.AddSection(typeof(PersonCountry3Collection));
                        });
                });

                config.AddCollection<Relatable2Person, LocalStorageRepository<Relatable2Person>>("person-relation-collection", "Relationship", "Magenta20", "Person With Relations Collection", collection =>
                {
                    collection
                        .SetTreeView(x => x.Name)
                        .SetListView(view =>
                        {
                            view.AddDefaultButton(DefaultButtonType.New);

                            view.AddRow(row =>
                            {
                                row.AddField(p => p.Id).SetName("ID");
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

                            editor.AddSection(section =>
                            {
                                section.VisibleWhen((entity, state) => state == EntityState.IsExisting);
                                section.AddRelatedCollectionList<Country, LocalStorageRepository<Country>>(relation =>
                                {
                                    relation.SetListEditor(listEditor =>
                                    {
                                        listEditor.AddDefaultButton(DefaultButtonType.Return);
                                        listEditor.AddDefaultButton(DefaultButtonType.Add);
                                        listEditor.AddDefaultButton(DefaultButtonType.New);

                                        listEditor.AddSection(row =>
                                        {
                                            row.AddField(p => p.Id).SetType(DisplayType.Label);
                                            row.AddField(p => p.Name);

                                            row.AddDefaultButton(DefaultButtonType.Pick, isPrimary: true);
                                            row.AddDefaultButton(DefaultButtonType.Remove, isPrimary: true);
                                            row.AddDefaultButton(DefaultButtonType.SaveExisting);
                                            row.AddDefaultButton(DefaultButtonType.SaveNew);
                                            row.AddDefaultButton(DefaultButtonType.Delete);
                                        });
                                    });
                                });
                            });

                            editor.AddSection(typeof(PersonCountry4Collection));
                        });
                });
            });

            await builder.Build().RunAsync();
        }
    }
}
