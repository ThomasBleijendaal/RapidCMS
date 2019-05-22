namespace RapidCMS.Common.Models.UI
{
    public class ButtonUI
    {
        public string ButtonId { get; internal set; }
        public string Label { get; internal set; }
        public string Icon { get; internal set; }
        public bool ShouldConfirm { get; internal set; }
        public bool IsPrimary { get; internal set; }

        public string CustomAlias { get; internal set; }
    }
}
