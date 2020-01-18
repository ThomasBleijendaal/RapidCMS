using System;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.UI;
using RapidCMS.UI.Components.Displays;
using RapidCMS.UI.Components.Editors;

namespace RapidCMS.UI.Extensions
{
    public static class FieldUIExtensions
    {
        public static RenderFragment? ToRenderFragment(this FieldUI field, EditContext editContext)
        {
            if (field is CustomExpressionFieldUI customExpressionField)
            {
                return builder =>
                {
                    var editorType = customExpressionField.CustomType;

                    builder.OpenComponent(0, editorType);

                    builder.AddAttribute(1, nameof(BaseDisplay.Entity), editContext.Entity);
                    builder.AddAttribute(2, nameof(BaseDisplay.EntityState), editContext.EntityState);
                    builder.AddAttribute(3, nameof(BaseDisplay.Parent), editContext.Parent);
                    builder.AddAttribute(4, nameof(BaseDisplay.Expression), customExpressionField.Expression);

                    builder.CloseComponent();
                };
            }
            else if (field is ExpressionFieldUI expressionField)
            {
                var displayType = expressionField.Type switch
                {
                    DisplayType.Label => typeof(LabelDisplay),
                    DisplayType.Pre => typeof(PreDisplay),
                    _ => null
                };

                if (displayType == null)
                {
                    return null;
                }

                return builder =>
                {
                    builder.OpenComponent(0, displayType);

                    builder.AddAttribute(1, nameof(BaseDisplay.Entity), editContext.Entity);
                    builder.AddAttribute(2, nameof(BaseDisplay.EntityState), editContext.EntityState);
                    builder.AddAttribute(3, nameof(BaseDisplay.Parent), editContext.Parent);
                    builder.AddAttribute(4, nameof(BaseDisplay.Expression), expressionField.Expression);

                    builder.CloseComponent();
                };
            }
            else if (field is CustomPropertyFieldUI customPropertyField)
            {
                return builder =>
                {
                    var editorType = customPropertyField.CustomType;

                    builder.OpenComponent(0, editorType);

                    builder.AddAttribute(1, nameof(BaseEditor.Entity), editContext.Entity);
                    builder.AddAttribute(2, nameof(BaseEditor.EntityState), editContext.EntityState);
                    builder.AddAttribute(3, nameof(BaseEditor.Parent), editContext.Parent);
                    builder.AddAttribute(4, nameof(BaseEditor.Property), customPropertyField.Property);
                    builder.AddAttribute(5, nameof(BaseEditor.IsDisabledFunc), customPropertyField.IsDisabled);

                    if (editorType.IsSubclassOf(typeof(BaseDataEditor)))
                    {
                        builder.AddAttribute(6, nameof(BaseDataEditor.DataCollection), customPropertyField.DataCollection);
                    }

                    builder.CloseComponent();
                };
            }
            else if (field is PropertyFieldUI propertyField)
            {
                var editorType = propertyField.Type switch
                {
                    EditorType.Readonly => typeof(ReadonlyEditor),
                    EditorType.Numeric => typeof(NumericEditor),
                    EditorType.Checkbox => typeof(CheckboxEditor),
                    EditorType.Date => typeof(DateEditor),
                    EditorType.TextArea => typeof(TextAreaEditor),
                    EditorType.TextBox => typeof(TextBoxEditor),
                    EditorType.Select => typeof(SelectEditor),
                    EditorType.MultiSelect => typeof(MultiSelectEditor),
                    EditorType.Dropdown => typeof(DropdownEditor),
                    _ => null
                };

                if (editorType == null)
                {
                    return null;
                }
                return builder =>
                {
                    builder.OpenComponent(0, editorType);
                    builder.AddAttribute(1, nameof(BaseEditor.Entity), editContext.Entity);
                    builder.AddAttribute(2, nameof(BaseEditor.EntityState), editContext.EntityState);
                    builder.AddAttribute(3, nameof(BaseEditor.Parent), editContext.Parent);
                    builder.AddAttribute(4, nameof(BaseEditor.Property), propertyField.Property);
                    builder.AddAttribute(5, nameof(BaseEditor.IsDisabledFunc), propertyField.IsDisabled);

                    if (editorType.IsSubclassOf(typeof(BaseDataEditor)))
                    {
                        builder.AddAttribute(6, nameof(BaseDataEditor.DataCollection), propertyField.DataCollection);
                    }
                    if (editorType.IsSubclassOf(typeof(BaseRelationEditor)))
                    {
                        builder.AddAttribute(7, nameof(BaseRelationEditor.DataCollection), propertyField.DataCollection);
                    }
                    builder.CloseComponent();
                };
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
