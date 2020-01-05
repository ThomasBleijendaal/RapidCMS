using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.UI;
using RapidCMS.UI.Models;

namespace RapidCMS.UI.Components.Pages
{
    public abstract class BaseNode : BasePage
    {
        [Inject] protected IPersistenceService PersistenceService { get; set; }
        [Inject] protected IUIResolverFactory UIResolverFactory { get; set; }

        protected EditContext? EditContext;
        protected IEnumerable<ButtonUI>? Buttons;
        protected IEnumerable<SectionUI>? Sections;

        protected override async Task LoadDataAsync(IEnumerable<string>? reloadEntityIds = null)
        {
            try
            {
                var editContext = await PersistenceService.GetEntityAsync(GetUsageType(), CollectionAlias, VariantAlias, GetParentPath(), Id);
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

                var command = await PersistenceService.ProcessEntityActionAsync(
                    args.EditContext,
                    args.ViewModel.ButtonId,
                    args.Data);

                await HandleViewCommandAsync(command);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}
