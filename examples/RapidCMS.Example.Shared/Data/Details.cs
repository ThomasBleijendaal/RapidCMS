using System;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Example.Shared.Data
{
    public class Details : IEntity, ICloneable
    {
        public string? Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Title { get; set; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string History { get; set; } = default!;

        public object Clone()
        {
            return new Details
            {
                History = History,
                Id = Id,
                Title = Title
            };
        }
    }
}
