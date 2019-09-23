using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace RapidCMS.Common.Data
{
    public interface IRepository
    {
        Task<IEntity> InternalGetByIdAsync(string id, string? parentId);
        Task<IEnumerable<IEntity>> InternalGetAllAsync(string? parentId, IQuery query);
        Task<IEnumerable<IEntity>> InternalGetAllRelatedAsync(IEntity relatedEntity, IQuery query);
        Task<IEnumerable<IEntity>> InternalGetAllNonRelatedAsync(IEntity relatedEntity, IQuery query);

        /// <summary>
        /// Create a new entity in-memory.
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="variantType"></param>
        /// <returns></returns>
        Task<IEntity> InternalNewAsync(string? parentId, Type? variantType);
        Task<IEntity> InternalInsertAsync(string? parentId, IEntity entity, IRelationContainer? relations);
        Task InternalUpdateAsync(string id, string? parentId, IEntity entity, IRelationContainer? relations);
        Task InternalDeleteAsync(string id, string? parentId);

        Task InternalAddAsync(IEntity relatedEntity, string id);
        Task InternalRemoveAsync(IEntity relatedEntity, string id);

        IChangeToken ChangeToken { get; }
    }

    // TODO: merge Struct and Class Repos (do it via IEntity instead of TParentKey)

    public interface IStructRepository<TKey, TParentKey, TEntity> : IRepository
        where TEntity : IEntity
        where TParentKey : struct
    {
        Task<TEntity> GetByIdAsync(TKey id, TParentKey? parentId);
        Task<IEnumerable<TEntity>> GetAllAsync(TParentKey? parentId, IQuery<TEntity> query);
        Task<IEnumerable<TEntity>?> GetAllRelatedAsync(IEntity relatedEntity, IQuery<TEntity> query);
        Task<IEnumerable<TEntity>?> GetAllNonRelatedAsync(IEntity relatedEntity, IQuery<TEntity> query);

        Task<TEntity> NewAsync(TParentKey? parentId, Type? variantType);
        Task<TEntity> InsertAsync(TParentKey? parentId, TEntity entity, IRelationContainer? relations);
        Task UpdateAsync(TKey id, TParentKey? parentId, TEntity entity, IRelationContainer? relations);
        Task DeleteAsync(TKey id, TParentKey? parentId);

        Task AddAsync(IEntity relatedEntity, TKey id);
        Task RemoveAsync(IEntity relatedEntity, TKey id);

        TKey ParseKey(string id);
        TParentKey? ParseParentKey(string? parentId);
    }

    public abstract class BaseStructRepository<TKey, TParentKey, TEntity> : IRepository, IStructRepository<TKey, TParentKey, TEntity>
        where TEntity : IEntity
        where TParentKey : struct
    {
        private SemaphoreSlim Semaphore { get; set; }

        public BaseStructRepository(SemaphoreSlim semaphore)
        {
            Semaphore = semaphore;
        }

        public abstract Task<TEntity> GetByIdAsync(TKey id, TParentKey? parentId);
        public abstract Task<IEnumerable<TEntity>> GetAllAsync(TParentKey? parentId, IQuery<TEntity> query);
        public virtual Task<IEnumerable<TEntity>?> GetAllRelatedAsync(IEntity relatedEntity, IQuery<TEntity> query)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(GetAllRelatedAsync)} on the {GetType()}.");
        public virtual Task<IEnumerable<TEntity>?> GetAllNonRelatedAsync(IEntity relatedEntity, IQuery<TEntity> query)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(GetAllNonRelatedAsync)} on the {GetType()}.");

        public abstract Task<TEntity> NewAsync(TParentKey? parentId, Type? variantType = null);
        public abstract Task<TEntity> InsertAsync(TParentKey? parentId, TEntity entity, IRelationContainer? relations);
        public abstract Task UpdateAsync(TKey id, TParentKey? parentId, TEntity entity, IRelationContainer? relations);
        public abstract Task DeleteAsync(TKey id, TParentKey? parentId);

        public virtual Task AddAsync(IEntity relatedEntity, TKey id)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(AddAsync)} on the {GetType()}.");
        public virtual Task RemoveAsync(IEntity relatedEntity, TKey id)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(RemoveAsync)} on the {GetType()}.");

        public abstract TKey ParseKey(string id);
        public abstract TParentKey? ParseParentKey(string? parentId);

        protected internal RepositoryChangeToken _repositoryChangeToken = new RepositoryChangeToken();

        public IChangeToken ChangeToken => _repositoryChangeToken;
        protected internal void NotifyUpdate()
        {
            _repositoryChangeToken.HasChanged = true;
            _repositoryChangeToken = new RepositoryChangeToken();
        }

        async Task<IEntity> IRepository.InternalGetByIdAsync(string id, string? parentId)
        {
            await Semaphore.WaitAsync();

            try
            {
                return (await GetByIdAsync(ParseKey(id), ParseParentKey(parentId))) as IEntity;
            }
            finally
            {
                Semaphore.Release();
            }
        }

        async Task<IEnumerable<IEntity>> IRepository.InternalGetAllAsync(string? parentId, IQuery query)
        {
            await Semaphore.WaitAsync();

            try
            {
                return (await GetAllAsync(ParseParentKey(parentId), TypedQuery<TEntity>.Convert(query))).Cast<IEntity>();
            }
            finally
            {
                Semaphore.Release();
            }
        }

        async Task<IEnumerable<IEntity>> IRepository.InternalGetAllRelatedAsync(IEntity relatedEntity, IQuery query)
        {
            await Semaphore.WaitAsync();

            try
            {
                return (await GetAllRelatedAsync(relatedEntity, TypedQuery<TEntity>.Convert(query)))?.Cast<IEntity>() ?? Enumerable.Empty<IEntity>();
            }
            finally
            {
                Semaphore.Release();
            }
        }

        async Task<IEnumerable<IEntity>> IRepository.InternalGetAllNonRelatedAsync(IEntity relatedEntity, IQuery query)
        {
            await Semaphore.WaitAsync();

            try
            {
                return (await GetAllNonRelatedAsync(relatedEntity, TypedQuery<TEntity>.Convert(query)))?.Cast<IEntity>() ?? Enumerable.Empty<IEntity>();
            }
            finally
            {
                Semaphore.Release();
            }
        }

        async Task<IEntity> IRepository.InternalNewAsync(string? parentId, Type? variantType)
        {
            await Semaphore.WaitAsync();

            try
            {
                return (await NewAsync(ParseParentKey(parentId), variantType)) as IEntity;
            }
            finally
            {
                Semaphore.Release();
            }
        }

        async Task<IEntity> IRepository.InternalInsertAsync(string? parentId, IEntity entity, IRelationContainer? relations)
        {
            await Semaphore.WaitAsync();

            try
            {
                return (await InsertAsync(ParseParentKey(parentId), (TEntity)entity, relations)) as IEntity;
            }
            finally
            {
                NotifyUpdate();
                Semaphore.Release();
            }
        }

        async Task IRepository.InternalUpdateAsync(string id, string? parentId, IEntity entity, IRelationContainer? relations)
        {
            await Semaphore.WaitAsync();

            try
            {
                await UpdateAsync(ParseKey(id), ParseParentKey(parentId), (TEntity)entity, relations);
            }
            finally
            {
                NotifyUpdate();
                Semaphore.Release();
            }
        }

        async Task IRepository.InternalDeleteAsync(string id, string? parentId)
        {
            await Semaphore.WaitAsync();

            try
            {
                await DeleteAsync(ParseKey(id), ParseParentKey(parentId));
            }
            finally
            {
                NotifyUpdate();
                Semaphore.Release();
            }
        }

        async Task IRepository.InternalAddAsync(IEntity relatedEntity, string id)
        {
            await Semaphore.WaitAsync();

            try
            {
                await AddAsync(relatedEntity, ParseKey(id));
            }
            finally
            {
                Semaphore.Release();
            }
        }
        async Task IRepository.InternalRemoveAsync(IEntity relatedEntity, string id)
        {
            await Semaphore.WaitAsync();

            try
            {
                await RemoveAsync(relatedEntity, ParseKey(id));
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }

    public interface IClassRepository<TKey, TParentKey, TEntity> : IRepository
        where TEntity : IEntity
        where TParentKey : class
    {

        Task<TEntity> GetByIdAsync(TKey id, TParentKey? parentId);
        Task<IEnumerable<TEntity>> GetAllAsync(TParentKey? parentId, IQuery<TEntity> query);
        Task<IEnumerable<TEntity>?> GetAllRelatedAsync(IEntity entity, IQuery<TEntity> query);
        Task<IEnumerable<TEntity>?> GetAllNonRelatedAsync(IEntity entity, IQuery<TEntity> query);

        Task<TEntity> NewAsync(TParentKey? parentId, Type? variantType);
        Task<TEntity> InsertAsync(TParentKey? parentId, TEntity entity, IRelationContainer? relations);
        Task UpdateAsync(TKey id, TParentKey? parentId, TEntity entity, IRelationContainer? relations);
        Task DeleteAsync(TKey id, TParentKey? parentId);

        Task AddAsync(IEntity relatedEntity, TKey id);
        Task RemoveAsync(IEntity relatedEntity, TKey id);

        TKey ParseKey(string id);
        TParentKey? ParseParentKey(string? parentId);
    }

    public abstract class BaseClassRepository<TKey, TParentKey, TEntity> : IRepository, IClassRepository<TKey, TParentKey, TEntity>
        where TEntity : IEntity
        where TParentKey : class
    {
        private SemaphoreSlim Semaphore { get; set; }

        public BaseClassRepository(SemaphoreSlim semaphore)
        {
            Semaphore = semaphore;
        }

        public abstract Task<TEntity> GetByIdAsync(TKey id, TParentKey? parentId);
        public abstract Task<IEnumerable<TEntity>> GetAllAsync(TParentKey? parentId, IQuery<TEntity> query);
        public virtual Task<IEnumerable<TEntity>?> GetAllRelatedAsync(IEntity relatedEntity, IQuery<TEntity> query)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(GetAllRelatedAsync)} on {GetType()}.");
        public virtual Task<IEnumerable<TEntity>?> GetAllNonRelatedAsync(IEntity relatedEntity, IQuery<TEntity> query)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(GetAllNonRelatedAsync)} on {GetType()}.");

        public abstract Task<TEntity> NewAsync(TParentKey? parentId, Type? variantType = null);
        public abstract Task<TEntity> InsertAsync(TParentKey? parentId, TEntity entity, IRelationContainer? relations);
        public abstract Task UpdateAsync(TKey id, TParentKey? parentId, TEntity entity, IRelationContainer? relations);
        public abstract Task DeleteAsync(TKey id, TParentKey? parentId);

        public virtual Task AddAsync(IEntity relatedEntity, TKey id)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(AddAsync)} on the {GetType()}.");
        public virtual Task RemoveAsync(IEntity relatedEntity, TKey id)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(RemoveAsync)} on the {GetType()}.");

        public abstract TKey ParseKey(string id);
        public abstract TParentKey? ParseParentKey(string? parentId);

        protected internal RepositoryChangeToken _repositoryChangeToken = new RepositoryChangeToken();

        public IChangeToken ChangeToken => _repositoryChangeToken;
        protected internal void NotifyUpdate()
        {
            _repositoryChangeToken.HasChanged = true;
            _repositoryChangeToken = new RepositoryChangeToken();
        }

        async Task<IEntity> IRepository.InternalGetByIdAsync(string id, string? parentId)
        {
            await Semaphore.WaitAsync();

            try
            {
                return (await GetByIdAsync(ParseKey(id), ParseParentKey(parentId))) as IEntity;
            }
            finally
            {
                Semaphore.Release();
            }
        }

        async Task<IEnumerable<IEntity>> IRepository.InternalGetAllAsync(string? parentId, IQuery query)
        {
            await Semaphore.WaitAsync();

            try
            {
                return (await GetAllAsync(ParseParentKey(parentId), TypedQuery<TEntity>.Convert(query))).Cast<IEntity>();
            }
            finally
            {
                Semaphore.Release();
            }
        }

        async Task<IEnumerable<IEntity>> IRepository.InternalGetAllRelatedAsync(IEntity relatedEntity, IQuery query)
        {
            await Semaphore.WaitAsync();

            try
            {
                return (await GetAllRelatedAsync(relatedEntity, TypedQuery<TEntity>.Convert(query)))?.Cast<IEntity>() ?? Enumerable.Empty<IEntity>();
            }
            finally
            {
                Semaphore.Release();
            }
        }

        async Task<IEnumerable<IEntity>> IRepository.InternalGetAllNonRelatedAsync(IEntity relatedEntity, IQuery query)
        {
            await Semaphore.WaitAsync();

            try
            {
                return (await GetAllNonRelatedAsync(relatedEntity, TypedQuery<TEntity>.Convert(query)))?.Cast<IEntity>() ?? Enumerable.Empty<IEntity>();
            }
            finally
            {
                Semaphore.Release();
            }
        }

        async Task<IEntity> IRepository.InternalNewAsync(string? parentId, Type? variantType)
        {
            await Semaphore.WaitAsync();

            try
            {
                return (await NewAsync(ParseParentKey(parentId), variantType)) as IEntity;
            }
            finally
            {
                Semaphore.Release();
            }
        }

        async Task<IEntity> IRepository.InternalInsertAsync(string? parentId, IEntity entity, IRelationContainer? relations)
        {
            await Semaphore.WaitAsync();

            try
            {
                return (await InsertAsync(ParseParentKey(parentId), (TEntity)entity, relations)) as IEntity;
            }
            finally
            {
                NotifyUpdate();
                Semaphore.Release();
            }
        }

        async Task IRepository.InternalUpdateAsync(string id, string? parentId, IEntity entity, IRelationContainer? relations)
        {
            await Semaphore.WaitAsync();

            try
            {
                await UpdateAsync(ParseKey(id), ParseParentKey(parentId), (TEntity)entity, relations);
            }
            finally
            {
                NotifyUpdate();
                Semaphore.Release();
            }
        }

        async Task IRepository.InternalDeleteAsync(string id, string? parentId)
        {
            await Semaphore.WaitAsync();

            try
            {
                await DeleteAsync(ParseKey(id), ParseParentKey(parentId));
            }
            finally
            {
                NotifyUpdate();
                Semaphore.Release();
            }
        }

        async Task IRepository.InternalAddAsync(IEntity relatedEntity, string id)
        {
            await Semaphore.WaitAsync();

            try
            {
                await AddAsync(relatedEntity, ParseKey(id));
            }
            finally
            {
                Semaphore.Release();
            }
        }

        async Task IRepository.InternalRemoveAsync(IEntity relatedEntity, string id)
        {
            await Semaphore.WaitAsync();

            try
            {
                await RemoveAsync(relatedEntity, ParseKey(id));
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }
}
