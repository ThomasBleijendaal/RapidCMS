using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Forms.Validation;
using RapidCMS.Example.Shared.ValidationAttributes;

namespace RapidCMS.Example.Shared.Data
{
    [CountryValidation]
    public class Country : IEntity, ICloneable
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [MaxTwo]
        public List<Person> People { get; set; } = new List<Person>();

        string? IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value ?? "0"); }

        public object Clone()
        {
            return new Country
            {
                Id = Id,
                Name = Name,
                People = People.ToList(),
                Metadata = new CountryMetadata
                {
                    Continent = Metadata.Continent
                }
            };
        }

        [Required, ValidateObject]
        public CountryMetadata Metadata { get; set; } = new CountryMetadata();

        [CountryMetadataValidation]
        public class CountryMetadata
        {
            [Required]
            [MinLength(8)]
            [MaxLength(10)]
            public string? Continent { get; set; }
        }
    }
}
