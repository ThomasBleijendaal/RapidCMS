using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    internal interface IDataViewResolver
    {
        Task<IEnumerable<IDataView>> GetDataViewsAsync(string collectionAlias);
        Task ApplyDataViewToQueryAsync(IQuery query);
    }
}
