using System;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
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

        public async Task<IResolvedSetup<PaneSetup>> ResolveSetupAsync(PaneConfig config, ICollectionSetup? collection = default)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            var cacheable = true;

            var buttons = (await _buttonSetupResolver.ResolveSetupAsync(config.Buttons, collection)).CheckIfCachable(ref cacheable).ToList();
            var fields = (await _fieldSetupResolver.ResolveSetupAsync(config.Fields, collection)).CheckIfCachable(ref cacheable).ToList();
            var subCollectionLists = (await _subCollectionSetupResolver.ResolveSetupAsync(config.SubCollectionLists, collection)).CheckIfCachable(ref cacheable).ToList();
            var relatedCollectionLists = (await _relatedCollectionSetupResolver.ResolveSetupAsync(config.RelatedCollectionLists, collection)).CheckIfCachable(ref cacheable).ToList();

            return new ResolvedSetup<PaneSetup>(new PaneSetup(
                config.CustomType,
                config.Label,
                config.IsVisible,
                config.VariantType,
                buttons,
                fields,
                subCollectionLists,
                relatedCollectionLists),
                cacheable);
        }
    }
}
