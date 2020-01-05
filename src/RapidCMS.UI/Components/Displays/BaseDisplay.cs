using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;

namespace RapidCMS.UI.Components.Displays
{
    public class BaseDisplay : ComponentBase
    {
        [Parameter] public IEntity Entity { get; set; }
        [Parameter] public IParent? Parent { get; set; }

        [Parameter] public EntityState EntityState { get; set; }

        [Parameter] public IExpressionMetadata Expression { get; set; }

        protected string GetValueAsString()
        {
            return Expression.StringGetter(Entity);
        }
    }
}
