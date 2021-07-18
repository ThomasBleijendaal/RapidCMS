using System;
using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Abstractions.Detail;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class MinDateTimeDetailConfig : IDetailConfig
    {
        public DateTime? DateTime { get; set; }

        public bool IsEnabled => DateTime.HasValue;

        public bool AlwaysIncluded => false;

        public bool IsApplicable(PropertyModel model) => model.EditorAlias.In(Constants.Editors.Date);

        public string? RelatedCollectionAlias => default;

        public string? ValidationMethodName => "GreaterThanOrEqualTo";

        public string? DataCollectionType => default;
    }
}
