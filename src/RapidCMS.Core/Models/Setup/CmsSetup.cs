using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup
{
    internal class CmsSetup : ICms, ICollectionResolver, IDashboard, ILogin
    {
        private Dictionary<string, CollectionSetup> _collectionMap { get; set; } = new Dictionary<string, CollectionSetup>();

        internal CmsSetup(CmsConfig config)
        {
            SiteName = config.SiteName;
            IsDevelopment = config.IsDevelopment;
            AllowAnonymousUsage = config.AllowAnonymousUsage;
            SemaphoreMaxCount = config.SemaphoreMaxCount;

            Collections = ConfigProcessingHelper.ProcessCollections(config);

            CustomDashboardSectionRegistrations = config.CustomDashboardSectionRegistrations.ToList(x => new CustomTypeRegistrationSetup(x));
            if (config.CustomLoginScreenRegistration != null)
            {
                CustomLoginScreenRegistration = new CustomTypeRegistrationSetup(config.CustomLoginScreenRegistration);
            }
            if (config.CustomLoginStatusRegistration != null)
            {
                CustomLoginStatusRegistration = new CustomTypeRegistrationSetup(config.CustomLoginStatusRegistration);
            }

            MapCollections(Collections);

            void MapCollections(IEnumerable<CollectionSetup> collections)
            {
                foreach (var collection in collections.Where(col => !col.Recursive))
                {
                    if (!_collectionMap.TryAdd(collection.Alias, collection))
                    {
                        throw new InvalidOperationException($"Duplicate collection alias '{collection.Alias}' not allowed.");
                    }

                    if (collection.Collections.Any())
                    {
                        MapCollections(collection.Collections);
                    }
                }
            }
        }

        internal string SiteName { get; set; }
        internal bool IsDevelopment { get; set; }
        internal bool AllowAnonymousUsage { get; set; }

        internal int SemaphoreMaxCount { get; set; }

        public List<CollectionSetup> Collections { get; set; }
        internal List<CustomTypeRegistrationSetup> CustomDashboardSectionRegistrations { get; set; }
        internal CustomTypeRegistrationSetup? CustomLoginScreenRegistration { get; set; }
        internal CustomTypeRegistrationSetup? CustomLoginStatusRegistration { get; set; }

        string ICms.SiteName => SiteName;
        bool ICms.IsDevelopment
        {
            get => IsDevelopment;
            set => IsDevelopment = value;
        }
        bool ICms.AllowAnonymousUsage => AllowAnonymousUsage;
        int ICms.SemaphoreMaxCount => SemaphoreMaxCount;

        ICollectionSetup ICollectionResolver.GetCollection(string alias)
        {
            return _collectionMap.FirstOrDefault(x => x.Key == alias).Value
                ?? throw new InvalidOperationException($"Failed to find collection with alias {alias}.");
        }

        IEnumerable<ICollectionSetup> ICollectionResolver.GetRootCollections()
        {
            return Collections;
        }

        IEnumerable<ITypeRegistration> IDashboard.CustomDashboardSectionRegistrations => CustomDashboardSectionRegistrations;

        ITypeRegistration? ILogin.CustomLoginScreenRegistration => CustomLoginScreenRegistration;
        ITypeRegistration? ILogin.CustomLoginStatusRegistration => CustomLoginStatusRegistration;
    }
}
