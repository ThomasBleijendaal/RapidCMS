using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Repositories
{
    /// <summary>
    /// Base class of repository when backing store requires a mapping (the database entity cannot be used directly in the view).
    /// 
    /// If TEntity == TQueryEntity, use BaseRepository
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity used in the views</typeparam>
    /// <typeparam name="TDatabaseEntity">Type of the entity in the backing store</typeparam>
    public abstract class BaseMappedRepository<TEntity, TDatabaseEntity> : IRepository
        where TEntity : class, IEntity
        where TDatabaseEntity : class
    {
        /// <summary>
        /// This method gets an entity belonging to the given id and parent(s).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public abstract Task<TEntity?> GetByIdAsync(string id, IParent? parent);

        /// <summary>
        /// This method gets all entities belonging to the given parent(s) and query instructions (paging / search).
        /// 
        /// This query is based on the TDatabaseEntity, and not TEntity to allow for mapping.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        public abstract Task<IEnumerable<TEntity>> GetAllAsync(IParent? parent, IView<TDatabaseEntity> view);

        /// <summary>
        /// This method gets all entities that are related to the given entity and query instruction (paging / search).
        /// 
        /// This query is based on the TDatabaseEntity, and not TEntity to allow for mapping.
        /// </summary>
        /// <param name="related"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        public virtual Task<IEnumerable<TEntity>?> GetAllRelatedAsync(IRelated related, IView<TDatabaseEntity> view)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(GetAllRelatedAsync)} on the {GetType()}.");

        /// <summary>
        /// This method gets all entities that match the given query instruction (paging / search) but are not related to the given entity.
        /// 
        /// This query is based on the TDatabaseEntity, and not TEntity to allow for mapping.
        /// </summary>
        /// <param name="related"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        public virtual Task<IEnumerable<TEntity>?> GetAllNonRelatedAsync(IRelated related, IView<TDatabaseEntity> view)
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
        /// The editContext parameter contains the state of the form at the time of saving, allowing to check which property was edited.
        /// 
        /// The relations parameter contains all the relations that are set to this entity.
        /// </summary>
        /// <param name="editContext"></param>
        /// <returns></returns>
        public abstract Task<TEntity?> InsertAsync(IEditContext<TEntity> editContext);

        /// <summary>
        /// This method updates an existing entity in the database.
        /// 
        /// The editContext parameter contains the state of the form at the time of saving, allowing to check which property was edited.
        /// 
        /// The relations parameter contains all the relations that are set to this entity.
        /// </summary>
        /// <param name="editContext"></param>
        /// <returns></returns>
        public abstract Task UpdateAsync(IEditContext<TEntity> editContext);

        /// <summary>
        /// This method deletes an existing entity in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public abstract Task DeleteAsync(string id, IParent? parent);

        /// <summary>
        /// This methods adds an related entity to the entity that corresponds with the given id. 
        /// This method is used when an new many-to-many relation between two entities is made.
        /// </summary>
        /// <param name="related"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Task AddAsync(IRelated related, string id)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(AddAsync)} on the {GetType()}.");

        /// <summary>
        /// This methods removes an related entity from the entity that corresponds with the given id. 
        /// This method is used when a many-to-many relation between two entities is removed.
        /// </summary>
        /// <param name="related"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Task RemoveAsync(IRelated related, string id)
            => throw new NotImplementedException($"In order to use many-to-many list editors, implement {nameof(RemoveAsync)} on the {GetType()}.");

        /// <summary>
        /// This method is called when an entity is reordered and put in before of the given beforeId.
        /// If the beforeId is null, the entity is put in as last.
        /// </summary>
        /// <param name="beforeId"></param>
        /// <param name="id"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public virtual Task ReorderAsync(string? beforeId, string id, IParent? parent)
            => throw new NotImplementedException($"In order to use reordering in list editors, implement {nameof(ReorderAsync)} on the {GetType()}.");

        async Task<IEntity?> IRepository.GetByIdAsync(string id, IViewContext viewContext) 
            => await GetByIdAsync(id, viewContext.Parent);

        async Task<IEnumerable<IEntity>> IRepository.GetAllAsync(IViewContext viewContext, IView view) 
            => (await GetAllAsync(viewContext.Parent, TypedView<TDatabaseEntity>.Convert(view))).Cast<IEntity>();

        async Task<IEnumerable<IEntity>> IRepository.GetAllRelatedAsync(IRelatedViewContext viewContext, IView view) 
            => (await GetAllRelatedAsync(viewContext.Related, TypedView<TDatabaseEntity>.Convert(view)))?.Cast<IEntity>() ?? Enumerable.Empty<IEntity>();

        async Task<IEnumerable<IEntity>> IRepository.GetAllNonRelatedAsync(IRelatedViewContext viewContext, IView view) 
            => (await GetAllNonRelatedAsync(viewContext.Related, TypedView<TDatabaseEntity>.Convert(view)))?.Cast<IEntity>() ?? Enumerable.Empty<IEntity>();

        async Task<IEntity> IRepository.NewAsync(IViewContext viewContext, Type? variantType) 
            => await NewAsync(viewContext.Parent, variantType);

        async Task<IEntity?> IRepository.InsertAsync(IEditContext editContext) 
            => await InsertAsync((IEditContext<TEntity>)editContext);

        async Task IRepository.UpdateAsync(IEditContext editContext) 
            => await UpdateAsync((IEditContext<TEntity>)editContext);

        async Task IRepository.DeleteAsync(string id, IViewContext viewContext) 
            => await DeleteAsync(id, viewContext.Parent);

        async Task IRepository.AddAsync(IRelatedViewContext viewContext, string id) 
            => await AddAsync(viewContext.Related, id);
        async Task IRepository.RemoveAsync(IRelatedViewContext viewContext, string id) 
            => await RemoveAsync(viewContext.Related, id);

        async Task IRepository.ReorderAsync(string? beforeId, string id, IViewContext viewContext) 
            => await ReorderAsync(beforeId, id, viewContext.Parent);
    }
}
