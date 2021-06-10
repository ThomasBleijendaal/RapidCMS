namespace RapidCMS.ModelMaker.Core.Abstractions.Validation
{
    public interface IPropertyValidationModel<TValidatorConfig>
        where TValidatorConfig : IValidatorConfig
    {
        TValidatorConfig Config { get; }
    }
}
