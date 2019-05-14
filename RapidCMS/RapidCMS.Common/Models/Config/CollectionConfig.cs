using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Helpers;

namespace RapidCMS.Common.Models.Config
{
    // TODO: validate incoming parameters

    public class RootConfig
    {
        internal List<CustomButtonRegistration> CustomButtonRegistrations { get; set; } = new List<CustomButtonRegistration>();
        internal List<CustomEditorRegistration> CustomEditorRegistrations { get; set; } = new List<CustomEditorRegistration>();
        internal List<CustomSectionRegistration> CustomSectionRegistrations { get; set; } = new List<CustomSectionRegistration>();

        public RootConfig AddCustomButton(Type buttonType)
        {
            CustomButtonRegistrations.Add(new CustomButtonRegistration(buttonType));

            return this;
        }

        public RootConfig AddCustomEditor(Type editorType)
        {
            CustomEditorRegistrations.Add(new CustomEditorRegistration(editorType));

            return this;
        }

        public RootConfig AddCustomSection(Type sectionType)
        {
            CustomSectionRegistrations.Add(new CustomSectionRegistration(sectionType));

            return this;
        }
    }

    public class CollectionConfig : ICollectionRoot
    {
        internal Type RepositoryType { get; set; }

        public List<Collection> Collections { get; set; } = new List<Collection>();
        public List<EntityVariantConfig> EntityVariants { get; set; } = new List<EntityVariantConfig>();

        public TreeViewConfig TreeView { get; set; }
        public ListViewConfig ListView { get; set; }
        public ListEditorConfig ListEditor { get; set; }
        public NodeEditorConfig NodeEditor { get; set; }
    }

    public class CollectionConfig<TEntity> : CollectionConfig
        where TEntity : IEntity
    {
        public CollectionConfig<TEntity> SetRepository<TRepository>()
           where TRepository : IRepository
        {
            RepositoryType = typeof(TRepository);

            return this;
        }

        public CollectionConfig<TEntity> AddEntityVariant<TDerivedEntity>(string name, string icon)
            where TDerivedEntity : TEntity
        {
            EntityVariants.Add(new EntityVariantConfig
            {
                Name = name,
                Icon = icon,
                Type = typeof(TDerivedEntity)
            });

            return this;
        }

        public CollectionConfig<TEntity> SetTreeView(Expression<Func<TEntity, string>> nameExpression)
        {
            return SetTreeView(default, default, nameExpression);
        }

        public CollectionConfig<TEntity> SetTreeView(EntityVisibilty entityVisibility, Expression<Func<TEntity, string>> nameExpression)
        {
            return SetTreeView(entityVisibility, default, nameExpression);
        }

        public CollectionConfig<TEntity> SetTreeView(EntityVisibilty entityVisibility, CollectionRootVisibility rootVisibility, Expression<Func<TEntity, string>> nameExpression)
        {
            TreeView = new TreeViewConfig
            {
                EntityVisibilty = entityVisibility,
                RootVisibility = rootVisibility,
                PropertyMetadata = PropertyMetadataHelper.GetExpressionMetadata(nameExpression)
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

        public CollectionConfig<TEntity> SetListEditor(ListEditorType listEditorType, Action<ListEditorConfig<TEntity>> configure)
        {
            var config = new ListEditorConfig<TEntity>();

            configure.Invoke(config);

            config.ListEditorType = listEditorType;

            ListEditor = config;

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
}
