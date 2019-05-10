using System;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Data;

#nullable enable

namespace RapidCMS.UI.Components.Editors
{
    public class BaseEditor<TValue> : ComponentBase
    {
        // TODO: move to two methods: one for getting and one for setting the value of this editor

        [Parameter]
        public TValue EditorValue { get; private set; }

        protected TValue Value
        {
            get
            {
                return EditorValue;
            }
            set
            {
                EditorValue = value;

                Callback?.Invoke(EditorValue);
            }
        }

        [Parameter]
        public Action<TValue>? Callback { get; private set; }

        [Parameter]
        protected IDataProvider? DataProvider { get; set; }
    }
}
