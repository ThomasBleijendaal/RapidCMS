using System;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class ListSetupResolver : ISetupResolver<IListSetup, ListConfig>
    {
        private readonly ISetupResolver<IPaneSetup, PaneConfig> _paneSetupResolver;
        private readonly ISetupResolver<IButtonSetup, ButtonConfig> _buttonSetupResolver;
        private readonly IConventionBasedResolver<ListConfig> _conventionListConfigResolver;

        public ListSetupResolver(
            ISetupResolver<IPaneSetup, PaneConfig> paneSetupResolver,
            ISetupResolver<IButtonSetup, ButtonConfig> buttonSetupResolver,
            IConventionBasedResolver<ListConfig> conventionListConfigResolver)
        {
            _paneSetupResolver = paneSetupResolver;
            _buttonSetupResolver = buttonSetupResolver;
            _conventionListConfigResolver = conventionListConfigResolver;
        }

        public async Task<IResolvedSetup<IListSetup>> ResolveSetupAsync(ListConfig config, ICollectionSetup? collection = default)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (config is IIsConventionBased isConventionBasedConfig)
            {
                config = await _conventionListConfigResolver.ResolveByConventionAsync(config.BaseType, isConventionBasedConfig.GetFeatures(), collection);
            }

            var cacheable = true;

            var panes = (await _paneSetupResolver.ResolveSetupAsync(config.Panes, collection)).CheckIfCachable(ref cacheable).ToList();
            var buttons = (await _buttonSetupResolver.ResolveSetupAsync(config.Buttons, collection)).CheckIfCachable(ref cacheable).ToList();

            return new ResolvedSetup<IListSetup>(new ListSetup(
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
