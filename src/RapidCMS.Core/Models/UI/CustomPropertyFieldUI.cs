using System;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Providers;

namespace RapidCMS.Core.Models.UI
{
    public class CustomPropertyFieldUI : PropertyFieldUI
    {
        internal CustomPropertyFieldUI(CustomPropertyFieldSetup field, DataProvider? dataProvider) : base(field, dataProvider)
        {
            CustomType = field.CustomType;
        }

        public Type CustomType { get; private set; }
    }
}
