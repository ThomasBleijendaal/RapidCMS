using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        private FormEditContext? _editContext;
        private IParent? _parent;

        private readonly IDisposable? _eventHandle;

        private List<object> _relatedIds = new List<object>();

        public CollectionDataProvider(
            IRepository repository,
            RepositoryRelationSetup setup,
            IPropertyMetadata property,
            IMediator mediator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _setup = setup ?? throw new ArgumentNullException(nameof(setup));
            _property = property ?? throw new ArgumentNullException(nameof(property));
            _mediator = mediator;

            _eventHandle = _mediator.RegisterCallback<CollectionRepositoryEventArgs>(OnRepositoryChangeAsync);
        }

        public event EventHandler? OnDataChange;

        private Task OnRepositoryChangeAsync(object? sender, CollectionRepositoryEventArgs args)
        {
            if ((!string.IsNullOrEmpty(_setup.RepositoryAlias) && args.RepositoryAlias == _setup.RepositoryAlias) ||
                (string.IsNullOrEmpty(_setup.RepositoryAlias) && args.CollectionAlias == _setup.CollectionAlias))
            {
                OnDataChange?.Invoke(sender, EventArgs.Empty);
            }

            return Task.CompletedTask;
        }

        public Task SetEntityAsync(FormEditContext editContext, IParent? parent)
        {
            _editContext = editContext;
            _parent = parent;

            var data = _setup.RelatedElementsGetter?.Getter(_property.Getter(_editContext.Entity)) ?? _property.Getter(_editContext.Entity);
            if (data is IEnumerable<IEntity> entityCollection)
            {
                _relatedIds = entityCollection.Select(x => (object)x.Id!).ToList();
            }
            else if (data is IEnumerable<object> objectCollection)
            {
                _relatedIds = objectCollection.ToList();
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

            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<IElement>> GetAvailableElementsAsync(IQuery query)
        {
            if (_editContext == null)
            {
                return new List<IElement>();
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

            query.CollectionAlias = _editContext.CollectionAlias;

            var entities = await _repository.GetAllAsync(new ViewContext(_editContext.CollectionAlias, parent), query);

            return entities
               .Select(entity => (IElement)new Element
               {
                   Id = _setup.IdProperty.Getter(entity),
                   Labels = _setup.DisplayProperties.Select(x => x.StringGetter(entity)).ToList()
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

        public Type GetRelatedEntityType() => _setup.RelatedEntityType ?? typeof(object);

        public void Dispose() => _eventHandle?.Dispose();
    }
}
