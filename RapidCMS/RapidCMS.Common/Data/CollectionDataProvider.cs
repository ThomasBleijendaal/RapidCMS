using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models.DTOs;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Data
{

    internal class CollectionDataProvider : IRelationDataCollection
    {
        private IRepository? _repository;
        private Type? _relatedEntityType;
        private IPropertyMetadata? _repositoryParentIdProperty;
        private IPropertyMetadata? _idProperty;
        private IExpressionMetadata? _labelProperty;

        private List<IElement>? _elements;
        private List<IElement>? _relatedElements;
        private ICollection<object>? _relatedIds;

        public void SetElementMetadata(IRepository repository, Type relatedEntityType, IPropertyMetadata? repositoryParentIdProperty, IPropertyMetadata idProperty, IExpressionMetadata labelProperty)
        {
            _repository = repository;
            _relatedEntityType = relatedEntityType;
            _repositoryParentIdProperty = repositoryParentIdProperty;
            _idProperty = idProperty;
            _labelProperty = labelProperty;
        }

        public async Task SetEntityAsync(IEntity entity)
        {
            if (_repository != null && _idProperty != null && _labelProperty != null)
            {
                var parentId = _repositoryParentIdProperty?.Getter.Invoke(entity) as string;
                var entities = await _repository._GetAllAsObjectsAsync(parentId);

                _elements = entities
                    .Select(entity => (IElement)new ElementDTO
                    {
                        Id = _idProperty.Getter(entity),
                        Label = _labelProperty.StringGetter(entity)
                    })
                    .ToList();
            }
        }

        public async Task SetRelationMetadataAsync(IEntity entity, IPropertyMetadata collectionProperty)
        {
            var data = collectionProperty.Getter(entity);

            await SetEntityAsync(entity);

            if (data is ICollection<IEntity> entityCollection)
            {
                _relatedIds = entityCollection.Select(x => (object)x.Id).ToList();
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
                    list.Add(element);
                }
                _relatedIds = list;
            }
            else
            {
                throw new InvalidOperationException("Failed to convert relation property to ICollection<object>");
            }

            UpdateRelatedElements();
        }

        private void UpdateRelatedElements()
        {
            _relatedElements = _elements?.Where(x => _relatedIds?.Contains(x.Id) ?? false).ToList();
        }

        public Task<IEnumerable<IElement>> GetAvailableElementsAsync()
        {
            return Task.FromResult(_elements ?? Enumerable.Empty<IElement>());
        }

        public Task<IEnumerable<IElement>> GetRelatedElementsAsync()
        {
            if (_relatedIds != null && _elements != null)
            {
                return Task.FromResult(_elements.Where(x => _relatedIds.Contains(x.Id)));
            }
            else
            {
                return Task.FromResult(Enumerable.Empty<IElement>());
            }
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

        public IEnumerable<IElement> GetCurrentRelatedElements()
        {
            return _relatedElements ?? Enumerable.Empty<IElement>();
        }

        public Type GetRelatedEntityType()
        {
            return _relatedEntityType ?? typeof(object);
        }
    }
}
