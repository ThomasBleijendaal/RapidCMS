using System;
using System.Collections.Generic;
using RapidCMS.Core.Helpers;

namespace RapidCMS.Core.Models.ApiBridge;

public static class EntityModel
{
    public static EntityModel<T> Create<T>(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        return new EntityModel<T>
        {
            Entity = entity,
            VariantAlias = AliasHelper.GetEntityVariantAlias(entity.GetType())
        };
    }

    public static IEnumerable<EntityModel<T>> Create<T>(IEnumerable<T> entities)
    {
        if (entities == null)
        {
            throw new ArgumentNullException(nameof(entities));
        }

        foreach (var entity in entities)
        {
            yield return Create(entity);
        }
    }
}

public class EntityModel<TEntity>
{
    public TEntity Entity { get; set; } = default!;
    public string VariantAlias { get; set; } = default!;
}
