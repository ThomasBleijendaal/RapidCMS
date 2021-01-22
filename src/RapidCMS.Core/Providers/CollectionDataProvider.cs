using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.EventArgs.Mediators;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Providers
{
    internal class CollectionDataProvider : IRelationDataCollection, IDisposable
    {
        private readonly IRepository _repository;
        private readonly RepositoryRelationSetup _setup;
        private readonly IPropertyMetadata _property;
        private readonly IMediator _mediator;
        private readonly IMemoryCache _memoryCache;

        private FormEditContext? _editContext;
        private IParent? _parent;

        private readonly IDisposable? _eventHandle;

        private List<IElement>? _elements;
        private List<IElement>? _relatedElements;
        private ICollection<object>? _relatedIds;

        public CollectionDataProvider(
            IRepository repository,
            RepositoryRelationSetup setup,
            IPropertyMetadata property,
            IMediator mediator,
            IMemoryCache memoryCache)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _setup = setup ?? throw new ArgumentNullException(nameof(setup));
            _property = property ?? throw new ArgumentNullException(nameof(property));
            _mediator = mediator;
            _memoryCache = memoryCache;

            _eventHandle = _mediator.RegisterCallback<CollectionRepositoryEventArgs>(OnRepositoryChangeAsync);
        }

        public event EventHandler? OnDataChange;

        private async Task OnRepositoryChangeAsync(object? sender, CollectionRepositoryEventArgs args)
        {
            if (args.RepositoryAlias == _setup.RepositoryAlias)
            {
                await SetElementsAsync(refreshCache: true);

                OnDataChange?.Invoke(sender, EventArgs.Empty);
            }
        }

        public async Task SetEntityAsync(FormEditContext editContext, IParent? parent)
        {
            _editContext = editContext;
            _parent = parent;

            await SetElementsAsync();

            var data = _setup.RelatedElementsGetter?.Getter(_property.Getter(_editContext.Entity)) ?? _property.Getter(_editContext.Entity);
            if (data is ICollection<IEntity> entityCollection)
            {
                _relatedIds = entityCollection.Select(x => (object)x.Id!).ToList();
            }
            else if (data is ICollection<object> objectCollection)
            {
                _relatedIds = objectCollection;
            }
            else if (data is IEnumerable enumerable)
            {
                var list = new List<object>();
                foreach (var element in enumerable)
                {
                    if (element != null)
                    {
                        list.Add(element);
                    }
                }
                _relatedIds = list;
            }
            else
            {
                return;
            }

            UpdateRelatedElements();
        }

        private async Task SetElementsAsync(bool refreshCache = false)
        {
            if (_editContext == null)
            {
                return;
            }

            var parent = default(IParent?);

            if (_setup.EntityAsParent && _editContext.EntityState != EntityState.IsNew)
            {
                parent = new ParentEntity(_editContext.Parent, _editContext.Entity, _editContext.RepositoryAlias);
            }

            if (_setup.RepositoryParentSelector != null && _parent != null)
            {
                parent = _setup.RepositoryParentSelector.Getter.Invoke(_parent) as IParent;
            }

            var cacheKey = $"{_setup.RepositoryAlias}{parent?.GetParentPath()?.ToPathString()}";
            if (refreshCache)
            {
                _memoryCache.Remove(cacheKey);
            }

            var entities = await _memoryCache.GetOrCreateAsync(cacheKey, (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);

                return _repository.GetAllAsync(parent, Query.Default(_editContext.CollectionAlias));
            });

            _elements = entities
                .Select(entity => (IElement)new Element
                {
                    Id = _setup.IdProperty.Getter(entity),
                    Labels = _setup.DisplayProperties.Select(x => x.StringGetter(entity)).ToList()
                })
                .ToList();
        }

        private void UpdateRelatedElements()
        {
            _relatedElements = _elements?.Where(x => _relatedIds?.Contains(x.Id) ?? false).ToList();
        }

        public Task<IEnumerable<IElement>> GetAvailableElementsAsync()
        {
            return Task.FromResult(_elements ?? Enumerable.Empty<IElement>());
        }

        public Task<IReadOnlyList<IElement>> GetRelatedElementsAsync()
        {
            var relatedElements = _relatedIds != null && _elements != null
                ? _elements.Where(x => _relatedIds.Contains(x.Id)).ToList()
                : new List<IElement>();

            return Task.FromResult(relatedElements as IReadOnlyList<IElement>);
        }

        public Task AddElementAsync(IElement option)
        {
            if (_elements != null)
            {
                _relatedElements?.Add(_elements.First(x => x.Id == option.Id));
            }

            return Task.CompletedTask;
        }

        public Task RemoveElementAsync(IElement option)
        {
            return Task.FromResult(_relatedElements?.RemoveAll(x => x.Id == option.Id));
        }

        public Task SetElementAsync(IElement option)
        {
            _relatedElements?.Clear();
            if (_elements != null)
            {
                _relatedElements?.Add(_elements.First(x => x.Id == option.Id));
            }

            return Task.CompletedTask;
        }

        public IReadOnlyList<IElement> GetCurrentRelatedElements()
        {
            return _relatedElements ?? new List<IElement>();
        }

        public Type GetRelatedEntityType()
        {
            return _setup.RelatedEntityType ?? typeof(object);
        }

        public void Dispose()
        {
            _eventHandle?.Dispose();
        }
    }
}
