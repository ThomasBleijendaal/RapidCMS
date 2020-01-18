using System;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Repositories;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface IRelatedCollectionListEditorConfig<TRelatedEntity, TRelatedRepository>
        where TRelatedRepository : IRepository
        where TRelatedEntity : IEntity
    {
        /// <summary>
        /// Sets the ListEditor of this related collection
        /// </summary>
        /// <param name="configure">Action used to configure the ListEditor</param>
        /// <returns></returns>
        IRelatedCollectionListEditorConfig<TRelatedEntity, TRelatedRepository> SetListEditor(Action<IListEditorConfig<TRelatedEntity>> configure);
    }
}
