using System.Collections.Generic;
using System.Linq;
using RapidCMS.ModelMaker.Abstractions.Validation;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class LimitedOptionsValidationConfig : IValidatorConfig
    {
        public List<string> Options { get; set; } = new List<string>();

        public bool IsEnabled => Options?.Any() == true;
    }
}
