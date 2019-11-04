using RapidCMS.Common.Enums;
using System;

namespace RapidCMS.Common.Attributes
{
    internal class RelationAttribute : Attribute
    {
        public RelationAttribute(RelationType type)
        {
            Type = type;
        }

        public RelationType Type { get; }
    }
}
