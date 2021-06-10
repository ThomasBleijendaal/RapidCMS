using System.Threading.Tasks;

namespace RapidCMS.ModelMaker.Core.Abstractions.Validation
{
    // TODO: Rename or merge with IValidatorConfig
    public interface IValidator
    {
        string? ValidationAttributeText(IValidatorConfig validatorConfig);
    }
}
