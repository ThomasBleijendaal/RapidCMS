using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;

namespace RapidCMS.UI.Components.Displays
{
    public class BaseDisplay : ComponentBase
    {
        [Parameter] public IEntity Entity { get; set; } = default!;
        [Parameter] public IParent? Parent { get; set; }

        [Parameter] public EntityState EntityState { get; set; }

        [Parameter] public IExpressionMetadata Expression { get; set; } = default!;

        protected string GetValueAsString()
        {
            return Expression.StringGetter(Entity);
        }
    }
}
