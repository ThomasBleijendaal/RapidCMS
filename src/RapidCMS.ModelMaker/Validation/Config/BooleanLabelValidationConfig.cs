using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Attributes;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms.Validation;
using RapidCMS.ModelMaker.Abstractions.Validation;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.UI.Components.Editors;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class BooleanLabelValidationConfig : IValidatorConfig
    {
        [Required]
        [ValidateObjectAsProperty]
        public LabelsConfig Labels { get; set; } = new LabelsConfig();

        public bool IsEnabled => true;

        public bool IsApplicable(PropertyModel model)
            => model.EditorAlias.In(Constants.Editors.Dropdown, Constants.Editors.Select);

        public class LabelsConfig: IEntity
        {
            [Required]
            [MinLength(1)]
            [Field(Name = "True label", EditorType = typeof(TextBoxEditor))]
            public string? TrueLabel { get; set; }

            [Required]
            [MinLength(1)]
            [Field(Name = "False label", EditorType = typeof(TextBoxEditor))]
            public string? FalseLabel { get; set; }

            string? IEntity.Id { get; set; }
        }
    }
}
