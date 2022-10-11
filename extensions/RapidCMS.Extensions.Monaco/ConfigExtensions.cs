using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;

namespace RapidCMS.Extensions.Monaco;

public static class ConfigExtensions
{
    /// <summary>
    /// Configures the field to have a monaco editor.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="field"></param>
    /// <returns></returns>
    public static IEditorFieldConfig<TEntity, TValue> SetAsMonacoEditor<TEntity, TValue>(this IEditorFieldConfig<TEntity, TValue> field) where TEntity : IEntity
        => field.SetType(typeof(MonacoCodeEditor));

    /// <summary>
    /// Configures the field to have a monaco editor.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="field"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IEditorFieldConfig<TEntity, TValue> SetAsMonacoEditor<TEntity, TValue>(this IEditorFieldConfig<TEntity, TValue> field, MonacoEditorConfiguration config) where TEntity : IEntity
        => field.SetType(typeof(MonacoCodeEditor)).SetConfiguration(config);

    /// <summary>
    /// Configures the field to have a monaco editor.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="field"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IEditorFieldConfig<TEntity, TValue> SetAsMonacoEditor<TEntity, TValue>(this IEditorFieldConfig<TEntity, TValue> field, Func<TEntity, EntityState, MonacoEditorConfiguration?> config) where TEntity : IEntity
        => field.SetType(typeof(MonacoCodeEditor)).SetConfiguration(config);

    /// <summary>
    /// Configures the field to have a monaco editor.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="field"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IEditorFieldConfig<TEntity, TValue> SetAsMonacoEditor<TEntity, TValue>(this IEditorFieldConfig<TEntity, TValue> field, Func<TEntity, EntityState, Task<MonacoEditorConfiguration?>> config) where TEntity : IEntity
        => field.SetType(typeof(MonacoCodeEditor)).SetConfiguration(config);
}
