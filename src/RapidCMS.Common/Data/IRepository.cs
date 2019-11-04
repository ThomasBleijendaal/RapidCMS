using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace RapidCMS.Common.Data
{
    public interface IRepository
    {
        Task<IEntity?> InternalGetByIdAsync(string id, IParent? parent);
        Task<IEnumerable<IEntity>> InternalGetAllAsync(IParent? parent, IQuery query);

        // TODO: replace with IRelated
        Task<IEnumerable<IEntity>> InternalGetAllRelatedAsync(IEntity relatedEntity, IQuery query);
        Task<IEnumerable<IEntity>> InternalGetAllNonRelatedAsync(IEntity relatedEntity, IQuery query);

        /// <summary>
        /// Create a new entity in-memory.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="variantType"></param>
        /// <returns></returns>
        Task<IEntity> InternalNewAsync(IParent? parent, Type? variantType);
        Task<IEntity?> InternalInsertAsync(IParent? parent, IEntity entity, IRelationContainer? relations);
        Task InternalUpdateAsync(string id, IParent? parent, IEntity entity, IRelationContainer? relations);
        Task InternalDeleteAsync(string id, IParent? parent);

        Task InternalAddAsync(IEntity relatedEntity, string id);
        Task InternalRemoveAsync(IEntity relatedEntity, string id);

        IChangeToken ChangeToken { get; }
    }

    public interface IRepository<TKey, TEntity> : IRepository
        where TEntity : class, IEntity
    {
        Task<TEntity?> GetByIdAsync(TKey id, IParent? parent);
        Task<IEnumerable<TEntity>> GetAllAsync(IParent? parent, IQuery<TEntity> query);
        Task<IEnumerable<TEntity>?> GetAllRelatedAsync(IEntity relatedEntity, IQuery<TEntity> query);
        Task<IEnumerable<TEntity>?> GetAllNonRelatedAsync(IEntity relatedEntity, IQuery<TEntity> query);

        Task<TEntity> NewAsync(IParent? parent, Type? variantType);
        Task<TEntity?> InsertAsync(IParent? parent, TEntity entity, IRelationContainer? relations);
        Task UpdateAsync(TKey id, IParent? parent, TEntity entity, IRelationContainer? relations);
        Task DeleteAsync(TKey id, IParent? parent);

        Task AddAsync(IEntity relatedEntity, TKey id);
        Task RemoveAsync(IEntity relatedEntity, TKey id);

        TKey ParseKey(string id);
    }
}
