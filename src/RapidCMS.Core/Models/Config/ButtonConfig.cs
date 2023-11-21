using System;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Config;

internal class ButtonConfig
{
    internal string Id { get; set; } = Guid.NewGuid().ToString();
    internal string? Label { get; set; }
    internal string? Icon { get; set; }
    internal bool IsPrimary { get; set; }
    internal Func<object, EntityState, bool>? IsVisible { get; private set; }

    internal ButtonConfig VisibleWhen(Func<IEntity, EntityState, bool>? predicate)
    {
        if (predicate != null)
        {
            IsVisible = (entity, state) => predicate.Invoke((IEntity)entity, state);
        }

        return this;
    }
}
