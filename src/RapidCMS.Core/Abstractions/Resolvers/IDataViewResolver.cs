using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    internal interface IDataViewResolver
    {
        Task<IEnumerable<IDataView>> GetDataViewsAsync(ICollectionSetup collection);
        Task ApplyDataViewToQueryAsync(IQuery query, ICollectionSetup collection);
    }
}
