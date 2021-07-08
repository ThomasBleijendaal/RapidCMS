using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;

namespace RapidCMS.UI.Components.Sections
{
    public abstract partial class BaseRootSection
    {
        [Inject] private ILogin LoginRegistration { get; set; }

        protected async Task LoadPageDataAsync(CancellationToken cancellationToken)
        {
            var contents = await PresentationService.GetPageAsync(CurrentState.PageType == PageType.Dashboard
                ? "__dashboard"
                : CurrentState.CollectionAlias);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            PageContents = contents;
        }
    }
}
