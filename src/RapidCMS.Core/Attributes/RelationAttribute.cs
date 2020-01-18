using RapidCMS.Core.Enums;
using System;

namespace RapidCMS.Core.Attributes
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
