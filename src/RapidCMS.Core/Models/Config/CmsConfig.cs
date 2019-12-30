using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Interfaces.Config;
using RapidCMS.Core.Interfaces.Data;
using RapidCMS.Core.Repositories;

namespace RapidCMS.Core.Models.Config
{
    internal class CmsConfig : ICmsConfig
    {
        internal string SiteName { get; set; } = "RapidCMS";
        internal bool IsDevelopment { get; set; }
        internal bool AllowAnonymousUsage { get; set; } = false;

        internal int SemaphoreMaxCount { get; set; } = 1;

        public string Alias => "__root";

        internal static List<string> CollectionAliases = new List<string>();

        public List<ICollectionConfig> Collections { get; set; } = new List<ICollectionConfig>();
        internal List<CustomTypeRegistration> CustomDashboardSectionRegistrations { get; set; } = new List<CustomTypeRegistration>();
        internal CustomTypeRegistration? CustomLoginScreenRegistration { get; set; }
        internal CustomTypeRegistration? CustomLoginStatusRegistration { get; set; }

        public ICmsConfig SetCustomLoginScreen(Type loginType)
        {
            if (!loginType.IsSameTypeOrDerivedFrom(typeof(ComponentBase)))
            {
                throw new InvalidOperationException($"{nameof(loginType)} must be derived of {nameof(ComponentBase)}.");
            }

            CustomLoginScreenRegistration = new CustomTypeRegistration(loginType);

            return this;
        }

        public ICmsConfig SetCustomLoginStatus(Type loginType)
        {
            if (!loginType.IsSameTypeOrDerivedFrom(typeof(ComponentBase)))
            {
                throw new InvalidOperationException($"{nameof(loginType)} must be derived of {nameof(ComponentBase)}.");
            }

            CustomLoginStatusRegistration = new CustomTypeRegistration(loginType);

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

        public ICmsConfig AddDashboardSection(Type customDashboardSectionType)
        {
            if (!customDashboardSectionType.IsSameTypeOrDerivedFrom(typeof(ComponentBase)))
            {
                throw new InvalidOperationException($"{nameof(customDashboardSectionType)} must be derived of {nameof(ComponentBase)}.");
            }

            CustomDashboardSectionRegistrations.Add(new CustomTypeRegistration(customDashboardSectionType));

            return this;
        }

        public ICmsConfig AddDashboardSection(string collectionAlias, bool edit = false)
        {
            // TODO
            //CustomDashboardSectionRegistrations.Add(
            //    new CustomTypeRegistration(
            //        typeof(Collection),
            //        new Dictionary<string, string> {
            //            { "Action", edit ? Constants.Edit : Constants.View },
            //            { "CollectionAlias", collectionAlias } }));

            return this;
        }

        public ICollectionConfig<TEntity> AddCollection<TEntity, TRepository>(string alias, string name, Action<ICollectionConfig<TEntity>> configure)
            where TEntity : class, IEntity
            where TRepository : BaseRepository<TEntity>
        {
            return AddCollection<TEntity, TRepository>(alias, default, name, configure);
        }

        public ICollectionConfig<TEntity> AddCollection<TEntity, TRepository>(string alias, string? icon, string name, Action<ICollectionConfig<TEntity>> configure)
            where TEntity : class, IEntity
            where TRepository : BaseRepository<TEntity>
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
                icon, 
                name,
                typeof(TRepository),
                new EntityVariantConfig(typeof(TEntity).Name, typeof(TEntity)));

            configure.Invoke(configReceiver);

            Collections.Add(configReceiver);

            return configReceiver;
        }
    }
}
