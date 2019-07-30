using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RapidCMS.Common.Data;
using TestLibrary.Data;
using TestLibrary.Entities;

namespace TestLibrary.DataViewBuilders
{
    public class CountryDataViewBuilder : DataViewBuilder<CountryEntity>
    {
        private readonly TestDbContext _dbContext;

        public CountryDataViewBuilder(TestDbContext dbContext )
        {
            _dbContext = dbContext;
        }

        public override async Task<IEnumerable<DataView<CountryEntity>>> GetDataViewsAsync()
        {
            var dbData = await _dbContext.Countries.ToListAsync();
            
            var data = dbData.GroupBy(x => x.Name.Length).OrderBy(x => x.Key).ToList();

            return data
                .Select(x => new DataView<CountryEntity>(
                    x.Key, 
                    $"{x.Key}-letter countries", 
                    y => y.Name.Length == x.Key))
                .ToList();
        }
    }
}
