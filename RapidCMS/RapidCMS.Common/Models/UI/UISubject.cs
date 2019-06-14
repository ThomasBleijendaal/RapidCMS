using System;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.UI
{
    [Obsolete("Replace with EditContext")]
    public class UISubject
    {
        public UsageType UsageType { get; internal set; }
        public IEntity Entity { get; internal set; }
    }


}
