using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.ChangeToken;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Repositories
{
    /// <summary>
    /// Base class of which most repositories should be derived. 
    /// 
    /// When backing store of this repository requires a mapping (the database entity cannot be used directly in the view), use MappedBaseRepository.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity used in views and in the backing store</typeparam>
    public abstract class BaseRepository<TEntity> : IRepository
        where TEntity : class, IEntity
    {
        /// <summary>
        /// This method gets an entity belonging to the given id and parent(s).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public abstract Task<TEntity?> GetByIdAsync(IRepositoryContext context, string id, IParent? parent);

        /// <summary>
        /// This method gets all entities belonging to the given parent(s) and query instructions (paging / search).
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public abstract Task<IEnumerable<TEntity>> GetAllAsync(IRepositoryContext context, IParent? parent, IQuery<TEntity> query);

        /// <summary>
        /// This method gets all entities that are related to the given entity and query instruction (paging / search).
        /// </summary>
        /// <param name="related"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual Task<IEnumerable<TEntity>?> GetAllRelatedAsync(IRepositoryContext context, IRelated related, IQuery<TEntity> query)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(GetAllRelatedAsync)} on the {GetType()}.");

        /// <summary>
        /// This method gets all entities that match the given query instruction (paging / search) but are not related to the given entity.
        /// </summary>
        /// <param name="related"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual Task<IEnumerable<TEntity>?> GetAllNonRelatedAsync(IRepositoryContext context, IRelated related, IQuery<TEntity> query)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(GetAllNonRelatedAsync)} on the {GetType()}.");

        /// <summary>
        /// This method creates a new entity in-memory, and does not affect the database.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="variantType"></param>
        /// <returns></returns>
        public abstract Task<TEntity> NewAsync(IRepositoryContext context, IParent? parent, Type? variantType = null);

        /// <summary>
        /// This method inserts a new entity in the database.
        /// 
        /// The editContext parameter contains the state of the form at the time of saving, allowing to check which property was edited.
        /// 
        /// The relations parameter contains all the relations that are set to this entity.
        /// </summary>
        /// <param name="editContext"></param>
        /// <returns></returns>
        public abstract Task<TEntity?> InsertAsync(IRepositoryContext context, IEditContext<TEntity> editContext);

        /// <summary>
        /// This method updates an existing entity in the database.
        /// 
        /// The editContext parameter contains the state of the form at the time of saving, allowing to check which property was edited.
        /// 
        /// The relations parameter contains all the relations that are set to this entity.
        /// </summary>
        /// <param name="editContext"></param>
        /// <returns></returns>
        public abstract Task UpdateAsync(IRepositoryContext context, IEditContext<TEntity> editContext);

        /// <summary>
        /// This method deletes an existing entity in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public abstract Task DeleteAsync(IRepositoryContext context, string id, IParent? parent);

        /// <summary>
        /// This methods adds an releated entity to the entity that corresponds with the given id. 
        /// This method is used when an new many-to-many relation between two entities is made.
        /// </summary>
        /// <param name="related"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Task AddAsync(IRepositoryContext context, IRelated related, string id)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(AddAsync)} on the {GetType()}.");

        /// <summary>
        /// This methods removes an releated entity from the entity that corresponds with the given id. 
        /// This method is used when a many-to-many relation between two entities is removed.
        /// </summary>
        /// <param name="related"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Task RemoveAsync(IRepositoryContext context, IRelated related, string id)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(RemoveAsync)} on the {GetType()}.");

        /// <summary>
        /// This method is called when an entity is reorderd and put in before of the given beforeId.
        /// If the beforeId is null, the entity is put in as last.
        /// </summary>
        /// <param name="beforeId"></param>
        /// <param name="id"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public virtual Task ReorderAsync(IRepositoryContext context, string? beforeId, string id, IParent? parent)
            => throw new NotImplementedException($"In order to use reordering in list editors, implement {nameof(ReorderAsync)} on the {GetType()}.");

        protected internal CmsChangeToken _repositoryChangeToken = new CmsChangeToken();

        public IChangeToken ChangeToken => _repositoryChangeToken;

        protected internal void NotifyUpdate()
        {
            var currentToken = _repositoryChangeToken;
            _repositoryChangeToken = new CmsChangeToken();
            currentToken.HasChanged = true;
        }

        async Task<IEntity?> IRepository.GetByIdAsync(IRepositoryContext context, string id, IParent? parent)
        {
            return await GetByIdAsync(context, id, parent).ConfigureAwait(false);
        }

        async Task<IEnumerable<IEntity>> IRepository.GetAllAsync(IRepositoryContext context, IParent? parent, IQuery query)
        {
            return (await GetAllAsync(context, parent, TypedQuery<TEntity>.Convert(query)).ConfigureAwait(false)).Cast<IEntity>();
        }

        async Task<IEnumerable<IEntity>> IRepository.GetAllRelatedAsync(IRepositoryContext context, IRelated related, IQuery query)
        {
            return (await GetAllRelatedAsync(context, related, TypedQuery<TEntity>.Convert(query)).ConfigureAwait(false))?.Cast<IEntity>() ?? Enumerable.Empty<IEntity>();
        }

        async Task<IEnumerable<IEntity>> IRepository.GetAllNonRelatedAsync(IRepositoryContext context, IRelated related, IQuery query)
        {
            return (await GetAllNonRelatedAsync(context, related, TypedQuery<TEntity>.Convert(query)).ConfigureAwait(false))?.Cast<IEntity>() ?? Enumerable.Empty<IEntity>();
        }

        async Task<IEntity> IRepository.NewAsync(IRepositoryContext context, IParent? parent, Type? variantType)
        {
            return await NewAsync(context, parent, variantType).ConfigureAwait(false);
        }

        async Task<IEntity?> IRepository.InsertAsync(IRepositoryContext context, IEditContext editContext)
        {
            var data = await InsertAsync(context, (IEditContext<TEntity>)editContext).ConfigureAwait(false) as IEntity;
            NotifyUpdate();
            return data;
        }

        async Task IRepository.UpdateAsync(IRepositoryContext context, IEditContext editContext)
        {
            await UpdateAsync(context, (IEditContext<TEntity>)editContext).ConfigureAwait(false);
            NotifyUpdate();
        }

        async Task IRepository.DeleteAsync(IRepositoryContext context, string id, IParent? parent)
        {
            await DeleteAsync(context, id, parent).ConfigureAwait(false);
            NotifyUpdate();
        }

        async Task IRepository.AddAsync(IRepositoryContext context, IRelated related, string id)
        {
            await AddAsync(context, related, id).ConfigureAwait(false);
        }
        async Task IRepository.RemoveAsync(IRepositoryContext context, IRelated related, string id)
        {
            await RemoveAsync(context, related, id).ConfigureAwait(false);
        }

        async Task IRepository.ReorderAsync(IRepositoryContext context, string? beforeId, string id, IParent? parent)
        {
            await ReorderAsync(context, beforeId, id, parent).ConfigureAwait(false);
            NotifyUpdate();
        }
    }
}
