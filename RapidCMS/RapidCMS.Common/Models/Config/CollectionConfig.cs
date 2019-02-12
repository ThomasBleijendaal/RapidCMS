using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Interfaces;

namespace RapidCMS.Common.Models.Config
{
    // TODO: validate incoming parameters
    // TODO: work out what polymorphistic optimizations can be used
    // TODO: this will contain a lot of logic (move to extensions?)

    public class CollectionConfig<TEntity> : ICollectionRoot
        where TEntity : IEntity
    {
        internal Type RepositoryType { get; set; }   

        public List<Collection> Collections { get; set; } = new List<Collection>();

        public CollectionConfig<TEntity> SetRepository<TRepository>()
           where TRepository : IRepository<int, TEntity>
        {
            RepositoryType = typeof(TRepository);

            return this;
        }

        public TreeViewConfig<TEntity> TreeView { get; set; }

        public ListViewConfig<TEntity> ListView { get; set; }
        
        public NodeEditorConfig<TEntity> NodeEditor { get; set; }

        public CollectionConfig<TEntity> SetTreeView(string name, ViewType viewType, Expression<Func<TEntity, string>> nameExpression)
        {
            TreeView = new TreeViewConfig<TEntity>
            {
                Name = name,
                ViewType = viewType,
                NameGetter = nameExpression
            };

            return this;
        }

        public CollectionConfig<TEntity> SetListView(Action<ListViewConfig<TEntity>> configure)
        {
            var config = new ListViewConfig<TEntity>();

            configure.Invoke(config);

            ListView = config;

            return this;
        }

        public CollectionConfig<TEntity> SetNodeEditor(Action<NodeEditorConfig<TEntity>> configure)
        {
            var config = new NodeEditorConfig<TEntity>();

            configure.Invoke(config);

            NodeEditor = config;

            return this;
        }
    }

    public class TreeViewConfig<TEntity>
        where TEntity : IEntity
    {
        public string Name { get; set; }
        public ViewType ViewType { get; set; }
        public Expression<Func<TEntity, string>> NameGetter { get; set; }
    }

    public class ListViewConfig<TEntity>
        where TEntity : IEntity
    {
        public List<ButtonConfig> Buttons { get; set; } = new List<ButtonConfig>();
        public List<ListViewPaneConfig<TEntity>> ListViewPanes { get; set; } = new List<ListViewPaneConfig<TEntity>>();

        public ListViewConfig<TEntity> AddDefaultButton(DefaultButtonType type, string label = null, string icon = null)
        {
            var button = new DefaultButtonConfig
            {
                ButtonType = type,
                Icon = icon ?? type.GetCustomAttribute<DefaultIconLabelAttribute>().Icon,
                Label = label ?? type.GetCustomAttribute<DefaultIconLabelAttribute>().Label
            };

            Buttons.Add(button);

            return this;
        }

        public ListViewConfig<TEntity> AddListPane(Action<ListViewPaneConfig<TEntity>> configure)
        {
            var config = new ListViewPaneConfig<TEntity>();

            configure.Invoke(config);

            ListViewPanes.Add(config);

            return this;
        }
    }

    public class ListViewPaneConfig<TEntity>
        where TEntity : IEntity
    {
        public List<PropertyConfig<TEntity>> Properties { get; set; } = new List<PropertyConfig<TEntity>>();
        
        public PropertyConfig<TEntity> AddProperty<TValue>(Expression<Func<TEntity, TValue>> propertyExpression, Action<PropertyConfig<TEntity>> configure = null)
        {
            var config = new PropertyConfig<TEntity>
            {
                NodeProperty = PropertyMetadataHelper.Create(propertyExpression)
            };
            config.Name = config.NodeProperty.PropertyName;

            configure?.Invoke(config);

            Properties.Add(config);

            return config;
        }
    }

    public class PropertyConfig<TEntity>
        where TEntity : IEntity
    {
        internal string Name { get; set; }
        internal string Description { get; set; }

        internal PropertyMetadata NodeProperty { get; set; }

        internal IValueMapper ValueMapper { get; set; }
        internal Type ValueMapperType { get; set; }

        public PropertyConfig<TEntity> SetName(string name)
        {
            Name = name;
            return this;
        }
        public PropertyConfig<TEntity> SetDescription(string description)
        {
            Description = description;
            return this;
        }

        // TODO: check for mapper compatibility with value type in NodeProperty
        public PropertyConfig<TEntity> SetValueMapper<TValue>(ValueMapper<TValue> valueMapper)
        {
            ValueMapper = valueMapper;

            return this;
        }

        // TODO: check for mapper compatibility with value type in NodeProperty
        public PropertyConfig<TEntity> SetValueMapper<TValueMapper>()
            where TValueMapper : IValueMapper
        {
            ValueMapperType = typeof(IValueMapper);

            return this;
        }
    }

    public class NodeEditorConfig<TEntity>
        where TEntity : IEntity
    {
        public List<ButtonConfig> Buttons { get; set; } = new List<ButtonConfig>();
        public List<EditorPaneConfig<TEntity>> EditorPanes { get; set; } = new List<EditorPaneConfig<TEntity>>();

        public NodeEditorConfig<TEntity> AddDefaultButton(DefaultButtonType type, string label = null, string icon = null)
        {
            var button = new DefaultButtonConfig
            {
                ButtonType = type,
                Icon = icon ?? type.GetCustomAttribute<DefaultIconLabelAttribute>().Icon,
                Label = label ?? type.GetCustomAttribute<DefaultIconLabelAttribute>().Label
            };

            Buttons.Add(button);

            return this;
        }

        public NodeEditorConfig<TEntity> AddEditorPane(Action<EditorPaneConfig<TEntity>> configure)
        {
            var config = new EditorPaneConfig<TEntity>();

            configure.Invoke(config);

            EditorPanes.Add(config);

            return this;
        }
    }

    public class EditorPaneConfig<TEntity>
        where TEntity : IEntity
    {
        public List<FieldConfig<TEntity>> Fields { get; set; } = new List<FieldConfig<TEntity>>();
        
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
    }

    public class FieldConfig<TEntity>
        where TEntity : IEntity
    {
        internal string Name { get; set; }
        internal string Description { get; set; }

        internal PropertyMetadata NodeProperty { get; set; }
        internal IValueMapper ValueMapper { get; set; }
        internal Type ValueMapperType { get; set; }
        internal EditorType Type { get; set; }

        public FieldConfig<TEntity> SetName(string name)
        {
            Name = name;
            return this;
        }
        public FieldConfig<TEntity> SetDescription(string description)
        {
            Description = description;
            return this;
        }
        public FieldConfig<TEntity> SetType(EditorType type)
        {
            Type = type;
            return this;
        }

        // TODO: check for mapper compatibility with value type in NodeProperty
        public FieldConfig<TEntity> SetValueMapper<TValue>(ValueMapper<TValue> valueMapper)
        {
            ValueMapper = valueMapper;

            return this;
        }

        // TODO: check for mapper compatibility with value type in NodeProperty
        public FieldConfig<TEntity> SetValueMapper<TValueMapper>()
            where TValueMapper : IValueMapper
        {
            ValueMapperType = typeof(IValueMapper);

            return this;
        }
    }

    public class ButtonConfig
    {
        internal string Label { get; set; }
        internal string Icon { get; set; }
    }

    public class DefaultButtonConfig : ButtonConfig
    { 
        internal DefaultButtonType ButtonType { get; set; }
    }
}
