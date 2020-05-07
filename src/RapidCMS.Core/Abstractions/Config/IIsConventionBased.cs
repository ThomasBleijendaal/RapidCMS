using System;
using System.ComponentModel;

namespace RapidCMS.Core.Abstractions.Config
{
    internal interface IIsConventionBased
    {
        [Obsolete("Create IConventionBasedConfigResolver to generate this")]
        T GenerateConfig<T>() where T: class;
    }
}
