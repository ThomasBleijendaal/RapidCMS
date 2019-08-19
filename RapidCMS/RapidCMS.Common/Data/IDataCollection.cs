using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RapidCMS.Common.Data
{
    // TODO: paginate etc
    public interface IDataCollection
    {
        Task SetEntityAsync(IEntity entity);
        Task<IEnumerable<IElement>> GetAvailableElementsAsync();

        event EventHandler OnDataChange;
    }
}
