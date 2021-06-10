using System.Threading.Tasks;

namespace RapidCMS.ModelMaker.Core.Abstractions.Validation
{
    public interface IValidator
    {
        Task<bool> IsValidAsync(object value, IValidatorConfig validatorConfig);
        Task<string> ErrorMessageAsync(IValidatorConfig validatorConfig);
    }
}
