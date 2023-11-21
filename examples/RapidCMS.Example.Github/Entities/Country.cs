using System;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Example.Github.Entities;

internal class Country : IEntity, ICloneable
{
    public string? Id { get; set; }
    public string? Name { get; set; }

    public object Clone()
    {
        return new Country
        {
            Id = Id,
            Name = Name
        };
    }
}
