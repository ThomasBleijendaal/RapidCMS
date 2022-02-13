using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Convention
{
    internal class ConventionBasedNodeConfigResolver : IConventionBasedResolver<NodeConfig>
    {
        private readonly IFieldConfigResolver _fieldConfigResolver;

        public ConventionBasedNodeConfigResolver(IFieldConfigResolver fieldConfigResolver)
        {
            _fieldConfigResolver = fieldConfigResolver;
        }

        public Task<NodeConfig> ResolveByConventionAsync(Type subject, Features features, CollectionSetup? collection)
        {
            var result = new NodeConfig(subject);

            if (features.HasFlag(Features.CanView) || features.HasFlag(Features.CanEdit))
            {
                result.Buttons.Add(new DefaultButtonConfig
                {
                    ButtonType = DefaultButtonType.Up
                });

                result.Panes = new List<PaneConfig>
                {
                    new PaneConfig(subject)
                    {
                        FieldIndex = 1,
                        Fields = _fieldConfigResolver.GetFields(subject, features).ToList(),
                        VariantType = subject
                    }
                };
            }

            if (features.HasFlag(Features.CanEdit))
            {
                result.Buttons.AddRange(new[] {
                    new DefaultButtonConfig
                    {
                        ButtonType = DefaultButtonType.SaveExisting
                    },
                    new DefaultButtonConfig
                    {
                        ButtonType = DefaultButtonType.SaveNew
                    },
                    new DefaultButtonConfig
                    {
                        ButtonType = DefaultButtonType.Delete
                    }
                });
            }

            if (collection?.Collections.Any() ?? false)
            {
                foreach (var subCollection in collection.Collections)
                {
                    result.Panes.Add(new PaneConfig(subject)
                    {
                        IsVisible = (entity, state) => state == EntityState.IsExisting,
                        SubCollectionLists = new List<CollectionListConfig>
                        {
                            new ReferencedCollectionListConfig(subCollection.Alias)
                        }
                    });
                }
            }

            return Task.FromResult(result);
        }
    }
}
