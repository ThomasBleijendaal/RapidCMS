using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Abstractions.Entities;
using RapidCMS.ModelMaker.Enums;

namespace RapidCMS.ModelMaker.Models.Entities
{
    public class ModelEntity : IPublishableModelMakerEntity
    {
        public string? Id
        {
            get => Alias;
            set => Alias = value ?? string.Empty;
        }

        public string Alias { get; set; } = default!;
        public string? ParentId { get; set; } = default!;
        public PublishState State { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime PublishedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; } = default!;

        public List<PropertyModel> PublishedProperties { get; set; } = new List<PropertyModel>();

        public List<PropertyModel> DraftProperties { get; set; } = new List<PropertyModel>();
    }
}
