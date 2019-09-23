using System;

namespace RapidCMS.Common.Models.UI
{
    public class ButtonUI
    {
        public string ButtonId { get; internal set; }
        public string Label { get; internal set; }
        public string Icon { get; internal set; }
        public bool ShouldConfirm { get; internal set; }
        public bool IsPrimary { get; internal set; }
        public bool RequiresValidForm { get; internal set; }

        public Type? CustomType { get; internal set; }
    }
}
