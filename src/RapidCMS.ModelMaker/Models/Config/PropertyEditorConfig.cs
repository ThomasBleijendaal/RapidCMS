using System;
using RapidCMS.ModelMaker.Abstractions.Config;

namespace RapidCMS.ModelMaker.Models.Config
{
    internal class PropertyEditorConfig : IPropertyEditorConfig
    {
        public PropertyEditorConfig(string alias, string name, Type editor)
        {
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Editor = editor ?? throw new ArgumentNullException(nameof(editor));
        }

        public string Alias { get; }

        public string Name { get; }

        public Type Editor { get; }
    }
}
