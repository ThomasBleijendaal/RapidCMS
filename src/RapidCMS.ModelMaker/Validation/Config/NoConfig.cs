using RapidCMS.ModelMaker.Core.Abstractions.Validation;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class NoConfig : IValidatorConfig
    {
        public bool IsEnabled => true;

        public bool AlwaysIncluded => true;

        public bool IsApplicable(PropertyModel model) => false;

        public string? RelatedCollectionAlias => default;

        public string? ValidationMethodName => default;

        public string? DataCollectionExpression => default;
    }
}
