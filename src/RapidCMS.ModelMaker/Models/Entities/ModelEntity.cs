using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.ModelMaker.Abstractions.Entities;

namespace RapidCMS.ModelMaker.Models.Entities
{
    public class ModelEntity : IModelMakerEntity
    {
        public string? Id
        {
            get => Alias;
            set => Alias = value ?? string.Empty;
        }

        public string Alias { get; set; } = default!;

        [Required]
        [MinLength(1)]
        public string Name { get; set; } = default!;

        public List<PropertyModel> PublishedProperties { get; set; } = new List<PropertyModel>();

        public List<PropertyModel> DraftProperties { get; set; } = new List<PropertyModel>();
    }
}
