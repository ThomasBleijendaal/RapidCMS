using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RapidCMS.Common.Data
{
    public abstract class DataViewBuilder<TEntity> : IDataViewBuilder
        where TEntity : IEntity
    {
        /// <summary>
        /// This method is called when the list view / editor want to get all data views for the list.
        /// </summary>
        /// <returns></returns>
        public abstract Task<IEnumerable<DataView<TEntity>>> GetDataViewsAsync();

        async Task<IEnumerable<IDataView>> IDataViewBuilder.GetDataViewsAsync()
        {
            var data = await GetDataViewsAsync();
            return data.Cast<IDataView>();
        }
    }
}
