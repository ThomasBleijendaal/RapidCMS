using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.UI;
using RapidCMS.UI.Models;
using RapidCMS.Core.Models.Request;
using RapidCMS.Core.Models.Response;

namespace RapidCMS.UI.Components.Pages
{
    public abstract class BaseNode : BasePage
    {
        [Inject] protected IPresentationService PresentationService { get; set; } = default!;
        [Inject] protected IInteractionService InteractionService { get; set; } = default!;
        [Inject] protected IUIResolverFactory UIResolverFactory { get; set; } = default!;

        protected EditContext? EditContext { get; set; } 
        protected IEnumerable<ButtonUI>? Buttons { get; set; }
        protected IEnumerable<SectionUI>? Sections { get; set; }

        protected override async Task LoadDataAsync(IEnumerable<string>? reloadEntityIds = null)
        {
            try
            {
                var editContext = await PresentationService.GetEntityAsync(new GetEntityRequestModel
                {
                    CollectionAlias = CollectionAlias,
                    Id = Id,
                    ParentPath = GetParentPath(),
                    UsageType = GetUsageType(),
                    VariantAlias = VariantAlias
                });

                var resolver = await UIResolverFactory.GetNodeUIResolverAsync(GetUsageType(), CollectionAlias);

                Buttons = await resolver.GetButtonsForEditContextAsync(editContext);
                Sections = await resolver.GetSectionsForEditContextAsync(editContext);

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
                });

                await HandleViewCommandAsync(command);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}
