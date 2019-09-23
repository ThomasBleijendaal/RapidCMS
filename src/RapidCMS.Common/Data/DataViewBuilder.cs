using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RapidCMS.Common.Data
{
    public abstract class DataViewBuilder<TEntity> : IDataViewBuilder
        where TEntity : IEntity
    {
        public abstract Task<IEnumerable<DataView<TEntity>>> GetDataViewsAsync();

        async Task<IEnumerable<IDataView>> IDataViewBuilder.GetDataViewsAsync()
        {
            var data = await GetDataViewsAsync();
            return data.Cast<IDataView>();
        }
    }
}
