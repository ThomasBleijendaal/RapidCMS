using System;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models.Metadata;
using RapidCMS.Common.ValueMappers;


namespace RapidCMS.UI.Components.Editors
{
    public class BaseEditor : ComponentBase
    {
        [Parameter]
        public IEntity Entity { get; private set; }

        [Parameter]
        public IPropertyMetadata Property { get; private set; }

        [Parameter]
        public IValueMapper ValueMapper { get; private set; }
    }

    public class BasePropertyEditor : BaseEditor
    {
        public new IFullPropertyMetadata Property
        {
            get
            {
                return base.Property as IFullPropertyMetadata ?? throw new InvalidOperationException($"{nameof(BasePropertyEditor)} requires usable Getter and Setter");
            }
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
