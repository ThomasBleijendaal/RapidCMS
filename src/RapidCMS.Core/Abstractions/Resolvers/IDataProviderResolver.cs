using System.Threading.Tasks;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Providers;

namespace RapidCMS.Core.Abstractions.Resolvers;

internal interface IDataProviderResolver
{
    Task<FormDataProvider?> GetDataProviderAsync(FieldSetup field);
}
