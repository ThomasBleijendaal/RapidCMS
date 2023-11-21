using System;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Repositories;

namespace RapidCMS.Core.Abstractions.Config;

public interface IRelatedCollectionListViewConfig<TRelatedEntity, TRelatedRepository>
    where TRelatedRepository : IRepository
    where TRelatedEntity : IEntity
{
    /// <summary>
    /// Sets the ListView of this related collection
    /// </summary>
    /// <param name="configure">Action used to configure the ListView</param>
    /// <returns></returns>
    IRelatedCollectionListViewConfig<TRelatedEntity, TRelatedRepository> SetListView(Action<IListViewConfig<TRelatedEntity>> configure);
}
