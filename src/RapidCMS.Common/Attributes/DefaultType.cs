using System;
using System.Collections.Generic;
using System.Text;

namespace RapidCMS.Common.Attributes
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
