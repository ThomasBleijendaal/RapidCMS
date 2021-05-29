using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.Abstractions.Factories;
using RapidCMS.ModelMaker.Abstractions.Validation;
using RapidCMS.ModelMaker.Models.Config;
using RapidCMS.ModelMaker.Validation.Base;
using RapidCMS.UI.Components.Editors;

namespace RapidCMS.ModelMaker.Models
{
    internal class ModelMakerConfig : IModelMakerConfig
    {
        private readonly List<IPropertyEditorConfig> _editors = new List<IPropertyEditorConfig>();
        private readonly List<IPropertyValidatorConfig> _validators = new List<IPropertyValidatorConfig>();
        private readonly List<IPropertyConfig> _properties = new List<IPropertyConfig>();

        public IEnumerable<IPropertyEditorConfig> Editors => _editors;

        public IEnumerable<IPropertyValidatorConfig> Validators => _validators;

        public IEnumerable<IPropertyConfig> Properties => _properties;

        public IModelMakerConfig AddProperty<TValue>(string alias, string name, string icon, IEnumerable<string> editorAliases, IEnumerable<string> validatorAliases)
        {
            _properties.Add(new PropertyConfig(
                alias,
                name,
                icon,
                validatorAliases.Select(x => _validators.FirstOrDefault(v => v.Alias == x) ?? throw new InvalidOperationException($"Cannot find validator with alias {x}")).ToList(),
                editorAliases.Select(x => _editors.FirstOrDefault(e => e.Alias == x) ?? throw new InvalidOperationException($"Cannot find editor with alias {x}")).ToList()));

            return this;
        }

        public IPropertyEditorConfig AddPropertyEditor(string alias, string name, EditorType editorType)
        {
            var editor = new PropertyEditorConfig(alias, name, GetEditorByEditorType(editorType));
            _editors.Add(editor);
            return editor;
        }

        public IPropertyEditorConfig AddPropertyEditor<TCustomEditor>(string alias, string name) where TCustomEditor : BasePropertyEditor
        {
            var editor = new PropertyEditorConfig(alias, name, typeof(TCustomEditor));
            _editors.Add(editor);
            return editor;
        }

        public IPropertyValidatorConfig AddPropertyValidator<TValidator, TValue, TValidatorConfig, TValueForEditor>(
                string alias, 
                string name, 
                string? description, 
                EditorType editorType,
                Expression<Func<IPropertyValidationModel<TValidatorConfig>, TValueForEditor>> configEditor)
            where TValidator : BaseValidator<TValue, TValidatorConfig>
            where TValidatorConfig : class, IValidatorConfig
        {
            var validator = new PropertyValidatorConfig(
                alias,
                name,
                description,
                typeof(TValue),
                GetEditorByEditorType(editorType),
                typeof(TValidator),
                typeof(TValidatorConfig),
                PropertyMetadataHelper.GetPropertyMetadata(configEditor) as IFullPropertyMetadata);

            _validators.Add(validator);

            return validator;
        }

        public IPropertyValidatorConfig AddPropertyValidator<TValidator, TValue, TValidatorConfig, TValueForEditor, TDataCollectionFactory>(
                string alias,
                string name,
                string? description,
                EditorType editorType,
                Expression<Func<IPropertyValidationModel<TValidatorConfig>, TValueForEditor>> configEditor)
            where TValidator : BaseValidator<TValue, TValidatorConfig>
            where TValidatorConfig : class, IValidatorConfig
            where TDataCollectionFactory : IDataCollectionFactory
        {
            var validator = new PropertyValidatorConfig(
                alias,
                name,
                description,
                typeof(TValue),
                GetEditorByEditorType(editorType),
                typeof(TValidator),
                typeof(TValidatorConfig),
                PropertyMetadataHelper.GetPropertyMetadata(configEditor) as IFullPropertyMetadata,
                typeof(TDataCollectionFactory));

            _validators.Add(validator);

            return validator;
        }

        public IPropertyValidatorConfig AddPropertyValidator<TValidator, TValue, TValidatorConfig, TCustomEditor>(string alias, string name, string? description)
            where TValidator : BaseValidator<TValue, TValidatorConfig>
            where TValidatorConfig : IValidatorConfig
            where TCustomEditor : BasePropertyEditor
        {
            var validator = new PropertyValidatorConfig(
                alias, 
                name, 
                description, 
                typeof(TValue),
                typeof(TCustomEditor), 
                typeof(TValidator), 
                typeof(TValidatorConfig));

            _validators.Add(validator);

            return validator;
        }

        public IPropertyValidatorConfig AddPropertyValidator<TValidator, TValue, TValidatorConfig, TCustomEditor, TDataCollectionFactory>(string alias, string name, string? description)
            where TValidator : BaseValidator<TValue, TValidatorConfig>
            where TValidatorConfig : IValidatorConfig
            where TCustomEditor : BasePropertyEditor
            where TDataCollectionFactory : IDataCollectionFactory
        {
            var validator = new PropertyValidatorConfig(
                alias,
                name,
                description,
                typeof(TValue),
                typeof(TCustomEditor),
                typeof(TValidator),
                typeof(TValidatorConfig),
                default,
                typeof(TDataCollectionFactory));

            _validators.Add(validator);

            return validator;
        }

        public IPropertyConfig? GetProperty(string alias) 
            => _properties.FirstOrDefault(x => x.Alias == alias);

        public IPropertyEditorConfig? GetPropertyEditor(EditorType editorType)
        {
            var editor = GetEditorByEditorType(editorType);

            return _editors.FirstOrDefault(x => x.Editor == editor);
        }

        public IPropertyEditorConfig? GetPropertyEditor<TCustomEditor>()
            => _editors.FirstOrDefault(x => x.Editor == typeof(TCustomEditor));
        public IPropertyEditorConfig? GetPropertyEditor(string alias)
            => _editors.FirstOrDefault(x => x.Alias == alias);

        public IPropertyValidatorConfig? GetPropertyValidator<TValidator>()
            => _validators.FirstOrDefault(x => x.Validator == typeof(TValidator));

        private static Type GetEditorByEditorType(EditorType editorType) 
            => editorType switch
            {
                // TODO: check if all types are supported 

                EditorType.Checkbox => typeof(CheckboxEditor),
                EditorType.TextBox => typeof(TextBoxEditor),
                EditorType.TextArea => typeof(TextAreaEditor),
                EditorType.Numeric => typeof(NumericEditor),
                EditorType.Date => typeof(DateEditor),
                EditorType.Dropdown => typeof(DropdownEditor),
                EditorType.Select => typeof(SelectEditor),
                EditorType.MultiSelect => typeof(MultiSelectEditor),
                EditorType.ListEditor => typeof(ListEditor),

                _ => throw new InvalidOperationException($"EditorType.{editorType} is not a valid option."),
            };

    }
}
