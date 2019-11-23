using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models.Metadata;

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
