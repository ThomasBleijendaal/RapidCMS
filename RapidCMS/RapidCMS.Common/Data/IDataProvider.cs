using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Common.Models;

namespace RapidCMS.Common.Data
{
    // TODO: paginate etc
    public interface IDataCollection
    {
        void SetElementMetadata(IRepository repository, IPropertyMetadata idProperty, IExpressionMetadata labelProperty);
        void SetRelationMetadata(IEntity entity, IPropertyMetadata collectionProperty);

        Task<IEnumerable<IElement>> GetAvailableElementsAsync();

        Task<IEnumerable<IElement>> GetRelatedElementsAsync();

        Task AddElementAsync(IElement option);
        Task SetElementAsync(IElement option);
        Task RemoveElementAsync(IElement option);

        // TODO: initialize function
    }
}
