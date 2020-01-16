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
        /// <param name="configure">Action used to configure the ListEditor</param>
        /// <returns></returns>
        ISubCollectionListViewConfig<TSubEntity, TSubRepository> SetListEditor(Action<IListViewConfig<TSubEntity>> configure);
    }

    public interface ISubCollectionListEditorConfig<TSubEntity, TSubRepository>
        where TSubRepository : IRepository
        where TSubEntity : IEntity
    {
        /// <summary>
        /// Sets the ListEditor of this related collection
        /// </summary>
        /// <param name="configure">Action used to configure the ListEditor</param>
        /// <returns></returns>
        ISubCollectionListEditorConfig<TSubEntity, TSubRepository> SetListEditor(Action<IListEditorConfig<TSubEntity>> configure);
    }
}
