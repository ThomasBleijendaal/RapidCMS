using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models.UI;
using RapidCMS.UI.Components.Editors;

namespace RapidCMS.UI.Extensions
{
    public static class FieldUIExtensions
    {
        public static RenderFragment? ToRenderFragment(this FieldUI field, IEntity entity)
        {
            if (field is ExpressionFieldUI expressionField)
            {
                return builder =>
                {
                    builder.AddContent(0, expressionField.Expression.StringGetter(entity));
                };
            }
            else if (field is CustomPropertyFieldUI customField)
            {
                return builder =>
                {
                    var editorType = customField.CustomType;

                    builder.OpenComponent(0, editorType);

                    builder.AddAttribute(1, nameof(BaseEditor.Entity), entity);
                    builder.AddAttribute(2, nameof(BaseEditor.Property), customField.Property);

                    if (editorType.IsSubclassOf(typeof(BaseDataEditor)))
                    {
                        builder.AddAttribute(4, nameof(BaseDataEditor.DataCollection), customField.DataCollection);
                    }

                    builder.CloseComponent();
                };
            }
            else
            {
                var editorType = field.Type switch
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
                    builder.AddAttribute(1, nameof(BaseEditor.Entity), entity);
                    if (field is PropertyFieldUI propertyField)
                    {
                        builder.AddAttribute(2, nameof(BaseEditor.Property), propertyField.Property);

                        if (editorType.IsSubclassOf(typeof(BaseDataEditor)))
                        {
                            builder.AddAttribute(4, nameof(BaseDataEditor.DataCollection), propertyField.DataCollection);
                        }
                        if (editorType.IsSubclassOf(typeof(BaseRelationEditor)))
                        {
                            builder.AddAttribute(5, nameof(BaseRelationEditor.DataCollection), propertyField.DataCollection);
                        }
                    }
                    builder.CloseComponent();
                };
            }
        }
    }
}
