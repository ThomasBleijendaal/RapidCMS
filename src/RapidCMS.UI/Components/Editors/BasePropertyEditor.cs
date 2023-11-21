using System;
using System.ComponentModel;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Metadata;

namespace RapidCMS.UI.Components.Editors;

public class BasePropertyEditor : BaseEditor
{
    private new IFullPropertyMetadata Property
    {
        get
        {
            return base.Property as IFullPropertyMetadata ?? throw new InvalidOperationException($"{nameof(BasePropertyEditor)} requires usable Getter and Setter");
        }
    }

    protected async Task SetValueFromObjectAsync(object value)
    {
        if (!IsDisabled)
        {
            Property.Setter(Entity, value);
        }

        await EditContext.NotifyPropertyChangedAsync(Property);
    }

    protected async Task SetValueFromStringAsync(string value)
    {
        if (Property.PropertyType == typeof(object))
        {
            await SetValueFromObjectAsync(value);
        }
        else
        {
            var obj = TypeDescriptor.GetConverter(Property.PropertyType).ConvertFromString(value);
            await SetValueFromObjectAsync(obj!);
        }
    }
}
