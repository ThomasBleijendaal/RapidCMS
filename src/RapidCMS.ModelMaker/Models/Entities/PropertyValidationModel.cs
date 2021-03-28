using RapidCMS.Core.Abstractions.Data;
using RapidCMS.ModelMaker.Abstractions.Validation;

namespace RapidCMS.ModelMaker.Models.Entities
{
    internal class PropertyValidationModel : IEntity
    {
        public string? Id { get; set; }

        public string? Alias { get; set; }
        public IValidatorConfig? Config { get; set; }
    }

    internal class PropertyValidationModel<TValidatorConfig> : PropertyValidationModel, IPropertyValidationModel<TValidatorConfig>
        where TValidatorConfig : class, IValidatorConfig, new()
    {
        TValidatorConfig IPropertyValidationModel<TValidatorConfig>.Config => Config as TValidatorConfig ?? new TValidatorConfig();
    }
}
