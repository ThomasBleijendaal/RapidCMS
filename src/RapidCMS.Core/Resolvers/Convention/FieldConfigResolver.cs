using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Providers;

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
                .Select((data, index) =>
                {
                    if (data.displayAttribute.ResourceType?.IsSameTypeOrDerivedFrom(typeof(ComponentBase)) == false)
                    {
                        throw new InvalidOperationException("ResourceType of [Display] must be a valid BaseEditor derived component.");
                    }

                    var propertyMetadata = PropertyMetadataHelper.GetPropertyMetadata(subject, data.property);

                    var propertyType = data.property.PropertyType.IsGenericType && data.property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                        ? Nullable.GetUnderlyingType(data.property.PropertyType)!
                        : data.property.PropertyType;

                    var displayType = features.HasFlag(Features.CanEdit) ? DisplayType.None : DisplayType.Label;
                    var editorType = !features.HasFlag(Features.CanEdit) ? EditorType.None
                        : data.displayAttribute.ResourceType != null ? EditorType.Custom
                        : EditorTypeHelper.TryFindDefaultEditorType(propertyType);
                    var customType = editorType == EditorType.Custom ? data.displayAttribute.ResourceType : null;

                    var relationConfig = editorType != EditorType.Select ? null :
                        new DataProviderRelationConfig(typeof(EnumDataProvider<>).MakeGenericType(propertyType));

                    return new FieldConfig
                    {
                        Description = features.HasFlag(Features.CanEdit) ? data.displayAttribute.Description : default,
                        DefaultOrder = data.displayAttribute.GetOrder() switch
                        {
                            1 => OrderByType.Ascending,
                            -1 => OrderByType.Descending,
                            _ => OrderByType.None
                        },
                        CustomType = customType,
                        DisplayType = displayType,
                        EditorType = editorType,
                        Index = !data.displayAttribute.GetOrder().HasValue ? index : data.displayAttribute.Order,
                        IsDisabled = (object x, EntityState y) => false,
                        IsVisible = (object x, EntityState y) => true,
                        Name = features.HasFlag(Features.CanEdit) ? data.displayAttribute.Name : data.displayAttribute.ShortName,
                        OrderByExpression = data.displayAttribute.GetOrder() == 0 ? null : propertyMetadata,
                        Placeholder = data.displayAttribute.GetPrompt(),
                        Property = propertyMetadata,
                        Relation = relationConfig
                    };
                });
        }
    }
}
