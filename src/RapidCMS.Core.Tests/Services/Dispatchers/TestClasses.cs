using RapidCMS.Core.Abstractions.Data;
using System.ComponentModel.DataAnnotations;

namespace RapidCMS.Core.Tests.Services.Dispatchers
{
    public class DefaultEntityVariant : IEntity
    {
        public string? Id { get; set; }
    }
    public class SubEntityVariant1 : DefaultEntityVariant
    {
    }
    public class SubEntityVariant2 : DefaultEntityVariant
    {
    }
    public class SubEntityVariant3 : DefaultEntityVariant
    {
    }

    public class ValidEntity : IEntity
    {
        public string? Id { get; set; }

        [Required]
        public string Name { get; set; } = "Something";
    }

    public class InvalidEntity : IEntity
    {
        public string? Id { get; set; }

        [Required]
        public string Name { get; set; } = default!;
    }
}
