using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers.Setup;
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

        public ListSetupResolver(
            ISetupResolver<PaneSetup, PaneConfig> paneSetupResolver,
            ISetupResolver<IButtonSetup, ButtonConfig> buttonSetupResolver)
        {
            _paneSetupResolver = paneSetupResolver;
            _buttonSetupResolver = buttonSetupResolver;
        }

        public ListSetup ResolveSetup(ListConfig config, ICollectionSetup collection)
        {
            if (config is IIsConventionBased isConventionBasedConfig)
            {

            }

            var panes = _paneSetupResolver.ResolveSetup(config.Panes, collection).ToList();
            var buttons = _buttonSetupResolver.ResolveSetup(config.Buttons, collection).ToList();

            return new ListSetup(
                config.PageSize,
                config.SearchBarVisible,
                config.ReorderingAllowed,
                config.ListEditorType,
                config.EmptyVariantColumnVisibility,
                panes,
                buttons); 
        }
    }
}
