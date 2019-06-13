using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Data;


namespace RapidCMS.UI.Components.Editors
{
    public class BaseDataEditor : BasePropertyEditor
    {
        [Parameter]
        public IDataCollection? DataCollection { get; private set; }
    }
}
