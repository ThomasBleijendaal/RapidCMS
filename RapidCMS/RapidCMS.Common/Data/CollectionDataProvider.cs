using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.DTOs;

namespace RapidCMS.Common.Data
{
    internal class CollectionDataProvider : IDataCollection
    {
        private readonly IRepository _repository;
        private readonly IPropertyMetadata _idProperty;
        private readonly IExpressionMetadata _labelProperty;

        private readonly Task _init;

        private List<IElement> _elements;

        public CollectionDataProvider(IRepository repository, IPropertyMetadata idProperty, IExpressionMetadata labelProperty)
        {
            _repository = repository;
            _idProperty = idProperty;
            _labelProperty = labelProperty;

            _init = InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            var entities = await _repository._GetAllAsObjectsAsync(null);

            _elements = entities.Select(entity => new ElementDTO
            {
                Id = _idProperty.Getter(entity),
                Label = _labelProperty.StringGetter(entity)
            } as IElement).ToList();
        }

        public async Task AddElementAsync(IElement option)
        {
            await _init;

            _elements.Add(option);
        }

        public Task<IEnumerable<IElement>> GetAvailableElementsAsync()
        {
            return Task.FromResult(_elements.AsEnumerable<IElement>());
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
