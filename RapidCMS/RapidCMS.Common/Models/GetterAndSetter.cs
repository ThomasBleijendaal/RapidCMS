using System;
using System.Collections.Generic;
using System.Text;

namespace RapidCMS.Common.Models
{
    internal class GetterAndSetter
    {
        internal string PropertyName { get; set; }
        internal Func<object, object> Getter { get; set; }
        internal Action<object, object> Setter { get; set; }
    }
}
