using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models.UI
{
    public class PropertyFieldUI : FieldUI
    {
        public EditorType Type { get; internal set; }

        public IPropertyMetadata Property { get; internal set; }
        public IDataCollection? DataCollection { get; internal set; }
    }
}
