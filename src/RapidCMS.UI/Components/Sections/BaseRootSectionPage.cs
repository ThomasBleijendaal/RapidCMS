using System.Threading.Tasks;
using RapidCMS.Core.Enums;

namespace RapidCMS.UI.Components.Sections
{
    public abstract partial class BaseRootSection
    {
        protected async Task LoadPageDataAsync()
        {
            PageContents = await PresentationService.GetPageAsync(CurrentState.PageType == PageType.Dashboard
                ? "__dashboard"
                : CurrentState.CollectionAlias);
        }
    }
}
