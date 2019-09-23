using System;
using System.Linq;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;

namespace RapidCMS.Common.Helpers
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

            return default;
        }
    }
}
