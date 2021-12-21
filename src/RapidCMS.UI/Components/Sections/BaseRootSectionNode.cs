using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.EventArgs.Mediators;
using RapidCMS.Core.Models.Request.Form;
using RapidCMS.Core.Models.Response;
using RapidCMS.UI.Models;

namespace RapidCMS.UI.Components.Sections
{
    public abstract partial class BaseRootSection
    {
        protected async Task LoadNodeDataAsync(CancellationToken cancellationToken)
        {
            if (CurrentNavigationState == null)
            {
                throw new InvalidOperationException();
            }

            var editContext = await PresentationService.GetEntityAsync<GetEntityRequestModel, FormEditContext>(new GetEntityRequestModel
            {
                CollectionAlias = CurrentNavigationState.CollectionAlias,
                Id = CurrentNavigationState.Id,
                ParentPath = CurrentNavigationState.ParentPath,
                UsageType = CurrentNavigationState.UsageType,
                VariantAlias = CurrentNavigationState.VariantAlias ?? throw new InvalidOperationException()
            });

            var resolver = await UIResolverFactory.GetNodeUIResolverAsync(CurrentNavigationState.UsageType, CurrentNavigationState.CollectionAlias);

            var buttons = await resolver.GetButtonsForEditContextAsync(editContext);
            var sections = new[] { (editContext, await resolver.GetSectionsForEditContextAsync(editContext)) }.ToList();

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            try
            {
                Buttons = buttons;
                Sections = sections;

                ListContext = null;
                Tabs = null;
                ListUI = null;
                PageContents = null;
            }
            catch
            {
            }

            StateHasChanged();
        }

        protected async Task ButtonOnClickAsync(ButtonClickEventArgs args)
        {
            try
            {
                if (args.ViewModel == null)
                {
                    throw new ArgumentException($"ViewModel required");
                }

                await HandleViewCommandAsync(
                    () => InteractionService.InteractAsync<PersistEntityRequestModel, NodeViewCommandResponseModel>(new PersistEntityRequestModel
                    {
                        ActionId = args.ViewModel.ButtonId,
                        CustomData = args.Data,
                        EditContext = args.EditContext
                    }));
            }
            catch (Exception ex)
            {
                Mediator.NotifyEvent(this, new ExceptionEventArgs(ex));
            }
        }
    }
}
