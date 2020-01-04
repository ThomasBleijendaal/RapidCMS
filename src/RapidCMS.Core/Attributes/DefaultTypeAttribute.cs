using System;
using System.Collections.Generic;

namespace RapidCMS.Core.Attributes
{
    internal class DefaultTypeAttribute : Attribute
    {
        public DefaultTypeAttribute(params Type[] types)
        {
            Types = types;
        }

        public IEnumerable<Type> Types { get; private set; }
    }
}
