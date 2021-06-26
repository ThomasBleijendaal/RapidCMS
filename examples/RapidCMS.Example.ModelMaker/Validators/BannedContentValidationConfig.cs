using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.Example.ModelMaker.Validators
{
    public class BannedContentValidationConfig : IValidatorConfig
    {
        public List<string> BannedWords { get; set; } = new List<string>();

        public bool IsEnabled => BannedWords.Any();

        public bool AlwaysIncluded => false;

        public bool IsApplicable(PropertyModel model)
            => model.EditorAlias.In(Constants.Editors.TextBox, Constants.Editors.TextArea);

        public string? RelatedCollectionAlias => default;

        public string? DataCollectionExpression => default;

        public string? ValidationMethodName => "BannedContent";

        public string? ValidationAttributeExpression => $"[RegularExpression(\"[^{string.Join("|", BannedWords)}]\")]";
    }

    public static class BannedContentValidator
    {
        public static IRuleBuilderOptions<T, string> BannedContent<T>(this IRuleBuilder<T, string> ruleBuilder, BannedContentValidationConfig config)
        {
            return ruleBuilder
                .Must(value => !config.BannedWords.Any(value.Contains))
                // this error message will be quite offecive if all banned words are displayed like this
                .WithMessage($"The value may not contain these words: {string.Join(",", config.BannedWords)}.");
        }
    }
}
