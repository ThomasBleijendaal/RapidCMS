using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;
using RapidCMS.ModelMaker.Models.Config;
using RapidCMS.UI.Components.Editors;
using RapidCMS.UI.Extensions;

namespace RapidCMS.ModelMaker.Models
{
    internal class ModelMakerConfig : IModelMakerConfig
    {
        private readonly List<IPropertyEditorConfig> _editors = new List<IPropertyEditorConfig>();
        private readonly List<IPropertyDetailConfig> _details = new List<IPropertyDetailConfig>();
        private readonly List<IPropertyConfig> _properties = new List<IPropertyConfig>();

        public ModelMakerConfig()
        {
            ModelFolder = "./RapidModels/";
        }

        public IEnumerable<IPropertyEditorConfig> Editors => _editors;

        public IEnumerable<IPropertyDetailConfig> PropertyDetails => _details;

        public IEnumerable<IPropertyConfig> Properties => _properties;

        public IPropertyConfig AddProperty<TValue>(string alias, string name, string icon, IEnumerable<string> editorAliases, IEnumerable<string> detailAliases)
        {
            var property = new PropertyConfig(
                alias,
                name,
                icon,
                typeof(TValue),
                detailAliases.Select(x => _details.FirstOrDefault(v => v.Alias == x) ?? throw new InvalidOperationException($"Cannot find detail with alias {x}")).ToList(),
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

        public IPropertyDetailConfig AddPropertyDetail<TDetailConfig, TValueForEditor>(
                string alias, 
                string name, 
                string? description, 
                EditorType editorType,
                Expression<Func<IPropertyDetailModel<TDetailConfig>, TValueForEditor>> configEditor)
            where TDetailConfig : class, IDetailConfig
        {
            var validator = new PropertyDetailConfig(
                alias,
                name,
                description,
                GetEditorByEditorType(editorType),
                typeof(TDetailConfig),
                PropertyMetadataHelper.GetPropertyMetadata(configEditor) as IFullPropertyMetadata);

            _details.Add(validator);

            return validator;
        }

        public IPropertyDetailConfig AddPropertyDetail<TDetailConfig, TCustomEditor>(string alias, string name, string? description)
            where TDetailConfig : IDetailConfig
            where TCustomEditor : BasePropertyEditor
        {
            var validator = new PropertyDetailConfig(
                alias, 
                name, 
                description, 
                typeof(TCustomEditor), 
                typeof(TDetailConfig));

            _details.Add(validator);

            return validator;
        }

        public IPropertyDetailConfig AddPropertyDetail<TDetailConfig, TValueForEditor, TDataCollection>(
                string alias,
                string name,
                string? description,
                EditorType editorType,
                Expression<Func<IPropertyDetailModel<TDetailConfig>, TValueForEditor>> configEditor)
            where TDetailConfig : class, IDetailConfig
            where TDataCollection : IDataCollection
        {
            var validator = new PropertyDetailConfig(
                alias,
                name,
                description,
                GetEditorByEditorType(editorType),
                typeof(TDetailConfig),
                PropertyMetadataHelper.GetPropertyMetadata(configEditor) as IFullPropertyMetadata,
                typeof(TDataCollection));

            _details.Add(validator);

            return validator;
        }

        public IPropertyDetailConfig AddPropertyDetail<TDetailConfig, TCustomEditor, TDataCollection>(string alias, string name, string? description)
            where TDetailConfig : IDetailConfig
            where TCustomEditor : BasePropertyEditor
            where TDataCollection : IDataCollection
        {
            var validator = new PropertyDetailConfig(
                alias,
                name,
                description,
                typeof(TCustomEditor),
                typeof(TDetailConfig),
                dataCollection: typeof(TDataCollection));

            _details.Add(validator);

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

        private static Type GetEditorByEditorType(EditorType editorType)
            => editorType.GetEditor() ?? throw new InvalidOperationException($"EditorType.{editorType} is not a valid option.");

        public string ModelFolder { get; private set; }

        public void SetModelFolder(string folder)
        {
            ModelFolder = folder;
        }
    }
}
