using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Providers;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    internal interface IDataProviderResolver
    {
        Task<FormDataProvider?> GetDataProviderAsync(IFieldSetup field);
    }
}
