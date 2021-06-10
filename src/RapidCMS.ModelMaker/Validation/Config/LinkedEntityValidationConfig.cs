using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class LinkedEntityValidationConfig : IValidatorConfig
    {
        [Required]
        public string CollectionAlias { get; set; } = string.Empty;

        public bool IsEnabled => !string.IsNullOrWhiteSpace(CollectionAlias);
        public bool AlwaysIncluded => false;

        public bool IsApplicable(PropertyModel model)
            => model.EditorAlias.In(Constants.Editors.EntityPicker, Constants.Editors.EntitiesPicker);
    }
}
