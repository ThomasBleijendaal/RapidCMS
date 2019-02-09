using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;

namespace TestClient.App.Editors
{
    public class BaseEditor : ComponentBase
    {
        [Parameter]
        protected string Alias { get; set; }

        [Parameter]
        protected object Value { get; set; }
    }
}
