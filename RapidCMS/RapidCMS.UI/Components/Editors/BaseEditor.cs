using System;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models;
using RapidCMS.Common.ValueMappers;

#nullable enable

namespace RapidCMS.UI.Components.Editors
{
    public class BaseEditor<TValue> : ComponentBase
    {
        [Parameter]
        public IEntity Entity { get; private set; }

        [Parameter]
        public IPropertyMetadata Property { get; private set; }

        [Parameter]
        public IValueMapper ValueMapper { get; private set; }

        [Parameter]
        public IDataProvider? DataProvider { get; private set; }
    }
}
