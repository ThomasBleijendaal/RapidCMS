using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Metadata;
using RapidCMS.UI.Components.Editors;

namespace RapidCMS.UI.Containers
{
    public class CustomEditorContainer : CustomContainer
    {
        public CustomEditorContainer(IEnumerable<CustomTypeRegistration> registrations) : base(registrations)
        {
        }

        public RenderFragment? GetCustomEditor(string editorAlias, IEntity entity, IPropertyMetadata property, IDataCollection dataCollection)
        {
            if (_customRegistrations != null && _customRegistrations.TryGetValue(editorAlias, out var registration))
            {
                return builder =>
                {
                    var editorType = registration.Type;

                    builder.OpenComponent(0, editorType);

                    builder.AddAttribute(1, nameof(BaseEditor.Entity), entity);
                    builder.AddAttribute(2, nameof(BaseEditor.Property), property);

                    if (editorType.GetProperties().Any(x => x.Name == nameof(BaseDataEditor.DataCollection)))
                    {
                        builder.AddAttribute(4, nameof(BaseDataEditor.DataCollection), dataCollection);
                    }

                    builder.CloseComponent();
                };
            }

            return null;
        }
    }
}
