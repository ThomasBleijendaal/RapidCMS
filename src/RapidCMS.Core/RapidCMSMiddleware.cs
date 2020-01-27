using System;
using System.Net.Http;
using System.Threading;
using Blazor.FileReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Dispatchers;
using RapidCMS.Core.Factories;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Handlers;
using RapidCMS.Core.Interactions;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Request;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Providers;
using RapidCMS.Core.Resolvers;
using RapidCMS.Core.Resolvers.Data;
using RapidCMS.Core.Resolvers.Repositories;
using RapidCMS.Core.Services.Auth;
using RapidCMS.Core.Services.Concurrency;
using RapidCMS.Core.Services.Exceptions;
using RapidCMS.Core.Services.Messages;
using RapidCMS.Core.Services.Parent;
using RapidCMS.Core.Services.Persistence;
using RapidCMS.Core.Services.Presentation;
using RapidCMS.Core.Services.SidePane;
using RapidCMS.Core.Services.State;
using RapidCMS.Core.Services.Tree;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RapidCMSMiddleware
    {
        public static IServiceCollection AddRapidCMS(this IServiceCollection services, Action<ICmsConfig>? config = null)
        {
            var rootConfig = new CmsConfig();
            config?.Invoke(rootConfig);

            var cmsSetup = new CmsSetup(rootConfig);

            services.AddSingleton<ICms>(cmsSetup);
            services.AddSingleton<IDashboard>(cmsSetup);
            services.AddSingleton<ILogin>(cmsSetup);

            if (cmsSetup.AllowAnonymousUsage)
            {
                services.AddSingleton<IAuthorizationHandler, AllowAllAuthorizationHandler>();
                services.AddSingleton<AuthenticationStateProvider, AnonymousAuthenticationStateProvider>();
            }

            services.AddTransient<IUIResolverFactory, UIResolverFactory>();

            services.AddSingleton<ICollectionResolver>(cmsSetup);
            services.AddTransient<IRepositoryResolver, RepositoryResolver>();
            services.AddTransient<IDataProviderResolver, DataProviderResolver>();
            services.AddTransient<IDataViewResolver, DataViewResolver>();

            services.AddTransient<IPresenationDispatcher<GetEntityRequestModel, EditContext>, GetEntityDispatcher>();
            services.AddTransient<IPresenationDispatcher<GetEntitiesRequestModel, ListContext>, GetEntitiesDispatcher>();
            services.AddTransient<IPresentationService, PresentationService>();

            services.AddTransient<IInteractionDispatcher, EntityInteractionDispatcher>();
            services.AddTransient<IInteractionDispatcher, EntitiesInteractionDispatcher>();
            services.AddTransient<IButtonInteraction, ButtonInteraction>();
            services.AddTransient<IDragInteraction, DragInteraction>();
            services.AddTransient<IInteractionService, InteractionService>();

            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IConcurrencyService, ConcurrencyService>();
            services.AddSingleton<IExceptionService, ExceptionService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddTransient<INavigationState, NavigationState>();
            services.AddTransient<IParentService, ParentService>();
            services.AddScoped<ISidePaneService, SidePaneService>();
            services.AddTransient<ITreeService, TreeService>();

            services.AddScoped<DefaultButtonActionHandler>();
            services.AddScoped(typeof(OpenPaneButtonActionHandler<>));

            services.AddScoped(typeof(EnumDataProvider<>), typeof(EnumDataProvider<>));

            // UI requirements
            services.AddHttpContextAccessor();
            services.AddScoped<HttpContextAccessor>();

            services.AddHttpClient();
            services.AddScoped<HttpClient>();

            // Semaphore for repositories
            services.AddSingleton(serviceProvider => new SemaphoreSlim(rootConfig.SemaphoreMaxCount, rootConfig.SemaphoreMaxCount));

            services.AddFileReaderService();

            services.AddMemoryCache();

            return services;
        }

        public static IApplicationBuilder UseRapidCMS(this IApplicationBuilder app, bool isDevelopment = false)
        {
            app.ApplicationServices.GetService<ICms>().IsDevelopment = isDevelopment;

            return app;
        }
    }
}
