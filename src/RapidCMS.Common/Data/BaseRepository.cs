using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using RapidCMS.Common.Forms;

namespace RapidCMS.Common.Data
{
    public abstract class BaseRepository<TKey, TEntity> : IRepository, IRepository<TKey, TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// This method gets an entity belonging to the given id and parent(s).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public abstract Task<TEntity?> GetByIdAsync(TKey id, IParent? parent);

        /// <summary>
        /// This method gets all entities belonging to the given parent(s) and query instructions (paging / search).
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public abstract Task<IEnumerable<TEntity>> GetAllAsync(IParent? parent, IQuery<TEntity> query);

        /// <summary>
        /// This method gets all entities that are related to the given entity and query instruction (paging / search).
        /// </summary>
        /// <param name="relatedEntity"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual Task<IEnumerable<TEntity>?> GetAllRelatedAsync(IEntity relatedEntity, IQuery<TEntity> query)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(GetAllRelatedAsync)} on the {GetType()}.");

        /// <summary>
        /// This method gets all entities that match the given query instruction (paging / search) but are not related to the given entity.
        /// </summary>
        /// <param name="relatedEntity"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual Task<IEnumerable<TEntity>?> GetAllNonRelatedAsync(IEntity relatedEntity, IQuery<TEntity> query)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(GetAllNonRelatedAsync)} on the {GetType()}.");

        /// <summary>
        /// This method creates a new entity in-memory, and does not affect the database.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="variantType"></param>
        /// <returns></returns>
        public abstract Task<TEntity> NewAsync(IParent? parent, Type? variantType = null);

        /// <summary>
        /// This method inserts a new entity in the database.
        /// 
        /// The relations parameter contains all the relations that are set to this entity.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="entity"></param>
        /// <param name="relations"></param>
        /// <returns></returns>
        public virtual Task<TEntity?> InsertAsync(IParent? parent, TEntity entity, IRelationContainer? relations)
            => throw new NotImplementedException($"Implement one of the {nameof(InsertAsync)} on the {GetType()}.");

        /// <summary>
        /// This method inserts a new entity in the database.
        /// 
        /// The editContext parameter contains the state of the form at the time of saving, allowing to check which property was edited.
        /// 
        /// The relations parameter contains all the relations that are set to this entity.
        /// </summary>
        /// <param name="editContext"></param>
        /// <returns></returns>
        public virtual Task<TEntity?> InsertAsync(IEditContext<TEntity> editContext)
            => InsertAsync(editContext.Parent, editContext.Entity, editContext.GetRelationContainer());

        /// <summary>
        /// This method updates an existing entity in the database.
        /// 
        /// The relations parameter contains all the relations that are set to this entity.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parent"></param>
        /// <param name="entity"></param>
        /// <param name="relations"></param>
        /// <returns></returns>
        public virtual Task UpdateAsync(TKey id, IParent? parent, TEntity entity, IRelationContainer? relations)
            => throw new NotImplementedException($"Implement one of the {nameof(UpdateAsync)} on the {GetType()}.");

        /// <summary>
        /// This method updates an existing entity in the database.
        /// 
        /// The editContext parameter contains the state of the form at the time of saving, allowing to check which property was edited.
        /// 
        /// The relations parameter contains all the relations that are set to this entity.
        /// </summary>
        /// <param name="editContext"></param>
        /// <returns></returns>
        public virtual Task UpdateAsync(IEditContext<TEntity> editContext)
            => UpdateAsync(ParseKey(editContext.Entity.Id!), editContext.Parent, editContext.Entity, editContext.GetRelationContainer());
        public abstract Task DeleteAsync(TKey id, IParent? parent);

        /// <summary>
        /// This methods adds an releated entity to the entity that corresponds with the given id. 
        /// This method is used when an new many-to-many relation between two entities is made.
        /// </summary>
        /// <param name="relatedEntity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Task AddAsync(IEntity relatedEntity, TKey id)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(AddAsync)} on the {GetType()}.");

        /// <summary>
        /// This methods removes an releated entity from the entity that corresponds with the given id. 
        /// This method is used when a many-to-many relation between two entities is removed.
        /// </summary>
        /// <param name="relatedEntity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Task RemoveAsync(IEntity relatedEntity, TKey id)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(RemoveAsync)} on the {GetType()}.");

        /// <summary>
        /// This method is invoked when the id of the entity must be parsed from a string (originating from the URL) to TKey.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract TKey ParseKey(string id);

        protected internal RepositoryChangeToken _repositoryChangeToken = new RepositoryChangeToken();

        public IChangeToken ChangeToken => _repositoryChangeToken;
        protected internal void NotifyUpdate()
        {
            var currentToken = _repositoryChangeToken;
            _repositoryChangeToken = new RepositoryChangeToken();
            currentToken.HasChanged = true;
        }

        async Task<IEntity?> IRepository.GetByIdAsync(string id, IParent? parent)
        {
            return (await GetByIdAsync(ParseKey(id), parent)) as IEntity;
        }

        async Task<IEnumerable<IEntity>> IRepository.GetAllAsync(IParent? parent, IQuery query)
        {
            return (await GetAllAsync(parent, TypedQuery<TEntity>.Convert(query))).Cast<IEntity>();
        }

        async Task<IEnumerable<IEntity>> IRepository.GetAllRelatedAsync(IEntity relatedEntity, IQuery query)
        {
            return (await GetAllRelatedAsync(relatedEntity, TypedQuery<TEntity>.Convert(query)))?.Cast<IEntity>() ?? Enumerable.Empty<IEntity>();
        }

        async Task<IEnumerable<IEntity>> IRepository.GetAllNonRelatedAsync(IEntity relatedEntity, IQuery query)
        {
            return (await GetAllNonRelatedAsync(relatedEntity, TypedQuery<TEntity>.Convert(query)))?.Cast<IEntity>() ?? Enumerable.Empty<IEntity>();
        }

        async Task<IEntity> IRepository.NewAsync(IParent? parent, Type? variantType)
        {
            return (await NewAsync(parent, variantType)) as IEntity;
        }

        async Task<IEntity?> IRepository.InsertAsync(EditContext editContext)
        {
            var data = (await InsertAsync(new EditContextWrapper<TEntity>(editContext))) as IEntity;
            NotifyUpdate();
            return data;
        }

        async Task IRepository.UpdateAsync(EditContext editContext)
        {
            await UpdateAsync(new EditContextWrapper<TEntity>(editContext));
            NotifyUpdate();
        }

        async Task IRepository.DeleteAsync(string id, IParent? parent)
        {
            await DeleteAsync(ParseKey(id), parent);
            NotifyUpdate();
        }

        async Task IRepository.AddAsync(IEntity relatedEntity, string id)
        {
            await AddAsync(relatedEntity, ParseKey(id));
        }
        async Task IRepository.RemoveAsync(IEntity relatedEntity, string id)
        {
            await RemoveAsync(relatedEntity, ParseKey(id));
        }
    }
}
