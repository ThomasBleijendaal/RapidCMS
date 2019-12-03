using System.ComponentModel.DataAnnotations;
using RapidCMS.Common.Data;

namespace RapidCMS.Example.Data
{
    public class MappedEntity : IEntity
    {
        public string? Id { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(20)]
        public string? Name { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(1000)]
        public string? Description { get; set; }
    }
}
