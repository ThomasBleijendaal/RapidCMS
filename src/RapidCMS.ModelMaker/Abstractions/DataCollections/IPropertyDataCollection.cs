using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.ModelMaker.Abstractions.Validation;

namespace RapidCMS.ModelMaker.Abstractions.DataCollections
{
    public interface IPropertyDataCollection : IDataCollection
    {
        Task SetConfigAsync(IValidatorConfig? config);
    }
}
