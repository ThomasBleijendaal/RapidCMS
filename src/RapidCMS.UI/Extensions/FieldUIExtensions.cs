using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Models.UI;
using RapidCMS.UI.Components.Editors;

namespace RapidCMS.UI.Extensions
{
    public static class FieldUIExtensions
    {
        public static RenderFragment? ToRenderFragment(this FieldUI field, EditContext editContext)
        {
            if (field is ExpressionFieldUI expressionField)
            {
                return builder =>
                {
                    builder.AddContent(0, expressionField.Expression.StringGetter(editContext.Entity));
                };
            }
            else if (field is CustomPropertyFieldUI customField)
            {
                return builder =>
                {
                    var editorType = customField.CustomType;

                    builder.OpenComponent(0, editorType);

                    builder.AddAttribute(1, nameof(BaseEditor.Entity), editContext.Entity);
                    builder.AddAttribute(2, nameof(BaseEditor.EntityState), editContext.EntityState);
                    builder.AddAttribute(3, nameof(BaseEditor.Parent), editContext.Parent);
                    builder.AddAttribute(4, nameof(BaseEditor.Property), customField.Property);
                    builder.AddAttribute(5, nameof(BaseEditor.IsDisabledFunc), customField.IsDisabled);

                    if (editorType.IsSubclassOf(typeof(BaseDataEditor)))
                    {
                        builder.AddAttribute(6, nameof(BaseDataEditor.DataCollection), customField.DataCollection);
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
                    builder.AddAttribute(1, nameof(BaseEditor.Entity), editContext.Entity);
                    builder.AddAttribute(2, nameof(BaseEditor.EntityState), editContext.EntityState);
                    builder.AddAttribute(3, nameof(BaseEditor.Parent), editContext.Parent);
                    if (field is PropertyFieldUI propertyField)
                    {
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
                    }
                    builder.CloseComponent();
                };
            }
        }
    }
}
