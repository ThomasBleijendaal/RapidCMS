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
        private readonly ISetupResolver<ITypeRegistration, CustomTypeRegistrationConfig> _typeRegistrationSetupResolver;

        public CmsSetup(CmsConfig config,
            ISetupResolver<IPageSetup> pageResolver,
            ISetupResolver<ICollectionSetup> collectionResolver,
            ISetupResolver<IEnumerable<ITreeElementSetup>> treeElementsResolver,
            ISetupResolver<ITypeRegistration, CustomTypeRegistrationConfig> typeRegistrationSetupResolver)
        {
            _pageResolver = pageResolver;
            _collectionResolver = collectionResolver;
            _treeElementsResolver = treeElementsResolver;
            _typeRegistrationSetupResolver = typeRegistrationSetupResolver;

            // TODO: resolve?
            SiteName = config.SiteName;
            IsDevelopment = config.IsDevelopment;

            if (config.CustomLoginScreenRegistration != null)
            {
                CustomLoginScreenRegistration = _typeRegistrationSetupResolver.ResolveSetup(config.CustomLoginScreenRegistration, default!); // TODO
            }
            if (config.CustomLoginStatusRegistration != null)
            {
                CustomLoginStatusRegistration = _typeRegistrationSetupResolver.ResolveSetup(config.CustomLoginStatusRegistration, default!);
            }
        }

        internal string SiteName { get; set; }
        internal bool IsDevelopment { get; set; }

        public ITypeRegistration? CustomLoginScreenRegistration { get; internal set; }
        public ITypeRegistration? CustomLoginStatusRegistration { get; internal set; }

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
    }
}
