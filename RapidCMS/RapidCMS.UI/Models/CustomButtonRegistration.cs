using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Metadata;
using RapidCMS.Common.Models.UI;
using RapidCMS.Common.ValueMappers;
using RapidCMS.UI.Components.Editors;
using RapidCMS.UI.Components.Sections;

namespace RapidCMS.UI.Models
{
    // TODO: move to more sane locations
    public class CustomButtonContainer
    {
        private Dictionary<string, Func<Type, RenderFragment>> _customButtons = new Dictionary<string, Func<Type, RenderFragment>>();

        public CustomButtonContainer(IEnumerable<CustomTypeRegistration>? registrations)
        {
            // TODO: make similair to CustomEditorContainer
            if (registrations != null)
            {
                foreach (var registration in registrations)
                {
                    _customButtons.Add(registration.Alias, (contextType) => builder =>
                    {
                        var genericButtonType = registration.Type.MakeGenericType(contextType);

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
        private Dictionary<string, CustomTypeRegistration>? _customButtons;

        public CustomEditorContainer(IEnumerable<CustomTypeRegistration>? registrations)
        {
            if (registrations != null)
            {
                _customButtons = registrations.ToDictionary(x => x.Alias);
            }
        }

        public RenderFragment? GetCustomEditor(string editorAlias, IEntity entity, IPropertyMetadata property, IValueMapper valueMapper, IDataCollection dataCollection)
        {
            if (_customButtons != null && _customButtons.TryGetValue(editorAlias, out var registration))
            {
                return builder =>
                {
                    var editorType = registration.Type;

                    builder.OpenComponent(0, editorType);

                    builder.AddAttribute(1, "Entity", entity);
                    builder.AddAttribute(2, "Property", property);
                    builder.AddAttribute(3, "ValueMapper", valueMapper);

                    // TODO: check for use of this property
                    // builder.AddAttribute(4, nameof(BaseRelationEditor.DataCollection), dataCollection);

                    builder.CloseComponent();
                };
            }

            return null;
        }
    }

    public class CustomSectionContainer
    {
        private Dictionary<string, CustomTypeRegistration>? _customButtons;

        public CustomSectionContainer(IEnumerable<CustomTypeRegistration>? registrations)
        {
            if (registrations != null)
            {
                _customButtons = registrations.ToDictionary(x => x.Alias);
            }
        }

        public RenderFragment? GetCustomSection(string sectionAlias, SectionUI section, UISubject subject)
        {
            if (_customButtons != null && _customButtons.TryGetValue(sectionAlias, out var registration))
            {
                return builder =>
                {
                    builder.OpenComponent(0, registration.Type);

                    builder.AddAttribute(1, nameof(BaseSection.Section), section);
                    builder.AddAttribute(2, nameof(BaseSection.Subject), subject);

                    builder.CloseComponent();
                };
            }

            return null;
        }
    }

    // TODO: extend with attributes
    public static class CustomRegistrationRenderFragmentExtensions
    {
        public static RenderFragment? ToRenderFragment(this CustomTypeRegistration? registration)
        {
            if (registration != null)
            {
                return builder =>
                {
                    builder.OpenComponent(0, registration.Type);
                    builder.CloseComponent();
                };
            }
            else
            {
                return default;
            }
        }
    }
}
