using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Attributes;
using RapidCMS.ModelMaker.Abstractions.Detail;

namespace RapidCMS.ModelMaker.Models.Entities
{
    public class PropertyDetailModel : IEntity
    {
        public string? Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Alias { get; set; } = default!;

        [Required]
        [ValidateObject]
        public IDetailConfig? Config { get; set; }
    }

    public class PropertyDetailModel<TValidatorConfig> : PropertyDetailModel, IPropertyDetailModel<TValidatorConfig>
        where TValidatorConfig : class, IDetailConfig, new()
    {
        TValidatorConfig IPropertyDetailModel<TValidatorConfig>.Config => Config as TValidatorConfig ?? new TValidatorConfig();
    }
}
