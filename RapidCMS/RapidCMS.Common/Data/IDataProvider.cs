using System.Collections.Generic;
using System.Threading.Tasks;

namespace RapidCMS.Common.Data
{
    // TODO: paginate etc
    public interface IDataCollection
    {
        Task<IEnumerable<IElement>> GetAvailableElementsAsync();

        Task<IEnumerable<IElement>> GetRelatedElementsAsync();

        Task AddElementAsync(IElement option);
        Task SetElementAsync(IElement option);
        Task RemoveElementAsync(IElement option);

        // TODO: initialize function
    }
}
