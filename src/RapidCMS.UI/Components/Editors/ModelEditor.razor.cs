using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.UI.Components.Editors
{
    public partial class ModelEditor
    {
        protected IEnumerable<FieldUI>? Fields { get; set; }

        protected FormEditContext? PropertyEditContext { get; set; }

        [Inject]
        private IUIResolverFactory UIResolverFactory { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            PropertyEditContext = EditContext.EntityProperty(Property);

            var nodeUI = await UIResolverFactory.GetConventionNodeUIResolverAsync(Property.PropertyType);

            var sections = await nodeUI.GetSectionsForEditContextAsync(PropertyEditContext);

            Fields = sections.FirstOrDefault()?.Elements?.OfType<FieldUI>();

            EditContext.OnValidationStateChanged += EditContext_OnValidationStateChanged;
            if (PropertyEditContext != null)
            {
                PropertyEditContext.OnValidationStateChanged += PropertyEditContext_OnValidationStateChanged;
            }

            await base.OnInitializedAsync();
        }

        protected override void DetachListener()
        {
            EditContext.OnValidationStateChanged -= EditContext_OnValidationStateChanged;
            if (PropertyEditContext != null)
            {
                PropertyEditContext.OnValidationStateChanged -= PropertyEditContext_OnValidationStateChanged;
            }

            base.DetachListener();
        }

        private void EditContext_OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
        {
            PropertyEditContext?.IsValidAsync();
        }

        private void PropertyEditContext_OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
        {
            if (!EditContext.IsValid(Property) && e.IsValid == true)
            {
                EditContext.IsValidAsync();
            }
        }
    }
}
