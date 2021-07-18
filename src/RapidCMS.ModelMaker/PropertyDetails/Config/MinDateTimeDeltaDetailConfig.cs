using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Abstractions.Detail;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class MinDateTimeDeltaDetailConfig : IDetailConfig
    {
        public int? Delta { get; set; }

        public bool IsEnabled => Delta.HasValue;

        public bool AlwaysIncluded => false;

        public bool IsApplicable(PropertyModel model) => model.EditorAlias.In(Constants.Editors.Date);

        public string? RelatedCollectionAlias => default;

        public string? ValidationMethodName => "MinDateTimeDelta";

        public string? DataCollectionType => default;
    }
}
