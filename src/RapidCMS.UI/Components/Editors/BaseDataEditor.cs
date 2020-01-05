using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.UI.Components.Editors
{
    public class BaseDataEditor : BasePropertyEditor
    {
        [Parameter] public IDataCollection? DataCollection { get; set; }
    }
}
