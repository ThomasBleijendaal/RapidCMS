using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Data
{
    public abstract class DataViewBuilder<TDatabaseEntity> : IDataViewBuilder
    {
        public abstract Task<IEnumerable<DataView<TDatabaseEntity>>> GetDataViewsAsync();

        async Task<IEnumerable<IDataView>> IDataViewBuilder.GetDataViewsAsync()
        {
            var elements = await GetDataViewsAsync().ConfigureAwait(false);
            return elements.AsEnumerable<IDataView>();
        }
    }
}
