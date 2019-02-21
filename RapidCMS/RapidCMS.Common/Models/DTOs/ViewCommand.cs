namespace RapidCMS.Common.Models.DTOs
{
    public abstract class ViewCommand
    {

    }

    public class NavigateCommand : ViewCommand
    {
        public string Uri { get; set; }
    }

    public class ReloadCommand : ViewCommand
    {

    }

    public class UpdateParameterCommand : ViewCommand
    {
        public string Action { get; set; }
        public string Alias { get; set; }
        public int? ParentId { get; set; }
        public int? Id { get; set; }
    }
}
