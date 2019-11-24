
namespace RapidCMS.Common.Models.Commands
{
    public class UpdateParameterCommand : ViewCommand
    {
        public string? Action { get; set; }
        public string? CollectionAlias { get; set; }
        public string? VariantAlias { get; set; }
        public string? ParentPath { get; set; }
        public string? Id { get; set; }
    }
}
