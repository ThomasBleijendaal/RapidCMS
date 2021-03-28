using System.Threading.Tasks;

namespace RapidCMS.ModelMaker.Abstractions.Validation
{
    public interface IValidator
    {
        Task<bool> IsValid(object? value, IValidatorConfig validatorConfig);
        Task<string> ErrorMessage(IValidatorConfig validatorConfig);
    }
}
