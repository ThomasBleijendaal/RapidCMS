using System.Collections.Generic;
using RapidCMS.Core.Enums;
using RapidCMS.UI.Components.Editors;

namespace RapidCMS.ModelMaker.Abstractions.Config
{
    public interface IModelMakerConfig
    {
        IModelMakerConfig AddPropertyValidator<TValidator, TValue, TValidatorConfig>(string alias, string name, string? description, EditorType editorType)
            where TValidatorConfig : IValidatorConfig
            where TValidator : BaseValidator<TValue, TValidatorConfig>;

        IModelMakerConfig AddPropertyValidator<TValidator, TValue, TValidatorConfig, TCustomEditor>(string alias, string name, string? description)
            where TValidatorConfig : IValidatorConfig
            where TValidator : BaseValidator<TValue, TValidatorConfig>
            where TCustomEditor : BasePropertyEditor;

        IModelMakerConfig AddPropertyEditor(string alias, string name, EditorType editorType);

        IModelMakerConfig AddPropertyEditor<TCustomEditor>(string alias, string name)
            where TCustomEditor : BasePropertyEditor;

        IModelMakerConfig AddProperty<TValue>(string alias, string name, string icon,
            IEnumerable<string> editorAliases,
            IEnumerable<string> validatorAliases);

        IPropertyEditorConfig? GetPropertyEditor(EditorType editorType);
        IPropertyEditorConfig? GetPropertyEditor<TCustomEditor>();
        IPropertyValidatorConfig? GetPropertyValidator<TValidator>();
        IPropertyValidatorConfig? GetProperty(string name);

        IEnumerable<IPropertyEditorConfig> Editors { get; }
        IEnumerable<IPropertyValidatorConfig> Validators { get; }
    }
}
