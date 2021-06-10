using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Forms.Validation;
using RapidCMS.ModelMaker.Abstractions.Entities;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;

namespace RapidCMS.ModelMaker.Models.Entities
{
    public class PropertyValidationModel : IModelMakerEntity
    {
        public string? Id { get; set; }

        public string Alias { get; set; } = default!;

        public string? Attribute { get; set; }

        [Required]
        [ValidateObject]
        public IValidatorConfig? Config { get; set; }
    }

    public class PropertyValidationModel<TValidatorConfig> : PropertyValidationModel, IPropertyValidationModel<TValidatorConfig>
        where TValidatorConfig : class, IValidatorConfig, new()
    {
        TValidatorConfig IPropertyValidationModel<TValidatorConfig>.Config => Config as TValidatorConfig ?? new TValidatorConfig();
    }
}
