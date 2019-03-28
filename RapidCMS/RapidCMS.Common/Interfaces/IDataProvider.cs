using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.DTOs;

namespace RapidCMS.Common.Interfaces
{
    // TODO: paginate etc
    public interface IDataProvider
    {
        Task<IEnumerable<IOption>> GetAllOptionsAsync();
    }

    public interface IOption
    {
        string Id { get; }
        
        // TODO: make label columnable
        string Label { get; }
    }

    public class CollectionDataProvider : IDataProvider
    {
        private readonly IRepository _repository;
        private readonly PropertyMetadata _idProperty;
        private readonly PropertyMetadata _labelProperty;

        public CollectionDataProvider(IRepository repository, PropertyMetadata idProperty, PropertyMetadata labelProperty)
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
