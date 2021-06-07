﻿using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Providers;

namespace RapidCMS.Core.Models.UI
{
    public class PropertyFieldUI : FieldUI
    {
        internal PropertyFieldUI(PropertyFieldSetup field, FormDataProvider? dataProvider) : base(field)
        {
            Type = field.EditorType;
            Property = field.Property;
            DataCollection = dataProvider?.Collection;
        }

        public EditorType Type { get; internal set; }

        public IDataCollection? DataCollection { get; internal set; }
    }
}
