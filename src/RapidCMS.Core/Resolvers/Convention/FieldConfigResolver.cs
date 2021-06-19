﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Attributes;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Config;
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
                    var fieldAttribute = property.GetCustomAttribute<FieldAttribute>();
                    if (fieldAttribute != null)
                    {
                        if ((features.HasFlag(Features.CanEdit) && !string.IsNullOrEmpty(fieldAttribute.Name)) ||
                            (!features.HasFlag(Features.CanEdit) && !string.IsNullOrEmpty(fieldAttribute.ListName)))
                        {
                            return (property, fieldAttribute);
                        }
                    }

                    return default;
                })
                .Where(x => x != default)
                .Select((data, index) =>
                {
                    if (data.fieldAttribute.EditorType?.IsSameTypeOrDerivedFrom(typeof(ComponentBase)) == false)
                    {
                        throw new InvalidOperationException("ResourceType of [Display] must be a valid BaseEditor derived component.");
                    }

                    var propertyMetadata = PropertyMetadataHelper.GetPropertyMetadata(subject, data.property);

                    var propertyType = data.property.PropertyType.IsGenericType && data.property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                        ? Nullable.GetUnderlyingType(data.property.PropertyType)!
                        : data.property.PropertyType;

                    var displayType = features.HasFlag(Features.CanEdit) ? DisplayType.None : DisplayType.Label;
                    var editorType = !features.HasFlag(Features.CanEdit) ? EditorType.None
                        : data.fieldAttribute.EditorType != null ? EditorType.Custom
                        : EditorTypeHelper.TryFindDefaultEditorType(propertyType);
                    var customType = editorType == EditorType.Custom ? data.fieldAttribute.EditorType : null;

                    var relationConfig = editorType != EditorType.Select ? null :
                        new DataProviderRelationConfig(typeof(EnumDataProvider<>).MakeGenericType(propertyType));

                    return new FieldConfig
                    {
                        Description = features.HasFlag(Features.CanEdit) ? data.fieldAttribute.Description : default,
                        DefaultOrder = data.fieldAttribute.OrderByType,
                        CustomType = customType,
                        DisplayType = displayType,
                        EditorType = editorType,
                        Index = data.fieldAttribute.Index,
                        IsDisabled = (object x, EntityState y) => false,
                        IsVisible = (object x, EntityState y) => true,
                        Name = features.HasFlag(Features.CanEdit) ? data.fieldAttribute.Name : data.fieldAttribute.ListName,
                        OrderByExpression = data.fieldAttribute.OrderByType == OrderByType.Disabled ? null : propertyMetadata,
                        Placeholder = data.fieldAttribute.Placeholder,
                        Property = propertyMetadata,
                        Relation = relationConfig
                    };
                });
        }
    }
}
