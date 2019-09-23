using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RapidCMS.Common.Data;
using TestLibrary.Validation;

namespace TestLibrary.Entities
{
    public class CountryEntity : IEntity
    {
#pragma warning disable IDE1006 // Naming Styles
        public int _Id { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        [Required]
        [MinLength(10)]
        public string Name { get; set; }
        public ICollection<PersonCountryEntity> Persons { get; set; }

        [NotMapped]
        string IEntity.Id { get => _Id.ToString(); set => _Id = int.Parse(value); }
    }
}
