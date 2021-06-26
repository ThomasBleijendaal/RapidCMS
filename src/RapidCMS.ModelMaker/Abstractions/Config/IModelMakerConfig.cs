using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;
using RapidCMS.UI.Components.Editors;

namespace RapidCMS.ModelMaker.Abstractions.Config
{
    public interface IModelMakerConfig
    {
        IPropertyDetailConfig AddPropertyDetail<TDetailConfig, TValueForEditor>(string alias, string name, string? description, EditorType editorType, Expression<Func<IPropertyDetailModel<TDetailConfig>, TValueForEditor>> configEditor)
            where TDetailConfig : class, IDetailConfig;

        IPropertyDetailConfig AddPropertyDetail<TDetailConfig, TCustomEditor>(string alias, string name, string? description)
            where TDetailConfig : IDetailConfig
            where TCustomEditor : BasePropertyEditor;

        IPropertyDetailConfig AddPropertyDetail<TDetailConfig, TValueForEditor, TDataCollection>(string alias, string name, string? description, EditorType editorType, Expression<Func<IPropertyDetailModel<TDetailConfig>, TValueForEditor>> configEditor)
            where TDetailConfig : class, IDetailConfig
            where TDataCollection : IDataCollection;

        IPropertyDetailConfig AddPropertyDetail<TDetailConfig, TCustomEditor, TDataCollection>(string alias, string name, string? description)
            where TDetailConfig : IDetailConfig
            where TCustomEditor : BasePropertyEditor
            where TDataCollection : IDataCollection;

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
        IEnumerable<IPropertyDetailConfig> PropertyDetails { get; }
        IEnumerable<IPropertyConfig> Properties { get; }

        string ModelFolder { get; }
        void SetModelFolder(string folder);
    }
}
