using System;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Abstractions.Validation
{
    public interface IValidatorConfig
    {
        bool IsEnabled { get; }
        bool IsApplicable(PropertyModel model);
    }
}
