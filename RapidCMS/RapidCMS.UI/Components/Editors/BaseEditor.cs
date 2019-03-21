using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Interfaces;
using System;

#nullable enable

namespace RapidCMS.UI.Components.Editors
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

        [Parameter]
        protected IDataProvider? DataProvider { get; set; }
    }
}
