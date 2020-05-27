using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using RapidCMS.Core.Abstractions.Data;

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
    }
}
