using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Abstractions.Detail;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class MaxDateTimeDeltaDetailConfig : IDetailConfig
    {
        public int? Delta { get; set; }

        public bool IsEnabled => Delta.HasValue;

        public bool AlwaysIncluded => false;

        public bool IsApplicable(PropertyModel model) => model.EditorAlias.In(Constants.Editors.Date);

        public string? RelatedCollectionAlias => default;

        public string? ValidationMethodName => "MaxDateTimeDelta";

        public string? DataCollectionType => default;
    }
}
