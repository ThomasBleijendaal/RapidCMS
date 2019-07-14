using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Metadata;
using RapidCMS.Common.ValueMappers;
using RapidCMS.UI.Components.Editors;

namespace RapidCMS.UI.Containers
{
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
}
