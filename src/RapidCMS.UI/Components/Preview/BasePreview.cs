using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;

namespace RapidCMS.UI.Components.Preview
{
    public class BasePreview : ComponentBase
    {
        [Parameter]
        public object? PreviewValue { get; set; }
    }
}
