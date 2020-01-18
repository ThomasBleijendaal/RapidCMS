using System;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Repositories;

namespace RapidCMS.Core.Models.Config
{
    internal class RelatedCollectionListConfig<TRelatedEntity, TRelatedRepository> : CollectionListConfig, 
            IRelatedCollectionListViewConfig<TRelatedEntity, TRelatedRepository>,
            IRelatedCollectionListEditorConfig<TRelatedEntity, TRelatedRepository>

        where TRelatedEntity : IEntity
        where TRelatedRepository : IRepository
    {
        internal RelatedCollectionListConfig(string collectionAlias) : base(collectionAlias)
        {
        }

        public IRelatedCollectionListEditorConfig<TRelatedEntity, TRelatedRepository> SetListEditor(Action<IListEditorConfig<TRelatedEntity>> configure)
        {
            RepositoryType = typeof(TRelatedRepository);
            EntityType = typeof(TRelatedEntity);

            var config = new ListEditorConfig<TRelatedEntity>();

            configure.Invoke(config);

            ListEditor = config;

            return this;
        }

        public IRelatedCollectionListViewConfig<TRelatedEntity, TRelatedRepository> SetListView(Action<IListViewConfig<TRelatedEntity>> configure)
        {
            RepositoryType = typeof(TRelatedRepository);
            EntityType = typeof(TRelatedEntity);

            var config = new ListViewConfig<TRelatedEntity>();

            configure.Invoke(config);

            ListView = config;

            return this;
        }
    }
}
