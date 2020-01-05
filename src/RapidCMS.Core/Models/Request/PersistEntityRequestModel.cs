using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Models.Request
{
    internal class PersistEntityRequestModel
    {
        internal EditContext EditContext { get; set; } = default!;
        internal string ActionId { get; set; } = default!;
        internal object? CustomData { get; set; }
    }
}
