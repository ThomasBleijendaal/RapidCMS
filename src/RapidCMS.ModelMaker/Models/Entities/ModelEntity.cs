using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Extensions;

namespace RapidCMS.ModelMaker.Models.Entities
{
    public class ModelEntity : IEntity
    {
        public string? Id { get => Alias; set => Alias = value ?? Name?.ToUrlFriendlyString() ?? "noalias"; }

        public string? ParentId { get; set; } = default!;
        
        [Required]
        [MinLength(1)]
        public string Name { get; set; } = default!;

        [Required]
        [MinLength(1)]
        public string Alias { get; set; } = default!;

        public List<PropertyModel> Properties { get; set; } = new List<PropertyModel>();
    }
}
