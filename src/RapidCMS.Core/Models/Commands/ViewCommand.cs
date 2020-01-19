using System.Collections.Generic;

namespace RapidCMS.Core.Models.Commands
{
    public sealed class ViewCommand
    {
        public bool ReloadData { get; set; }
        public IEnumerable<string> RefreshIds { get; internal set; } = new List<string>();
    }
}
