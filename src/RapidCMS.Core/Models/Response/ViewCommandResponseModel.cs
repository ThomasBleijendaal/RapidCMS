using RapidCMS.Core.Models.Commands;

namespace RapidCMS.Core.Models.Response
{
    public class ViewCommandResponseModel
    {
        internal ViewCommand ViewCommand { get; set; } = default!;
    }

    public class ListViewCommandResponseModel : ViewCommandResponseModel
    {

    }

    public class NodeViewCommandResponseModel : ViewCommandResponseModel
    {

    }

    public class NodeInListViewCommandResponseModel : ViewCommandResponseModel
    {

    }
}
