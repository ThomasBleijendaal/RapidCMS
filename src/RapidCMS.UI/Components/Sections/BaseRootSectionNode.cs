using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.EventArgs.Mediators;
using RapidCMS.Core.Models.Queries;
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

            Buttons = null;
            Sections = null;

            var result = await Mediator.SendAsync(new GetEntityQuery(
                CurrentNavigationState.CollectionAlias,
                CurrentNavigationState.VariantAlias,
                CurrentNavigationState.Id,
                CurrentNavigationState.ParentPath,
                CurrentNavigationState.UsageType));

            var editContext = new FormEditContext(
                result.CollectionAlias,
                result.RepositoryAlias,
                result.EntityVariantAlias,
                result.Entity,
                result.Parent,
                result.UsageType,
                result.Validators,
                ServiceProvider);

            var resolver = await UIResolverFactory.GetNodeUIResolverAsync(CurrentNavigationState);

            var buttons = await resolver.GetButtonsForEditContextAsync(editContext);

            var nodeSection = await resolver.GetSectionsForEditContextAsync(editContext, CurrentNavigationState);
            var sections = new[] { (editContext, nodeSection) }.ToList();

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
                PageContents = default;
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
                        EditContext = args.EditContext,
                        NavigationState = CurrentNavigationState
                    }));
            }
            catch (Exception ex)
            {
                Mediator.NotifyEvent(this, new ExceptionEventArgs(ex));
            }
        }
    }
}
