using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Common.Data;

namespace RapidCMS.Example.Data
{
    public class TagGroup : IEntity, ICloneable
    {
        public string Id { get; set; }
        public string? Name { get; set; }

        public List<Tag> Tags { get; set; } = new List<Tag>();

        public object Clone()
        {
            return new TagGroup()
            {
                Id = Id,
                Name = Name,
                Tags = Tags.Select(x => (Tag)x.Clone()).ToList()
            };
        }
    }

    public class Tag : IEntity, ICloneable
    {
        public string Id { get; set; }
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
