using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Resolvers.Data
{
    internal class ApiDataViewResolver : IDataViewResolver
    {
        public Task ApplyDataViewToQueryAsync(IQuery query, ICollectionSetup collection)
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<IDataView>> GetDataViewsAsync(ICollectionSetup collection)
        {
            return Task.FromResult(Enumerable.Empty<IDataView>());
        }
    }
}
