using System.Threading.Tasks;

namespace RapidCMS.ModelMaker.Abstractions.Validation
{
    public interface IValidator
    {
        Task<bool> IsValidAsync(object? value, IValidatorConfig validatorConfig);
        Task<string> ErrorMessageAsync(IValidatorConfig validatorConfig);
    }
}
