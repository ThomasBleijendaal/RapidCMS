using System;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Example.Shared.Data
{
    public class DatabaseEntity : IEntity, ICloneable
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public object Clone()
        {
            return new DatabaseEntity
            {
                Id = Id,
                Description = Description,
                Name = Name
            };
        }
    }
}
