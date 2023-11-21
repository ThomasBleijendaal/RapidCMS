using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Providers;

public class EnumDataProvider<TEnum> : IDataCollection
    where TEnum : Enum
{
#pragma warning disable CS0067
    public event EventHandler? OnDataChange;
#pragma warning restore CS0067

    public void Configure(object configuration) { }

    public Task<IReadOnlyList<IElement>> GetAvailableElementsAsync(IView view)
    {
        var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

        var list = new List<IElement>();

        foreach (var value in values)
        {
            var attribute = value.GetCustomAttribute<DisplayAttribute>();

            if (attribute != null)
            {
                list.Add(new Element
                {
                    Id = value,
                    Labels = new[] { attribute.Name ?? string.Empty }
                });
            }
            else
            {
                list.Add(new Element
                {
                    Id = value,
                    Labels = new[] { value.ToString() }
                });
            }
        }

        return Task.FromResult<IReadOnlyList<IElement>>(list);
    }

    public Task SetEntityAsync(FormEditContext editContext, IPropertyMetadata property, IParent? parent)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {

    }
}
