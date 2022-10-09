using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Extensions.Markdown
{
    public static class ConfigExtensions
    {
        public static IEditorFieldConfig<TEntity, TValue> SetAsMarkdownEditor<TEntity, TValue>(this IEditorFieldConfig<TEntity, TValue> field) where TEntity : IEntity
            => field.SetType(typeof(MarkdownEditor));
    }
}
