namespace RapidCMS.Core.Models.UI
{
    public class TabUI
    {
        public TabUI(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
        public string? Label { get; internal set; }
    }
}
