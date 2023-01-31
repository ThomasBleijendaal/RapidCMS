using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    public interface IDataViewResolver
    {
        Task<IEnumerable<IDataView>> GetDataViewsAsync(string collectionAlias);
        Task ApplyDataViewToViewAsync(IView view);
    }
}
