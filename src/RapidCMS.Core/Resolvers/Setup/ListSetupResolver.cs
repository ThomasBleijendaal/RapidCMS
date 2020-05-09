using System;
using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class ListSetupResolver : ISetupResolver<ListSetup, ListConfig>
    {
        private readonly ISetupResolver<PaneSetup, PaneConfig> _paneSetupResolver;
        private readonly ISetupResolver<IButtonSetup, ButtonConfig> _buttonSetupResolver;
        private readonly IConventionBasedResolver<ListConfig> _conventionListConfigResolver;

        public ListSetupResolver(
            ISetupResolver<PaneSetup, PaneConfig> paneSetupResolver,
            ISetupResolver<IButtonSetup, ButtonConfig> buttonSetupResolver,
            IConventionBasedResolver<ListConfig> conventionListConfigResolver)
        {
            _paneSetupResolver = paneSetupResolver;
            _buttonSetupResolver = buttonSetupResolver;
            _conventionListConfigResolver = conventionListConfigResolver;
        }

        public IResolvedSetup<ListSetup> ResolveSetup(ListConfig config, ICollectionSetup? collection = default)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (config is IIsConventionBased isConventionBasedConfig)
            {
                config = _conventionListConfigResolver.ResolveByConvention(config.BaseType, isConventionBasedConfig.GetFeatures());
            }

            var cacheable = true;

            var panes = _paneSetupResolver.ResolveSetup(config.Panes, collection).CheckIfCachable(ref cacheable).ToList();
            var buttons = _buttonSetupResolver.ResolveSetup(config.Buttons, collection).CheckIfCachable(ref cacheable).ToList();

            return new ResolvedSetup<ListSetup>(new ListSetup(
                config.PageSize,
                config.SearchBarVisible,
                config.ReorderingAllowed,
                config.ListEditorType,
                config.EmptyVariantColumnVisibility,
                panes,
                buttons), cacheable); 
        }
    }
}
