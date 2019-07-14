using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Models;
using RapidCMS.UI.Components.Buttons;
using RapidCMS.UI.Models;

namespace RapidCMS.UI.Containers
{
    public class CustomButtonContainer : CustomContainer
    {
        public CustomButtonContainer(IEnumerable<CustomTypeRegistration> registrations) : base(registrations)
        {
        }

        public RenderFragment? GetCustomButton(string buttonAlias, ButtonViewModel model)
        {
            if (_customRegistrations != null && _customRegistrations.TryGetValue(buttonAlias, out var registration))
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
