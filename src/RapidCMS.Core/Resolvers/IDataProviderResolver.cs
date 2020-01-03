using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Providers;

namespace RapidCMS.Core.Resolvers
{
    internal interface IDataProviderResolver
    {
        DataProvider? GetDataProvider(FieldSetup field);
    }
}
