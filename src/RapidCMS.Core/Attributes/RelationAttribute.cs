using RapidCMS.Core.Enums;
using System;

namespace RapidCMS.Core.Attributes;

public class RelationAttribute : Attribute
{
    public RelationAttribute(RelationType type)
    {
        Type = type;
    }

    public RelationType Type { get; }
}
