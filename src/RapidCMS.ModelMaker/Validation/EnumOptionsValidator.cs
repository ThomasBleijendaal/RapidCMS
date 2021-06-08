using System;
using System.Threading.Tasks;
using RapidCMS.ModelMaker.Validation.Base;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.Validation
{
    public class EnumOptionsValidator<TEnum> : BaseValidator<TEnum, NoConfig>
        where TEnum : Enum
    {
        protected override Task<string> ErrorMessage(NoConfig validatorConfig) => Task.FromResult("");

        protected override Task<bool> IsValid(TEnum? value, NoConfig validatorConfig) => Task.FromResult(true);
    }
}
