using System;
using RapidCMS.Core.Enums;
using RapidCMS.UI.Components.Editors;

namespace RapidCMS.UI.Extensions
{
    public static class EditorTypeExtensions
    {
        public static Type? GetEditor(this EditorType editorType)
            => editorType switch
            {
                EditorType.Checkbox => typeof(CheckboxEditor),
                EditorType.Date => typeof(DateEditor),
                EditorType.Dropdown => typeof(DropdownEditor),
                EditorType.EnumFlagPicker => typeof(EnumFlagPicker),
                EditorType.EntitiesPicker => typeof(EntitiesPicker),
                EditorType.EntityPicker => typeof(EntityPicker),
                EditorType.ListEditor => typeof(ListEditor),
                EditorType.ModelEditor => typeof(ModelEditor),
                EditorType.MultiSelect => typeof(MultiSelectEditor),
                EditorType.Numeric => typeof(NumericEditor),
                EditorType.Readonly => typeof(ReadonlyEditor),
                EditorType.Select => typeof(SelectEditor),
                EditorType.TextArea => typeof(TextAreaEditor),
                EditorType.TextBox => typeof(TextBoxEditor),
                _ => null
            };
    }
}
