using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Attributes;
using RapidCMS.UI.Components.Editors;

namespace RapidCMS.Example.Shared.Data
{
    public class Details : IEntity, ICloneable
    {
        public string? Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Title { get; set; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string History { get; set; } = default!;

        [Required]
        [ValidateObjectAsProperty]
        public NestedDetails Nested { get; set; } = new NestedDetails();

        public object Clone()
        {
            return new Details
            {
                History = History,
                Id = Id,
                Title = Title,
                Nested = new NestedDetails
                {
                    Description = Nested.Description,
                    Tags = Nested.Tags
                }
            };
        }

        public class NestedDetails : IEntity
        {
            [MinLength(2)]
            [Field(Name = "Tags", EditorType = typeof(ListEditor))]
            public List<string> Tags { get; set; } = new List<string>();

            [Required]
            [MinLength(10)]
            [Field(Name = "Description")]
            public string? Description { get; set; }

            string? IEntity.Id { get; set; }
        }
    }
}
