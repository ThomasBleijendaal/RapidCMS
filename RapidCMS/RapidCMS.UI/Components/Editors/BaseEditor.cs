using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models.Metadata;
using RapidCMS.Common.Validation;
using RapidCMS.Common.ValueMappers;


namespace RapidCMS.UI.Components.Editors
{
    public class BaseEditor : ComponentBase
    {
        [Parameter]
        protected IEntity Entity { get; private set; }

        [Parameter]
        protected IPropertyMetadata Property { get; private set; }

        [Parameter]
        protected IValueMapper ValueMapper { get; private set; }

        protected object GetValue(bool useValueMapper = true)
        {
            if (useValueMapper)
            {
                return ValueMapper.MapToEditor(null, Property.Getter(Entity));
            }
            else
            {
                return Property.Getter(Entity);
            }
        }
    }

    public class BasePropertyEditor : BaseEditor
    {
        [CascadingParameter(Name = "EditContext")]
        private EditContext CascadedEditContext { get; set; }

        protected EditContext EditContext { get; set; }

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

            EditContext.NotifyFieldChanged(Property);
        }

        protected override Task OnParametersSetAsync()
        {
            if (EditContext == null)
            {
                EditContext = CascadedEditContext;
            }
            else if (EditContext != CascadedEditContext)
            {
                throw new InvalidOperationException($"{GetType()} does not support changing the {nameof(EditContext)} dynamically.");
            }

            return base.OnParametersSetAsync();
        }
    }

    public class BaseDataEditor : BasePropertyEditor
    {
        [Parameter]
        public IDataCollection? DataCollection { get; private set; }
    }

    public class BaseRelationEditor : BaseEditor
    {
        [Parameter]
        private IDataCollection? DataCollection { get; set; }

        public IRelationDataCollection? RelationDataCollection => DataCollection as IRelationDataCollection;
    }
}
