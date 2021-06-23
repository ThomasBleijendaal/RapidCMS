using System;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Attributes;

namespace RapidCMS.Example.Github.Entities
{
    internal class ConventionPerson : IEntity, ICloneable
    {
        public string? Id { get; set; }

        [Field(Name = "Name", ListName = "Name")]
        public string? Name { get; set; }

        [Field(Name = "Email")]
        public string? Email { get; set; }

        [Field(Name = "Bio", Description = "If this field gets longer than 50, the summary on the ListView will get truncated.")]
        public string? Bio { get; set; }

        [Field(ListName = "Bio summary")]
        public string? ShortBio => Bio?.Substring(0, Math.Min(Bio?.Length ?? 0, 50));

        public object Clone()
        {
            return new ConventionPerson
            {
                Bio = Bio,
                Email = Email,
                Id = Id,
                Name = Name
            };
        }
    }
}
