using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Mediators;

namespace RapidCMS.Repositories
{
    public class LocalStorageRepository<TEntity> : InMemoryRepository<TEntity>
        where TEntity : class, IEntity, ICloneable, new()
    {
        private readonly ILocalStorageService _localStorage;

        private readonly Task _initializationTask;

        public LocalStorageRepository(
            ILocalStorageService localStorage,
            IMediator mediator,
            IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
            _localStorage = localStorage;

            _initializationTask = InitializationTaskAsync();
        }

        private async Task InitializationTaskAsync()
        {
            var dataStorage = await _localStorage.GetItemAsync<Dictionary<string, List<TEntity>>>(GetType().FullName);
            if (dataStorage != null)
            {
                _data = dataStorage;
            }

            var relationStorage = await _localStorage.GetItemAsync<Dictionary<string, List<string>>>($"{GetType().FullName}-relation");
            if (relationStorage != null)
            {
                _relations = relationStorage;
            }

            await UpdateStorageAsync();
        }

        private async Task UpdateStorageAsync()
        {
            try
            {
                await _localStorage.SetItemAsync(GetType().FullName, _data);
                await _localStorage.SetItemAsync($"{GetType().FullName}-relation", _relations);
            }
            catch { }
        }

        public override async Task<IEnumerable<TEntity>> GetAllAsync(IParent? parent, IQuery<TEntity> query)
        {
            await _initializationTask;
            return await base.GetAllAsync(parent, query);
        }

        public override async Task<IEnumerable<TEntity>?> GetAllNonRelatedAsync(IRelated related, IQuery<TEntity> query)
        {
            await _initializationTask;
            return await base.GetAllNonRelatedAsync(related, query);
        }

        public override async Task<IEnumerable<TEntity>?> GetAllRelatedAsync(IRelated related, IQuery<TEntity> query)
        {
            await _initializationTask;
            return await base.GetAllRelatedAsync(related, query);
        }

        public override async Task<TEntity?> GetByIdAsync(string id, IParent? parent)
        {
            await _initializationTask;
            return await base.GetByIdAsync(id, parent);
        }

        public override async Task AddAsync(IRelated related, string id)
        {
            await base.AddAsync(related, id);
            await UpdateStorageAsync();
        }

        public override async Task DeleteAsync(string id, IParent? parent)
        {
            await base.DeleteAsync(id, parent);
            await UpdateStorageAsync();
        }

        public override async Task<TEntity?> InsertAsync(IEditContext<TEntity> editContext)
        {
            var entity = await base.InsertAsync(editContext);
            await UpdateStorageAsync();
            return entity;
        }

        public override async Task RemoveAsync(IRelated related, string id)
        {
            await base.RemoveAsync(related, id);
            await UpdateStorageAsync();
        }

        public override async Task ReorderAsync(string? beforeId, string id, IParent? parent)
        {
            await base.ReorderAsync(beforeId, id, parent);
            await UpdateStorageAsync();
        }

        public override async Task UpdateAsync(IEditContext<TEntity> editContext)
        {
            await base.UpdateAsync(editContext);
            await UpdateStorageAsync();
        }
    }
}
