using System;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Forms.Validation;

namespace RapidCMS.Example.Shared.Data
{
    public class Person : IEntity, ICloneable
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; } = default!;

        [ValidateObject]
        public PersonDetails Details { get; set; } = new PersonDetails();

        public int FavouriteChildId { get; set; }

        string? IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value ?? "0"); }

        public object Clone()
        {
            return new Person
            {
                Id = Id,
                Name = Name,
                FavouriteChildId = FavouriteChildId,
                Details = new PersonDetails
                {
                    Bio = Details.Bio,
                    Email = Details.Email
                }
            };
        }

        public class PersonDetails
        {
            [Required]
            [MinLength(5)]
            public string Email { get; set; } = default!;
            public string? Bio { get; set; }
        }
    }
}
