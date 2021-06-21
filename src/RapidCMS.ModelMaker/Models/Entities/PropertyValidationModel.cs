using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Attributes;
using RapidCMS.Core.Forms.Validation;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;

namespace RapidCMS.ModelMaker.Models.Entities
{
    public class PropertyValidationModel : IEntity
    {
        public string? Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Alias { get; set; } = default!;

        public string? AttributeExpression { get; set; }

        public string? DataCollectionExpression { get; set; }

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
