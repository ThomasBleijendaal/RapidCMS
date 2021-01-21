using System;
using System.Linq;
using RapidCMS.Core.Attributes;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Core.Helpers
{
    internal static class EditorTypeHelper
    {
        public static EditorType TryFindDefaultEditorType(Type editorType)
        {
            foreach (var type in EnumHelper.GetValues<EditorType>())
            {
                if (type.GetCustomAttribute<DefaultTypeAttribute>()?.Types.Contains(editorType) ?? false)
                {
                    return type;
                }
            }

            if (typeof(Enum).IsAssignableFrom(editorType))
            {
                return EditorType.Select;
            }

            return default;
        }
    }
}
