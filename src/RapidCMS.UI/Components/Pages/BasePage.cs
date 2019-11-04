using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RapidCMS.Common;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Exceptions;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Models.Commands;

namespace RapidCMS.UI.Components.Pages
{
    public abstract class BasePage : ComponentBase
    {
        private UpdateParameterCommand? _previousParameterCommand = null;

        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private IExceptionHelper ExceptionHelper { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }

        [Parameter] public string Action { get; set; }
        [Parameter] public string CollectionAlias { get; set; }
        [Parameter] public string VariantAlias { get; set; }
        [Parameter] public string? Path { get; set; } = null;
        [Parameter] public string? Id { get; set; } = null;

        protected ParentPath? GetParentPath()
        {
            return ParentPath.TryParse(Path);
        }

        protected async Task HandleViewCommandAsync(ViewCommand command)
        {
            try
            {
                if (command == null)
                {
                    return;
                }

                if (command is ReturnCommand)
                {
                    if (_previousParameterCommand != null)
                    {
                        command = _previousParameterCommand;
                    }
                    else
                    {
                        command = new NavigateBackCommand();
                    }
                }

                switch (command)
                {
                    case NavigateCommand navigateCommand:

                        NavigationManager.NavigateTo(navigateCommand.Uri);

                        break;

                    case NavigateBackCommand navigateBackCommand:

                        // TODO: improve this
                        await JSRuntime.InvokeAsync<bool>("history.back");

                        break;

                    case UpdateParameterCommand parameterCommand:

                        _previousParameterCommand = new UpdateParameterCommand
                        {
                            Action = Action,
                            CollectionAlias = CollectionAlias,
                            Id = Id,
                            ParentPath = Path,
                            VariantAlias = VariantAlias
                        };

                        var data = new Dictionary<string, object>()
                        {
                            { nameof(Action), parameterCommand.Action },
                            { nameof(CollectionAlias), parameterCommand.CollectionAlias }
                        };

                        if (parameterCommand.VariantAlias != null)
                        {
                            data.Add(nameof(VariantAlias), parameterCommand.VariantAlias);
                        }

                        if (parameterCommand.Id != null)
                        {
                            data.Add(nameof(Id), parameterCommand.Id);
                        }

                        if (parameterCommand.ParentPath != null)
                        {
                            data.Add(nameof(Path), parameterCommand.ParentPath);
                        }

                        var update = ParameterView.FromDictionary(data);
                        await SetParametersAsync(update);

                        break;

                    case ReloadCommand reloadCommand:
                        await LoadDataAsync(reloadCommand.EntityId);

                        break;

                    case NoOperationCommand _:
                    default:

                        StateHasChanged();

                        break;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected void HandleException(Exception ex)
        {
            if (ex is UnauthorizedAccessException)
            {
                NavigationManager.NavigateTo("/unauthorized");
            }
            else if (ex is InvalidEntityException)
            {
                // trigger validation since entity is invalid
                // EditContext.IsValid();
            }
            else
            {
                ExceptionHelper.StoreException(ex);

                NavigationManager.NavigateTo("/error");
            }
        }

        protected virtual Task LoadDataAsync(IEnumerable<string>? reloadEntityIds = null)
        {
            return Task.CompletedTask;
        }

        protected UsageType GetUsageType()
        {
            return Action switch
            {
                Constants.Edit => UsageType.Edit,
                Constants.New => UsageType.New,
                Constants.Add => UsageType.Add,
                Constants.View => UsageType.View,
                Constants.List => UsageType.List,
                Constants.Pick => UsageType.Pick,
                _ => (UsageType)0
            };
        }
    }
}
