#nullable enable

namespace RapidCMS.Common.Models.DTOs
{
    public abstract class ViewCommand
    {

    }

    public class NullOperationCommand : ViewCommand
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
        public string CollectionAlias { get; set; }
        public string VariantAlias { get; set; }
        public string? ParentId { get; set; }
        public string? Id { get; set; }
    }
}
