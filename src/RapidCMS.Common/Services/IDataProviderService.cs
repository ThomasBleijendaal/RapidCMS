using RapidCMS.Common.Data;
using RapidCMS.Common.Models;

namespace RapidCMS.Common.Services
{
    public interface IDataProviderService
    {
        DataProvider? GetDataProvider(Field field);
    }
}
