using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Abstractions.Detail;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class LimitedOptionsDetailConfig : IDetailConfig
    {
        [Required]
        [MinLength(1)]
        public List<string> Options { get; set; } = new List<string>();

        public bool IsEnabled => Options?.Any() == true;
        public bool AlwaysIncluded => false;

        public bool IsApplicable(PropertyModel model)
            => model.EditorAlias.In(Constants.Editors.Dropdown);

        public string? RelatedCollectionAlias => default; // TODO: wut: $"[RegularExpression(\"^[{string.Join("|", Options)}]\")]$";

        public string? ValidationMethodName => default;

        public string? DataCollectionType => default; // TODO
    }
}
