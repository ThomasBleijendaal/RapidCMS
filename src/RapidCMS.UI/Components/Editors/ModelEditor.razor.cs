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
            // TODO: this does not proxy validation errors from PropertyEditContext to EditContext and reversed
            PropertyEditContext = EditContext.EntityProperty(Property);

            PropertyEditContext.OnValidationStateChanged += PropertyEditContext_OnValidationStateChanged;

            var nodeUI = await UIResolverFactory.GetConventionNodeUIResolverAsync(Property.PropertyType);

            var sections = await nodeUI.GetSectionsForEditContextAsync(PropertyEditContext);

            Fields = sections.FirstOrDefault()?.Elements?.OfType<FieldUI>();

            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    if (field.Property != null)
                    {
                        EditContext.NotifyPropertyIncludedInForm(field.Property);
                    }
                }
            }
            await base.OnInitializedAsync();
        }

        private void PropertyEditContext_OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
        {
        }
    }
}
