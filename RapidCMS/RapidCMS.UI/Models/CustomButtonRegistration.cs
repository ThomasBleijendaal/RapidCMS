using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Metadata;
using RapidCMS.Common.Models.UI;
using RapidCMS.Common.ValueMappers;
using RapidCMS.UI.Components.Sections;

namespace RapidCMS.UI.Models
{
    // TODO: move to more sane locations
    public abstract class CustomContainer
    {
        protected Dictionary<string, CustomTypeRegistration>? _customRegistrations;

        public CustomContainer(IEnumerable<CustomTypeRegistration>? registrations)
        {
            if (registrations != null)
            {
                _customRegistrations = registrations.ToDictionary(x => x.Alias);
            }
        }
    }

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

                    builder.AddAttribute(1, "Model", model);
                    
                    builder.CloseComponent();
                };
            }

            return null;
        }
    }

    public class CustomEditorContainer : CustomContainer
    {
        public CustomEditorContainer(IEnumerable<CustomTypeRegistration> registrations) : base(registrations)
        {
        }

        public RenderFragment? GetCustomEditor(string editorAlias, IEntity entity, IPropertyMetadata property, IValueMapper valueMapper, IDataCollection dataCollection)
        {
            if (_customRegistrations != null && _customRegistrations.TryGetValue(editorAlias, out var registration))
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

    public class CustomSectionContainer : CustomContainer
    {
        public CustomSectionContainer(IEnumerable<CustomTypeRegistration> registrations) : base(registrations)
        {
        }

        public RenderFragment? GetCustomSection(string sectionAlias, SectionUI section)
        {
            if (_customRegistrations != null && _customRegistrations.TryGetValue(sectionAlias, out var registration))
            {
                return builder =>
                {
                    builder.OpenComponent(0, registration.Type);

                    builder.AddAttribute(1, nameof(BaseSection.Section), section);

                    builder.CloseComponent();
                };
            }

            return null;
        }
    }

    // TODO: extend with attributes (wait for blazor update to pass dictionay as attributes in)
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
