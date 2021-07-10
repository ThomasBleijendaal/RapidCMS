using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Core.Models.Config
{
    internal class CmsConfig : ICmsConfig
    {
        internal CmsAdvancedConfig AdvancedConfig { get; set; } = new CmsAdvancedConfig
        {
            SemaphoreCount = 1
        };
        internal string SiteName { get; set; } = "RapidCMS";
        internal bool IsDevelopment { get; set; }
        internal bool AllowAnonymousUsage { get; set; } = false;

        public string Alias => "__root";
        public string Name => "Root";
        public string? ParentAlias => default;
        public bool Recursive => throw new NotImplementedException();
        Type ICollectionConfig.RepositoryType => throw new NotImplementedException();

        internal static List<string> CollectionAliases = new List<string>();

        internal CustomTypeRegistrationConfig? CustomLoginScreenRegistration { get; set; }
        internal CustomTypeRegistrationConfig? CustomLoginStatusRegistration { get; set; }
        internal CustomTypeRegistrationConfig? CustomLandingPageRegistration { get; set; }

        IEnumerable<ITreeElementConfig> ICollectionConfig.CollectionsAndPages => CollectionsAndPages;

        IEnumerable<Type> ICmsConfig.Plugins => Plugins;
        
        IEnumerable<Type> ICollectionConfig.RepositoryTypes 
            => CollectionsAndPages.SelectNotNull(c => c as ICollectionConfig).SelectMany(c => c.RepositoryTypes);

        public List<ITreeElementConfig> CollectionsAndPages { get; set; } = new List<ITreeElementConfig>
        {
            new PageConfig("Dashboard", "apps", "Gray30", "__dashboard")
        };

        public List<Type> Plugins { get; set; } = new List<Type>();

        public IAdvancedCmsConfig Advanced => AdvancedConfig;

        public IPageConfig Dashboard => CollectionsAndPages.SelectNotNull(x => x as IPageConfig).First();

        public ICmsConfig SetCustomLoginScreen(Type loginType)
        {
            if (!loginType.IsSameTypeOrDerivedFrom(typeof(ComponentBase)))
            {
                throw new InvalidOperationException($"{nameof(loginType)} must be derived of {nameof(ComponentBase)}.");
            }

            CustomLoginScreenRegistration = new CustomTypeRegistrationConfig(loginType);

            return this;
        }

        public ICmsConfig SetCustomLoginStatus(Type loginType)
        {
            if (!loginType.IsSameTypeOrDerivedFrom(typeof(ComponentBase)))
            {
                throw new InvalidOperationException($"{nameof(loginType)} must be derived of {nameof(ComponentBase)}.");
            }

            CustomLoginStatusRegistration = new CustomTypeRegistrationConfig(loginType);

            return this;
        }

        public ICmsConfig SetEmptyLandingPage(Type loginType)
        {
            if (!loginType.IsSameTypeOrDerivedFrom(typeof(ComponentBase)))
            {
                throw new InvalidOperationException($"{nameof(loginType)} must be derived of {nameof(ComponentBase)}.");
            }

            CustomLandingPageRegistration = new CustomTypeRegistrationConfig(loginType);

            return this;
        }

        public ICmsConfig SetSiteName(string siteName)
        {
            SiteName = siteName;

            return this;
        }

        public ICmsConfig AllowAnonymousUser()
        {
            AllowAnonymousUsage = true;

            return this;
        }

        public ICollectionConfig<TEntity> AddCollection<TEntity, TRepository>(string alias, string name, Action<ICollectionConfig<TEntity>> configure)
            where TEntity : class, IEntity
            where TRepository : IRepository
        {
            return AddCollection<TEntity, TRepository>(alias, default, default, name, configure);
        }

        public ICollectionConfig<TEntity> AddCollection<TEntity, TRepository>(string alias, string? icon, string name, Action<ICollectionConfig<TEntity>> configure)
            where TEntity : class, IEntity
            where TRepository : IRepository
        {
            return AddCollection<TEntity, TRepository>(alias, icon, default, name, configure);
        }

        public ICollectionConfig<TEntity> AddCollection<TEntity, TRepository>(string alias, string? icon, string? color, string name, Action<ICollectionConfig<TEntity>> configure)
            where TEntity : class, IEntity
            where TRepository : IRepository
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            if (alias != alias.ToUrlFriendlyString())
            {
                throw new ArgumentException($"Use lowercase, hyphened strings as alias for collections, '{alias.ToUrlFriendlyString()}' instead of '{alias}'.");
            }
            if (CollectionAliases.Contains(alias))
            {
                throw new NotUniqueException(nameof(alias));
            }

            CollectionAliases.Add(alias);

            var configReceiver = new CollectionConfig<TEntity>(
                alias,
                default,
                icon,
                color,
                name,
                typeof(TRepository),
                new EntityVariantConfig(typeof(TEntity).Name, typeof(TEntity)));

            configure.Invoke(configReceiver);

            CollectionsAndPages.Add(configReceiver);

            return configReceiver;
        }

        public ICmsConfig AddPage(string name, Action<IPageConfig> configure)
        {
            return AddPage("Document", name, configure);
        }

        public ICmsConfig AddPage(string icon, string name, Action<IPageConfig> configure)
        {
            return AddPage(icon, "Green10", name, configure);
        }

        public ICmsConfig AddPage(string icon, string color, string name, Action<IPageConfig> configure)
        {
            var alias = name.ToUrlFriendlyString();

            if (CollectionAliases.Contains(alias))
            {
                throw new NotUniqueException(nameof(alias));
            }

            CollectionAliases.Add(alias);

            var page = new PageConfig(name, icon, color, alias);

            configure.Invoke(page);

            CollectionsAndPages.Add(page);

            return this;
        }

        ICmsConfig ICmsConfig.AddPlugin<TPlugin>()
        {
            Plugins.Add(typeof(TPlugin));

            return this;
        }
    }
}
