using RapidCMS.Core.Extensions;

namespace FluentValidation
{
    public static class UrlFriendlyValidator
    {
        public static IRuleBuilderOptions<T, string> UrlFriendly<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Must(value => value == value?.ToUrlFriendlyString())
                .WithMessage("Value can only contain lowercase letters, numbers and dashes.");
        }
    }
}
