using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Example.Github.Entities
{
    internal class RelatablePerson : IEntity, ICloneable
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Bio { get; set; }

        public List<Country> Countries { get; set; } = new List<Country>();

        public object Clone()
        {
            return new RelatablePerson
            {
                Bio = Bio,
                Email = Email,
                Id = Id,
                Name = Name,
                Countries = Countries.ToList(x => (Country)x.Clone())
            };
        }
    }
}
