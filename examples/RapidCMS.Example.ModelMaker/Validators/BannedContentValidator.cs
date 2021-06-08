using System;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.ModelMaker.Validation.Base;

namespace RapidCMS.Example.ModelMaker.Validators
{
    public class BannedContentValidator : BaseValidator<string, BannedContentValidationConfig>
    {
        protected override Task<string> ErrorMessage(BannedContentValidationConfig validatorConfig)
            // this error message will be quite offensive.
            => Task.FromResult($"This field is not allowed to contain: {string.Join(", ", validatorConfig.BannedWords)}.");

        protected override Task<bool> IsValid(string? value, BannedContentValidationConfig validatorConfig)
            => Task.FromResult(
                string.IsNullOrWhiteSpace(value) || 
                !validatorConfig.BannedWords.Any(word => value.Contains(word, StringComparison.InvariantCultureIgnoreCase)));
    }
}
