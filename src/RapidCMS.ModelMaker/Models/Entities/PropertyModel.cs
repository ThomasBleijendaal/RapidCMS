using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Forms.Validation;
using RapidCMS.ModelMaker.Abstractions.Entities;

namespace RapidCMS.ModelMaker.Models.Entities
{
    public class PropertyModel : IModelMakerEntity
    {
        public string? Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Alias { get; set; } = default!;

        [Required]
        [MinLength(1)]
        public string Name { get; set; } = default!;

        public bool IsTitle { get; set; }

        public bool IsRequired { get; set; }

        [Required]
        public string? PropertyAlias { get; set; }

        [Required]
        public string? EditorAlias { get; set; }

        [Required]
        [ValidateEnumerable]
        public List<PropertyValidationModel> Validations { get; set; } = new List<PropertyValidationModel>();

        public string? Type { get; set; }

        public bool IsRelationToOne { get; set; }

        public bool IsRelationToMany { get; set; }
    }
}
