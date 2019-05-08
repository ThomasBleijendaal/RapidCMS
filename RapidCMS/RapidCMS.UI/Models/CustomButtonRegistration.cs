using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Models;

#nullable enable

namespace RapidCMS.UI.Models
{
    // TODO: make generic for all typs of containers
    public class CustomButtonContainer
    {
        private Dictionary<string, Func<Type, RenderFragment>> _customButtons = new Dictionary<string, Func<Type, RenderFragment>>();

        public CustomButtonContainer(IEnumerable<CustomButtonRegistration>? registrations)
        {
            if (registrations != null)
            {
                foreach (var registration in registrations)
                {
                    _customButtons.Add(registration.ButtonAlias, (contextType) => builder =>
                    {
                        var buttonType = registration.ButtonType;

                        var genericButtonType = buttonType.MakeGenericType(contextType);

                        builder.OpenComponent(0, genericButtonType);
                        builder.CloseComponent();
                    });
                }
            }
        }

        public RenderFragment? GetCustomButton(string buttonAlias, Type contextType)
        {
            return _customButtons.TryGetValue(buttonAlias, out var customButton)
                ? customButton.Invoke(contextType)
                : null;
        }
    }
}
