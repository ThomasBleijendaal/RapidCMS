using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using RapidCMS.Common.Forms;

namespace RapidCMS.Common.Data
{
    internal interface IRepository
    {
        Task<IEntity?> GetByIdAsync(string id, IParent? parent);
        Task<IEnumerable<IEntity>> GetAllAsync(IParent? parent, IQuery query);

        // TODO: replace with IRelated
        Task<IEnumerable<IEntity>> GetAllRelatedAsync(IEntity relatedEntity, IQuery query);
        Task<IEnumerable<IEntity>> GetAllNonRelatedAsync(IEntity relatedEntity, IQuery query);

        /// <summary>
        /// Create a new entity in-memory.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="variantType"></param>
        /// <returns></returns>
        Task<IEntity> NewAsync(IParent? parent, Type? variantType);
        Task<IEntity?> InsertAsync(EditContext editContext);
        Task UpdateAsync(EditContext editContext);
        Task DeleteAsync(string id, IParent? parent);

        Task AddAsync(IEntity relatedEntity, string id);
        Task RemoveAsync(IEntity relatedEntity, string id);

        IChangeToken ChangeToken { get; }
    }

    internal interface IRepository<TKey, TEntity> : IRepository
        where TEntity : class, IEntity
    {
        Task<TEntity?> GetByIdAsync(TKey id, IParent? parent);
        Task<IEnumerable<TEntity>> GetAllAsync(IParent? parent, IQuery<TEntity> query);
        Task<IEnumerable<TEntity>?> GetAllRelatedAsync(IEntity relatedEntity, IQuery<TEntity> query);
        Task<IEnumerable<TEntity>?> GetAllNonRelatedAsync(IEntity relatedEntity, IQuery<TEntity> query);

        new Task<TEntity> NewAsync(IParent? parent, Type? variantType);
        Task<TEntity?> InsertAsync(IParent? parent, TEntity entity, IRelationContainer? relations);
        Task<TEntity?> InsertAsync(IEditContext<TEntity> editContext);
        Task UpdateAsync(TKey id, IParent? parent, TEntity entity, IRelationContainer? relations);
        Task UpdateAsync(IEditContext<TEntity> editContext);
        Task DeleteAsync(TKey id, IParent? parent);

        Task AddAsync(IEntity relatedEntity, TKey id);
        Task RemoveAsync(IEntity relatedEntity, TKey id);

        TKey ParseKey(string id);
    }
}
