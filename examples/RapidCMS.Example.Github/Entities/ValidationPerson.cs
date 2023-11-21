using System;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Example.Github.Attributes;

namespace RapidCMS.Example.Github.Entities;

internal class ValidationPerson : IEntity, ICloneable
{
    public string? Id { get; set; }

    [Required]
    [MinLength(10)]
    public string? Name { get; set; }

    [EmailAddress]
    [Required]
    public string? Email { get; set; }

    [Required]
    [BioValidation]
    public string? Bio { get; set; }

    public object Clone()
    {
        return new ValidationPerson
        {
            Bio = Bio,
            Email = Email,
            Id = Id,
            Name = Name
        };
    }
}
