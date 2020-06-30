using System;
using System.Threading.Tasks;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Request.Form;
using RapidCMS.Core.Models.Response;
using RapidCMS.UI.Models;

namespace RapidCMS.UI.Components.Sections
{
    public abstract partial class BaseRootSection
    {
        protected EditContext? EditContext { get; set; }

        protected async Task LoadNodeDataAsync()
        {
            try
            {
                if (CurrentState == null)
                {
                    throw new InvalidOperationException();
                }

                var editContext = await PresentationService.GetEntityAsync<GetEntityRequestModel, EditContext>(new GetEntityRequestModel
                {
                    CollectionAlias = CurrentState.CollectionAlias,
                    Id = CurrentState.Id,
                    ParentPath = CurrentState.ParentPath,
                    UsageType = CurrentState.UsageType,
                    VariantAlias = CurrentState.VariantAlias
                });

                var resolver = await UIResolverFactory.GetNodeUIResolverAsync(CurrentState.UsageType, CurrentState.CollectionAlias);

                Buttons = await resolver.GetButtonsForEditContextAsync(editContext);
                Sections = new[] { (editContext, await resolver.GetSectionsForEditContextAsync(editContext)) };

                EditContext = editContext;

                EditContext.OnFieldChanged += (s, a) => StateHasChanged();

                StateHasChanged();
            }
            catch
            {
                EditContext = null;

                throw;
            }
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
                HandleException(ex);
            }
        }
    }
}
