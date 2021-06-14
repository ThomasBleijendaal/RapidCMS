using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.ModelMaker.Models.Entities
{
    public class ModelEntity : IEntity
    {
        public string? Id { get; set; }

        public string? ParentId { get; set; } = default!;
        
        [Required]
        [MinLength(1)]
        public string Name { get; set; } = default!;

        public List<PropertyModel> PublishedProperties { get; set; } = new List<PropertyModel>();

        public List<PropertyModel> DraftProperties { get; set; } = new List<PropertyModel>();
    }
}
