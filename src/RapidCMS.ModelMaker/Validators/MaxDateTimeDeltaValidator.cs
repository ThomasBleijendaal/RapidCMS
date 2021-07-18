using System;

namespace FluentValidation
{
    public static class MaxDateTimeDeltaValidator
    {
        public static IRuleBuilderOptions<T, DateTime> MaxDateTimeDelta<T>(this IRuleBuilder<T, DateTime> ruleBuilder, int delta)
        {
            return ruleBuilder
                .LessThanOrEqualTo(x => DateTime.UtcNow.Date.AddDays(delta));
        }
    }
}
