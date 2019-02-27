using RapidCMS.Common.Enums;
using RapidCMS.Common.Models;

namespace RapidCMS.Common.Data
{
    public sealed class ViewContext
    {
        public UsageType Usage { get; set; }
        // TODO: nullable
        public EntityVariant EntityVariant { get; set; }
    }
}
