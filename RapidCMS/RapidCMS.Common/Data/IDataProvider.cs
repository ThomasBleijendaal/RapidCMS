using System.Collections.Generic;
using System.Threading.Tasks;

namespace RapidCMS.Common.Data
{
    // TODO: paginate etc
    public interface IDataProvider
    {
        Task<IEnumerable<IOption>> GetAllOptionsAsync();
    }
}
