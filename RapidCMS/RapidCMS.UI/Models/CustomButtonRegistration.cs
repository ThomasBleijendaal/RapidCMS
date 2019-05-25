using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.UI;
using RapidCMS.Common.ValueMappers;
using RapidCMS.UI.Components.Editors;
using RapidCMS.UI.Components.Sections;

#nullable enable

namespace RapidCMS.UI.Models
{
    // TODO: move to more sane locations
    public class CustomButtonContainer
    {
        private Dictionary<string, Func<Type, RenderFragment>> _customButtons = new Dictionary<string, Func<Type, RenderFragment>>();

        public CustomButtonContainer(IEnumerable<CustomButtonRegistration>? registrations)
        {
            // TODO: make similair to CustomEditorContainer
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

        public RenderFragment? GetCustomEditor(string editorAlias, IEntity entity, IPropertyMetadata property, IValueMapper valueMapper, IDataCollection dataCollection)
        {
            if (_customButtons != null && _customButtons.TryGetValue(editorAlias, out var registration))
            {
                return builder =>
                {
                    var editorType = registration.EditorType;

                    builder.OpenComponent(0, editorType);

                    builder.AddAttribute(1, nameof(BaseEditor.Entity), entity);
                    builder.AddAttribute(2, nameof(BaseEditor.Property), property);
                    builder.AddAttribute(3, nameof(BaseEditor.ValueMapper), valueMapper);

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
        private Dictionary<string, CustomSectionRegistration>? _customButtons;

        public CustomSectionContainer(IEnumerable<CustomSectionRegistration>? registrations)
        {
            if (registrations != null)
            {
                _customButtons = registrations.ToDictionary(x => x.SectionAlias);
            }
        }

        public RenderFragment? GetCustomSection(string sectionAlias, SectionUI section, UISubject subject)
        {
            if (_customButtons != null && _customButtons.TryGetValue(sectionAlias, out var registration))
            {
                return builder =>
                {
                    builder.OpenComponent(0, registration.SectionType);

                    builder.AddAttribute(1, nameof(BaseSection.Section), section);
                    builder.AddAttribute(2, nameof(BaseSection.Subject), subject);

                    builder.CloseComponent();
                };
            }

            return null;
        }
    }
}
