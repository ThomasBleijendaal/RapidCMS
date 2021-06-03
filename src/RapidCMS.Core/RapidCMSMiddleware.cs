using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Abstractions.State;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Dispatchers;
using RapidCMS.Core.Dispatchers.Form;
using RapidCMS.Core.Factories;
using RapidCMS.Core.Handlers;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Interactions;
using RapidCMS.Core.Mediators;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Providers;
using RapidCMS.Core.Resolvers.Buttons;
using RapidCMS.Core.Resolvers.Convention;
using RapidCMS.Core.Resolvers.Data;
using RapidCMS.Core.Resolvers.Language;
using RapidCMS.Core.Resolvers.Repositories;
using RapidCMS.Core.Resolvers.Setup;
using RapidCMS.Core.Services.Concurrency;
using RapidCMS.Core.Services.Parent;
using RapidCMS.Core.Services.Persistence;
using RapidCMS.Core.Services.Presentation;
using RapidCMS.Core.Services.State;
using RapidCMS.Core.Services.Tree;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class RapidCMSMiddleware
    {
        private static IServiceCollection AddRapidCMSCore(this IServiceCollection services, CmsConfig rootConfig)
        {
            services.AddSingleton(rootConfig);
            services.AddSingleton<ICmsConfig>(rootConfig);

            services.AddSingleton<ICms, CmsSetup>();
            services.AddSingleton(x => (ILogin)x.GetRequiredService(typeof(ICms)));

            services.AddSingleton<ISetupResolver<IPageSetup>, PageSetupResolver>();
            services.AddSingleton<ISetupResolver<ICollectionSetup>, CollectionSetupResolver>();
            services.AddSingleton<ISetupResolver<IEnumerable<ITreeElementSetup>>, TreeElementsSetupResolver>();
            services.AddSingleton<ISetupResolver<IEnumerable<ITreeElementSetup>, IEnumerable<ITreeElementConfig>>, TreeElementSetupResolver>();

            services.AddSingleton<ISetupResolver<ITypeRegistration, CustomTypeRegistrationConfig>, TypeRegistrationSetupResolver>();
            services.AddSingleton<ISetupResolver<IEntityVariantSetup, EntityVariantConfig>, EntityVariantSetupResolver>();
            services.AddSingleton<ISetupResolver<TreeViewSetup, TreeViewConfig>, TreeViewSetupResolver>();
            services.AddSingleton<ISetupResolver<PaneSetup, PaneConfig>, PaneSetupResolver>();
            services.AddSingleton<ISetupResolver<ListSetup, ListConfig>, ListSetupResolver>();
            services.AddSingleton<ISetupResolver<NodeSetup, NodeConfig>, NodeSetupResolver>();
            services.AddSingleton<ISetupResolver<FieldSetup, FieldConfig>, FieldSetupResolver>();
            services.AddSingleton<ISetupResolver<IButtonSetup, ButtonConfig>, ButtonSetupResolver>();
            services.AddSingleton<ISetupResolver<SubCollectionListSetup, CollectionListConfig>, SubCollectionListSetupResolver>();
            services.AddSingleton<ISetupResolver<RelatedCollectionListSetup, CollectionListConfig>, RelatedCollectionListSetupResolver>();

            services.AddSingleton<IConventionBasedResolver<ListConfig>, ConventionBasedListConfigResolver>();
            services.AddSingleton<IConventionBasedResolver<NodeConfig>, ConventionBasedNodeConfigResolver>();
            services.AddSingleton<IFieldConfigResolver, FieldConfigResolver>();
            services.AddSingleton<ILanguageResolver, LanguageResolver>();

            services.AddSingleton<ISetupResolver<IEnumerable<ITreeElementSetup>, IPlugin>, PluginTreeElementsSetupResolver>();

            if (rootConfig.AllowAnonymousUsage)
            {
                services.AddSingleton<IAuthorizationHandler, AllowAllAuthorizationHandler>();
                services.AddSingleton<AuthenticationStateProvider, AnonymousAuthenticationStateProvider>();
            }

            services.AddTransient<IEditContextFactory, FormEditContextWrapperFactory>();
            services.AddTransient<IUIResolverFactory, UIResolverFactory>();

            services.AddTransient<IButtonActionHandlerResolver, ButtonActionHandlerResolver>();
            services.AddTransient<IDataProviderResolver, DataProviderResolver>();
            services.AddTransient<IRepositoryResolver, RepositoryResolver>();

            services.AddTransient<IPresentationDispatcher, GetEntityDispatcher>();
            services.AddTransient<IPresentationDispatcher, GetEntitiesDispatcher>();
            services.AddTransient<IPresentationDispatcher, GetPageDispatcher>();
            services.AddTransient<IPresentationService, PresentationService>();

            services.AddTransient<IInteractionDispatcher, EntityInteractionDispatcher>();
            services.AddTransient<IInteractionDispatcher, EntitiesInteractionDispatcher>();
            services.AddTransient<IButtonInteraction, ButtonInteraction>();
            services.AddTransient<IDragInteraction, DragInteraction>();
            services.AddTransient<IInteractionService, InteractionService>();

            services.AddTransient<IConcurrencyService, ConcurrencyService>();
            services.AddTransient<IPageState, PageState>();
            services.AddTransient<IParentService, ParentService>();
            services.AddTransient<ITreeService, TreeService>();

            services.AddScoped<IMediator, Mediator>();

            services.AddScoped<DefaultButtonActionHandler>();
            services.AddScoped(typeof(OpenPaneButtonActionHandler<>));
            services.AddScoped(typeof(NavigateButtonActionHandler<>));

            services.AddScoped(typeof(EnumDataProvider<>), typeof(EnumDataProvider<>));

            AddServicesRequiringRepositories(services, rootConfig);

            services.AddScoped<IMediatorEventListener, RepositoryMediatorEventConverter>();

            // UI requirements
            services.AddHttpContextAccessor();
            services.AddHttpClient();

            services.AddMemoryCache();

            return services;
        }

        private static void AddServicesRequiringRepositories(IServiceCollection services, CmsConfig rootConfig)
        {
            var repositoryTypeDictionary = new Dictionary<string, Type>();
            var reverseRepositoryTypeDictionary = new Dictionary<Type, string>();
            var collectionAliasDictionary = new Dictionary<string, List<string>>();

            foreach (var collection in rootConfig.CollectionsAndPages.OfType<ICollectionConfig>())
            {
                ProcessCollection(collection);
            }

            services.AddSingleton<IRepositoryTypeResolver>(new CmsRepositoryTypeResolver(repositoryTypeDictionary, reverseRepositoryTypeDictionary));
            services.AddSingleton<ICollectionAliasResolver>(new CollectionAliasResolver(collectionAliasDictionary));

            void ProcessCollection(ICollectionConfig collection)
            {
                var descriptor = services.FirstOrDefault(x => x.ServiceType == collection.RepositoryType);
                if (descriptor == null)
                {
                    throw new InvalidOperationException($"Could not find service descriptor for {collection.RepositoryType}. Please add it to the service collection.");
                }

                var implementationType = descriptor.ImplementationType;
                if (implementationType == null)
                {
                    throw new InvalidOperationException($"Could not find implementation type for {collection.RepositoryType}. Please add it as type to the service collection.");
                }

                var repositoryAlias = AliasHelper.GetRepositoryAlias(implementationType);

                repositoryTypeDictionary[repositoryAlias] = collection.RepositoryType;
                reverseRepositoryTypeDictionary[collection.RepositoryType] = repositoryAlias;
                if (implementationType != collection.RepositoryType)
                {
                    reverseRepositoryTypeDictionary[implementationType] = repositoryAlias;
                }

                if (!collectionAliasDictionary.ContainsKey(repositoryAlias))
                {
                    collectionAliasDictionary.Add(repositoryAlias, new List<string>());
                }
                collectionAliasDictionary[repositoryAlias].Add(collection.Alias);

                foreach (var subCollection in collection.CollectionsAndPages.OfType<ICollectionConfig>().Where(x => x is not ReferencedCollectionConfig)) // !x.Recursive))
                {
                    ProcessCollection(subCollection);
                }
            }
        }

        public static IApplicationBuilder UseRapidCMS(this IApplicationBuilder app, bool isDevelopment = false)
        {
            app.ApplicationServices.GetRequiredService<ICms>().IsDevelopment = isDevelopment;

            return app;
        }

        private static CmsConfig GetRootConfig(Action<ICmsConfig>? config = null)
        {
            var rootConfig = new CmsConfig();
            config?.Invoke(rootConfig);
            return rootConfig;
        }
    }
}
