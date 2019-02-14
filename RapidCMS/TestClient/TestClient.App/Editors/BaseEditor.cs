using Microsoft.AspNetCore.Components;
using System;

namespace TestClient.App.Editors
{
    public class BaseEditor : ComponentBase
    {
        [Parameter]
        private string Value { get; set; }

        protected string LocalValue
        {
            get
            {
                return Value;
            }
            set
            {
                Value = value;

                Callback.Invoke(Value);
            }
        }

        [Parameter]
        private Action<string> Callback { get; set; }
    }
}
