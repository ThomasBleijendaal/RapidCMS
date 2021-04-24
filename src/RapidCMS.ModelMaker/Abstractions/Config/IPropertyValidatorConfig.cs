using System;
using RapidCMS.Core.Abstractions.Metadata;

namespace RapidCMS.ModelMaker.Abstractions.Config
{
    public interface IPropertyValidatorConfig
    {
        string Alias { get; }
        string Name { get; }
        string? Description { get; }

        Type Validator { get; }
        Type Value { get; }
        Type Config { get; }
        Type Editor { get; }

        IFullPropertyMetadata? ConfigToEditor { get; }

        Type? DataCollection { get; }
    }
}
