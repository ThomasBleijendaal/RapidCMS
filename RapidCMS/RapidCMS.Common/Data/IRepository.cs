using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RapidCMS.Common.Interfaces;

namespace RapidCMS.Common.Data
{
#nullable enable

    public interface IRepository
    {
#pragma warning disable IDE1006 // Naming Styles
        Task<IEntity> _GetByIdAsync(string id, string? parentId);
        Task<IEnumerable<IEntity>> _GetAllAsObjectsAsync(string? parentId);

        Task<IEntity> _NewAsync(string? parentId, Type? variantType);
        Task<IEntity> _InsertAsync(string? parentId, IEntity entity);
        Task _UpdateAsync(string id, string? parentId, IEntity entity);
        Task _DeleteAsync(string id, string? parentId);
#pragma warning restore IDE1006 // Naming Styles
    }

    public interface IRepository<TKey, TParentKey, TEntity> : IRepository
        where TEntity : IEntity
        where TParentKey : struct
    {
        Task<TEntity> GetByIdAsync(TKey id, TParentKey? parentId);
        Task<IEnumerable<TEntity>> GetAllAsync(TParentKey? parentId);

        Task<TEntity> NewAsync(TParentKey? parentId, Type? variantType);
        Task<TEntity> InsertAsync(TParentKey? parentId, TEntity entity);
        Task UpdateAsync(TKey id, TParentKey? parentId, TEntity entity);
        Task DeleteAsync(TKey id, TParentKey? parentId);

        TKey ParseKey(string id);
        TParentKey? ParseParentKey(string? parentId);
    }

    public abstract class BaseRepository<TKey, TParentKey, TEntity> : IRepository, IRepository<TKey, TParentKey, TEntity>
        where TEntity : IEntity
        where TParentKey : struct
    {
        public abstract Task<TEntity> GetByIdAsync(TKey id, TParentKey? parentId);
        public abstract Task<IEnumerable<TEntity>> GetAllAsync(TParentKey? parentId);
        public abstract Task<TEntity> NewAsync(TParentKey? parentId, Type? variantType = null);
        public abstract Task<TEntity> InsertAsync(TParentKey? parentId, TEntity entity);
        public abstract Task UpdateAsync(TKey id, TParentKey? parentId, TEntity entity);
        public abstract Task DeleteAsync(TKey id, TParentKey? parentId);

        public abstract TKey ParseKey(string id);
        public abstract TParentKey? ParseParentKey(string? parentId);

        async Task<IEntity> IRepository._GetByIdAsync(string id, string parentId)
        {
            return (await GetByIdAsync(ParseKey(id), ParseParentKey(parentId))) as IEntity;
        }

        async Task<IEnumerable<IEntity>> IRepository._GetAllAsObjectsAsync(string parentId)
        {
            return (await GetAllAsync(ParseParentKey(parentId))).Cast<IEntity>();
        }

        async Task<IEntity> IRepository._NewAsync(string parentId, Type? variantType)
        {
            return (await NewAsync(ParseParentKey(parentId), variantType)) as IEntity;
        }

        async Task<IEntity> IRepository._InsertAsync(string parentId, IEntity entity)
        {
            return (await InsertAsync(ParseParentKey(parentId), (TEntity)entity)) as IEntity;
        }

        async Task IRepository._UpdateAsync(string id, string parentId, IEntity entity)
        {
            await UpdateAsync(ParseKey(id), ParseParentKey(parentId), (TEntity)entity);
        }

        async Task IRepository._DeleteAsync(string id, string parentId)
        {
            await DeleteAsync(ParseKey(id), ParseParentKey(parentId));
        }
    }
}
