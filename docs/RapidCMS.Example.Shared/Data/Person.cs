using System;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Example.Shared.Data
{
    public class Person : IEntity, ICloneable
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Bio { get; set; }

        public int FavouriteChildId { get; set; }

        string? IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value ?? "0"); }

        public object Clone()
        {
            return new Person
            {
                Bio = Bio,
                Email = Email,
                Id = Id,
                Name = Name,
                FavouriteChildId = FavouriteChildId
            };
        }
    }
}
