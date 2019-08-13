using System;

namespace RapidCMS.Common.Models.Config
{
    public class CustomButtonConfig : ButtonConfig
    {
        public CustomButtonConfig(Type customButtonType, Type actionHandler)
        {
            Alias = customButtonType?.FullName ?? throw new ArgumentNullException(nameof(customButtonType));
            ActionHandler = actionHandler ?? throw new ArgumentNullException(nameof(actionHandler));
        }

        internal string Alias { get; set; }
        internal Type ActionHandler { get; set; }
    }
}
