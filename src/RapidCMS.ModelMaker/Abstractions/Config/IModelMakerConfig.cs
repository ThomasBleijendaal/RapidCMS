using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Core.Enums;
using RapidCMS.ModelMaker.Abstractions.DataCollections;
using RapidCMS.ModelMaker.Abstractions.Validation;
using RapidCMS.ModelMaker.Validation.Base;
using RapidCMS.UI.Components.Editors;

namespace RapidCMS.ModelMaker.Abstractions.Config
{
    public interface IModelMakerConfig
    {
        IPropertyValidatorConfig AddPropertyValidator<TValidator, TValue, TValidatorConfig, TValueForEditor>(string alias, string name, string? description, EditorType editorType, Expression<Func<IPropertyValidationModel<TValidatorConfig>, TValueForEditor>> configEditor)
            where TValidatorConfig : class, IValidatorConfig
            where TValidator : BaseValidator<TValue, TValidatorConfig>;

        IPropertyValidatorConfig AddPropertyValidator<TValidator, TValue, TValidatorConfig, TCustomEditor>(string alias, string name, string? description)
            where TValidatorConfig : IValidatorConfig
            where TValidator : BaseValidator<TValue, TValidatorConfig>
            where TCustomEditor : BasePropertyEditor;

        IPropertyValidatorConfig AddPropertyValidator<TValidator, TValue, TValidatorConfig, TValueForEditor, TDataCollection>(string alias, string name, string? description, EditorType editorType, Expression<Func<IPropertyValidationModel<TValidatorConfig>, TValueForEditor>> configEditor)
            where TValidatorConfig : class, IValidatorConfig
            where TValidator : BaseValidator<TValue, TValidatorConfig>
            where TDataCollection : IPropertyDataCollection, new();

        IPropertyValidatorConfig AddPropertyValidator<TValidator, TValue, TValidatorConfig, TCustomEditor, TDataCollection>(string alias, string name, string? description)
            where TValidatorConfig : IValidatorConfig
            where TValidator : BaseValidator<TValue, TValidatorConfig>
            where TCustomEditor : BasePropertyEditor
            where TDataCollection : IPropertyDataCollection, new();

        IModelMakerConfig AddPropertyEditor(string alias, string name, EditorType editorType);

        IModelMakerConfig AddPropertyEditor<TCustomEditor>(string alias, string name)
            where TCustomEditor : BasePropertyEditor;

        IModelMakerConfig AddProperty<TValue>(string alias, string name, string icon,
            IEnumerable<string> editorAliases,
            IEnumerable<string> validatorAliases);

        IPropertyEditorConfig? GetPropertyEditor(EditorType editorType);
        IPropertyEditorConfig? GetPropertyEditor<TCustomEditor>();
        IPropertyValidatorConfig? GetPropertyValidator<TValidator>();
        IPropertyConfig? GetProperty(string name);

        IEnumerable<IPropertyEditorConfig> Editors { get; }
        IEnumerable<IPropertyValidatorConfig> Validators { get; }
        IEnumerable<IPropertyConfig> Properties { get; }
    }
}
