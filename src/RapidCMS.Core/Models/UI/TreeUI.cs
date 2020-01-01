namespace RapidCMS.Core.Models.UI
{
    public class TreeUI
    {
        public string Alias { get; set; }
        public string Name { get; set; }
        public string? Icon { get; set; }
        public string? Path { get; set; }

        public bool EntitiesVisible { get; set; }
        public bool RootVisible { get; set; }
    }
}
