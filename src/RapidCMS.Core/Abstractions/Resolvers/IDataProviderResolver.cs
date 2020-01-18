using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Providers;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    internal interface IDataProviderResolver
    {
        DataProvider? GetDataProvider(FieldSetup field);
    }
}
