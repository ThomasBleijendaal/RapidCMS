using System;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Repositories;

namespace RapidCMS.Core.Models.Config
{
    internal class SubCollectionListConfig<TSubEntity, TSubRepository> : CollectionListConfig, 
            ISubCollectionListEditorConfig<TSubEntity, TSubRepository>,
            ISubCollectionListViewConfig<TSubEntity, TSubRepository>
        where TSubEntity : IEntity
        where TSubRepository : IRepository
    {
        internal SubCollectionListConfig(string collectionAlias) : base(collectionAlias)
        {
        }

        public ISubCollectionListEditorConfig<TSubEntity, TSubRepository> SetListEditor(Action<IListEditorConfig<TSubEntity>> configure)
        {
            RepositoryType = typeof(TSubRepository);
            EntityType = typeof(TSubEntity);

            var config = new ListEditorConfig<TSubEntity>();

            configure.Invoke(config);

            ListEditor = config;

            return this;
        }

        public ISubCollectionListViewConfig<TSubEntity, TSubRepository> SetListView(Action<IListViewConfig<TSubEntity>> configure)
        {
            RepositoryType = typeof(TSubRepository);
            EntityType = typeof(TSubEntity);

            var config = new ListViewConfig<TSubEntity>();

            configure.Invoke(config);

            ListView = config;

            return this;
        }
    }
}
