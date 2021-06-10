using System;
using RapidCMS.ModelMaker.Validation.Base;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.Validation
{
    public class EnumOptionsValidator<TEnum> : BaseValidator<TEnum, NoConfig>
        where TEnum : Enum
    {
        protected override string? ValidationAttributeText(NoConfig validatorConfig) 
            =>$"[EnumDataType(typeof({typeof(TEnum).FullName}))]";
    }
}
