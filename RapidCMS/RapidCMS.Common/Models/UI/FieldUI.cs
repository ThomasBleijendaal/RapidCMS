using System;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models.Metadata;
using RapidCMS.Common.ValueMappers;

namespace RapidCMS.Common.Models.UI
{
    public class FieldUI : Element
    {
        public string CustomAlias { get; internal set; }

        public EditorType Type { get; internal set; }

        public IValueMapper ValueMapper { get; internal set; }
        public IExpressionMetadata Expression { get; internal set; }
        public IPropertyMetadata Property { get; internal set; }
        public IDataCollection DataCollection { get; internal set; }
    }
}
