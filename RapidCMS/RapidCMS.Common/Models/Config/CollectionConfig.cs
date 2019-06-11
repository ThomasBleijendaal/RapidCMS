using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Helpers;


namespace RapidCMS.Common.Models.Config
{
    // TODO: validate incoming parameters

    public class CmsConfig : ICollectionRoot
    {
        internal string SiteName { get; set; } = "RapidCMS";
        internal bool AllowAnonymousUsage { get; set; } = false;

        public List<CollectionConfig> Collections { get; set; } = new List<CollectionConfig>();
        internal List<CustomTypeRegistration> CustomButtonRegistrations { get; set; } = new List<CustomTypeRegistration>();
        internal List<CustomTypeRegistration> CustomEditorRegistrations { get; set; } = new List<CustomTypeRegistration>();
        internal List<CustomTypeRegistration> CustomSectionRegistrations { get; set; } = new List<CustomTypeRegistration>();
        internal CustomTypeRegistration? CustomLoginRegistration { get; set; }

        public CmsConfig AddCustomButton(Type buttonType)
        {
            CustomButtonRegistrations.Add(new CustomTypeRegistration(buttonType));

            return this;
        }

        public CmsConfig AddCustomEditor(Type editorType)
        {
            CustomEditorRegistrations.Add(new CustomTypeRegistration(editorType));

            return this;
        }

        public CmsConfig AddCustomSection(Type sectionType)
        {
            CustomSectionRegistrations.Add(new CustomTypeRegistration(sectionType));

            return this;
        }

        public CmsConfig SetCustomLogin(Type loginType)
        {
            CustomLoginRegistration = new CustomTypeRegistration(loginType);

            return this;
        }

        public CmsConfig SetSiteName(string siteName)
        {
            SiteName = siteName;

            return this;
        }

        public CmsConfig AllowAnonymousUser()
        {
            AllowAnonymousUsage = true;

            return this;
        }

        public bool IsUnique(string alias)
        {
            return !Collections.Any(col => col.Alias == alias);
        }
    }

    public class CollectionConfig : ICollectionRoot
    {
        internal string Alias { get; set; }
        internal string Name { get; set; }

        internal Type RepositoryType { get; set; }

        public List<CollectionConfig> Collections { get; set; } = new List<CollectionConfig>();
        internal List<EntityVariantConfig> SubEntityVariants { get; set; } = new List<EntityVariantConfig>();
        internal EntityVariantConfig EntityVariant { get; set; }

        internal TreeViewConfig TreeView { get; set; }
        internal ListViewConfig ListView { get; set; }
        internal ListEditorConfig ListEditor { get; set; }
        internal NodeViewConfig NodeView { get; set; }
        internal NodeEditorConfig NodeEditor { get; set; }

        public bool IsUnique(string alias)
        {
            return !Collections.Any(col => col.Alias == alias);
        }
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
            SubEntityVariants.Add(new EntityVariantConfig
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
                Name = PropertyMetadataHelper.GetExpressionMetadata(nameExpression)
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

        public CollectionConfig<TEntity> SetNodeView(Action<NodeViewConfig<TEntity>> configure)
        {
            var config = new NodeViewConfig<TEntity>();

            configure.Invoke(config);

            NodeView = config;

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
