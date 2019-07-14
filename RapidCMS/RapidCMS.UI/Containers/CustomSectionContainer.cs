using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.UI;
using RapidCMS.UI.Components.Sections;
using RapidCMS.UI.Containers;

namespace RapidCMS.UI.Models
{
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
}
