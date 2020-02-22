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
    internal class CmsSetup : ICms, ICollectionResolver, ILogin
    {
        private Dictionary<string, CollectionSetup> _collectionMap { get; set; } = new Dictionary<string, CollectionSetup>();

        internal CmsSetup(CmsConfig config)
        {
            SiteName = config.SiteName;
            IsDevelopment = config.IsDevelopment;

            CollectionsAndPages = ConfigProcessingHelper.ProcessCollections(config);
            
            if (config.CustomLoginScreenRegistration != null)
            {
                CustomLoginScreenRegistration = new CustomTypeRegistrationSetup(config.CustomLoginScreenRegistration);
            }
            if (config.CustomLoginStatusRegistration != null)
            {
                CustomLoginStatusRegistration = new CustomTypeRegistrationSetup(config.CustomLoginStatusRegistration);
            }

            MapCollections(CollectionsAndPages.SelectNotNull(x => x as CollectionSetup));

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

        public List<ITreeElementSetup> CollectionsAndPages { get; set; }
        internal CustomTypeRegistrationSetup? CustomLoginScreenRegistration { get; set; }
        internal CustomTypeRegistrationSetup? CustomLoginStatusRegistration { get; set; }

        string ICms.SiteName => SiteName;
        bool ICms.IsDevelopment
        {
            get => IsDevelopment;
            set => IsDevelopment = value;
        }
        
        ICollectionSetup ICollectionResolver.GetCollection(string alias)
        {
            return _collectionMap.FirstOrDefault(x => x.Key == alias).Value
                ?? throw new InvalidOperationException($"Failed to find collection with alias {alias}.");
        }

        IPageSetup ICollectionResolver.GetPage(string alias)
        {
            return CollectionsAndPages.SelectNotNull(x => x as IPageSetup).First(x => x.Alias == alias);
        }

        IEnumerable<ITreeElementSetup> ICollectionResolver.GetRootCollections()
        {
            return CollectionsAndPages.Skip(1);
        }

        ITypeRegistration? ILogin.CustomLoginScreenRegistration => CustomLoginScreenRegistration;
        ITypeRegistration? ILogin.CustomLoginStatusRegistration => CustomLoginStatusRegistration;
    }
}
