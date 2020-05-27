using System;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Example.Shared.Data
{
    public class ConventionalPerson : IEntity, ICloneable
    {
        public int Id { get; set; }

        [Display(Name = "Name", Description = "The name of the person", ShortName = "Name")]
        [Required]
        public string? Name { get; set; }

        [Display(Name = "Email", Description = "The email of the person")]
        public string? Email { get; set; }

        [Display(Name = "Biography", Description = "The biography of the persion")]
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
}
