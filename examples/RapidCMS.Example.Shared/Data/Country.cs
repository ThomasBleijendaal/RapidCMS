using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Example.Shared.Data
{
    // this entity is validated by the CountryValidator
    public class Country : IEntity, ICloneable
    {
        public int Id { get; set; }

        public string? Name { get; set; }

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
                    Continent = Metadata.Continent,
                    Tag = Metadata.Tag
                }
            };
        }

        public CountryMetadata Metadata { get; set; } = new CountryMetadata();

        public class CountryMetadata
        {
            public string? Continent { get; set; }

            public string? Tag { get; set; }
        }
    }
}
