using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.UI.Components.Editors;

public abstract class BasePicker : BaseDataEditor
{
    protected string? _searchTerm;
    protected int _currentPage = 1;
    protected int? _maxPage;

    protected string _group = Guid.NewGuid().ToString("n");
    protected string ElementId => $"picker-{_group}";

    protected IEnumerable<IElement>? _options;
    protected List<IElement> _selectedElements = new();
    protected CancellationTokenSource _cts = new();

    protected virtual bool IsMultiple { get; set; }

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    private IRelationDataCollection RelationDataCollection
        => DataCollection as IRelationDataCollection 
            ?? throw new InvalidOperationException("Incorrect DataCollection assigned to Entity/iesPicker");

    protected override async Task OnInitializedAsync()
    {
        if (DataCollection != null)
        {
            DataCollection.OnDataChange += UpdateOptionsAsync;

            await DataCollection.SetEntityAsync(EditContext, Property, Parent);
            await UpdateOptionsAsync();
        }
    }

    private async void UpdateOptionsAsync(object? sender, EventArgs args)
    {
        if (DataCollection == null)
        {
            return;
        }

        await InvokeAsync(async () =>
        {
            var currentValue = GetValueAsObject();

            await UpdateOptionsAsync();

            if (currentValue != null && _options != null && !_options.Any(x => x.Id.Equals(currentValue)))
            {
                await SetValueFromObjectAsync(default!);
            }

            StateHasChanged();
        });
    }

    protected async Task PageChangedAsync(int page)
    {
        _currentPage = page;

        await UpdateOptionsAsync();

        await JsRuntime.InvokeVoidAsync("RapidCMS.scrollToTop", ElementId);
    }

    protected async Task ResetViewAsync()
    {
        _searchTerm = "";
        _currentPage = 1;
        _maxPage = null;

        await UpdateOptionsAsync();
    }

    private async Task UpdateOptionsAsync()
    {
        if (DataCollection == null)
        {
            return;
        }

        var view = View.Create(25, _currentPage, _searchTerm, default);
        _options = await DataCollection.GetAvailableElementsAsync(view);

        if (view.MoreDataAvailable)
        {
            _maxPage = null;
        }
        else
        {
            _maxPage = _currentPage;
        }
    }

    protected bool IsSelected(object id)
    {
        if (IsMultiple)
        {
            return RelationDataCollection.IsRelated(id);
        }
        else
        {
            return GetValueAsObject()?.Equals(id) ?? false;
        }
    }

    protected async Task SelectElementAsync(object id, bool? selected)
    {
        if (_options == null)
        {
            return;
        }

        if (IsMultiple)
        {
            if (!IsDisabled)
            {
                if (selected == true)
                {
                    RelationDataCollection.AddElement(id);
                }
                else
                {
                    RelationDataCollection.RemoveElement(id);
                }

                await EditContext.NotifyPropertyChangedAsync(Property);
            }
        }
        else
        {
            await SetValueFromObjectAsync(id);
        }
    }

    public override void Dispose()
    {
        base.Dispose();

        if (DataCollection != null)
        {
            DataCollection.OnDataChange -= UpdateOptionsAsync;
            DataCollection.Dispose();
        }
    }

    protected async void SearchAsync(string searchValue)
    {
        _cts.Cancel();
        _cts = new();

        await Task.Delay(300);

        if (!_cts.IsCancellationRequested)
        {
            _searchTerm = searchValue;
            _currentPage = 1;

            await UpdateOptionsAsync();

            StateHasChanged();
        }
    }
}
