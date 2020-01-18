using System;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Repositories;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface ISubCollectionListViewConfig<TSubEntity, TSubRepository>
        where TSubRepository : IRepository
        where TSubEntity : IEntity
    {
        /// <summary>
        /// Sets the ListView of this related collection
        /// </summary>
        /// <param name="configure">Action used to configure the ListView</param>
        /// <returns></returns>
        ISubCollectionListViewConfig<TSubEntity, TSubRepository> SetListView(Action<IListViewConfig<TSubEntity>> configure);
    }
}
