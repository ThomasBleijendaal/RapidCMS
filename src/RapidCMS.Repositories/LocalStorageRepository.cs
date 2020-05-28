using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Repositories;

namespace RapidCMS.Repositories
{
    public class LocalStorageRepository<TEntity> : InMemoryRepository<TEntity>
        where TEntity : class, IEntity, ICloneable, new()
    {
        private readonly ILocalStorageService _localStorage;

        private readonly Task _initializationTask;

        public LocalStorageRepository(
            ILocalStorageService localStorage,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _localStorage = localStorage;

            _initializationTask = InitializationTaskAsync();
        }

        private async Task InitializationTaskAsync()
        {
            var storage = await _localStorage.GetItemAsync<Dictionary<string, List<TEntity>>>(GetType().FullName);

            if (storage != null)
            {
                _data = storage;
            }

            UpdateStorageAsync(null);
        }

        private async void UpdateStorageAsync(object? obj)
        {
            try
            {
                await _localStorage.SetItemAsync(GetType().FullName, _data);
            }
            catch { }

            ChangeToken.RegisterChangeCallback(UpdateStorageAsync, default);
        }

        public override async Task<IEnumerable<TEntity>> GetAllAsync(IRepositoryContext context, IParent? parent, IQuery<TEntity> query)
        {
            await _initializationTask;
            return await base.GetAllAsync(context, parent, query);
        }

        public override async Task<IEnumerable<TEntity>?> GetAllNonRelatedAsync(IRepositoryContext context, IRelated related, IQuery<TEntity> query)
        {
            await _initializationTask;
            return await base.GetAllNonRelatedAsync(context, related, query);
        }

        public override async Task<IEnumerable<TEntity>?> GetAllRelatedAsync(IRepositoryContext context, IRelated related, IQuery<TEntity> query)
        {
            await _initializationTask;
            return await base.GetAllRelatedAsync(context, related, query);
        }
    }
}
