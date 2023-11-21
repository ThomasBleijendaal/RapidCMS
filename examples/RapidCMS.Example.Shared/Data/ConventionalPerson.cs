using System;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Attributes;
using RapidCMS.Core.Enums;
using RapidCMS.UI.Components.Editors;

namespace RapidCMS.Example.Shared.Data;

public class ConventionalPerson : IEntity, ICloneable
{
    public int Id { get; set; }

    [Field(Name = "Email", Description = "The email of the person", ListName = "Email", OrderByType = OrderByType.Descending)]
    public string? Email { get; set; }
    
    [Field(Name = "Name", Description = "The name of the person", ListName = "Name", OrderByType = OrderByType.Ascending)]
    [Required]
    public string? Name { get; set; }

    [Field(Name = "Biography", Description = "The biography of the person", EditorType = typeof(TextAreaEditor))]
    public string? Bio { get; set; }

    string? IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value ?? "0"); }

    public object Clone()
    {
        return new ConventionalPerson
        {
            Bio = Bio,
            Email = Email,
            Id = Id,
            Name = Name
        };
    }
}
