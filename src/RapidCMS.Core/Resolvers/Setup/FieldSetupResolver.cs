using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class FieldSetupResolver : ISetupResolver<FieldSetup, FieldConfig>
    {
        private readonly IRepositoryTypeResolver _repositoryTypeResolver;

        public FieldSetupResolver(IRepositoryTypeResolver repositoryTypeResolver)
        {
            _repositoryTypeResolver = repositoryTypeResolver;
        }

        public Task<IResolvedSetup<FieldSetup>> ResolveSetupAsync(FieldConfig config, ICollectionSetup? collection = default)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            var setup = config switch
            {
                _ when config.EditorType == EditorType.Custom && config.Property != null => (FieldSetup)new CustomPropertyFieldSetup(config, config.CustomType!),
                _ when config.EditorType != EditorType.None && config.Property != null => (FieldSetup)new PropertyFieldSetup(config),
                _ when config.DisplayType != DisplayType.None && config.Property != null => (FieldSetup)new ExpressionFieldSetup(config, config.Property),
                _ when config.DisplayType == DisplayType.Custom && config.Expression != null => (FieldSetup)new CustomExpressionFieldSetup(config, config.Expression, config.CustomType!),
                _ when config.DisplayType != DisplayType.None && config.Expression != null => (FieldSetup)new ExpressionFieldSetup(config, config.Expression),
                _ => throw new InvalidOperationException()
            };

            if (config.Relation != null && setup is PropertyFieldSetup propertySetup)
            {
                propertySetup.Relation = config.Relation switch
                {
                    RepositoryRelationConfig collectionConfig => (RelationSetup)new RepositoryRelationSetup(
                        collectionConfig.RepositoryType == null ? null : _repositoryTypeResolver.GetAlias(collectionConfig.RepositoryType),
                        collectionConfig.CollectionAlias,
                        collectionConfig.RelatedEntityType!,
                        collectionConfig.IdProperty!,
                        collectionConfig.DisplayProperties!)
                    {
                        RepositoryParentSelector = collectionConfig.RepositoryParentProperty,
                        EntityAsParent = collectionConfig.EntityAsParent,
                        RelatedElementsGetter = collectionConfig.RelatedElementsGetter
                    },
                    DataProviderRelationConfig dataProviderConfig => (RelationSetup)new DataProviderRelationSetup(
                        dataProviderConfig.DataCollectionType),
                    _ => throw new InvalidOperationException("Invalid RelationConfig")
                };
            }

            return Task.FromResult<IResolvedSetup<FieldSetup>>(new ResolvedSetup<FieldSetup>(setup, true));
        }
    }
}
