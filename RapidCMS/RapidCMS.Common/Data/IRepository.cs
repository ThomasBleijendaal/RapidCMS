using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable enable

namespace RapidCMS.Common.Data
{
    public interface IRepository
    {
#pragma warning disable IDE1006 // Naming Styles
        Task<IEntity> _GetByIdAsync(string id, string? parentId);
        Task<IEnumerable<IEntity>> _GetAllAsObjectsAsync(string? parentId);

        Task<IEntity> _NewAsync(string? parentId, Type? variantType);
        Task<IEntity> _InsertAsync(string? parentId, IEntity entity, IEnumerable<IRelation> relations);
        Task _UpdateAsync(string id, string? parentId, IEntity entity, IEnumerable<IRelation> relations);
        Task _DeleteAsync(string id, string? parentId);
#pragma warning restore IDE1006 // Naming Styles
    }

    // TODO: merge Struct and Class Repos

    public interface IStructRepository<TKey, TParentKey, TEntity> : IRepository
        where TEntity : IEntity
        where TParentKey : struct
    {
        Task<TEntity> GetByIdAsync(TKey id, TParentKey? parentId);
        Task<IEnumerable<TEntity>> GetAllAsync(TParentKey? parentId);

        Task<TEntity> NewAsync(TParentKey? parentId, Type? variantType);
        Task<TEntity> InsertAsync(TParentKey? parentId, TEntity entity, IEnumerable<IRelation> relations);
        Task UpdateAsync(TKey id, TParentKey? parentId, TEntity entity, IEnumerable<IRelation> relations);
        Task DeleteAsync(TKey id, TParentKey? parentId);

        TKey ParseKey(string id);
        TParentKey? ParseParentKey(string? parentId);
    }

    public abstract class BaseStructRepository<TKey, TParentKey, TEntity> : IRepository, IStructRepository<TKey, TParentKey, TEntity>
        where TEntity : IEntity
        where TParentKey : struct
    {
        public abstract Task<TEntity> GetByIdAsync(TKey id, TParentKey? parentId);
        public abstract Task<IEnumerable<TEntity>> GetAllAsync(TParentKey? parentId);
        public abstract Task<TEntity> NewAsync(TParentKey? parentId, Type? variantType = null);
        public abstract Task<TEntity> InsertAsync(TParentKey? parentId, TEntity entity, IEnumerable<IRelation> relations);
        public abstract Task UpdateAsync(TKey id, TParentKey? parentId, TEntity entity, IEnumerable<IRelation> relations);
        public abstract Task DeleteAsync(TKey id, TParentKey? parentId);

        public abstract TKey ParseKey(string id);
        public abstract TParentKey? ParseParentKey(string? parentId);

        async Task<IEntity> IRepository._GetByIdAsync(string id, string? parentId)
        {
            return (await GetByIdAsync(ParseKey(id), ParseParentKey(parentId))) as IEntity;
        }

        async Task<IEnumerable<IEntity>> IRepository._GetAllAsObjectsAsync(string? parentId)
        {
            return (await GetAllAsync(ParseParentKey(parentId))).Cast<IEntity>();
        }

        async Task<IEntity> IRepository._NewAsync(string? parentId, Type? variantType)
        {
            return (await NewAsync(ParseParentKey(parentId), variantType)) as IEntity;
        }

        async Task<IEntity> IRepository._InsertAsync(string? parentId, IEntity entity, IEnumerable<IRelation> relations)
        {
            return (await InsertAsync(ParseParentKey(parentId), (TEntity)entity, relations)) as IEntity;
        }

        async Task IRepository._UpdateAsync(string id, string? parentId, IEntity entity, IEnumerable<IRelation> relations)
        {
            await UpdateAsync(ParseKey(id), ParseParentKey(parentId), (TEntity)entity, relations);
        }

        async Task IRepository._DeleteAsync(string id, string? parentId)
        {
            await DeleteAsync(ParseKey(id), ParseParentKey(parentId));
        }
    }

    public interface IClassRepository<TKey, TParentKey, TEntity> : IRepository
        where TEntity : IEntity
        where TParentKey : class
    {
        Task<TEntity> GetByIdAsync(TKey id, TParentKey? parentId);
        Task<IEnumerable<TEntity>> GetAllAsync(TParentKey? parentId);

        Task<TEntity> NewAsync(TParentKey? parentId, Type? variantType);
        Task<TEntity> InsertAsync(TParentKey? parentId, TEntity entity, IEnumerable<IRelation> relations);
        Task UpdateAsync(TKey id, TParentKey? parentId, TEntity entity, IEnumerable<IRelation> relations);
        Task DeleteAsync(TKey id, TParentKey? parentId);

        TKey ParseKey(string id);
        TParentKey? ParseParentKey(string? parentId);
    }

    public abstract class BaseClassRepository<TKey, TParentKey, TEntity> : IRepository, IClassRepository<TKey, TParentKey, TEntity>
        where TEntity : IEntity
        where TParentKey : class
    {
        public abstract Task<TEntity> GetByIdAsync(TKey id, TParentKey? parentId);
        public abstract Task<IEnumerable<TEntity>> GetAllAsync(TParentKey? parentId);
        public abstract Task<TEntity> NewAsync(TParentKey? parentId, Type? variantType = null);
        public abstract Task<TEntity> InsertAsync(TParentKey? parentId, TEntity entity, IEnumerable<IRelation> relations);
        public abstract Task UpdateAsync(TKey id, TParentKey? parentId, TEntity entity, IEnumerable<IRelation> relations);
        public abstract Task DeleteAsync(TKey id, TParentKey? parentId);

        public abstract TKey ParseKey(string id);
        public abstract TParentKey? ParseParentKey(string? parentId);

        async Task<IEntity> IRepository._GetByIdAsync(string id, string? parentId)
        {
            return (await GetByIdAsync(ParseKey(id), ParseParentKey(parentId))) as IEntity;
        }

        async Task<IEnumerable<IEntity>> IRepository._GetAllAsObjectsAsync(string? parentId)
        {
            return (await GetAllAsync(ParseParentKey(parentId))).Cast<IEntity>();
        }

        async Task<IEntity> IRepository._NewAsync(string? parentId, Type? variantType)
        {
            return (await NewAsync(ParseParentKey(parentId), variantType)) as IEntity;
        }

        async Task<IEntity> IRepository._InsertAsync(string? parentId, IEntity entity, IEnumerable<IRelation> relations)
        {
            return (await InsertAsync(ParseParentKey(parentId), (TEntity)entity, relations)) as IEntity;
        }

        async Task IRepository._UpdateAsync(string id, string? parentId, IEntity entity, IEnumerable<IRelation> relations)
        {
            await UpdateAsync(ParseKey(id), ParseParentKey(parentId), (TEntity)entity, relations);
        }

        async Task IRepository._DeleteAsync(string id, string? parentId)
        {
            await DeleteAsync(ParseKey(id), ParseParentKey(parentId));
        }
    }
}
