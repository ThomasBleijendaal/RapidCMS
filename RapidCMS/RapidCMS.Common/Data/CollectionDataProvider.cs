using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.DTOs;

#nullable enable

namespace RapidCMS.Common.Data
{
    internal class CollectionDataProvider : IRelationDataCollection
    {
        private IRepository? _repository;
        private IPropertyMetadata? _idProperty;
        private IExpressionMetadata? _labelProperty;

        private Task _init = Task.CompletedTask;

        private List<IElement>? _elements;
        private List<IElement>? _relatedElements;
        private ICollection<object>? _relatedIds;

        public void SetElementMetadata(IRepository repository, IPropertyMetadata idProperty, IExpressionMetadata labelProperty)
        {
            _repository = repository;
            _idProperty = idProperty;
            _labelProperty = labelProperty;

            _init = InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            if (_repository != null && _idProperty != null && _labelProperty != null)
            {
                var entities = await _repository._GetAllAsObjectsAsync(null);

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
            await _init;

            var data = collectionProperty.Getter(entity);

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

            _relatedElements = _elements?.Where(x => _relatedIds.Contains(x.Id)).ToList();
        }

        public async Task<IEnumerable<IElement>> GetAvailableElementsAsync()
        {
            await _init;

            return _elements ?? Enumerable.Empty<IElement>();
        }

        public async Task<IEnumerable<IElement>> GetRelatedElementsAsync()
        {
            await _init;

            if (_relatedIds != null)
            {
                return _elements.Where(x => _relatedIds.Contains(x.Id));
            }
            else
            {
                return Enumerable.Empty<IElement>();
            }
        }

        public async Task AddElementAsync(IElement option)
        {
            await _init;

            _relatedElements?.Add(_elements.First(x => x.Id == option.Id));
        }
        public async Task RemoveElementAsync(IElement option)
        {
            await _init;

            _relatedElements?.Add(_elements.First(x => x.Id == option.Id));
        }

        public async Task SetElementAsync(IElement option)
        {
            await _init;

            _relatedElements?.Clear();
            _relatedElements?.Add(_elements.First(x => x.Id == option.Id));
        }

        public IEnumerable<IElement> GetCurrentRelatedElements()
        {
            return _relatedElements ?? Enumerable.Empty<IElement>();
        }
    }
}
