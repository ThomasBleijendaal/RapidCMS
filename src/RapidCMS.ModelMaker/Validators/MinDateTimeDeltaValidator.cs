using System;

namespace FluentValidation
{
    public static class MinDateTimeDeltaValidator
    {
        public static IRuleBuilderOptions<T, DateTime> MinDateTimeDelta<T>(this IRuleBuilder<T, DateTime> ruleBuilder, int delta)
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(x => DateTime.UtcNow.Date.AddDays(-delta));
        }
    }
}
