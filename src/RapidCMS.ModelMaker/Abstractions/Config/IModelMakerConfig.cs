using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Core.Enums;
using RapidCMS.ModelMaker.Core.Abstractions.Factories;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;
using RapidCMS.UI.Components.Editors;

namespace RapidCMS.ModelMaker.Abstractions.Config
{
    public interface IModelMakerConfig
    {
        IPropertyValidatorConfig AddPropertyValidator<TValue, TValidatorConfig, TValueForEditor>(string alias, string name, string? description, EditorType editorType, Expression<Func<IPropertyValidationModel<TValidatorConfig>, TValueForEditor>> configEditor)
            where TValidatorConfig : class, IValidatorConfig;

        IPropertyValidatorConfig AddPropertyValidator<TValue, TValidatorConfig, TCustomEditor>(string alias, string name, string? description)
            where TValidatorConfig : IValidatorConfig
            where TCustomEditor : BasePropertyEditor;

        IPropertyValidatorConfig AddPropertyValidator<TValue, TValidatorConfig, TValueForEditor, TDataCollection>(string alias, string name, string? description, EditorType editorType, Expression<Func<IPropertyValidationModel<TValidatorConfig>, TValueForEditor>> configEditor)
            where TValidatorConfig : class, IValidatorConfig
            where TDataCollection : IDataCollectionFactory;

        IPropertyValidatorConfig AddPropertyValidator<TValue, TValidatorConfig, TCustomEditor, TDataCollection>(string alias, string name, string? description)
            where TValidatorConfig : IValidatorConfig
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
        IPropertyConfig? GetProperty(string alias);

        IEnumerable<IPropertyEditorConfig> Editors { get; }
        IEnumerable<IPropertyValidatorConfig> Validators { get; }
        IEnumerable<IPropertyConfig> Properties { get; }

        string ModelFolder { get; }
        void SetModelFolder(string folder);
    }
}
