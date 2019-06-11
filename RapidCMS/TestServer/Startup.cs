using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.WindowsAzure.Storage;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;
using RapidCMS.Common.ValueMappers;
using TestLibrary;
using TestLibrary.Authorization;
using TestLibrary.Data;
using TestLibrary.DataProvider;
using TestLibrary.Entities;
using TestLibrary.Repositories;
using TestServer.ActionHandlers;
using TestServer.Components.CustomButtons;
using TestServer.Components.CustomEditors;
using TestServer.Components.CustomLogin;
using TestServer.Components.CustomSections;

namespace TestServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            #region Authorization

            // ***********************************************
            // For more info on:
            // Microsoft.AspNetCore.Authentication.AzureAD.UI
            // see:
            // https://bit.ly/2Fv6Zxp
            // This creates a 'virtual' controller 
            // called 'Account' in an Area called 'AzureAd' that allows the
            // 'AzureAd/Account/SignIn' and 'AzureAd/Account/SignOut'
            // links to work
            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => Configuration.Bind("AzureAd", options));

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential
                // cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // This configures the 'middleware' pipeline
            // This is where code to determine what happens
            // when a person logs in is configured and processed
            services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Instead of using the default validation 
                    // (validating against a single issuer value, as we do in
                    // line of business apps), we inject our own multitenant validation logic
                    ValidateIssuer = false,
                    // If the app is meant to be accessed by entire organizations, 
                    // add your issuer validation logic here.
                    //IssuerValidator = (issuer, securityToken, validationParameters) => {
                    //    if (myIssuerValidationLogic(issuer)) return issuer;
                    //}
                };
                options.Events = new OpenIdConnectEvents
                {
                    OnTicketReceived = context =>
                    {
                        // If your authentication logic is based on users 
                        // then add your logic here
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.Redirect("/Error");
                        context.HandleResponse(); // Suppress the exception
                        return Task.CompletedTask;
                    },
                    OnSignedOutCallbackRedirect = context =>
                    {
                        // This is called when a user logs out
                        // redirect them back to the main page
                        context.Response.Redirect("/");
                        context.HandleResponse();
                        return Task.CompletedTask;
                    },
                    // If your application needs to do authenticate single users, 
                    // add your user validation below.
                    //OnTokenValidated = context =>
                    //{
                    //    return myUserValidationLogic(context.Ticket.Principal);
                    //}
                };
            });

            #endregion

            services.AddDbContext<TestDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlConnectionString"));
            });

            services.AddSingleton<RepositoryA>();
            services.AddSingleton<RepositoryB>();
            services.AddSingleton<RepositoryC>();
            services.AddSingleton<RepositoryD>();
            services.AddSingleton<RepositoryE>();
            services.AddSingleton<RepositoryF>();
            services.AddSingleton<VariantRepository>();
            services.AddSingleton<ValidationRepository>();

            services.AddSingleton<RelationRepository>();

            services.AddScoped<CountryRepository>();
            services.AddScoped<PersonRepository>();

            services.AddSingleton(CloudStorageAccount.DevelopmentStorageAccount);
            services.AddSingleton<AzureTableStorageRepository>();

            services.AddTransient<CreateButtonActionHandler>();

            services.AddTransient<DummyDataProvider>();

            services.AddSingleton<IAuthorizationHandler, CountryEntityAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, PersonEntityAuthorizationHandler>();

            services.AddRapidCMS(config =>
            {
                config.AllowAnonymousUser();

                config.AddCustomButton(typeof(CreateButton<>));
                config.AddCustomEditor(typeof(PasswordEditor));
                config.AddCustomSection(typeof(DashboardSection));

                config.SetCustomLogin(typeof(LoginControl));

                config.SetSiteName("Test Client");

                config.AddCollection<ValidationEntity>("validation-collection", "Validation entities", collection =>
                {
                    collection
                        .SetRepository<ValidationRepository>()
                        .SetTreeView(e => e.Name)
                        .SetListView(list =>
                        {
                            list.AddDefaultButton(DefaultButtonType.New);
                            list.SetListPane(pane =>
                            {
                                pane.AddProperty(p => p.Name);
                                pane.AddDefaultButton(DefaultButtonType.Edit);
                            });
                        })
                        .SetNodeEditor(editor =>
                        {
                            editor.AddDefaultButton(DefaultButtonType.SaveNew);
                            editor.AddDefaultButton(DefaultButtonType.SaveExisting);
                            editor.AddDefaultButton(DefaultButtonType.Delete);
                            editor.AddEditorPane(pane =>
                            {
                                pane.AddField(f => f.Name);
                                pane.AddField(f => f.NotRequired);
                                pane.AddField(f => f.Range)
                                    .SetValueMapper<LongValueMapper>();
                            });
                        });
                });

                //config.AddCollection<CountryEntity>("country-collection", "Countries", collection =>
                //{
                //    collection
                //        .SetRepository<CountryRepository>()
                //        .SetTreeView(entity => entity.Name)
                //        .SetListView(list =>
                //        {
                //            list.AddDefaultButton(DefaultButtonType.New);
                //            list.SetListPane(pane =>
                //            {
                //                pane.AddProperty(p => p.Name);
                //                pane.AddDefaultButton(DefaultButtonType.Edit);
                //            });
                //        })
                //        .SetNodeView(editor =>
                //        {
                //            editor.AddDefaultButton(DefaultButtonType.SaveNew);
                //            editor.AddDefaultButton(DefaultButtonType.SaveExisting);
                //            editor.AddDefaultButton(DefaultButtonType.Delete);
                //            editor.AddViewPane(pane =>
                //            {
                //                pane.AddProperty(f => f.Name);
                //            });
                //        })
                //        .SetNodeEditor(editor =>
                //        {
                //            editor.AddDefaultButton(DefaultButtonType.SaveNew);
                //            editor.AddDefaultButton(DefaultButtonType.SaveExisting);
                //            editor.AddDefaultButton(DefaultButtonType.Delete);
                //            editor.AddEditorPane(pane =>
                //            {
                //                pane.AddField(f => f.Name);
                //            });
                //        });
                //});

                //config.AddCollection<PersonEntity>("person-collection", "Persons", collection =>
                //{
                //    collection
                //        .SetRepository<PersonRepository>()
                //        .SetTreeView(entity => entity.Name)
                //        .SetListView(list =>
                //        {
                //            list.AddDefaultButton(DefaultButtonType.New);
                //            list.SetListPane(pane =>
                //            {
                //                pane.AddProperty(p => p.Name);
                //                pane.AddDefaultButton(DefaultButtonType.View);
                //                pane.AddDefaultButton(DefaultButtonType.Edit);
                //                pane.AddDefaultButton(DefaultButtonType.Delete);
                //            });
                //        })
                //        .SetNodeView(editor =>
                //        {
                //            editor.AddDefaultButton(DefaultButtonType.SaveNew);
                //            editor.AddDefaultButton(DefaultButtonType.SaveExisting);
                //            editor.AddDefaultButton(DefaultButtonType.Delete);
                //            editor.AddViewPane(pane =>
                //            {
                //                pane.AddProperty(f => f.Name);
                //                pane.AddProperty(f => string.Join(", ", f.Countries == null ? Enumerable.Empty<string>() : f.Countries.Select(x => x.Country.Name)))
                //                    .SetName("Countries");
                //            });
                //        })
                //        .SetNodeEditor(editor =>
                //        {
                //            editor.AddDefaultButton(DefaultButtonType.SaveNew);
                //            editor.AddDefaultButton(DefaultButtonType.SaveExisting);
                //            editor.AddDefaultButton(DefaultButtonType.Delete);
                //            editor.AddEditorPane(pane =>
                //            {
                //                pane.AddField(f => f.Name);
                //                pane.AddField(f => f.Countries.Select(x => x.CountryId))
                //                    .SetName("Countries")
                //                    .SetType(EditorType.MultiSelect)
                //                    .SetCollectionRelation<CountryEntity>("country-collection", relation =>
                //                    {
                //                        relation
                //                            .SetIdProperty(x => x._Id)
                //                            .SetDisplayProperty(x => x.Name);
                //                    });
                //            });
                //        });
                //});

                //config.AddCollection<RelationEntity>("collection-11", "Azure Table Storage Collecation with relations", collection =>
                //{
                //    collection
                //        .SetRepository<RelationRepository>()
                //        .SetTreeView(EntityVisibilty.Visible, entity => entity.Name)
                //        .SetListView(config =>
                //        {
                //            config.AddDefaultButton(DefaultButtonType.New);

                //            config.SetListPane(listPaneConfig =>
                //            {
                //                listPaneConfig.AddProperty(x => x.Id);
                //                listPaneConfig.AddProperty(x => x.Name);
                //                listPaneConfig.AddProperty(x => x.AzureTableStorageEntityId)
                //                    .SetName("Entity");
                //                listPaneConfig.AddProperty(x => string.Join(", ", x.AzureTableStorageEntityIds ?? Enumerable.Empty<string>()))
                //                    .SetName("Entities");

                //                listPaneConfig.AddDefaultButton(DefaultButtonType.Edit, isPrimary: true);
                //                listPaneConfig.AddDefaultButton(DefaultButtonType.Delete);
                //            });
                //        })
                //        .SetNodeEditor(config =>
                //        {
                //            config.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                //            config.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);
                //            config.AddDefaultButton(DefaultButtonType.Delete);

                //            config.AddEditorPane(editorPaneConfig =>
                //            {
                //                editorPaneConfig.AddField(x => x.Name);

                //                editorPaneConfig.AddField(x => x.Location)
                //                    .SetType(EditorType.Dropdown)
                //                    .SetDataRelation<DummyDataProvider>();

                //                editorPaneConfig.AddField(x => x.AzureTableStorageEntityId)
                //                    .SetName("Entity")
                //                    .SetType(EditorType.Select)
                //                    .SetCollectionRelation<AzureTableStorageEntity>("collection-10", relation =>
                //                    {
                //                        relation
                //                            .SetIdProperty(x => x.Id)
                //                            .SetDisplayProperty(x => x.Description);
                //                    });

                //                editorPaneConfig.AddField(x => x.AzureTableStorageEntityIds)
                //                    .SetName("Entities")
                //                    .SetType(EditorType.MultiSelect)
                //                    .SetCollectionRelation<AzureTableStorageEntity>("collection-10", relation =>
                //                    {
                //                        relation
                //                            .SetIdProperty(x => x.Id)
                //                            .SetDisplayProperty(x => x.Description);
                //                    });
                //            });
                //        });
                //});

                //config.AddCollection<AzureTableStorageEntity>("collection-10", "Azure Table Storage Collection", collection =>
                //{
                //    collection
                //        .SetRepository<AzureTableStorageRepository>()
                //        .SetTreeView(EntityVisibilty.Visible, entity => entity.Title)
                //        .SetListView(AzureTableStorageListView)
                //        .SetNodeEditor(AzureTableStorageEditor)

                //        .AddCollection<AzureTableStorageEntity>("collection-10-a", "Sub collection", subCollection =>
                //        {
                //            subCollection
                //                .SetRepository<AzureTableStorageRepository>()
                //                .SetTreeView(EntityVisibilty.Visible, entity => entity.Title)
                //                .SetListView(AzureTableStorageListView)
                //                .SetNodeEditor(AzureTableStorageEditor);
                //        });
                //});

                //config.AddCollection<TestEntity>("collection-6", "Variant collection as blocks", collection =>
                //{
                //    collection
                //        .SetRepository<VariantRepository>()
                //        .SetTreeView(EntityVisibilty.Hidden, entity => entity.Name)
                //        .AddEntityVariant<TestEntityVariantA>("Variant A", "align-left")
                //        .AddEntityVariant<TestEntityVariantB>("Variant B", "align-center")
                //        .AddEntityVariant<TestEntityVariantC>("Variant C", "align-right")
                //        .SetListEditor(ListEditorType.Block, listNodeEditorWithPolymorphism)
                //        .SetNodeEditor(nodeEditorWithPolymorphism);
                //});

                //config.AddCollection<TestEntity>("collection-5", "Collections with variant sub collection", collection =>
                //{
                //    collection
                //        .SetRepository<RepositoryF>()
                //        .SetTreeView(EntityVisibilty.Visible, entity => entity.Name)
                //        .SetListView(listView)
                //        .SetNodeEditor(nodeEditorWithPolymorphicSubCollection)
                //        .AddCollection<TestEntity>("sub-collection-3", "Sub Collection 3", subCollection =>
                //        {
                //            subCollection
                //                .SetRepository<VariantRepository>()
                //                .SetTreeView(EntityVisibilty.Visible, CollectionRootVisibility.Hidden, entity => entity.Name)
                //                .AddEntityVariant<TestEntityVariantA>("Variant A", "align-left")
                //                .AddEntityVariant<TestEntityVariantB>("Variant B", "align-center")
                //                .AddEntityVariant<TestEntityVariantC>("Variant C", "align-right")
                //                .SetListView(listViewWithPolymorphism)
                //                .SetListEditor(ListEditorType.Table, listNodeEditorWithPolymorphism)
                //                .SetNodeEditor(nodeEditorWithPolymorphism);
                //        });
                //});

                //config.AddCollection<TestEntity>("collection-4", "Collection with sub collections", collection =>
                //{
                //    collection
                //        .SetRepository<RepositoryA>()
                //        .SetTreeView(entity => entity.Name)
                //        .SetListView(listView)
                //        .SetNodeEditor(nodeEditorWithSubCollection)
                //        .AddCollection<TestEntity>("sub-collection-1", "Sub Collection 1", subCollection =>
                //        {
                //            subCollection
                //                .SetRepository<RepositoryB>()
                //                //.SetTreeView(EntityVisibilty.Hidden, CollectionRootVisibility.Hidden, entity => entity.Name)
                //                .SetListView(listView)
                //                .SetListEditor(ListEditorType.Table, subListNodeEditor)
                //                .SetNodeEditor(nodeEditor);
                //        })
                //        .AddCollection<TestEntity>("sub-collection-2", "Sub Collection 2", subCollection =>
                //        {
                //            subCollection
                //                .SetRepository<RepositoryC>()
                //                //.SetTreeView(EntityVisibilty.Hidden, CollectionRootVisibility.Hidden, entity => entity.Name)
                //                .SetListEditor(ListEditorType.Block, subListNodeEditor)
                //                .SetNodeEditor(nodeEditor);
                //        });
                //});

                //config.AddCollection<TestEntity>("collection-3", "Variant collection", collection =>
                //{
                //    collection
                //        .SetRepository<VariantRepository>()
                //        .SetTreeView(EntityVisibilty.Visible, entity => entity.Name)
                //        .AddEntityVariant<TestEntityVariantA>("Variant A", "align-left")
                //        .AddEntityVariant<TestEntityVariantB>("Variant B", "align-center")
                //        .AddEntityVariant<TestEntityVariantC>("Variant C", "align-right")
                //        .SetListView(listView)
                //        .SetNodeEditor(nodeEditorWithPolymorphism);
                //});

                //config.AddCollection<TestEntity>("collection-2", "List editor collection", collection =>
                //{
                //    collection
                //        .SetRepository<RepositoryD>()
                //        .SetTreeView(EntityVisibilty.Hidden, entity => entity.Name)
                //        .SetListEditor(ListEditorType.Table, listNodeEditor)
                //        .SetNodeEditor(nodeEditor);
                //});

                //config.AddCollection<TestEntity>("collection-1", "Simple collection", collection =>
                //{
                //    collection
                //        .SetRepository<RepositoryE>()
                //        .SetTreeView(EntityVisibilty.Visible, entity => entity.Name)
                //        .SetListView(listView)
                //        .SetNodeEditor(nodeEditor);
                //});
            });

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            }).AddNewtonsoftJson();

            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddCors();

            #region Editors

            void listView(ListViewConfig<TestEntity> listViewConfig)
            {
                listViewConfig
                    .AddDefaultButton(DefaultButtonType.New, "New", isPrimary: true)
                    .SetListPane(pane =>
                    {
                        pane.AddProperty(x => x._Id.ToString()).SetName("Id");
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
                        pane.AddProperty(x => x._Id.ToString()).SetName("Id");
                        pane.AddProperty(x => x.Name).SetDescription("This is a name");
                        pane.AddProperty(x => x.Description).SetDescription("This is a description");
                        pane.AddDefaultButton(DefaultButtonType.View, string.Empty);
                        pane.AddDefaultButton(DefaultButtonType.Edit, string.Empty);

                    });
            }

            void nodeEditor(NodeEditorConfig<TestEntity> nodeEditorConfig)
            {
                nodeEditorConfig
                    .AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true)
                    .AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true)
                    .AddDefaultButton(DefaultButtonType.Delete)
                    .AddEditorPane(pane =>
                    {
                        pane.SetLabel("General");

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
                        pane.AddSubCollectionListEditor<TestEntity>("sub-collection-1");
                    })

                    .AddEditorPane(pane =>
                    {
                        pane.AddSubCollectionListEditor<TestEntity>("sub-collection-2");
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
                        pane.AddSubCollectionListEditor<TestEntity>("sub-collection-3");
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

            void AzureTableStorageListView(ListViewConfig<AzureTableStorageEntity> config)
            {
                config
                    .AddDefaultButton(DefaultButtonType.New, isPrimary: true)
                    .AddCustomButton<CreateButtonActionHandler>(typeof(CreateButton<>), "Custom create!");

                config.SetListPane(listPaneConfig =>
                {
                    listPaneConfig.AddProperty(x => x.Id);
                    listPaneConfig.AddProperty(x => x.Title);
                    listPaneConfig.AddProperty(x => x.Description);
                    listPaneConfig.AddProperty(x => x.Password);
                    listPaneConfig.AddProperty(x => x.Destroy == true ? "True" : x.Destroy == false ? "False" : "Null")
                        .SetName("Destroy");

                    listPaneConfig.AddDefaultButton(DefaultButtonType.Edit, isPrimary: true);
                    listPaneConfig.AddDefaultButton(DefaultButtonType.Delete);
                });
            }

            void AzureTableStorageEditor(NodeEditorConfig<AzureTableStorageEntity> config)
            {
                config.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                config.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);
                config.AddDefaultButton(DefaultButtonType.Delete);

                config.AddEditorPane(typeof(DashboardSection));

                config.AddEditorPane(editorPaneConfig =>
                {
                    editorPaneConfig.AddField(x => x.Title);
                    editorPaneConfig.AddField(x => x.Description);
                    editorPaneConfig.AddField(x => x.Password)
                        .SetType(typeof(PasswordEditor));
                    editorPaneConfig.AddField(x => x.Destroy)
                        .SetValueMapper<BoolValueMapper>();
                });
            }

            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRapidCMS();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:5001", "https://localhost:44391/"));

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseRouting();

            app.UseMvc(routes =>
            {

            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
