using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RapidCMS.Core.Data;
using RapidCMS.Core.Models.Data;
using RapidCMS.Example.Shared.Data;

namespace RapidCMS.Example.Shared.DataViews
{
    // a DataViewBuilder allows you to create multiple dataviews - views in the same list editor / list views that
    // allow the user to more easily find entities
    public class DatabaseEntityDataViewBuilder : DataViewBuilder<DatabaseEntity>
    {
        public override Task<IEnumerable<DataView<DatabaseEntity>>> GetDataViewsAsync()
        {
            var list = new List<DataView<DatabaseEntity>>
            {
                // when the user selects this dataview (by clicking on the corresponding tab)
                // the expression is given to the repository, so it can use that expression to perform the correct query
                new DataView<DatabaseEntity>(1, "[A-K]", x => Regex.IsMatch(x.Name, "^[A-K]")),
                new DataView<DatabaseEntity>(2, "[L-Z]", x => Regex.IsMatch(x.Name, "^[L-Z]")),
                new DataView<DatabaseEntity>(3, "[other]", x => Regex.IsMatch(x.Name, "^[^A-Z]"))
            };

            return Task.FromResult(list.AsEnumerable());
        }
    }
}
