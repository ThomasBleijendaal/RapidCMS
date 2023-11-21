using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Example.Github.Entities;

internal class Relatable2Person : IEntity, ICloneable
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Bio { get; set; }

    public List<Country> Countries { get; } = new List<Country>();

    public object Clone()
    {
        return new Relatable2Person
        {
            Bio = Bio,
            Email = Email,
            Id = Id,
            Name = Name
        };
    }
}
