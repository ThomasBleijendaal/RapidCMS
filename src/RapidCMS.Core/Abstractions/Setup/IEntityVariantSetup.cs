using System;

namespace RapidCMS.Core.Abstractions.Setup
{
    public interface IEntityVariantSetup
    {
        string Name { get; }
        string? Icon { get; }
        Type Type { get; }
        string Alias { get; }
    }
}
