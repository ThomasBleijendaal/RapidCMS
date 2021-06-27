using System;
using RapidCMS.Core.Abstractions.Metadata;

namespace RapidCMS.ModelMaker.Abstractions.Config
{
    public interface IPropertyDetailConfig
    {
        string Alias { get; }
        string Name { get; }
        string? Description { get; }

        Type? Editor { get; }

        Type Config { get; }

        IFullPropertyMetadata? ConfigToEditor { get; }

        Type? DataCollection { get; }
    }
}
