using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.UI
{
    public class UISubject
    {
        public UsageType UsageType { get; set; }
        public IEntity Entity { get; set; }
    }
}
