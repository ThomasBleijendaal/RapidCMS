using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Enums;

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
        public string PluralName { get; set; } = default!;

        [JsonConverter(typeof(StringEnumConverter))]
        public Color IconColor { get; set; } = Color.Gray40;

        public string? Icon { get; set; }

        [Required]
        [MinLength(1)]
        public string Alias { get; set; } = default!;

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public OutputItem Output { get; set; } = OutputItem.Collection | OutputItem.Context | OutputItem.Entity | OutputItem.Repository | OutputItem.Validation;

        public List<PropertyModel> Properties { get; set; } = new List<PropertyModel>();
    }
}
