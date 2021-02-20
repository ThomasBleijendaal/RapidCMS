using System;
using System.Threading;
using System.Threading.Tasks;
using RapidCMS.Core.Enums;

namespace RapidCMS.UI.Components.Sections
{
    public abstract partial class BaseRootSection
    {
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
