using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class MaxLengthDetailConfig : IDetailConfig
    {
        [Range(1, int.MaxValue)]
        public int? MaxLength { get; set; }

        public bool IsEnabled => MaxLength.HasValue;
        public bool AlwaysIncluded => false;

        public bool IsApplicable(PropertyModel model)
            => model.EditorAlias.In(Constants.Editors.TextArea, Constants.Editors.TextBox);

        public string? RelatedCollectionAlias => default;

        public string? ValidationMethodName => "MaximumLength";

        public string? DataCollectionType => default;
    }
}
