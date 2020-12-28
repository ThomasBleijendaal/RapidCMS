using System;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Example.Github.Entities
{
    internal class CountryPerson : IEntity, ICloneable
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Bio { get; set; }

        public string? FavouriteCountryId1 { get; set; }
        public string? FavouriteCountryId2 { get; set; }

        public object Clone()
        {
            return new CountryPerson
            {
                Bio = Bio,
                Email = Email,
                Id = Id,
                Name = Name,
                FavouriteCountryId1 = FavouriteCountryId1,
                FavouriteCountryId2 = FavouriteCountryId2
            };
        }
    }
}
