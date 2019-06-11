using System.ComponentModel.DataAnnotations;
using RapidCMS.Common.Data;

namespace TestLibrary.Entities
{
    public class ValidationEntity : IEntity
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1.0, 10.0)]
        public long Range { get; set; }

        [MaxLength(10)]
        public string NotRequired { get; set; }
    }
}
