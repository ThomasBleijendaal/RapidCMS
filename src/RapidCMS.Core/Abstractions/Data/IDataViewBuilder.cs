using System.Collections.Generic;
using System.Threading.Tasks;

namespace RapidCMS.Core.Abstractions.Data
{
    public interface IDataViewBuilder
    {
        Task<IEnumerable<IDataView>> GetDataViewsAsync();
    }
}
