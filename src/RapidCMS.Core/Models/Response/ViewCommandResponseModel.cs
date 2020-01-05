using RapidCMS.Core.Models.Commands;

namespace RapidCMS.Core.Models.Response
{
    internal class ViewCommandResponseModel
    {
        internal ViewCommand ViewCommand { get; set; } = default!;
    }

    internal class ListViewCommandResponseModel : ViewCommandResponseModel
    {

    }

    internal class NodeViewCommandResponseModel : ViewCommandResponseModel
    {

    }
}
