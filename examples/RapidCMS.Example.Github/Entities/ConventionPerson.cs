using System;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Example.Github.Entities
{
    internal class ConventionPerson : IEntity, ICloneable
    {
        public string Id { get; set; }

        [Display(Name = "Name", ShortName = "Name")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Bio", Description = "If this field gets longer than 50, the summary on the ListView will get truncated.")]
        public string Bio { get; set; }

        [Display(ShortName = "Bio summary")]
        public string ShortBio => Bio?.Substring(0, Math.Min(Bio?.Length ?? 0, 50));

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
