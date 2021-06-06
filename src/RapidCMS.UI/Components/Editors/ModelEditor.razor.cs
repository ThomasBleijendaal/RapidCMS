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
        protected SectionUI? Section { get; set; }

        protected FormEditContext? PropertyEditContext { get; set; }

        [Inject]
        private IUIResolverFactory UIResolverFactory { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            PropertyEditContext = EditContext.EntityProperty(Property);

            var nodeUI = await UIResolverFactory.GetConventionNodeUIResolverAsync(Property.PropertyType);

            var sections = await nodeUI.GetSectionsForEditContextAsync(PropertyEditContext);

            Section = sections.FirstOrDefault();

            await base.OnInitializedAsync();
        }
    }
}
