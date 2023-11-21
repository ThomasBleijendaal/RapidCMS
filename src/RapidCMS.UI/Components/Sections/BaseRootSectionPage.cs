using System;
using System.Threading;
using System.Threading.Tasks;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Request.Form;

namespace RapidCMS.UI.Components.Sections;

public abstract partial class BaseRootSection
{


    protected async Task LoadPageDataAsync(CancellationToken cancellationToken)
    {
        if (CurrentNavigationState == null)
        {
            throw new InvalidOperationException();
        }

        PageContents = default;

        var pageAlias = CurrentNavigationState.PageType == PageType.Dashboard
            ? "__dashboard"
            : CurrentNavigationState.CollectionAlias;

        var pageContext = await PresentationService.GetEntityAsync<GetEntityOfPageRequestModel, PageContext>(new GetEntityOfPageRequestModel
        {
            PageAlias = pageAlias,
            ParentPath = CurrentNavigationState.ParentPath
        });

        var contents = await PresentationService.GetPageAsync(pageAlias);

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        PageContents = (pageContext, contents);
    }
}
