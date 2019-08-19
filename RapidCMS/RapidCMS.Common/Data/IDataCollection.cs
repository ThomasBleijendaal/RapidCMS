using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RapidCMS.Common.Data
{
    // TODO: paginate etc
    // TODO: clean API bit up (SetEntityAsync and SetRelationMetadataAsync do almost the same)
    public interface IDataCollection
    {
        Task SetEntityAsync(IEntity entity);
        Task<IEnumerable<IElement>> GetAvailableElementsAsync();

        event EventHandler OnDataChange;
    }
}
