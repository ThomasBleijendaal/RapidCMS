using RapidCMS.Common.Enums;
using System;

namespace RapidCMS.Common.Attributes
{
    // TODO: why needed?
    // TODO: investigate whether this attribute can be moved to somewhere a custom editor can also use this (perhaps on the base class of an editor)
    internal class RelationAttribute : Attribute
    {
        public RelationAttribute(RelationType type)
        {
            Type = type;
        }

        public RelationType Type { get; }
    }
}
