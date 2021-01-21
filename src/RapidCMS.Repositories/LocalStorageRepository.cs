﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using RapidCMS.Core.Abstractions.Data;
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

            UpdateStorageAsync(null);
        }

        private async void UpdateStorageAsync(object? obj)
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
    }
}
