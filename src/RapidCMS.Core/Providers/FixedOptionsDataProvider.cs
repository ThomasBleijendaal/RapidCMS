using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Providers;

public class FixedOptionsDataProvider : IDataCollection
{
    private readonly IEnumerable<(object id, string label)> _options;

    public FixedOptionsDataProvider(IEnumerable<string>? options)
    {
        _options = options?.Select(x => (x as object, x)) ?? Enumerable.Empty<(object, string)>();
    }

    public FixedOptionsDataProvider(IEnumerable<(object, string)>? options)
    {
        _options = options ?? Enumerable.Empty<(object, string)>();
    }

    public void Configure(object configuration) { }

#pragma warning disable CS0067
    public event EventHandler? OnDataChange;
#pragma warning restore CS0067

    public void Dispose()
    {
    }

    public Task<IReadOnlyList<IElement>> GetAvailableElementsAsync(IView view)
    {
        return Task.FromResult<IReadOnlyList<IElement>>(
            _options
            .Select(item => new Element
            {
                Id = item.id,
                Labels = new[] { item.label }
            })
            .ToList());
    }

    public Task SetEntityAsync(FormEditContext editContext, IPropertyMetadata property, IParent? parent)
    {
        return Task.CompletedTask;
    }
}
