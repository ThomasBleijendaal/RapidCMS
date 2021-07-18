using System;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.UI.Configurability
{
    public class DefaultValueEditorConfig
    {
        private readonly Func<IEntity, object?> _defaultValue;

        public DefaultValueEditorConfig(Func<IEntity, object?> defaultValue)
        {
            _defaultValue = defaultValue;
        }

        public object? GetDefaultValue(IEntity entity) => _defaultValue.Invoke(entity);
    }

    public class DefaultValueEditorConfig<TEntity> : DefaultValueEditorConfig
        where TEntity : IEntity
    {
        public DefaultValueEditorConfig(Func<TEntity, object?> defaultValue) : base(entity => defaultValue.Invoke((TEntity)entity))
        {
        }
    }
}
