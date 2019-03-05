using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Interfaces;

namespace RapidCMS.Common.Models.Config
{
    public class NodeEditorPaneConfig
    {
        public Type VariantType { get; set; }
        public List<FieldConfig> Fields { get; set; } = new List<FieldConfig>();
        public List<SubCollectionListEditorConfig> SubCollectionListEditors { get; set; } = new List<SubCollectionListEditorConfig>();
    }

    public class NodeEditorPaneConfig<TEntity> : NodeEditorPaneConfig
        where TEntity : IEntity
    {
        public FieldConfig<TEntity> AddField<TValue>(Expression<Func<TEntity, TValue>> propertyExpression, Action<FieldConfig<TEntity>> configure = null)
        {
            var config = new FieldConfig<TEntity>()
            {
                NodeProperty = PropertyMetadataHelper.Create(propertyExpression)
            };
            config.Name = config.NodeProperty.PropertyName;

            // try to find the default editor for this type
            foreach (var type in EnumHelper.GetValues<EditorType>())
            {
                if (type.GetCustomAttribute<DefaultTypeAttribute>()?.Types.Contains(config.NodeProperty.PropertyType) ?? false)
                {
                    config.Type = type;

                    break;
                }
            }

            configure?.Invoke(config);

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

            SubCollectionListEditors.Add(config);

            return this;
        }
    }
}
