using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Providers
{
    internal class CollectionDataProvider : IRelationDataCollection, IDisposable
    {
        private readonly IRepository _repository;
        private readonly Type _relatedEntityType;
        private readonly IPropertyMetadata _property;
        private readonly IPropertyMetadata? _relatedElementsGetter;
        private readonly IPropertyMetadata? _repositoryParentSelector;
        private readonly bool _entityAsParent;
        private readonly IPropertyMetadata _idProperty;
        private readonly IEnumerable<IExpressionMetadata> _labelProperties;
        private readonly IMemoryCache _memoryCache;
        private EditContext? _editContext;
        private IParent? _parent;

        private IDisposable? _eventHandle;

        private List<IElement>? _elements;
        private List<IElement>? _relatedElements;
        private ICollection<object>? _relatedIds;

        public CollectionDataProvider(
            IRepository repository,
            Type relatedEntityType,
            IPropertyMetadata property,
            IPropertyMetadata? relatedElementsGetter,
            IPropertyMetadata? repositoryParentSelector,
            bool entityAsParent,
            IPropertyMetadata idProperty,
            IEnumerable<IExpressionMetadata> labelProperties,
            IMemoryCache memoryCache)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _relatedEntityType = relatedEntityType ?? throw new ArgumentNullException(nameof(relatedEntityType));
            _property = property ?? throw new ArgumentNullException(nameof(property));
            _relatedElementsGetter = relatedElementsGetter;
            _repositoryParentSelector = repositoryParentSelector;
            _entityAsParent = entityAsParent;
            _idProperty = idProperty ?? throw new ArgumentNullException(nameof(idProperty));
            _labelProperties = labelProperties ?? throw new ArgumentNullException(nameof(labelProperties));
            _memoryCache = memoryCache;
        }

        public event EventHandler? OnDataChange;

        public async Task SetEntityAsync(EditContext editContext, IParent? parent)
        {
            _editContext = editContext;
            _parent = parent;

            await SetElementsAsync();

            var data = _relatedElementsGetter?.Getter(_property.Getter(_editContext.Entity)) ?? _property.Getter(_editContext.Entity);
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

        private async Task SetElementsAsync()
        {
            if (_editContext == null)
            {
                return;
            }

            _eventHandle?.Dispose();
            _eventHandle = _repository.ChangeToken.RegisterChangeCallback(async x =>
            {
                await SetElementsAsync();

                OnDataChange?.Invoke(this, new EventArgs());

            }, null);

            var parent = default(IParent?);

            if (_entityAsParent && _editContext.EntityState != EntityState.IsNew)
            {
                parent = new ParentEntity(_editContext.Parent, _editContext.Entity, _editContext.RepositoryAlias);
            }

            if (_repositoryParentSelector != null && _parent != null)
            {
                parent = _repositoryParentSelector.Getter.Invoke(_parent) as IParent;
            }

            var entities = await _memoryCache.GetOrCreateAsync(new { _repository, parent }, async (entry) =>
            {
                entry.AddExpirationToken(_repository.ChangeToken);

                return await _repository.GetAllAsync(parent, Query.Default(_editContext.CollectionAlias));
            });

            _elements = entities
                .Select(entity => (IElement)new Element
                {
                    Id = _idProperty.Getter(entity),
                    Labels = _labelProperties.Select(x => x.StringGetter(entity)).ToList()
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
            _relatedElements?.Add(_elements.First(x => x.Id == option.Id));

            return Task.CompletedTask;
        }

        public Task RemoveElementAsync(IElement option)
        {
            return Task.FromResult(_relatedElements?.RemoveAll(x => x.Id == option.Id));
        }

        public Task SetElementAsync(IElement option)
        {
            _relatedElements?.Clear();
            _relatedElements?.Add(_elements.First(x => x.Id == option.Id));

            return Task.CompletedTask;
        }

        public IReadOnlyList<IElement> GetCurrentRelatedElements()
        {
            return _relatedElements ?? new List<IElement>();
        }

        public Type GetRelatedEntityType()
        {
            return _relatedEntityType ?? typeof(object);
        }

        public void Dispose()
        {
            _eventHandle?.Dispose();
        }
    }
}
