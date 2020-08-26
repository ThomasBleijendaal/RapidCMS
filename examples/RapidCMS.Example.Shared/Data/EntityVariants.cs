using System;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Example.Shared.Data
{
    public class EntityVariantBase : IEntity, ICloneable
    {
        public int Id { get; set; }
        
        [Required]
        public string? Name { get; set; }

        string? IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value ?? "0"); }

        public object Clone()
        {
            return this switch
            {
                EntityVariantA a => new EntityVariantA
                {
                    Id = a.Id,
                    Name = a.Name,
                    NameA1 = a.NameA1
                },
                EntityVariantB b => new EntityVariantB
                {
                    Id = b.Id,
                    Name = b.Name,
                    NameB1 = b.NameB1,
                    NameB2 = b.NameB2
                },
                EntityVariantC c => new EntityVariantC
                {
                    Id = c.Id,
                    Name = c.Name,
                    NameC1 = c.NameC1,
                    NameC2 = c.NameC2,
                    NameC3 = c.NameC3
                },
                EntityVariantBase @base => new EntityVariantBase
                {
                    Id = @base.Id,
                    Name = @base.Name
                }
            };
        }
    }

    public class EntityVariantA : EntityVariantBase
    {
        public string? NameA1 { get; set; }
    }

    public class EntityVariantB : EntityVariantBase
    {
        public string? NameB1 { get; set; }
        public string? NameB2 { get; set; }
    }

    public class EntityVariantC : EntityVariantBase
    {
        public string? NameC1 { get; set; }
        public string? NameC2 { get; set; }
        public string? NameC3 { get; set; }
    }
}
