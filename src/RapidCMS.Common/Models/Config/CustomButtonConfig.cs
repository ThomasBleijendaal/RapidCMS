using System;

namespace RapidCMS.Common.Models.Config
{
    public class CustomButtonConfig : ButtonConfig
    {
        public CustomButtonConfig(Type customButtonType, Type actionHandler)
        {
            CustomType = customButtonType ?? throw new ArgumentNullException(nameof(customButtonType));
            ActionHandler = actionHandler ?? throw new ArgumentNullException(nameof(actionHandler));
        }

        internal Type CustomType { get; set; }
        internal Type ActionHandler { get; set; }
    }
}
