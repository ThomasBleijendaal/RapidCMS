using RapidCMS.Core.Enums;
using System;

namespace RapidCMS.Core.Attributes
{
    internal class ActionsAttribute : Attribute
    {
        public ActionsAttribute(params UsageType[] usages)
        {
            Usages = usages;
        }

        public UsageType[] Usages { get; private set; }
    }
}
