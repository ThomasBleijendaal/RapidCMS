using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.DTOs;

namespace RapidCMS.Common.Data
{
    internal class CollectionDataProvider : IDataProvider
    {
        private readonly IRepository _repository;
        private readonly IPropertyMetadata _idProperty;
        private readonly IExpressionMetadata _labelProperty;

        public CollectionDataProvider(IRepository repository, IPropertyMetadata idProperty, IExpressionMetadata labelProperty)
        {
            _repository = repository;
            _idProperty = idProperty;
            _labelProperty = labelProperty;
        }

        public async Task<IEnumerable<IOption>> GetAllOptionsAsync()
        {
            // TODO: parent id?
            var entities = await _repository._GetAllAsObjectsAsync(null);

            return entities.Select(entity => new OptionDTO
            {
                Id = _idProperty.Getter(entity).ToString(),
                Label = _labelProperty.Getter(entity).ToString()
            });
        }
    }
}
