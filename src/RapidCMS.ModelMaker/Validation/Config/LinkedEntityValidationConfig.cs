using RapidCMS.ModelMaker.Abstractions.Validation;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class LinkedEntityValidationConfig : IValidatorConfig
    {
        public string CollectionAlias { get; set; } = string.Empty;

        public bool IsEnabled => !string.IsNullOrWhiteSpace(CollectionAlias);
    }
}
