using System;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Attributes;

namespace RapidCMS.Example.Shared.Data;

public class Counter : IEntity, ICloneable
{
    public string? Id { get; set; }

    [Field(Name = "Current count", ListName = "Current count")]
    public int CurrentCount { get; set; }

    public object Clone()
    {
        return new Counter()
        {
            CurrentCount = CurrentCount,
            Id = Id
        };
    }
}
