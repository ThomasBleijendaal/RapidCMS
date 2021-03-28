using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;
using RapidCMS.ModelMaker.Abstractions.Config;
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

        public IModelMakerConfig AddPropertyEditor(string alias, string name, EditorType editorType)
        {
            _editors.Add(new PropertyEditorConfig(alias, name, GetEditorByEditorType(editorType)));

            return this;
        }

        public IModelMakerConfig AddPropertyEditor<TCustomEditor>(string alias, string name) where TCustomEditor : BasePropertyEditor
        {
            _editors.Add(new PropertyEditorConfig(alias, name, typeof(TCustomEditor)));

            return this;
        }

        public IModelMakerConfig AddPropertyValidator<TValidator, TValue, TValidatorConfig, TValueForEditor>(
                string alias, 
                string name, 
                string? description, 
                EditorType editorType,
                Expression<Func<IPropertyValidationModel<TValidatorConfig>, TValueForEditor>> configEditor)
            where TValidator : BaseValidator<TValue, TValidatorConfig>
            where TValidatorConfig : class, IValidatorConfig
        {
            _validators.Add(new PropertyValidatorConfig(
                alias, 
                name, 
                description, 
                typeof(TValue), 
                GetEditorByEditorType(editorType), 
                typeof(TValidator), 
                typeof(TValidatorConfig),
                PropertyMetadataHelper.GetPropertyMetadata(configEditor) as IFullPropertyMetadata));

            return this;
        }

        public IModelMakerConfig AddPropertyValidator<TValidator, TValue, TValidatorConfig, TCustomEditor>(string alias, string name, string? description)
            where TValidator : BaseValidator<TValue, TValidatorConfig>
            where TValidatorConfig : IValidatorConfig
            where TCustomEditor : BasePropertyEditor
        {
            _validators.Add(new PropertyValidatorConfig(
                alias, 
                name, 
                description, 
                typeof(TValue),
                typeof(TCustomEditor), 
                typeof(TValidator), 
                typeof(TValidatorConfig)));

            return this;
        }

        public IPropertyConfig? GetProperty(string name)
        {
            return _properties.FirstOrDefault(x => x.Name == name);
        }

        public IPropertyEditorConfig? GetPropertyEditor(EditorType editorType)
        {
            var editor = GetEditorByEditorType(editorType);

            return _editors.FirstOrDefault(x => x.Editor == editor);
        }

        public IPropertyEditorConfig? GetPropertyEditor<TCustomEditor>()
        {
            return _editors.FirstOrDefault(x => x.Editor == typeof(TCustomEditor));
        }

        public IPropertyValidatorConfig? GetPropertyValidator<TValidator>()
        {
            return _validators.FirstOrDefault(x => x.Validator == typeof(TValidator));
        }

        private static Type GetEditorByEditorType(EditorType editorType) 
            => editorType switch
            {
                EditorType.Checkbox => typeof(CheckboxEditor),
                EditorType.TextBox => typeof(TextBoxEditor),
                EditorType.TextArea => typeof(TextAreaEditor),
                EditorType.Numeric => typeof(NumericEditor),
                EditorType.Date => typeof(DateEditor),
                EditorType.Dropdown => typeof(DropdownEditor),
                EditorType.Select => typeof(SelectEditor),
                EditorType.MultiSelect => typeof(MultiSelectEditor),

                _ => throw new InvalidOperationException($"EditorType.{editorType} is not a valid option."),
            };
    }
}
