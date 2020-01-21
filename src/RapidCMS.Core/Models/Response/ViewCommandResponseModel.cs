using System.Collections.Generic;

namespace RapidCMS.Core.Models.Response
{
    public class ViewCommandResponseModel
    {
        public IEnumerable<string> RefreshIds { get; internal set; } = new List<string>();
        public bool NoOp { get; internal set; }
    }
}
