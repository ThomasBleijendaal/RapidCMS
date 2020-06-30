using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Example.Shared.Data
{
    public class TagGroup : IEntity, ICloneable
    {
        public string? Id { get; set; }
        public string? Name { get; set; }

        public string? DefaultTagId { get; set; }

        public List<Tag> Tags { get; set; } = new List<Tag>();

        public object Clone()
        {
            return new TagGroup()
            {
                Id = Id,
                DefaultTagId = DefaultTagId,
                Name = Name,
                Tags = Tags.Select(x => (Tag)x.Clone()).ToList()
            };
        }
    }

    public class Tag : IEntity, ICloneable
    {
        public string? Id { get; set; }
        public string? Name { get; set; }

        public object Clone()
        {
            return new Tag
            {
                Id = Id,
                Name = Name
            };
        }
    }
}
