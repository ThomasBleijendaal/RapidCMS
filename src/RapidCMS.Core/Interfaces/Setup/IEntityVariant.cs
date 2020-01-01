using System;

namespace RapidCMS.Core.Interfaces.Setup
{
    public interface IEntityVariant
    {
        string Name { get; }
        string? Icon { get; }
        Type Type { get; }
        string Alias { get; }
    }
}
