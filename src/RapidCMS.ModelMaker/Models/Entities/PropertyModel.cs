using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Forms.Validation;

namespace RapidCMS.ModelMaker.Models.Entities
{
    public class PropertyModel : IEntity
    {
        public string? Id { get; set; }

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

        public string? EditorType { get; set; }

        public string? DataCollectionExpression { get; set; }

        public string? RelatedCollectionAlias { get; set; }
    }
}
