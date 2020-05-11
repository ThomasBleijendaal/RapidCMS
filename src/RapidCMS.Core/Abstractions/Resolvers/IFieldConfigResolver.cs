using System;
using System.Collections.Generic;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    internal interface IFieldConfigResolver
    {
        IEnumerable<FieldConfig> GetFields(Type subject, Features features);
    }
}
