namespace RapidCMS.Common.Models.UI
{
    public class ButtonUI
    {
        public string ButtonId { get; set; }
        public string Label { get; set; }
        public string Icon { get; set; }
        public bool ShouldConfirm { get; set; }
        public bool IsPrimary { get; set; }

        public string Alias { get; set; }
    }
}
