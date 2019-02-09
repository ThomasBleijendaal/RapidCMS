using System;
using System.Collections.Generic;
using System.Text;

namespace RapidCMS.Common.Attributes
{
    internal class DefaultTypeAttribute : Attribute
    {
        public IEnumerable<Type> Types { get; private set; }

        public DefaultTypeAttribute(params Type[] types)
        {
            Types = types;
        }
    }
}
