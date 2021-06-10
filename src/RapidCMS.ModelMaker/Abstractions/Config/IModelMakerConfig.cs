using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Core.Enums;
using RapidCMS.ModelMaker.Core.Abstractions.Factories;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;
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
            where TDataCollection : IDataCollectionFactory;

        IPropertyValidatorConfig AddPropertyValidator<TValidator, TValue, TValidatorConfig, TCustomEditor, TDataCollection>(string alias, string name, string? description)
            where TValidatorConfig : IValidatorConfig
            where TValidator : BaseValidator<TValue, TValidatorConfig>
            where TCustomEditor : BasePropertyEditor
            where TDataCollection : IDataCollectionFactory;

        IPropertyEditorConfig AddPropertyEditor(string alias, string name, EditorType editorType);

        IPropertyEditorConfig AddPropertyEditor<TCustomEditor>(string alias, string name)
            where TCustomEditor : BasePropertyEditor;

        IPropertyConfig AddProperty<TValue>(string alias, string name, string icon,
            IEnumerable<string> editorAliases,
            IEnumerable<string> validatorAliases);

        IPropertyEditorConfig? GetPropertyEditor(EditorType editorType);
        IPropertyEditorConfig? GetPropertyEditor<TCustomEditor>();
        IPropertyEditorConfig? GetPropertyEditor(string alias);
        IPropertyValidatorConfig? GetPropertyValidator<TValidator>();
        IPropertyConfig? GetProperty(string alias);

        IEnumerable<IPropertyEditorConfig> Editors { get; }
        IEnumerable<IPropertyValidatorConfig> Validators { get; }
        IEnumerable<IPropertyConfig> Properties { get; }

        string ModelFolder { get; }
        void SetModelFolder(string folder);
    }
}
