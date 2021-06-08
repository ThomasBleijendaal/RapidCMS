using System;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.ModelMaker.Abstractions.Config;

namespace RapidCMS.ModelMaker.Models.Config
{
    internal class PropertyValidatorConfig : IPropertyValidatorConfig
    {
        public PropertyValidatorConfig(
            string alias, 
            string name, 
            string? description, 
            Type value, 
            Type editor, 
            Type validator,
            Type config,
            IFullPropertyMetadata? configToEditor = null,
            Type? dataCollectionFactory = null)
        {
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Editor = editor ?? throw new ArgumentNullException(nameof(editor));
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
            Config = config ?? throw new ArgumentNullException(nameof(config));

            ConfigToEditor = configToEditor;
            DataCollectionFactory = dataCollectionFactory;
        }

        public string Alias { get; }
        public string Name { get; }
        public string? Description { get; }

        public Type Validator { get; }

        public Type Value { get; }

        public Type Config { get; }

        public Type Editor { get; }

        public IFullPropertyMetadata? ConfigToEditor { get; }
        public Type? DataCollectionFactory { get; }
    }
}
