using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Resolvers.Setup;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup
{
    internal class CmsSetup : ICms, ICollectionResolver, ILogin
    {
        private readonly ISetupResolver<IPageSetup> _pageResolver;
        private readonly ISetupResolver<ICollectionSetup> _collectionResolver;
        private readonly ISetupResolver<IEnumerable<ITreeElementSetup>> _treeElementsResolver;

        public CmsSetup(CmsConfig config,
            ISetupResolver<IPageSetup> pageResolver,
            ISetupResolver<ICollectionSetup> collectionResolver,
            ISetupResolver<IEnumerable<ITreeElementSetup>> treeElementsResolver)
        {
            _pageResolver = pageResolver;
            _collectionResolver = collectionResolver;
            _treeElementsResolver = treeElementsResolver;

            SiteName = config.SiteName;
            IsDevelopment = config.IsDevelopment;

            // TODO: resolvers
            if (config.CustomLoginScreenRegistration != null)
            {
                CustomLoginScreenRegistration = new CustomTypeRegistrationSetup(config.CustomLoginScreenRegistration);
            }
            if (config.CustomLoginStatusRegistration != null)
            {
                CustomLoginStatusRegistration = new CustomTypeRegistrationSetup(config.CustomLoginStatusRegistration);
            }


        }

        internal string SiteName { get; set; }
        internal bool IsDevelopment { get; set; }

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
            return _collectionResolver.ResolveSetup(alias);
        }

        IPageSetup ICollectionResolver.GetPage(string alias)
        {
            return _pageResolver.ResolveSetup(alias);
        }

        IEnumerable<ITreeElementSetup> ICollectionResolver.GetRootCollections()
        {
            return _treeElementsResolver.ResolveSetup();
        }

        ITypeRegistration? ILogin.CustomLoginScreenRegistration => CustomLoginScreenRegistration;
        ITypeRegistration? ILogin.CustomLoginStatusRegistration => CustomLoginStatusRegistration;
    }
}
