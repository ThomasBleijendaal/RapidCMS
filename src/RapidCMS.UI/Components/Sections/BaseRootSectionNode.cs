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
            if (CurrentState == null)
            {
                throw new InvalidOperationException();
            }

            var editContext = await PresentationService.GetEntityAsync<GetEntityRequestModel, FormEditContext>(new GetEntityRequestModel
            {
                CollectionAlias = CurrentState.CollectionAlias,
                Id = CurrentState.Id,
                ParentPath = CurrentState.ParentPath,
                UsageType = CurrentState.UsageType,
                VariantAlias = CurrentState.VariantAlias
            });

            var resolver = await UIResolverFactory.GetNodeUIResolverAsync(CurrentState.UsageType, CurrentState.CollectionAlias);

            var buttons = await resolver.GetButtonsForEditContextAsync(editContext);
            var sections = new[] { (editContext, await resolver.GetSectionsForEditContextAsync(editContext)) }.ToList();

            if (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("CANCEL");
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

                editContext.OnFieldChanged += (s, a) => StateHasChanged();
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

                var command = await InteractionService.InteractAsync<PersistEntityRequestModel, NodeViewCommandResponseModel>(new PersistEntityRequestModel
                {
                    ActionId = args.ViewModel.ButtonId,
                    CustomData = args.Data,
                    EditContext = args.EditContext
                }, CurrentViewState);

                await HandleViewCommandAsync(command);
            }
            catch (Exception ex)
            {
                Mediator.NotifyEvent(this, new ExceptionEventArgs(ex));
            }
        }
    }
}
