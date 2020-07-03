using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Resolvers.Convention
{
    internal class FieldConfigResolver : IFieldConfigResolver
    {
        public IEnumerable<FieldConfig> GetFields(Type subject, Features features)
        {
            var properties = subject.GetProperties();
            return properties
                .Select(property =>
                {
                    var displayAttribute = property.GetCustomAttribute<DisplayAttribute>();
                    if (displayAttribute != null)
                    {
                        if ((features.HasFlag(Features.CanEdit) && !string.IsNullOrEmpty(displayAttribute.Name)) || 
                            (!features.HasFlag(Features.CanEdit) && !string.IsNullOrEmpty(displayAttribute.ShortName)))
                        {
                            return (property, displayAttribute);
                        }   
                    }

                    return default;
                })
                .Where(x => x != default)
                .Select((data, index) => new FieldConfig
                {
                    Description = features.HasFlag(Features.CanEdit) ? data.displayAttribute.Description : default,
                    DisplayType = features.HasFlag(Features.CanEdit) ? DisplayType.None : DisplayType.Label,
                    EditorType = !features.HasFlag(Features.CanEdit) ? EditorType.None : EditorTypeHelper.TryFindDefaultEditorType(data.property.PropertyType),
                    Index = !data.displayAttribute.GetOrder().HasValue ? index : data.displayAttribute.Order,
                    IsDisabled = (object x, EntityState y) => false,
                    IsVisible = (object x, EntityState y) => true,
                    Name = features.HasFlag(Features.CanEdit) ? data.displayAttribute.Name : data.displayAttribute.ShortName,
                    Property = PropertyMetadataHelper.GetPropertyMetadata(subject, data.property)
                });
        }
    }
}
