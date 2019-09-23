using System;
using RapidCMS.Common.Data;

namespace RapidCMS.Example.Data
{
    public class Person : IEntity, ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }

        string IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value); }

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
