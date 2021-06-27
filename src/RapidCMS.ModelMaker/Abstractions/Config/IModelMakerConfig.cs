using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;
using RapidCMS.ModelMaker.Abstractions.Detail;
using RapidCMS.UI.Components.Editors;

namespace RapidCMS.ModelMaker.Abstractions.Config
{
    public interface IModelMakerConfig
    {
        /// <summary>
        /// Adds a property detail which can be added to one or more properties. Since no DetailConfig object is added, this detail is always active and not configurable by the user.
        /// </summary>
        /// <typeparam name="TDataCollection"></typeparam>
        /// <param name="alias"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        IPropertyDetailConfig AddPropertyDetail<TDataCollection>(string alias, string name, string? description)
            where TDataCollection : IDataCollection;

        /// <summary>
        /// Adds a property detail which can be added to one or more properties. An editor allows the user to configure the property detail. The TDetailConfig should set its Enabled flag high for its config to be applied to generated code.
        /// </summary>
        /// <typeparam name="TDetailConfig"></typeparam>
        /// <typeparam name="TValueForEditor"></typeparam>
        /// <param name="alias"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="editorType"></param>
        /// <param name="configEditor">Expression to configure what property of TDetailConfig should be used for the editor.</param>
        /// <returns></returns>
        IPropertyDetailConfig AddPropertyDetail<TDetailConfig, TValueForEditor>(string alias, string name, string? description, EditorType editorType, Expression<Func<IPropertyDetailModel<TDetailConfig>, TValueForEditor>> configEditor)
            where TDetailConfig : class, IDetailConfig;

        /// <summary>
        /// Adds a property detail which can be added to one or more properties. An editor allows the user to configure the property detail. The TDetailConfig should set its Enabled flag high for its config to be applied to generated code.
        /// </summary>
        /// <typeparam name="TDetailConfig"></typeparam>
        /// <typeparam name="TCustomEditor"></typeparam>
        /// <param name="alias"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        IPropertyDetailConfig AddPropertyDetail<TDetailConfig, TCustomEditor>(string alias, string name, string? description)
            where TDetailConfig : IDetailConfig
            where TCustomEditor : BasePropertyEditor;

        /// <summary>
        /// Adds a property detail which can be added to one or more properties. An editor allows the user to configure the property detail. The TDetailConfig should set its Enabled flag high for its config to be applied to generated code.
        /// </summary>
        /// <typeparam name="TDetailConfig"></typeparam>
        /// <typeparam name="TValueForEditor"></typeparam>
        /// <typeparam name="TDataCollection"></typeparam>
        /// <param name="alias"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="editorType"></param>
        /// <param name="configEditor">Expression to configure what property of TDetailConfig should be used for the editor.</param>
        /// <returns></returns>
        IPropertyDetailConfig AddPropertyDetail<TDetailConfig, TValueForEditor, TDataCollection>(string alias, string name, string? description, EditorType editorType, Expression<Func<IPropertyDetailModel<TDetailConfig>, TValueForEditor>> configEditor)
            where TDetailConfig : class, IDetailConfig
            where TDataCollection : IDataCollection;

        /// <summary>
        /// Adds a property detail which can be added to one or more properties. An editor allows the user to configure the property detail. The TDetailConfig should set its Enabled flag high for its config to be applied to generated code.
        /// </summary>
        /// <typeparam name="TDetailConfig"></typeparam>
        /// <typeparam name="TCustomEditor"></typeparam>
        /// <typeparam name="TDataCollection"></typeparam>
        /// <param name="alias"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        IPropertyDetailConfig AddPropertyDetail<TDetailConfig, TCustomEditor, TDataCollection>(string alias, string name, string? description)
            where TDetailConfig : IDetailConfig
            where TCustomEditor : BasePropertyEditor
            where TDataCollection : IDataCollection;

        /// <summary>
        /// Adds a property editor which can be added to one or more properties.
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="name"></param>
        /// <param name="editorType"></param>
        /// <returns></returns>
        IPropertyEditorConfig AddPropertyEditor(string alias, string name, EditorType editorType);

        /// <summary>
        /// Adds a property editor which can be added to one or more properties.
        /// </summary>
        /// <typeparam name="TCustomEditor"></typeparam>
        /// <param name="alias"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        IPropertyEditorConfig AddPropertyEditor<TCustomEditor>(string alias, string name)
            where TCustomEditor : BasePropertyEditor;

        /// <summary>
        /// Adds a property which can be added to models. Add editors and details to enrich the property.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="alias"></param>
        /// <param name="name"></param>
        /// <param name="icon"></param>
        /// <param name="editorAliases"></param>
        /// <param name="detailAliases"></param>
        /// <returns></returns>
        IPropertyConfig AddProperty<TValue>(string alias, string name, string icon,
            IEnumerable<string> editorAliases,
            IEnumerable<string> detailAliases);

        /// <summary>
        /// Gets the current configuration for the given property editor type.
        /// </summary>
        /// <param name="editorType"></param>
        /// <returns></returns>
        IPropertyEditorConfig? GetPropertyEditor(EditorType editorType);

        /// <summary>
        /// Gets the current configuration for the given property editor type.
        /// </summary>
        /// <typeparam name="TCustomEditor"></typeparam>
        /// <returns></returns>
        IPropertyEditorConfig? GetPropertyEditor<TCustomEditor>();

        /// <summary>
        /// Gets the current configuration for the given property editor alias.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        IPropertyEditorConfig? GetPropertyEditor(string alias);

        /// <summary>
        /// Gets the current configuration for the given property alias.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        IPropertyConfig? GetProperty(string alias);

        /// <summary>
        /// Gets the available editors.
        /// </summary>
        IEnumerable<IPropertyEditorConfig> Editors { get; }

        /// <summary>
        /// Gets the available property details.
        /// </summary>
        IEnumerable<IPropertyDetailConfig> PropertyDetails { get; }

        /// <summary>
        /// Gets the available properties.
        /// </summary>
        IEnumerable<IPropertyConfig> Properties { get; }

        /// <summary>
        /// Gets the current folder to save model jsons.
        /// </summary>
        string ModelFolder { get; }

        /// <summary>
        /// Sets the folder to save model jsons.
        /// </summary>
        /// <param name="folder"></param>
        void SetModelFolder(string folder);
    }
}
