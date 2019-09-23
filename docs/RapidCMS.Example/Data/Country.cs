using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Data;
using RapidCMS.Example.ValidationAttributes;

namespace RapidCMS.Example.Data
{
    public class Country : IEntity, ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [MaxTwo]
        public List<Person> People { get; set; } = new List<Person>();

        string IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value); }

        public object Clone()
        {
            return new Country
            {
                Id = Id,
                Name = Name,
                People = People.ToList()
            };
        }
    }
}
