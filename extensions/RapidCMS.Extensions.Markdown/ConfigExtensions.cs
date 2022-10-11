using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;

namespace RapidCMS.Extensions.Markdown;

public static class ConfigExtensions
{
    /// <summary>
    /// Configures the field to have a markdown editor.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="field"></param>
    /// <returns></returns>
    public static IEditorFieldConfig<TEntity, TValue> SetAsMarkdownEditor<TEntity, TValue>(this IEditorFieldConfig<TEntity, TValue> field) where TEntity : IEntity
        => field.SetType(typeof(MarkdownEditor));

    /// <summary>
    /// Configures the field to have a markdown editor.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="field"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IEditorFieldConfig<TEntity, TValue> SetAsMarkdownEditor<TEntity, TValue>(this IEditorFieldConfig<TEntity, TValue> field, MarkdownEditorConfiguration config) where TEntity : IEntity
        => field.SetType(typeof(MarkdownEditor)).SetConfiguration(config);

    /// <summary>
    /// Configures the field to have a markdown editor.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="field"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IEditorFieldConfig<TEntity, TValue> SetAsMarkdownEditor<TEntity, TValue>(this IEditorFieldConfig<TEntity, TValue> field, Func<TEntity, EntityState, MarkdownEditorConfiguration?> config) where TEntity : IEntity
        => field.SetType(typeof(MarkdownEditor)).SetConfiguration(config);

    /// <summary>
    /// Configures the field to have a markdown editor.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="field"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IEditorFieldConfig<TEntity, TValue> SetAsMarkdownEditor<TEntity, TValue>(this IEditorFieldConfig<TEntity, TValue> field, Func<TEntity, EntityState, Task<MarkdownEditorConfiguration?>> config) where TEntity : IEntity
        => field.SetType(typeof(MarkdownEditor)).SetConfiguration(config);
}
