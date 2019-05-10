using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Models;

#nullable enable

namespace RapidCMS.UI.Models
{
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
                        var genericButtonType = registration.ButtonType.MakeGenericType(contextType);

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

    public class CustomEditorContainer
    {
        private Dictionary<string, CustomEditorRegistration>? _customButtons;

        public CustomEditorContainer(IEnumerable<CustomEditorRegistration>? registrations)
        {
            if (registrations != null)
            {
                _customButtons = registrations.ToDictionary(x => x.EditorAlias);
            }
        }

        public RenderFragment? GetCustomEditor<TValue>(string editorAlias, TValue value, Action<TValue> callback)
        {
            if (_customButtons != null && _customButtons.TryGetValue(editorAlias, out var registration))
            {
                return builder =>
                {
                    var editorType = (registration.EditorType.IsGenericTypeDefinition)
                        ? registration.EditorType.MakeGenericType(typeof(TValue))
                        : registration.EditorType;

                    builder.OpenComponent(0, editorType);

                    builder.AddAttribute(1, "EditorValue", value);
                    builder.AddAttribute(2, "Callback", callback);

                    builder.CloseComponent();
                };
            }

            return null;
        }
    }
}
