using System;
using System.Collections.Generic;
using RapidCMS.ModelMaker.Abstractions.Config;

namespace RapidCMS.ModelMaker.Models.Config
{
    internal class PropertyConfig : IPropertyConfig
    {
        public PropertyConfig(string alias, string name, string icon, IList<IPropertyValidatorConfig> validators, IList<IPropertyEditorConfig> editors)
        {
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Icon = icon ?? throw new ArgumentNullException(nameof(icon));
            Validators = validators ?? throw new ArgumentNullException(nameof(validators));
            Editors = editors ?? throw new ArgumentNullException(nameof(editors));
        }

        public string Alias { get; }

        public string Name { get; }

        public string Icon { get; }

        public IList<IPropertyValidatorConfig> Validators { get; }

        public IList<IPropertyEditorConfig> Editors { get; }
    }
}
