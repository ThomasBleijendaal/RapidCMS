using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.ModelMaker.Abstractions.Entities;

namespace RapidCMS.ModelMaker.Models.Entities
{
    public class ModelEntity : IModelMakerEntity
    {
        public string? Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Alias { get; set; } = default!;

        [Required]
        [MinLength(1)]
        public string Name { get; set; } = default!;

        public List<PropertyModel> Properties { get; set; } = new List<PropertyModel>();
    }
}
