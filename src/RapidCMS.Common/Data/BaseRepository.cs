using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace RapidCMS.Common.Data
{
    public abstract class BaseRepository<TKey, TEntity> : IRepository, IRepository<TKey, TEntity>
        where TEntity : class, IEntity
    {
        public abstract Task<TEntity?> GetByIdAsync(TKey id, IParent? parent);
        public abstract Task<IEnumerable<TEntity>> GetAllAsync(IParent? parent, IQuery<TEntity> query);
        public virtual Task<IEnumerable<TEntity>?> GetAllRelatedAsync(IEntity relatedEntity, IQuery<TEntity> query)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(GetAllRelatedAsync)} on the {GetType()}.");
        public virtual Task<IEnumerable<TEntity>?> GetAllNonRelatedAsync(IEntity relatedEntity, IQuery<TEntity> query)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(GetAllNonRelatedAsync)} on the {GetType()}.");

        public abstract Task<TEntity> NewAsync(IParent? parent, Type? variantType = null);
        public abstract Task<TEntity?> InsertAsync(IParent? parent, TEntity entity, IRelationContainer? relations);
        public abstract Task UpdateAsync(TKey id, IParent? parent, TEntity entity, IRelationContainer? relations);
        public abstract Task DeleteAsync(TKey id, IParent? parent);

        public virtual Task AddAsync(IEntity relatedEntity, TKey id)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(AddAsync)} on the {GetType()}.");
        public virtual Task RemoveAsync(IEntity relatedEntity, TKey id)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(RemoveAsync)} on the {GetType()}.");

        public abstract TKey ParseKey(string id);

        protected internal RepositoryChangeToken _repositoryChangeToken = new RepositoryChangeToken();

        public IChangeToken ChangeToken => _repositoryChangeToken;
        protected internal void NotifyUpdate()
        {
            var currentToken = _repositoryChangeToken;
            _repositoryChangeToken = new RepositoryChangeToken();
            currentToken.HasChanged = true;
        }

        async Task<IEntity?> IRepository.InternalGetByIdAsync(string id, IParent? parent)
        {
            return (await GetByIdAsync(ParseKey(id), parent)) as IEntity;
        }

        async Task<IEnumerable<IEntity>> IRepository.InternalGetAllAsync(IParent? parent, IQuery query)
        {
            return (await GetAllAsync(parent, TypedQuery<TEntity>.Convert(query))).Cast<IEntity>();
        }

        async Task<IEnumerable<IEntity>> IRepository.InternalGetAllRelatedAsync(IEntity relatedEntity, IQuery query)
        {
            return (await GetAllRelatedAsync(relatedEntity, TypedQuery<TEntity>.Convert(query)))?.Cast<IEntity>() ?? Enumerable.Empty<IEntity>();
        }

        async Task<IEnumerable<IEntity>> IRepository.InternalGetAllNonRelatedAsync(IEntity relatedEntity, IQuery query)
        {
            return (await GetAllNonRelatedAsync(relatedEntity, TypedQuery<TEntity>.Convert(query)))?.Cast<IEntity>() ?? Enumerable.Empty<IEntity>();
        }

        async Task<IEntity> IRepository.InternalNewAsync(IParent? parent, Type? variantType)
        {
            return (await NewAsync(parent, variantType)) as IEntity;
        }

        async Task<IEntity?> IRepository.InternalInsertAsync(IParent? parent, IEntity entity, IRelationContainer? relations)
        {
            var data = (await InsertAsync(parent, (TEntity)entity, relations)) as IEntity;
            NotifyUpdate();
            return data;
        }

        async Task IRepository.InternalUpdateAsync(string id, IParent? parent, IEntity entity, IRelationContainer? relations)
        {
            await UpdateAsync(ParseKey(id), parent, (TEntity)entity, relations);
            NotifyUpdate();
        }

        async Task IRepository.InternalDeleteAsync(string id, IParent? parent)
        {
            await DeleteAsync(ParseKey(id), parent);
            NotifyUpdate();
        }

        async Task IRepository.InternalAddAsync(IEntity relatedEntity, string id)
        {
            await AddAsync(relatedEntity, ParseKey(id));
        }
        async Task IRepository.InternalRemoveAsync(IEntity relatedEntity, string id)
        {
            await RemoveAsync(relatedEntity, ParseKey(id));
        }
    }
}
