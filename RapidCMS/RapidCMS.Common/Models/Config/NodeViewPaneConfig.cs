using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Common.Data;
using RapidCMS.Common.Exceptions;
using RapidCMS.Common.Helpers;


namespace RapidCMS.Common.Models.Config
{
    // TODO: add button support
    // TODO: merge with NodeEditorPaneConfig
    public abstract class NodeViewPaneConfig
    {
        protected NodeViewPaneConfig(Type variantType)
        {
            VariantType = variantType ?? throw new ArgumentNullException(nameof(variantType));
        }

        internal string? CustomAlias { get; set; }
        internal string? Label { get; set; }

        internal Type VariantType { get; set; }

        internal int FieldIndex { get; set; } = 0;

        internal List<PropertyConfig> Properties { get; set; } = new List<PropertyConfig>();
        internal List<SubCollectionListConfig> SubCollectionLists { get; set; } = new List<SubCollectionListConfig>();
    }

    public class NodeViewPaneConfig<TEntity> : NodeViewPaneConfig
        where TEntity : IEntity
    {
        public NodeViewPaneConfig(Type variantType) : base(variantType)
        {
        }

        public NodeViewPaneConfig(Type variantType, Type customSectionType) : base(variantType)
        {
            CustomAlias = customSectionType.FullName;
        }

        public NodeViewPaneConfig<TEntity> SetLabel(string label)
        {
            Label = label;

            return this;
        }

        public PropertyConfig<TEntity> AddProperty(Expression<Func<TEntity, string>> propertyExpression, Action<PropertyConfig<TEntity>>? configure = null)
        {
            var config = new PropertyConfig<TEntity>()
            {
                Property = PropertyMetadataHelper.GetExpressionMetadata(propertyExpression) ?? throw new InvalidExpressionException(nameof(propertyExpression)),
            };
            config.Name = config.Property.PropertyName;

            configure?.Invoke(config);

            config.Index = FieldIndex++;

            Properties.Add(config);

            return config;
        }


        // TODO: check if sub collection is part of collection
        public NodeViewPaneConfig<TEntity> AddSubCollectionListView<TSubEntity>(string collectionAlias, Action<SubCollectionListConfig<TSubEntity>>? configure = null)
            where TSubEntity : IEntity
        {
            var config = new SubCollectionListConfig<TSubEntity>
            {
                CollectionAlias = collectionAlias
            };

            configure?.Invoke(config);

            config.Index = FieldIndex++;

            SubCollectionLists.Add(config);

            return this;
        }
    }
}
