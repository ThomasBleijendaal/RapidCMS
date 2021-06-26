using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Abstractions.Services;
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
        private readonly IConcurrencyService _concurrencyService;
        private readonly string? _repositoryAlias;
        private readonly string? _collectionAlias;
        private readonly IPropertyMetadata? _relatedElementsGetter;
        private readonly bool _entityAsParent;
        private readonly IPropertyMetadata? _repositoryParentSelector;
        private readonly IPropertyMetadata _idProperty;
        private readonly IReadOnlyList<IExpressionMetadata> _displayProperties;
        private readonly Type _relatedEntityType;
        private readonly IPropertyMetadata _property;
        private readonly IMediator _mediator;

        private FormEditContext? _editContext { get; set; }
        private IParent? _parent;

        private readonly IDisposable? _eventHandle;

        private List<object> _relatedIds = new List<object>();

        public CollectionDataProvider(
            IRepository repository,
            IConcurrencyService concurrencyService,
            string? repositoryAlias,
            string? collectionAlias,
            IPropertyMetadata? relatedElementsGetter,
            bool entityAsParent,
            IPropertyMetadata? repositoryParentSelector,
            IPropertyMetadata idProperty,
            IReadOnlyList<IExpressionMetadata> displayProperties,
            Type relatedEntityType,
            IPropertyMetadata property,
            IMediator mediator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _concurrencyService = concurrencyService;
            _repositoryAlias = repositoryAlias;
            _collectionAlias = collectionAlias;
            _relatedElementsGetter = relatedElementsGetter;
            _entityAsParent = entityAsParent;
            _repositoryParentSelector = repositoryParentSelector;
            _idProperty = idProperty ?? throw new ArgumentNullException(nameof(idProperty));
            _displayProperties = displayProperties ?? throw new ArgumentNullException(nameof(displayProperties));
            _relatedEntityType = relatedEntityType ?? throw new ArgumentNullException(nameof(relatedEntityType));
            _property = property ?? throw new ArgumentNullException(nameof(property));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            _eventHandle = _mediator.RegisterCallback<CollectionRepositoryEventArgs>(OnRepositoryChangeAsync);
        }

        public void Configure(object configuration) { }

        public event EventHandler? OnDataChange;

        private Task OnRepositoryChangeAsync(object? sender, CollectionRepositoryEventArgs args)
        {
            if ((!string.IsNullOrEmpty(_repositoryAlias) && args.RepositoryAlias == _repositoryAlias) ||
                (string.IsNullOrEmpty(_repositoryAlias) && args.CollectionAlias == _collectionAlias))
            {
                OnDataChange?.Invoke(sender, EventArgs.Empty);
            }

            return Task.CompletedTask;
        }

        public Task SetEntityAsync(FormEditContext editContext, IParent? parent)
        {
            _editContext = editContext;
            _parent = parent;

            var data = _relatedElementsGetter?.Getter(_property.Getter(_editContext.Entity)) ?? _property.Getter(_editContext.Entity);
            if (data is IEnumerable<IEntity> entityCollection)
            {
                _relatedIds = entityCollection.Select(x => (object)x.Id!).ToList();
            }
            else if (data is IEnumerable<object> objectCollection)
            {
                _relatedIds = objectCollection.ToList();
            }
            else if (data is IEnumerable enumerable && data is not string)
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

            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<IElement>> GetAvailableElementsAsync(IQuery query)
        {
            if (_editContext == null)
            {
                return new List<IElement>();
            }

            var parent = default(IParent?);

            if (_entityAsParent && _editContext.EntityState != EntityState.IsNew)
            {
                parent = new ParentEntity(_editContext.Parent, _editContext.Entity, _editContext.RepositoryAlias);
            }

            if (_repositoryParentSelector != null && _parent != null)
            {
                parent = _repositoryParentSelector.Getter.Invoke(_parent) as IParent;
            }

            query.CollectionAlias = _editContext.CollectionAlias;

            var entities = await _concurrencyService.EnsureCorrectConcurrencyAsync(() => _repository.GetAllAsync(new ViewContext(_editContext.CollectionAlias, parent), query));

            return entities
               .Select(entity => (IElement)new Element
               {
                   Id = _idProperty.Getter(entity),
                   Labels = _displayProperties.Select(x => x.StringGetter(entity)).ToList()
               })
               .ToList();
        }

        public async Task<IReadOnlyList<IElement>> GetRelatedElementsAsync()
        {
            if (!_relatedIds.Any())
            {
                return new List<IElement>();
            }

            var elements = await GetAvailableElementsAsync(Query.Default());
            return elements.Where(x => _relatedIds.Contains(x.Id)).ToList();
        }

        public void AddElement(object id) => _relatedIds.Add(id);

        public void RemoveElement(object id) => _relatedIds.Remove(id);

        public bool IsRelated(object id) => _relatedIds.Any(x => x.Equals(id));

        public IReadOnlyList<object> GetCurrentRelatedElementIds() => _relatedIds;

        public Type GetRelatedEntityType() => _relatedEntityType ?? typeof(object);

        public void Dispose() => _eventHandle?.Dispose();
    }
}
