using System.Collections.Generic;

namespace RapidCMS.ModelMaker.Abstractions.Config
{
    public interface IPropertyConfig
    {
        string Alias { get; }
        string Name { get; }
        string Icon { get; }

        IList<IPropertyValidatorConfig> Validators { get; }
        IList<IPropertyEditorConfig> Editors { get; }
    }
}
