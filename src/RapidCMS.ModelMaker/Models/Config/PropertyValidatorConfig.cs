using System;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.ModelMaker.Abstractions.Config;

namespace RapidCMS.ModelMaker.Models.Config
{
    internal class PropertyDetailConfig : IPropertyDetailConfig
    {
        public PropertyDetailConfig(
            string alias,
            string name,
            string? description,
            Type editor,
            Type config,
            IFullPropertyMetadata? configToEditor = null,
            Type? dataCollection = null)
        {
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Editor = editor ?? throw new ArgumentNullException(nameof(editor));
            Config = config ?? throw new ArgumentNullException(nameof(config));

            ConfigToEditor = configToEditor;
            DataCollection = dataCollection;
        }

        public string Alias { get; }
        public string Name { get; }
        public string? Description { get; }

        public Type Config { get; }

        public Type Editor { get; }

        public IFullPropertyMetadata? ConfigToEditor { get; }

        public Type? DataCollection { get; }
    }
}
