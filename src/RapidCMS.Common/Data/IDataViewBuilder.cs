using System.Collections.Generic;
using System.Threading.Tasks;

namespace RapidCMS.Common.Data
{
    public interface IDataViewBuilder
    {
        Task<IEnumerable<IDataView>> GetDataViewsAsync();
    }
}
