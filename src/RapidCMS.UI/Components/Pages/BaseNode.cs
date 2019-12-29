using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Models.UI;
using RapidCMS.Common.Services;
using RapidCMS.Common.Services.UI;
using RapidCMS.UI.Models;

namespace RapidCMS.UI.Components.Pages
{
    public abstract class BaseNode : BasePage
    {
        [Inject] private IEditContextService EditContextService { get; set; }
        [Inject] private IEditorService EditorService { get; set; }

        protected EditContext? EditContext;
        protected IEnumerable<ButtonUI>? Buttons;
        protected IEnumerable<SectionUI>? Sections;

        protected override async Task LoadDataAsync(IEnumerable<string>? reloadEntityIds = null)
        {
            try
            {
                var editContext = await EditContextService.GetEntityAsync(GetUsageType(), CollectionAlias, VariantAlias, GetParentPath(), Id);
                var resolver = await EditorService.GetNodeUIResolverAsync(GetUsageType(), CollectionAlias);

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

                var command = await EditContextService.ProcessEntityActionAsync(
                    GetUsageType(),
                    CollectionAlias,
                    GetParentPath(),
                    Id,
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
