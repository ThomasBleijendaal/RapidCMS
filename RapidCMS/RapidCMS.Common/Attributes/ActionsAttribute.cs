using RapidCMS.Common.Enums;
using System;

namespace RapidCMS.Common.Attributes
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
