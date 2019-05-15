using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Helpers;

#nullable enable

namespace RapidCMS.Common.Models.Config
{
    public class NodeEditorPaneConfig
    {
        internal string? CustomAlias { get; set; }
        internal string? Label { get; set; }

        internal Type VariantType { get; set; }

        internal int FieldIndex { get; set; }

        internal List<FieldConfig> Fields { get; set; } = new List<FieldConfig>();
        internal List<SubCollectionListEditorConfig> SubCollectionListEditors { get; set; } = new List<SubCollectionListEditorConfig>();
    }

    public class NodeEditorPaneConfig<TEntity> : NodeEditorPaneConfig
        where TEntity : IEntity
    {
        public NodeEditorPaneConfig()
        {
        }

        public NodeEditorPaneConfig(Type customSectionType)
        {
            CustomAlias = customSectionType.FullName;
        }

        public NodeEditorPaneConfig<TEntity> SetLabel(string label)
        {
            Label = label;

            return this;
        }

        public FieldConfig<TEntity> AddField<TValue>(Expression<Func<TEntity, TValue>> propertyExpression, Action<FieldConfig<TEntity>> configure = null)
        {
            var config = new FieldConfig<TEntity>()
            {
                // TODO: update
                NodeProperty = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression),
            };
            config.Name = config.NodeProperty.PropertyName;
            config.Type = EditorTypeHelper.TryFindDefaultEditorType(config.NodeProperty.PropertyType);

            configure?.Invoke(config);

            config.Index = FieldIndex++;

            Fields.Add(config);

            return config;
        }

        
        // TODO: check if sub collection is part of collection
        public NodeEditorPaneConfig<TEntity> AddSubCollectionListEditor(string collectionAlias, Action<SubCollectionListEditorConfig<TEntity>> configure = null)
        {
            var config = new SubCollectionListEditorConfig<TEntity>
            {
                CollectionAlias = collectionAlias
            };

            configure?.Invoke(config);

            config.Index = FieldIndex++;

            SubCollectionListEditors.Add(config);

            return this;
        }
    }
}
