using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.Core.Abstractions.Factories;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;
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

        public ModelMakerConfig()
        {
            ModelFolder = "./RapidModels/";
        }

        public IEnumerable<IPropertyEditorConfig> Editors => _editors;

        public IEnumerable<IPropertyValidatorConfig> Validators => _validators;

        public IEnumerable<IPropertyConfig> Properties => _properties;

        public IPropertyConfig AddProperty<TValue>(string alias, string name, string icon, IEnumerable<string> editorAliases, IEnumerable<string> validatorAliases)
        {
            var property = new PropertyConfig(
                alias,
                name,
                icon,
                typeof(TValue),
                validatorAliases.Select(x => _validators.FirstOrDefault(v => v.Alias == x) ?? throw new InvalidOperationException($"Cannot find validator with alias {x}")).ToList(),
                editorAliases.Select(x => _editors.FirstOrDefault(e => e.Alias == x) ?? throw new InvalidOperationException($"Cannot find editor with alias {x}")).ToList());

            _properties.Add(property);

            return property;
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
            var property = PropertyMetadataHelper.GetPropertyMetadata(configEditor) as IFullPropertyMetadata;

            if (!string.IsNullOrWhiteSpace(property?.PropertyName) && 
                _validators.Any(x => x.ConfigToEditor?.PropertyName == property?.PropertyName))
            {
                throw new InvalidOperationException($"Validation models must have unique ConfigToEditor property metadata, {property.PropertyName} already used. [ValidateEnumerable] won't be able to distinguish between models.");
            }

            var validator = new PropertyValidatorConfig(
                alias,
                name,
                description,
                typeof(TValue),
                GetEditorByEditorType(editorType),
                typeof(TValidator),
                typeof(TValidatorConfig),
                property,
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
                EditorType.Checkbox => typeof(CheckboxEditor),
                EditorType.Date => typeof(DateEditor),
                EditorType.Dropdown => typeof(DropdownEditor),
                EditorType.EntitiesPicker => typeof(EntitiesPickerEditor),
                EditorType.EntityPicker => typeof(EntityPickerEditor),
                EditorType.ListEditor => typeof(ListEditor),
                EditorType.MultiSelect => typeof(MultiSelectEditor),
                EditorType.ModelEditor => typeof(ModelEditor),
                EditorType.Numeric => typeof(NumericEditor),
                EditorType.Select => typeof(SelectEditor),
                EditorType.TextBox => typeof(TextBoxEditor),
                EditorType.TextArea => typeof(TextAreaEditor),

                // TODO: perhaps an empty component?
                EditorType.None => typeof(TextBoxEditor),

                _ => throw new InvalidOperationException($"EditorType.{editorType} is not a valid option."),
            };

        public string ModelFolder { get; private set; }

        public void SetModelFolder(string folder)
        {
            ModelFolder = folder;
        }
    }
}
