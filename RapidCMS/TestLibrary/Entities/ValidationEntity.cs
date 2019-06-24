using System.ComponentModel.DataAnnotations;
using RapidCMS.Common.Data;
using TestLibrary.Enums;
using TestLibrary.Validation;

namespace TestLibrary.Entities
{
    public class ValidationEntity : IEntity
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Dummy { get; set; }

        [Required]
        [Range(1.0, 10.0)]
        public long Range { get; set; }

        [MaxLength(10)]
        public string NotRequired { get; set; }

        [Required]
        [True]
        public bool Accept { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(20)]
        public string Textarea { get; set; }

        [Required]
        public TestEnum? Enum { get; set; }

        [Required]
        public int CountryId { get; set; }
    }
}
