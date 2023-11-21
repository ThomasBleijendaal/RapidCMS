using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Data;
using RapidCMS.Core.Models.Data;
using RapidCMS.Example.Github.Entities;

namespace RapidCMS.Example.Github.DataViewBuilders;

internal class CountryDataViewBuilder : DataViewBuilder<Country>
{   
    public override Task<IEnumerable<DataView<Country>>> GetDataViewsAsync()
    {
        return Task.FromResult(Enumerable.Range(1, 10).Select(index => new DataView<Country>(index, $"{index}-letter country", x => x.Name != null && x.Name.Length == index)));
    }
}
