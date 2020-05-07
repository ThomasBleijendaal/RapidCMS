using System.Linq;
using RapidCMS.Core.Abstractions.Resolvers.Setup;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class PaneSetupResolver : ISetupResolver<PaneSetup, PaneConfig>
    {
        private readonly ISetupResolver<IButtonSetup, ButtonConfig> _buttonSetupResolver;
        private readonly ISetupResolver<FieldSetup, FieldConfig> _fieldSetupResolver;
        private readonly ISetupResolver<SubCollectionListSetup, CollectionListConfig> _subCollectionSetupResolver;
        private readonly ISetupResolver<RelatedCollectionListSetup, CollectionListConfig> _relatedCollectionSetupResolver;

        public PaneSetupResolver(
            ISetupResolver<IButtonSetup, ButtonConfig> buttonSetupResolver,
            ISetupResolver<FieldSetup, FieldConfig> fieldSetupResolver,
            ISetupResolver<SubCollectionListSetup, CollectionListConfig> subCollectionSetupResolver,
            ISetupResolver<RelatedCollectionListSetup, CollectionListConfig> relatedCollectionSetupResolver)
        {
            _buttonSetupResolver = buttonSetupResolver;
            _fieldSetupResolver = fieldSetupResolver;
            _subCollectionSetupResolver = subCollectionSetupResolver;
            _relatedCollectionSetupResolver = relatedCollectionSetupResolver;
        }

        public PaneSetup ResolveSetup(PaneConfig config, ICollectionSetup collection)
        {
            var buttons = _buttonSetupResolver.ResolveSetup(config.Buttons, collection).ToList();
            var fields = _fieldSetupResolver.ResolveSetup(config.Fields, collection).ToList();
            var subCollectionLists = _subCollectionSetupResolver.ResolveSetup(config.SubCollectionLists, collection).ToList();
            var relatedCollectionLists = _relatedCollectionSetupResolver.ResolveSetup(config.RelatedCollectionLists, collection).ToList();

            return new PaneSetup(
                config.CustomType,
                config.Label,
                config.IsVisible,
                config.VariantType,
                buttons,
                fields,
                subCollectionLists,
                relatedCollectionLists);
        }
    }
}
