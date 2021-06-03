using System.Collections.Generic;

namespace RapidCMS.ModelMaker.Abstractions.Config
{
    public interface IPropertyConfig
    {
        string Alias { get; }
        string Name { get; }
        string Icon { get; }

        bool UsableAsTitle { get; }

        IList<IPropertyValidatorConfig> Validators { get; }
        IList<IPropertyEditorConfig> Editors { get; }

        IPropertyConfig CanBeUsedAsTitle(bool usedAsTitle);
    }
}
