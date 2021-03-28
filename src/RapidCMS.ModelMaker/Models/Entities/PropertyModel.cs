using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.ModelMaker.Models.Entities
{
    internal class PropertyModel : IEntity
    {
        public string? Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Alias { get; set; } = default!;

        [Required]
        [MinLength(1)]
        public string Name { get; set; } = default!;

        public bool IsTitle { get; set; }

        public string? PropertyAlias { get; set; }
        public string? EditorAlias { get; set; }
        public List<PropertyValidationModel> Validations { get; set; } = new List<PropertyValidationModel>();
    }
}
