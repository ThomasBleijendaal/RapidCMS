using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.DTOs;

namespace RapidCMS.Common.Data
{
    internal class CollectionDataProvider : IDataCollection
    {
        private IRepository _repository;
        private IPropertyMetadata _idProperty;
        private IExpressionMetadata _labelProperty;

        private Task _init;

        private List<IElement> _elements;
        private ICollection<object> _releatedElements;

        public void SetElementMetadata(IRepository repository, IPropertyMetadata idProperty, IExpressionMetadata labelProperty)
        {
            _repository = repository;
            _idProperty = idProperty;
            _labelProperty = labelProperty;

            _init = InitializeAsync();
        }

        public void SetRelationMetadata(IEntity entity, IPropertyMetadata collectionProperty)
        {
            var data = collectionProperty.Getter(entity);

            _releatedElements = data as ICollection<object>;
        }

        public async Task InitializeAsync()
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

        public async Task AddElementAsync(IElement option)
        {
            await _init;

            _elements.Add(option);
        }

        public Task<IEnumerable<IElement>> GetAvailableElementsAsync()
        {
            return Task.FromResult(_elements.AsEnumerable());
        }

        public async Task<IEnumerable<IElement>> GetRelatedElementsAsync()
        {
            await _init;

            throw new System.NotImplementedException();
        }


        public async Task RemoveElementAsync(IElement option)
        {
            await _init;

            throw new System.NotImplementedException();
        }

        public async Task SetElementAsync(IElement option)
        {
            await _init;

            throw new System.NotImplementedException();
        }

        
    }
}
