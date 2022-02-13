using System;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Config
{
    internal class DetailPageConfig<TDetailEntity> : CollectionConfig<TDetailEntity>, ICollectionDetailPageEditorConfig<TDetailEntity>
        where TDetailEntity : IEntity
    {
        private readonly NodeEditorConfig<TDetailEntity> _detailNodeEditor;

        internal DetailPageConfig(string alias, string? parentAlias, string? icon, string? color, string name, Type repositoryType, EntityVariantConfig entityVariant)
            : base(alias, parentAlias, icon, color, name, repositoryType, entityVariant)
        {
            _detailNodeEditor = new NodeEditorConfig<TDetailEntity>();

            TreeView = new TreeViewConfig
            {
                EntityVisibilty = EntityVisibilty.Hidden,
                RootVisibility = CollectionRootVisibility.Visible,
            };

            NodeEditor = _detailNodeEditor;
        }

        public INodeEditorConfig<TDetailEntity> AddCustomButton<TActionHandler>(Type buttonType, string? label = null, string? icon = null, Func<IEntity, EntityState, bool>? isVisible = null)
            where TActionHandler : IButtonSetupActionHandler
            => _detailNodeEditor.AddCustomButton<TActionHandler>(buttonType, label, icon, isVisible);


        public INodeEditorConfig<TDetailEntity> AddDefaultButton(DefaultButtonType type, string? label = null, string? icon = null, bool isPrimary = false, Func<IEntity, EntityState, bool>? isVisible = null)
            => _detailNodeEditor.AddDefaultButton(type, label, icon, isPrimary, isVisible);

        public INodeEditorConfig<TDetailEntity> AddPaneButton(Type paneType, string? label = null, string? icon = null, Func<IEntity, EntityState, bool>? isVisible = null)
            => _detailNodeEditor.AddPaneButton(paneType, label, icon, isVisible);

        public INodeEditorConfig<TDetailEntity> AddNavigationButton<TNavigationHandler>(string? label = null, string? icon = null, Func<IEntity, EntityState, bool>? isVisible = null)
            where TNavigationHandler : INavigationHandler
            => _detailNodeEditor.AddNavigationButton<TNavigationHandler>(label, icon, isVisible);

        public INodeEditorConfig<TDetailEntity> AddSection(Action<IEditorPaneConfig<TDetailEntity>> configure)
            => _detailNodeEditor.AddSection(configure);

        public INodeEditorConfig<TDetailEntity> AddSection(Type customSectionType, Action<IEditorPaneConfig<TDetailEntity>>? configure = null)
            => _detailNodeEditor.AddSection(customSectionType, configure);

        public INodeEditorConfig<TDetailEntity> AddSection<TDerivedEntity>(Action<IEditorPaneConfig<TDerivedEntity>> configure) where TDerivedEntity : TDetailEntity
            => _detailNodeEditor.AddSection(configure);

        public INodeEditorConfig<TDetailEntity> AddSection<TDerivedEntity>(Type? customSectionType, Action<IEditorPaneConfig<TDerivedEntity>>? configure) where TDerivedEntity : TDetailEntity
            => _detailNodeEditor.AddSection<TDerivedEntity>(customSectionType, configure);
    }
}
