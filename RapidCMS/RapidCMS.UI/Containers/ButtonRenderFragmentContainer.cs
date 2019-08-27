using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Models;
using RapidCMS.UI.Components.Buttons;
using RapidCMS.UI.Models;

namespace RapidCMS.UI.Containers
{
    public class ButtonRenderFragmentContainer : RenderFragmentContainer
    {
        public ButtonRenderFragmentContainer(IEnumerable<CustomTypeRegistration> registrations) : base(registrations)
        {
        }

        public RenderFragment? GetButton(string? buttonAlias, ButtonViewModel model)
        {
            if (buttonAlias == null)
            {
                return builder =>
                {
                    builder.OpenComponent(0, typeof(DefaultButton));
                    builder.AddAttribute(1, nameof(DefaultButton.Model), model);
                    builder.CloseComponent();
                };
            }
            else if (_customRegistrations != null && _customRegistrations.TryGetValue(buttonAlias, out var registration))
            {
                return builder =>
                {
                    builder.OpenComponent(0, registration.Type);
                    builder.AddAttribute(1, nameof(BaseButton.Model), model);
                    builder.CloseComponent();
                };
            }

            return null;
        }
    }
}
