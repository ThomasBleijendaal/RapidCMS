using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Data;

namespace RapidCMS.UI.Components.Editors
{
    public class BaseRelationEditor : BaseEditor
    {
        [Parameter] public IDataCollection? DataCollection { get; set; }

        public IRelationDataCollection? RelationDataCollection => DataCollection as IRelationDataCollection;
    }
}
