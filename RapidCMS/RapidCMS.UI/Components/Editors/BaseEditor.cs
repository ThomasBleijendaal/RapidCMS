using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models;
using RapidCMS.Common.ValueMappers;

#nullable enable

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

    public class BaseDataEditor : BaseEditor
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
