using System;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Example.Github
{
    internal class Person : IEntity, ICloneable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }

        public object Clone()
        {
            return new Person
            {
                Bio = Bio,
                Email = Email,
                Id = Id,
                Name = Name
            };
        }
    }
}
