using RapidCMS.Common.Data;
using RapidCMS.Common.Models;

namespace RapidCMS.Common.Services
{
    internal interface IDataProviderService
    {
        DataProvider? GetDataProvider(Field field);
    }
}
