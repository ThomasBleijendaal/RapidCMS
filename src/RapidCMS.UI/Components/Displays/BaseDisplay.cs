using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Abstractions.UI;
using RapidCMS.Core.Enums;

namespace RapidCMS.UI.Components.Displays
{
    public class BaseDisplay : ComponentBase, IBaseUIElement
    {
        [Parameter] public IEntity Entity { get; set; } = default!;
        [Parameter] public IParent? Parent { get; set; }

        [Parameter] public EntityState EntityState { get; set; }
        [Parameter] public ListType DisplayType { get; set; } 

        [Parameter] public IExpressionMetadata Expression { get; set; } = default!;

        /// <summary>
        /// Implement the IWantConfiguration or IRequireConfiguration interfaces to 
        /// leverage the GetConfigAsync extension method so handling configuration is more easy.
        /// </summary>
        [Parameter] public Func<object, EntityState, Task<object?>>? Configuration { get; set; }

        protected string GetValueAsString()
        {
            return Expression.StringGetter(Entity);
        }
    }
}
