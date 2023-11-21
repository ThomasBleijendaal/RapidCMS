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

public class EnumRelationDataProvider<TEnum> : IRelationDataCollection
    where TEnum : Enum
{
    private readonly List<TEnum> _selectedEnums = new();
    private readonly List<Element> _elements;

    public EnumRelationDataProvider()
    {
        var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

        _elements = new List<Element>();

        foreach (var value in values)
        {
            var attribute = value.GetCustomAttribute<DisplayAttribute>();

            if (attribute != null)
            {
                _elements.Add(new Element
                {
                    Id = value,
                    Labels = new[] { attribute.Name ?? string.Empty }
                });
            }
            else
            {
                _elements.Add(new Element
                {
                    Id = value,
                    Labels = new[] { value.ToString() }
                });
            }
        }
    }

#pragma warning disable CS0067
    public event EventHandler? OnDataChange;
#pragma warning restore CS0067

    public void Configure(object configuration) { }

    public Task<IReadOnlyList<IElement>> GetAvailableElementsAsync(IView view)
        => Task.FromResult<IReadOnlyList<IElement>>(_elements);

    public Task SetEntityAsync(FormEditContext editContext, IPropertyMetadata property, IParent? parent)
    {
        var data = property.Getter(editContext.Entity);
        if (data is IEnumerable<TEnum> enumCollection)
        {
            _selectedEnums.AddRange(enumCollection);
        }
        else if (data is IEnumerable<object> objectCollection)
        {
            _selectedEnums.AddRange(objectCollection.Cast<TEnum>());
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {

    }

    public Task<IReadOnlyList<IElement>> GetRelatedElementsAsync()
        => Task.FromResult<IReadOnlyList<IElement>>(_elements.Where(x => _selectedEnums.Contains((TEnum)x.Id)).ToList());

    public void AddElement(object id)
        => _selectedEnums.Add((TEnum)id);

    public void RemoveElement(object id)
        => _selectedEnums.Remove((TEnum)id);

    public bool IsRelated(object id)
        => _selectedEnums.Contains((TEnum)id);

    public IReadOnlyList<object> GetCurrentRelatedElementIds()
        => _selectedEnums.Cast<object>().ToList();

    public Type GetRelatedEntityType()
        => typeof(TEnum);
}
