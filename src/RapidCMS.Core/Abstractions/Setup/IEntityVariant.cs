using System;

namespace RapidCMS.Core.Abstractions.Setup
{
    public interface IEntityVariant
    {
        string Name { get; }
        string? Icon { get; }
        Type Type { get; }
        string Alias { get; }
    }
}
