using System;
using RapidCMS.Common.Models.Metadata;
using RapidCMS.Common.Validation;


namespace RapidCMS.UI.Components.Editors
{
    public class BasePropertyEditor : BaseEditor
    {
        private new IFullPropertyMetadata Property
        {
            get
            {
                return base.Property as IFullPropertyMetadata ?? throw new InvalidOperationException($"{nameof(BasePropertyEditor)} requires usable Getter and Setter");
            }
        }

        protected void SetValue(object value, bool useValueMapper = true)
        {
            if (useValueMapper)
            {
                Property.Setter(Entity, ValueMapper.MapFromEditor(null, value));
            }
            else
            {
                Property.Setter(Entity, value);
            }

            EditContext.NotifyPropertyChanged(Property);
        }
    }
}
